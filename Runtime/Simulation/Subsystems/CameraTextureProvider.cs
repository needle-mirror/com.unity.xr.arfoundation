using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;
#if MODULE_URP_ENABLED
using UnityEngine.Rendering.Universal;
#endif // end MODULE_URP_ENABLED

using ImageType = UnityEngine.XR.Simulation.SimulationXRCpuImageApi.ImageType;

namespace UnityEngine.XR.Simulation
{
    class CameraTextureProvider : MonoBehaviour
    {
        internal readonly struct TextureReadbackEventArgs
        {
            public readonly ImageType imageType;
            public readonly NativeSlice<byte> textureData;
            public readonly Vector2Int textureDimensions;
            public readonly TextureFormat textureFormat;
            public readonly long timestampNs;

            public TextureReadbackEventArgs(
                ImageType imageType,
                NativeArray<byte> textureData,
                Vector2Int textureDimensions,
                TextureFormat textureFormat,
                long timestampNs)
            {
                this.imageType = imageType;
                this.textureData = textureData;
                this.textureDimensions = textureDimensions;
                this.textureFormat = textureFormat;
                this.timestampNs = timestampNs;
            }
        }

        internal static event Action<Camera> preRenderCamera;
        internal static event Action<Camera> postRenderCamera;
        internal event Action<CameraTextureFrameEventArgs> cameraFrameReceived;
        internal event Action<TextureReadbackEventArgs> onTextureReadbackFulfilled;

        static Material s_DepthCopyShader;

        Camera m_XrCamera;
        Camera m_SimulationRenderCamera;
        RenderTexture m_SimulationRenderTexture;
        RenderTexture m_SimulationRenderDepthTexture;
        Texture2D m_SimulationProviderTexture;
        Texture2D m_SimulationProviderDepthTexture;
        CameraTextureFrameEventArgs? m_CameraFrameEventArgs;
        SimulationXRayManager m_XRayManager;

        readonly List<Texture2D> m_CameraImagePlanes = new();

        internal CameraTextureFrameEventArgs? CameraFrameEventArgs => m_CameraFrameEventArgs;

        bool m_Initialized;
        CommandBuffer m_ReadbackCommandBuffer;
#if MODULE_URP_ENABLED
        SimulationCameraTextureReadbackPass m_SimulationReadbackRenderPass;
#endif // end MODULE_URP_ENABLED

        SimulationOcclusionSubsystem m_OcclusionSubsystem;

        bool m_EnableDepthReadback;

        internal static CameraTextureProvider AddTextureProviderToCamera(Camera simulationCamera, Camera xrCamera)
        {
            simulationCamera.depth = xrCamera.depth - 1;
            simulationCamera.cullingMask = 1 << XRSimulationRuntimeSettings.Instance.environmentLayer;
            simulationCamera.clearFlags = CameraClearFlags.Color;
            simulationCamera.backgroundColor = Color.clear;
            simulationCamera.depthTextureMode = DepthTextureMode.Depth;

            if (!simulationCamera.gameObject.TryGetComponent<CameraTextureProvider>(out var cameraTextureProvider))
                cameraTextureProvider = simulationCamera.gameObject.AddComponent<CameraTextureProvider>();

            cameraTextureProvider.InitializeProvider(xrCamera, simulationCamera);

            return cameraTextureProvider;
        }

        void Awake()
        {
            if (s_DepthCopyShader == null)
                s_DepthCopyShader = new Material(Shader.Find("Hidden/DepthCopy"));
        }

        void OnDestroy()
        {
            BaseSimulationSceneManager.environmentSetupFinished -= OnEnvironmentSetupFinished;

            if (m_SimulationRenderCamera != null)
                m_SimulationRenderCamera.targetTexture = null;

            if (m_SimulationRenderTexture != null)
                m_SimulationRenderTexture.Release();

            if (m_SimulationRenderDepthTexture != null)
                m_SimulationRenderDepthTexture.Release();

            DestroyTextureIfExists(ref m_SimulationProviderTexture);
            DestroyTextureIfExists(ref m_SimulationProviderDepthTexture);
        }

        static void DestroyTextureIfExists(ref Texture2D texture)
        {
            if (texture != null)
            {
                UnityObjectUtils.Destroy(texture);
                texture = null;
            }
        }

