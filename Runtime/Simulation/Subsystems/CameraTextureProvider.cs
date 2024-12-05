using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;
#if URP_7_OR_NEWER
using UnityEngine.Rendering.Universal;
#endif // URP_7_OR_NEWER
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
        Camera m_SimulationCamera;
        RenderTexture m_SimulationCameraRenderTexture;
        RenderTexture m_SimulationCameraDepthRenderTexture;
        Texture2D m_SimulationReadbackTexture;
        Texture2D m_SimulationReadbackDepthTexture;
        CameraTextureFrameEventArgs? m_CameraFrameEventArgs;
        SimulationXRayManager m_XRayManager;

        readonly List<Texture2D> m_CameraImagePlanes = new();

        internal CameraTextureFrameEventArgs? CameraFrameEventArgs => m_CameraFrameEventArgs;

        bool m_Initialized;
        CommandBuffer m_ReadbackCommandBuffer;
#if URP_7_OR_NEWER
        SimulationCameraTextureReadbackPass m_SimulationReadbackRenderPass;
#endif // URP_7_OR_NEWER

        SimulationOcclusionSubsystem m_OcclusionSubsystem;

        bool m_EnableDepthReadback;

        int m_TextureSingleDepthPropertyNameId;
        int m_TextureSinglePropertyNameId;

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

            m_TextureSingleDepthPropertyNameId = Shader.PropertyToID(SimulationOcclusionSubsystem.k_TextureSingleDepthPropertyName);
            m_TextureSinglePropertyNameId = Shader.PropertyToID(SimulationCameraSubsystem.k_TextureSinglePropertyName);
        }

        void InitializeProvider(Camera xrCamera, Camera simulationCamera)
        {
            if (m_Initialized)
                return;

            m_XRayManager = new SimulationXRayManager();

            m_XrCamera = xrCamera;
            m_SimulationCamera = simulationCamera;
            CopyLimitedSettingsToCamera(m_XrCamera, m_SimulationCamera);

            var descriptor = new RenderTextureDescriptor(m_XrCamera.scaledPixelWidth, m_XrCamera.scaledPixelHeight);

            // Need to make sure we set the graphics format to our valid format
            // or we will get an out of range value for the render texture format
            // when we try creating the render texture
            descriptor.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            // Need to enable depth buffer if the target camera did not already have it.
            if (descriptor.depthBufferBits < 24)
                descriptor.depthBufferBits = 24;

            m_SimulationCameraRenderTexture = new RenderTexture(descriptor)
            {
                name = "XR Camera Render Texture",
                hideFlags = HideFlags.HideAndDontSave,
            };

            if (m_SimulationCameraRenderTexture.Create())
                m_SimulationCamera.targetTexture = m_SimulationCameraRenderTexture;

            if (m_SimulationReadbackTexture == null)
            {
                m_SimulationReadbackTexture = new Texture2D(
                    width: descriptor.width,
                    height: descriptor.height,
                    format: descriptor.graphicsFormat,
                    mipCount: 1,
                    flags: TextureCreationFlags.None)
                {
                    name = "Simulation Readback Texture",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            descriptor.graphicsFormat = GraphicsFormat.R32_SFloat;

            m_SimulationCameraDepthRenderTexture = new RenderTexture(descriptor)
            {
                name = "XR Camera Depth Render Texture",
                hideFlags = HideFlags.HideAndDontSave,
            };

            m_SimulationCameraDepthRenderTexture.Create();

            if (m_SimulationReadbackDepthTexture == null)
            {
                m_SimulationReadbackDepthTexture = new Texture2D(
                    width: descriptor.width,
                    height: descriptor.height,
                    format: descriptor.graphicsFormat,
                    mipCount: 1,
                    flags: TextureCreationFlags.None)
                {
                    name = "Simulation Readback Depth Texture",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            BaseSimulationSceneManager.environmentSetupFinished += OnEnvironmentSetupFinished;
#if URP_7_OR_NEWER
            if (!ARRenderingUtils.useLegacyRenderPipeline)
                m_SimulationReadbackRenderPass ??= new SimulationCameraTextureReadbackPass(this);
#endif // URP_7_OR_NEWER

            m_Initialized = true;
        }

        void OnDestroy()
        {
            BaseSimulationSceneManager.environmentSetupFinished -= OnEnvironmentSetupFinished;

            if (m_SimulationCamera != null)
                m_SimulationCamera.targetTexture = null;

            if (m_SimulationCameraRenderTexture != null)
                m_SimulationCameraRenderTexture.Release();

            if (m_SimulationCameraDepthRenderTexture != null)
                m_SimulationCameraDepthRenderTexture.Release();

            DestroyTextureIfExists(ref m_SimulationReadbackTexture);
            DestroyTextureIfExists(ref m_SimulationReadbackDepthTexture);
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
            CopyLimitedSettingsToCamera(m_XrCamera, m_SimulationCamera);

#if URP_7_OR_NEWER
            if (!ARRenderingUtils.useLegacyRenderPipeline)
                ConfigureTextureReadbackForURP();
            else
#endif // URP_7_OR_NEWER
                ConfigureBuiltInCommandBufferIfNeeded();

            DoCameraRender(m_SimulationCamera);

            if (!m_SimulationCameraRenderTexture.IsCreated()
                && !m_SimulationCameraRenderTexture.Create())
                return;

            EnsureTextureSizesMatch(
                m_SimulationReadbackTexture,
                m_SimulationCameraRenderTexture);

            m_CameraImagePlanes.Clear();
            m_CameraImagePlanes.Add(m_SimulationReadbackTexture);

            m_CameraFrameEventArgs = new CameraTextureFrameEventArgs
            {
                timestampNs = (long)(Time.time * 1e9),
                projectionMatrix = m_XrCamera.projectionMatrix,
                textures = m_CameraImagePlanes,
            };

            cameraFrameReceived?.Invoke(m_CameraFrameEventArgs.Value);

            if (!m_EnableDepthReadback
                || !m_SimulationCameraDepthRenderTexture.IsCreated()
                && !m_SimulationCameraDepthRenderTexture.Create())
                return;

            EnsureTextureSizesMatch(
                m_SimulationReadbackDepthTexture,
                m_SimulationCameraDepthRenderTexture);
        }

#if URP_7_OR_NEWER
        void ConfigureTextureReadbackForURP()
        {
            var universalAdditionalCameraData = m_SimulationCamera.GetUniversalAdditionalCameraData();
            universalAdditionalCameraData.requiresDepthOption = CameraOverrideOption.On;
            universalAdditionalCameraData.scriptableRenderer.EnqueuePass(m_SimulationReadbackRenderPass);
        }
#endif  // URP_7_OR_NEWER

        void ConfigureBuiltInCommandBufferIfNeeded()
        {
            if (m_ReadbackCommandBuffer != null)
                return;

            ConfigureBuiltInCommandBuffer();
        }

        void ConfigureBuiltInCommandBuffer()
        {
            if (m_SimulationCamera == null)
                return;

            if (m_ReadbackCommandBuffer != null)
                m_SimulationCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_ReadbackCommandBuffer);

            m_ReadbackCommandBuffer = new CommandBuffer { name = "Simulation Environment Readback Command Buffer" };
            if (TryConfigureReadbackCommandBuffer(m_ReadbackCommandBuffer))
            {
                m_SimulationCamera.AddCommandBuffer(CameraEvent.AfterEverything, m_ReadbackCommandBuffer);
            }
        }

        void OnEnvironmentSetupFinished()
        {
            var simulationSceneManager = SimulationSessionSubsystem.simulationSceneManager;
            m_SimulationCamera.scene = simulationSceneManager.environmentScene;
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

        /// <summary>
        /// Performs the Simulation camera render texture (color and/or depth texture) async readback with RenderGraph disabled.
        /// </summary>
        /// <returns><c>true</c> if async readback is successful, <c>false</c> otherwise.</returns>
        /// <param name="cmd">The <c>CommandBuffer</c> object to enqueue rendering commands.</param>
        /// <seealso cref="SimulationCameraTextureReadbackPass.Execute"/>
        internal bool TryConfigureReadbackCommandBuffer(CommandBuffer commandBuffer)
        {
            commandBuffer.Clear();
            commandBuffer.name = "Simulation Background Pre-Render";
            // Must default to true so if depth isn't needed, this function still returns true.
            bool canReadbackDepth = true;
            if (m_EnableDepthReadback)
            {
                var colorRenderBuffer = m_SimulationCamera.activeTexture.colorBuffer;
                var depthRenderBuffer = m_SimulationCamera.activeTexture.depthBuffer;
                commandBuffer.Blit(m_SimulationCameraRenderTexture, m_SimulationCameraDepthRenderTexture, s_DepthCopyShader);
                commandBuffer.CopyTexture(m_SimulationCameraDepthRenderTexture, m_SimulationReadbackDepthTexture);
                commandBuffer.SetRenderTarget(colorRenderBuffer, depthRenderBuffer);
                canReadbackDepth = TryRequestReadbackForImageType(
                    commandBuffer,
                    ImageType.Depth,
                    m_SimulationReadbackDepthTexture);
            }

            commandBuffer.CopyTexture(m_SimulationCameraRenderTexture, m_SimulationReadbackTexture);
            return TryRequestReadbackForImageType(commandBuffer, ImageType.Camera, m_SimulationReadbackTexture) && canReadbackDepth;
        }

        /// <summary>
        /// Performs the Simulation camera render texture (color and/or depth texture) async readback with RenderGraph enabled.
        /// </summary>
        /// <param name="cmd">The <c>CommandBuffer</c> object to enqueue rendering commands.</param>
        /// <seealso cref="SimulationCameraTextureReadbackPass.ExecuteRenderGraphReadbackPass"/>
        internal void TryConfigureRenderGraphReadbackCommandBuffer(CommandBuffer commandBuffer)
        {
            bool canReadbackDepth = m_SimulationReadbackDepthTexture != null;
            if (m_EnableDepthReadback && canReadbackDepth)
            {
                // Populates the GPU formatted depth RenderTexture using the color RenderTexture. s_DepthCopyShader
                // simply samples the shader variable _CameraDepthTexture at each pixel and stores it in
                // m_SimulationCameraDepthRenderTexture
                commandBuffer.Blit(m_SimulationCameraRenderTexture, m_SimulationCameraDepthRenderTexture, s_DepthCopyShader);
                // Converts the GPU formatted RenderTexture into the CPU formatted Texture2D. Async readback can only be
                // done using a CPU formatted Texture2D.
                commandBuffer.CopyTexture(m_SimulationCameraDepthRenderTexture, m_SimulationReadbackDepthTexture);
                // Performs the readback using the Texture2D variable m_SimulationReadbackDepthTexture.
                canReadbackDepth = TryRequestReadbackForImageType(
                    commandBuffer,
                    ImageType.Depth,
                    m_SimulationReadbackDepthTexture);

                if (!canReadbackDepth)
                    throw new ArgumentNullException(nameof(m_SimulationReadbackDepthTexture));
            }
            // Converts the GPU formatted RenderTexture into the CPU formatted Texture2D. Async readback can only be
            // done using a CPU formatted Texture2D.
            commandBuffer.CopyTexture(m_SimulationCameraRenderTexture, m_SimulationReadbackTexture);
            // Performs the readback using the Texture2D variable m_SimulationReadbackTexture.
            bool canReadbackColor = TryRequestReadbackForImageType(
                commandBuffer,
                ImageType.Camera,
                m_SimulationReadbackTexture);

            if (!canReadbackColor)
                throw new ArgumentNullException(nameof(m_SimulationReadbackTexture));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryRequestReadbackForImageType(CommandBuffer commandBuffer, ImageType imageType, Texture2D readbackTexture)
        {
            if (readbackTexture == null)
                return false;

            var textureDimensions = new Vector2Int(readbackTexture.width, readbackTexture.height);
            var textureFormat = readbackTexture.format;
            commandBuffer.RequestAsyncReadback(
                readbackTexture,
                asyncGPUReadbackRequest =>
                {
                    if (asyncGPUReadbackRequest.hasError)
                    {
                        Debug.LogError("Error reading back texture");
                        return;
                    }

                    using var textureData = asyncGPUReadbackRequest.GetData<byte>();
                    if (textureData.IsCreated)
                    {
                        onTextureReadbackFulfilled?.Invoke(
                            new TextureReadbackEventArgs(
                                imageType,
                                textureData,
                                textureDimensions,
                                textureFormat,
                                (long)(Time.time * 1e9)));
                    }
                });
            return true;
        }

        internal void SetEnableDepthReadback(bool useDepth)
        {
            if (m_EnableDepthReadback != useDepth)
            {
                m_EnableDepthReadback = useDepth;
#if !URP_7_OR_NEWER
                ConfigureBuiltInCommandBuffer();
#endif
            }
        }

        internal void TryGetTextureDescriptors(out NativeArray<XRTextureDescriptor> planeDescriptors,
            Allocator allocator)
        {
            Shader.SetGlobalTexture(m_TextureSinglePropertyNameId, m_SimulationReadbackTexture);
            var isValid = TryGetLatestImagePtr(out var nativePtr);
            var descriptors = new XRTextureDescriptor[1];
            if (isValid)
            {
                descriptors[0] = new XRTextureDescriptor(
                    nativeTexture: nativePtr,
                    width: m_SimulationReadbackTexture.width,
                    height: m_SimulationReadbackTexture.height,
                    mipmapCount: m_SimulationReadbackTexture.mipmapCount,
                    format: m_SimulationReadbackTexture.format,
                    propertyNameId: m_TextureSinglePropertyNameId,
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
            Shader.SetGlobalTexture(m_TextureSingleDepthPropertyNameId, m_SimulationReadbackDepthTexture);
            var isValid = TryGetLatestDepthImagePtr(out var nativePtr);
            var descriptors = new XRTextureDescriptor[1];
            if (isValid)
            {
                descriptors[0] = new XRTextureDescriptor(
                    nativeTexture: nativePtr,
                    width: m_SimulationReadbackDepthTexture.width,
                    height: m_SimulationReadbackDepthTexture.height,
                    mipmapCount: m_SimulationReadbackDepthTexture.mipmapCount,
                    format: m_SimulationReadbackDepthTexture.format,
                    propertyNameId: m_TextureSingleDepthPropertyNameId,
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
                && m_SimulationReadbackTexture != null
                && m_SimulationReadbackTexture.isReadable)
            {
                nativePtr = m_SimulationReadbackTexture.GetNativeTexturePtr();
                return true;
            }

            nativePtr = IntPtr.Zero;
            return false;
        }

        internal bool TryGetLatestDepthImagePtr(out IntPtr nativePtr)
        {
            if (m_SimulationReadbackDepthTexture != null
                && m_SimulationReadbackDepthTexture.isReadable)
            {
                nativePtr = m_SimulationReadbackDepthTexture.GetNativeTexturePtr();
                return true;
            }

            nativePtr = IntPtr.Zero;
            return false;
        }
    }
}