        void EnsureTextureSizesMatch(Texture2D texture, RenderTexture renderTexture)
        {
            if (texture.width != renderTexture.width
                || texture.height != renderTexture.height)
            {
                texture.Reinitialize(renderTexture.width, renderTexture.height);
            }
        }

        void Update()
        {
            if (!m_Initialized)
                return;

            // Currently assuming the main camera is being set to the correct settings for rendering to the target device
            m_XrCamera.ResetProjectionMatrix();
            CopyLimitedSettingsToCamera(m_XrCamera, m_SimulationRenderCamera);

#if MODULE_URP_ENABLED
            if (!ARRenderingUtils.useLegacyRenderPipeline)
                ConfigureTextureReadbackForURP();
            else
#endif // end MODULE_URP_ENABLED
                ConfigureBuiltInCommandBufferIfNeeded();

            DoCameraRender(m_SimulationRenderCamera);

            if (!m_SimulationRenderTexture.IsCreated()
                && !m_SimulationRenderTexture.Create())
                return;

            EnsureTextureSizesMatch(
                m_SimulationProviderTexture,
                m_SimulationRenderTexture);

            Graphics.CopyTexture(m_SimulationRenderTexture, m_SimulationProviderTexture);

            m_CameraImagePlanes.Clear();
            m_CameraImagePlanes.Add(m_SimulationProviderTexture);

            m_CameraFrameEventArgs = new CameraTextureFrameEventArgs
            {
                timestampNs = (long)(Time.time * 1e9),
                projectionMatrix = m_XrCamera.projectionMatrix,
                textures = m_CameraImagePlanes,
            };

            cameraFrameReceived?.Invoke(m_CameraFrameEventArgs.Value);

            if (!m_EnableDepthReadback
                || !m_SimulationRenderDepthTexture.IsCreated()
                && !m_SimulationRenderDepthTexture.Create())
                return;

            EnsureTextureSizesMatch(
                m_SimulationProviderDepthTexture,
                m_SimulationRenderDepthTexture);

            Graphics.CopyTexture(m_SimulationRenderDepthTexture, m_SimulationProviderDepthTexture);
        }

#if MODULE_URP_ENABLED
        void ConfigureTextureReadbackForURP()
        {
            var universalAdditionalCameraData = m_SimulationRenderCamera.GetUniversalAdditionalCameraData();
            universalAdditionalCameraData.scriptableRenderer.EnqueuePass(m_SimulationReadbackRenderPass);
        }
#endif // end MODULE_URP_ENABLED

        void ConfigureBuiltInCommandBufferIfNeeded()
        {
            if (m_ReadbackCommandBuffer != null)
                return;

            ConfigureBuiltInCommandBuffer();
        }

        void ConfigureBuiltInCommandBuffer()
        {
            if (m_ReadbackCommandBuffer != null)
                m_SimulationRenderCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_ReadbackCommandBuffer);

            m_ReadbackCommandBuffer = new CommandBuffer { name = "Simulation Environment Readback Command Buffer" };
            if (TryConfigureReadbackCommandBuffer(m_ReadbackCommandBuffer))
            {
                m_SimulationRenderCamera.AddCommandBuffer(CameraEvent.AfterEverything, m_ReadbackCommandBuffer);
            }
        }

        void InitializeProvider(Camera xrCamera, Camera simulationCamera)
        {
            if (m_Initialized)
                return;

            m_XRayManager = new SimulationXRayManager();

            m_XrCamera = xrCamera;
            m_SimulationRenderCamera = simulationCamera;
            CopyLimitedSettingsToCamera(m_XrCamera, m_SimulationRenderCamera);

            var descriptor = new RenderTextureDescriptor(m_XrCamera.scaledPixelWidth, m_XrCamera.scaledPixelHeight);

            // Need to make sure we set the graphics format to our valid format
            // or we will get an out of range value for the render texture format
            // when we try creating the render texture
            descriptor.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            // Need to enable depth buffer if the target camera did not already have it.
            if (descriptor.depthBufferBits < 24)
                descriptor.depthBufferBits = 24;

            m_SimulationRenderTexture = new RenderTexture(descriptor)
            {
                name = "XR Render Camera",
                hideFlags = HideFlags.HideAndDontSave,
            };

            if (m_SimulationRenderTexture.Create())
                m_SimulationRenderCamera.targetTexture = m_SimulationRenderTexture;

            if (m_SimulationProviderTexture == null)
            {
                m_SimulationProviderTexture = new Texture2D(
                    width: descriptor.width,
                    height: descriptor.height,
                    format: descriptor.graphicsFormat,
                    mipCount: 1,
                    flags: TextureCreationFlags.None)
                {
                    name = "Simulated Native Camera Texture",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            descriptor.graphicsFormat = GraphicsFormat.R32_SFloat;

            m_SimulationRenderDepthTexture = new RenderTexture(descriptor)
            {
                name = "XR Render Camera Depth",
                hideFlags = HideFlags.HideAndDontSave,
            };

            m_SimulationRenderDepthTexture.Create();

            if (m_SimulationProviderDepthTexture == null)
            {
                m_SimulationProviderDepthTexture = new Texture2D(
                    width: descriptor.width,
                    height: descriptor.height,
                    format: descriptor.graphicsFormat,
                    mipCount:1,
                    flags: TextureCreationFlags.None)
                {
                    name = "Simulated Native Camera Depth Texture",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            BaseSimulationSceneManager.environmentSetupFinished += OnEnvironmentSetupFinished;
#if MODULE_URP_ENABLED
            if (!ARRenderingUtils.useLegacyRenderPipeline)
                m_SimulationReadbackRenderPass ??= new SimulationCameraTextureReadbackPass(this);
#endif // end MODULE_URP_ENABLED

            m_Initialized = true;
        }

        void OnEnvironmentSetupFinished()
        {
            var simulationSceneManager = SimulationSessionSubsystem.simulationSceneManager;
            m_SimulationRenderCamera.scene = simulationSceneManager.environmentScene;
        }

        static void CopyLimitedSettingsToCamera(Camera source, Camera destination)
        {
            var destinationTransform = destination.transform;
            var scene = destination.scene;
            var cullingMask = destination.cullingMask;
            var clearFlags = destination.clearFlags;
            var backgroundColor = destination.backgroundColor;
            var depth = destination.depth;
            var depthTextureMode = destination.depthTextureMode;
            var targetTexture = destination.targetTexture;
            var localPosition = destinationTransform.localPosition;
            var localRotation = destinationTransform.localRotation;
            var localScale = destinationTransform.localScale;

            destination.CopyFrom(source);
            destination.projectionMatrix = source.projectionMatrix;

            destination.scene = scene;
            destination.cullingMask = cullingMask;
            destination.clearFlags = clearFlags;
            destination.backgroundColor = backgroundColor;
            destination.depth = depth;
            destination.depthTextureMode = depthTextureMode;
            destination.targetTexture = targetTexture;
            destinationTransform.localPosition = localPosition;
            destinationTransform.localRotation = localRotation;
            destinationTransform.localScale = localScale;
        }

        void DoCameraRender(Camera renderCamera)
        {
            m_XRayManager.UpdateXRayShader(false, null);

            preRenderCamera?.Invoke(renderCamera);
            renderCamera.Render();
            postRenderCamera?.Invoke(renderCamera);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryRequestReadbackForImageType(CommandBuffer commandBuffer, ImageType imageType, Texture2D texture)
        {
            if (texture == null)
                return false;

            var textureDimensions = new Vector2Int(texture.width, texture.height);
            var textureFormat = texture.format;
            commandBuffer.RequestAsyncReadback(
                texture,
                request =>
                {
                    OnTextureReadbackComplete(
                        request,
                        imageType,
                        (long)(Time.time * 1e9),
                        textureDimensions,
                        textureFormat);
                });
            return true;
        }

        void OnTextureReadbackComplete(
            AsyncGPUReadbackRequest request,
            ImageType imageType,
            long timestampNs,
            Vector2Int textureDimensions,
            TextureFormat textureFormat)
        {
            if (request.hasError)
            {
                Debug.LogError("Error reading back texture");
                return;
            }

            using var textureData = request.GetData<byte>();
            if (textureData.IsCreated)
            {
                onTextureReadbackFulfilled?.Invoke(
                    new TextureReadbackEventArgs(
                        imageType,
                        textureData,
                        textureDimensions,
                        textureFormat,
                        timestampNs));
            }
        }

        internal void SetEnableDepthReadback(bool useDepth)
        {
            if (m_EnableDepthReadback != useDepth)
            {
                m_EnableDepthReadback = useDepth;
                ConfigureBuiltInCommandBuffer();
            }
        }

        internal bool TryConfigureReadbackCommandBuffer(CommandBuffer commandBuffer)
        {
            commandBuffer.Clear();
            commandBuffer.name = "Simulation Background Pre-Render";
            // Must default to true so if depth isn't needed, this function still returns true.
            bool depthReabackReturn = true;
            if (m_EnableDepthReadback)
            {
                var colorTarget = m_SimulationRenderCamera.activeTexture.colorBuffer;
                var depthTarget = m_SimulationRenderCamera.activeTexture.depthBuffer;
                commandBuffer.Blit(m_SimulationRenderTexture, m_SimulationRenderDepthTexture, s_DepthCopyShader);
                commandBuffer.SetRenderTarget(colorTarget, depthTarget);
                depthReabackReturn = TryRequestReadbackForImageType(
                    commandBuffer,
                    ImageType.Depth,
                    m_SimulationProviderDepthTexture);
            }

            return TryRequestReadbackForImageType(commandBuffer, ImageType.Camera, m_SimulationProviderTexture) &&
                depthReabackReturn;
        }

        internal void TryGetTextureDescriptors(out NativeArray<XRTextureDescriptor> planeDescriptors,
            Allocator allocator)
        {
            Shader.SetGlobalTexture(SimulationCameraSubsystem.textureSinglePropertyNameId, m_SimulationProviderTexture);
            var isValid = TryGetLatestImagePtr(out var nativePtr);
            var descriptors = new XRTextureDescriptor[1];
            if (isValid)
            {
                descriptors[0] = new XRTextureDescriptor(
                    nativeTexture: nativePtr,
                    width: m_SimulationProviderTexture.width,
                    height: m_SimulationProviderTexture.height,
                    mipmapCount: m_SimulationProviderTexture.mipmapCount,
                    format: m_SimulationProviderTexture.format,
                    propertyNameId: SimulationCameraSubsystem.textureSinglePropertyNameId,
                    depth: 0,
                    dimension: TextureDimension.Tex2D);
            }
            else
                descriptors[0] = default;

            planeDescriptors = new NativeArray<XRTextureDescriptor>(descriptors, allocator);
        }

        internal void TryGetDepthTextureDescriptors(out NativeArray<XRTextureDescriptor> planeDescriptors,
            Allocator allocator)
        {
            Shader.SetGlobalTexture(SimulationOcclusionSubsystem.textureSingleDepthPropertyNameId, m_SimulationProviderDepthTexture);
            var isValid = TryGetLatestDepthImagePtr(out var nativePtr);
            var descriptors = new XRTextureDescriptor[1];
            if (isValid)
            {
                descriptors[0] = new XRTextureDescriptor(
                    nativeTexture: nativePtr,
                    width: m_SimulationProviderDepthTexture.width,
                    height: m_SimulationProviderDepthTexture.height,
                    mipmapCount: m_SimulationProviderDepthTexture.mipmapCount,
                    format: m_SimulationProviderDepthTexture.format,
                    propertyNameId: SimulationOcclusionSubsystem.textureSingleDepthPropertyNameId,
                    depth: 1,
                    dimension: TextureDimension.Tex2D);
            }
            else
            {
                descriptors[0] = default;
            }

            planeDescriptors = new NativeArray<XRTextureDescriptor>(descriptors, allocator);
        }

        internal bool TryGetLatestImagePtr(out IntPtr nativePtr)
        {
            if (m_CameraImagePlanes is { Count: > 0 }
                && m_SimulationProviderTexture != null
                && m_SimulationProviderTexture.isReadable)
            {
                nativePtr =  m_SimulationProviderTexture.GetNativeTexturePtr();
                return true;
            }

            nativePtr = IntPtr.Zero;
            return false;
        }

        internal bool TryGetLatestDepthImagePtr(out IntPtr nativePtr)
        {
            if (m_SimulationProviderDepthTexture != null
                && m_SimulationProviderDepthTexture.isReadable)
            {
                nativePtr = m_SimulationProviderDepthTexture.GetNativeTexturePtr();
                return true;
            }

            nativePtr = IntPtr.Zero;
            return false;
        }
    }
}
