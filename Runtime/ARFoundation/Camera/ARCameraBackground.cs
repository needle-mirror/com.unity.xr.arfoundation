using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
#if URP_7_OR_NEWER
using UnityEngine.Experimental.Rendering;
#endif

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Add this component to a <c>Camera</c> to copy the color camera's texture onto the background.
    ///
    /// If you are using the Universal Render Pipeline (version 7.0.0 or later), you must also add the
    /// <see cref="ARBackgroundRendererFeature"/> to the list of render features for the scriptable renderer.
    /// </summary>
    /// <remarks>
    /// For information about how to set up the Universal Rendering Pipeline, refer to [Universal Rendering Pipeline](xref:arfoundation-universal-render-pipeline).
    ///
    /// Related information: <a href="xref:arfoundation-camera-components">Camera components</a>
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(ARCameraManager))]
    [AddComponentMenu("XR/AR Foundation/AR Camera Background")]
    [HelpUrl("features/camera/camera-components", "ar-camera-background-component")]
    [Icon(Constants.arvrIconPath)]
    public class ARCameraBackground : MonoBehaviour
    {
        const string k_BuiltInRenderPipelineObsoleteMessage =
            "The Built-in Render Pipeline is not supported in Unity 7000.0. AR Foundation no longer uses this member, and it will be removed in a future release. Use the Universal Render Pipeline and the ARBackgroundRendererFeature instead.";

        const string k_CameraRenderingModeChangedObsoleteMessage =
            "OnCameraRenderingModeChanged is no longer invoked. Only the Built-in Render Pipeline background rendering path raised it, and the Built-in Render Pipeline is not supported in Unity 7000.0. It will be removed in a future release.";

#if URP_7_OR_NEWER
        /// <summary>
        /// The signature of <see cref="BeforeBackgroundRenderHandler(int)"/>.
        /// </summary>
        delegate void BeforeBackgroundRenderHandlerDelegate(int eventId);

        /// <summary>
        /// A delegate representation of <see cref="BeforeBackgroundRenderHandler(int)"/>. This maintains a strong
        /// reference to the delegate, which is converted to an IntPtr by <see cref="s_BeforeBackgroundRenderHandlerFuncPtr"/>.
        /// </summary>
        /// <seealso cref="AddBeforeBackgroundRenderHandler(RasterCommandBuffer)"/>
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static BeforeBackgroundRenderHandlerDelegate s_BeforeBackgroundRenderHandler = BeforeBackgroundRenderHandler;

        /// <summary>
        /// A pointer to a method to be called immediately before rendering that is implemented in the XRCameraSubsystem implementation.
        /// It is called via [CommandBuffer.IssuePluginEvent](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.IssuePluginEvent.html).
        /// </summary>
        static readonly IntPtr s_BeforeBackgroundRenderHandlerFuncPtr = Marshal.GetFunctionPointerForDelegate(s_BeforeBackgroundRenderHandler);
#endif // URP_7_OR_NEWER

        /// <summary>
        /// Static reference to the active XRCameraSubsystem. Necessary here for access from a static delegate.
        /// </summary>
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static XRCameraSubsystem s_CameraSubsystem;

        /// <summary>
        /// (Obsolete) A delegate that was invoked when the <see cref="currentRenderingMode"/> changed while using the
        /// Built-in Render Pipeline. AR Foundation no longer invokes it because the Built-in Render Pipeline is not
        /// supported in Unity 7000.0.
        /// </summary>
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        [Obsolete(k_CameraRenderingModeChangedObsoleteMessage, true)]
        public static Action<XRCameraBackgroundRenderingMode> OnCameraRenderingModeChanged;

        /// <summary>
        /// Whether an <see cref="ARShaderOcclusion"/> component is enabled on this GameObject.
        /// </summary>
        bool m_IsShaderOcclusionComponentEnabled;

        /// <summary>
        /// Property ID for the shader parameter for the display transform matrix.
        /// </summary>
        int m_DisplayTransformId;

        /// <summary>
        /// The Property ID for the shader parameter for the forward vector's scaled length.
        /// </summary>
        int m_CameraForwardScaleId;

        /// <summary>
        /// The previous clear flags for the camera, if any.
        /// </summary>
        CameraClearFlags? m_PreviousCameraClearFlags;

        /// <summary>
        /// The original field of view of the camera, before enabling background rendering.
        /// </summary>
        float? m_PreviousCameraFieldOfView;

        internal ARDefaultCameraBackgroundRenderingParams defaultCameraBackgroundRenderingParams => m_DefaultCameraBackgroundRenderingParams;
        ARDefaultCameraBackgroundRenderingParams m_DefaultCameraBackgroundRenderingParams;

        /// <summary>
        /// The camera to which the projection matrix is set on each frame event.
        /// </summary>
        /// <value>The camera to which the projection matrix is set on each frame event.</value>
#if UNITY_EDITOR && !UNITY_6000_5_OR_NEWER
        protected new Camera camera => m_Camera;
#else
        protected Camera camera => m_Camera;
#endif // UNITY_EDITOR && !UNITY_6000_5_OR_NEWER
        Camera m_Camera;

        /// <summary>
        /// The camera manager from which frame information is pulled.
        /// </summary>
        /// <value>The camera manager.</value>
        protected ARCameraManager cameraManager => m_CameraManager;
        ARCameraManager m_CameraManager;

        /// <summary>
        /// The occlusion manager, which might not exist, from which occlusion information is pulled.
        /// </summary>
        protected internal AROcclusionManager occlusionManager => m_OcclusionManager;
        AROcclusionManager m_OcclusionManager;

        /// <summary>
        /// Whether culling should be inverted. Used during command buffer configuration,
        /// see [CommandBuffer.SetInvertCulling](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.SetInvertCulling.html).
        /// </summary>
        protected bool shouldInvertCulling
        {
            get
            {
                if (m_CameraManager == null || m_CameraManager.subsystem == null)
                    return false;

                return m_CameraManager.subsystem.invertCulling;
            }
        }

        /// <summary>
        /// Whether to use the custom material for rendering the background.
        /// </summary>
        /// <value><see langword="true"/> if the custom material should be used for rendering the camera background.
        /// Otherwise,<see langword="false"/>.</value>
        public bool useCustomMaterial
        {
            get => m_UseCustomMaterial;
            set => m_UseCustomMaterial = value;
        }
        [SerializeField, FormerlySerializedAs("m_OverrideMaterial")]
        bool m_UseCustomMaterial;

        /// <summary>
        /// A custom <c>Material</c> for rendering the background with your own shader.
        /// </summary>
        /// <remarks>
        /// Set this property to use your own shader to render the background.
        /// AR Foundation uses the <c>Material</c> from the active provider plug-in by default, but you can override the
        /// default with your own <c>Material</c>.
        /// </remarks>
        /// <value>The custom material.</value>
        public Material customMaterial
        {
            get => m_CustomMaterial;
            set => m_CustomMaterial = value;
        }
        [SerializeField, FormerlySerializedAs("m_Material")]
        Material m_CustomMaterial;

        /// <summary>
        /// Get whether background rendering is enabled and at least one camera frame has been received.
        /// </summary>
        /// <value><see langword="true"/> if background rendering is enabled and at least one camera frame has been received.
        /// Otherwise, <see langword="false"/>.</value>
        public bool backgroundRenderingEnabled => m_BackgroundRenderingEnabled;
        bool m_BackgroundRenderingEnabled;

        /// <summary>
        /// The current <c>Material</c> used for background rendering.
        /// </summary>
        /// <value>The material.</value>
        public Material material => useCustomMaterial && customMaterial != null ? customMaterial : cameraManager.cameraMaterial;

        /// <summary>
        /// The current <see cref="XRCameraBackgroundRenderingMode"/>. Determines which render order to use.
        /// </summary>
        public XRCameraBackgroundRenderingMode currentRenderingMode =>
            m_CameraManager == null ? XRCameraBackgroundRenderingMode.None : m_CameraManager.currentRenderingMode;

#if URP_7_OR_NEWER
        /// <summary>
        /// A function that can be invoked by
        /// [CommandBuffer.IssuePluginEvent](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.IssuePluginEvent.html).
        /// This function calls the XRCameraSubsystem method that should be called immediately before background rendering.
        /// </summary>
        /// <param name="eventId">The id of the event</param>
        [MonoPInvokeCallback(typeof(BeforeBackgroundRenderHandlerDelegate))]
        static void BeforeBackgroundRenderHandler(int eventId)
        {
            s_CameraSubsystem?.OnBeforeBackgroundRender(eventId);
        }
#endif // URP_7_OR_NEWER

        void Awake()
        {
            m_Camera = GetComponent<Camera>();
            m_CameraManager = GetComponent<ARCameraManager>();
            m_OcclusionManager = GetComponent<AROcclusionManager>();
            m_DefaultCameraBackgroundRenderingParams = new ARDefaultCameraBackgroundRenderingParams();
            m_DisplayTransformId = Shader.PropertyToID(RenderingUtility.k_DisplayTransformName);
            m_CameraForwardScaleId = Shader.PropertyToID(RenderingUtility.k_CameraForwardScaleName);
        }

        void OnEnable()
        {
            // Ensure that background rendering is disabled until the first camera frame is received.
            m_BackgroundRenderingEnabled = false;
            cameraManager.frameReceived += OnCameraFrameReceived;
            if (occlusionManager != null)
                occlusionManager.frameReceived += OnOcclusionFrameReceived;

            var shaderOcclusion = GetComponent<ARShaderOcclusion>();
            m_IsShaderOcclusionComponentEnabled = shaderOcclusion != null && shaderOcclusion.enabled;
            ARShaderOcclusion.shaderOcclusionComponentEnabled += OnShaderOcclusionComponentEnabled;
            ARShaderOcclusion.shaderOcclusionComponentDisabled += OnShaderOcclusionComponentDisabled;
        }

        void OnShaderOcclusionComponentEnabled(GameObject shaderGameObject)
        {
            if (shaderGameObject == gameObject)
                m_IsShaderOcclusionComponentEnabled = true;
        }

        void OnShaderOcclusionComponentDisabled(GameObject shaderGameObject)
        {
            if (shaderGameObject == gameObject)
                m_IsShaderOcclusionComponentEnabled = false;
        }

        void OnDisable()
        {
            if (occlusionManager != null)
                occlusionManager.frameReceived -= OnOcclusionFrameReceived;

            cameraManager.frameReceived -= OnCameraFrameReceived;
            DisableBackgroundRendering();

            // We are no longer setting the projection matrix, so tell the camera to resume its normal calculations.
            camera.ResetProjectionMatrix();

            ARShaderOcclusion.shaderOcclusionComponentEnabled -= OnShaderOcclusionComponentEnabled;
            ARShaderOcclusion.shaderOcclusionComponentDisabled -= OnShaderOcclusionComponentDisabled;
        }

        /// <summary>
        /// Enable background rendering by disabling the camera's clear flags.
        /// </summary>
        void EnableBackgroundRendering()
        {
            m_BackgroundRenderingEnabled = true;

            // We must hold a static reference to the camera subsystem so that it is accessible to the
            // static callback needed for calling OnBeforeBackgroundRender() from the render thread
            s_CameraSubsystem = m_CameraManager ? m_CameraManager.subsystem : null;

            DisableBackgroundClearFlags();
            m_PreviousCameraFieldOfView = m_Camera.fieldOfView;
        }

        /// <summary>
        /// Disable background rendering by restoring the camera's clear flags.
        /// </summary>
        void DisableBackgroundRendering()
        {
            m_BackgroundRenderingEnabled = false;
            RestoreBackgroundClearFlags();

            if (m_PreviousCameraFieldOfView.HasValue)
            {
                m_Camera.fieldOfView = m_PreviousCameraFieldOfView.Value;
                m_PreviousCameraFieldOfView = null;
            }

            s_CameraSubsystem = null;
        }

        /// <summary>
        /// Set the camera's clear flags to do nothing while preserving the previous camera clear flags.
        /// </summary>
        void DisableBackgroundClearFlags()
        {
            m_PreviousCameraClearFlags = m_Camera.clearFlags;
            m_Camera.clearFlags = currentRenderingMode == XRCameraBackgroundRenderingMode.AfterOpaques ?
                CameraClearFlags.Depth : CameraClearFlags.Nothing;
        }

        /// <summary>
        /// Restore the previous camera's clear flags, if any.
        /// </summary>
        void RestoreBackgroundClearFlags()
        {
            if (m_PreviousCameraClearFlags != null)
                m_Camera.clearFlags = m_PreviousCameraClearFlags.Value;
        }

        /// <summary>
        /// (Obsolete) The Built-in Render Pipeline camera events that background rendering used. AR Foundation no
        /// longer uses this property because the Built-in Render Pipeline is not supported in Unity 7000.0.
        /// </summary>
        /// <value>An empty collection.</value>
        [Obsolete(k_BuiltInRenderPipelineObsoleteMessage, true)]
        protected virtual IEnumerable<CameraEvent> legacyCameraEvents => Array.Empty<CameraEvent>();

        /// <summary>
        /// Attempts to read the platform specific rendering parameters from the camera subsystem.
        /// If it fails, it will return a default value built from the current <see cref="currentRenderingMode"/>.
        /// </summary>
        /// <param name="renderingParams">
        /// The platform specific rendering parameters if they are available. Otherwise, a default value built from the
        /// <see cref="currentRenderingMode"/>.
        /// </param>
        /// <returns><see langword="true"/> if the platform-specific rendering parameters are available.
        /// Otherwise, <see langword="false"/>.</returns>
        internal bool TryGetRenderingParameters(out XRCameraBackgroundRenderingParams renderingParams)
        {
            renderingParams = default;
            return s_CameraSubsystem != null && s_CameraSubsystem.TryGetRenderingParameters(out renderingParams);
        }

        /// <summary>
        /// (Obsolete) Configured the Built-in Render Pipeline background rendering command buffer. AR Foundation no
        /// longer calls this method because the Built-in Render Pipeline is not supported in Unity 7000.0.
        /// </summary>
        /// <param name="commandBuffer">The command buffer to configure.</param>
        [Obsolete(k_BuiltInRenderPipelineObsoleteMessage, true)]
        protected virtual void ConfigureLegacyCommandBuffer(CommandBuffer commandBuffer) { }

#if URP_7_OR_NEWER
        /// <summary>
        /// This adds a command to the <paramref name="commandBuffer"/> to make a call from the render thread
        /// to a callback on the `XRCameraSubsystem` implementation. The callback handles any implementation-specific
        /// functionality needed immediately before the background is rendered.
        /// </summary>
        /// <param name="commandBuffer">The raster command buffer to add the command to.</param>
        internal static void AddBeforeBackgroundRenderHandler(RasterCommandBuffer commandBuffer)
        {
            commandBuffer.IssuePluginEvent(s_BeforeBackgroundRenderHandlerFuncPtr, 0);
        }
#endif // URP_7_OR_NEWER

        /// <summary>
        /// Callback for the camera frame event.
        /// </summary>
        /// <param name="eventArgs">The camera event arguments.</param>
        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            var activeRenderingMode = currentRenderingMode;

            // Enable background rendering when first frame is received.
            if (m_BackgroundRenderingEnabled)
            {
                if (eventArgs.textures.Count == 0 || activeRenderingMode == XRCameraBackgroundRenderingMode.None)
                {
                    DisableBackgroundRendering();
                    return;
                }
            }
            else if (eventArgs.textures.Count > 0 && activeRenderingMode != XRCameraBackgroundRenderingMode.None)
            {
                EnableBackgroundRendering();
            }

            var mat = material;
            if (mat != null)
            {
                var count = eventArgs.textures.Count;
                for (int i = 0; i < count; ++i)
                {
                    mat.SetTexture(eventArgs.propertyNameIds[i], eventArgs.textures[i]);
                }

                if (eventArgs.displayMatrix.HasValue)
                    mat.SetMatrix(m_DisplayTransformId, eventArgs.displayMatrix.Value);

                RenderingUtility.SetShaderKeywords(mat, eventArgs.shaderKeywords);
            }

            if (eventArgs.projectionMatrix.HasValue)
            {
                camera.projectionMatrix = eventArgs.projectionMatrix.Value;
                const float twiceRad2Deg = 2 * Mathf.Rad2Deg;
                var halfHeightOverNear = 1 / camera.projectionMatrix[1, 1];
                camera.fieldOfView = Mathf.Atan(halfHeightOverNear) * twiceRad2Deg;
            }
        }

        /// <summary>
        /// Callback for the occlusion frame event.
        /// </summary>
        /// <param name="eventArgs">The occlusion frame event arguments.</param>
        void OnOcclusionFrameReceived(AROcclusionFrameEventArgs eventArgs)
        {
            // ARShaderOcclusion handles the occlusion textures when it's enabled.
            if (m_IsShaderOcclusionComponentEnabled)
                return;

            var mat = material;
            if (mat != null)
            {
                foreach (var tex in eventArgs.externalTextures)
                {
                    mat.SetTexture(tex.propertyId, tex.texture);
                }

                RenderingUtility.SetShaderKeywords(mat, eventArgs.shaderKeywords);

                // Set scale: this computes the affect the camera's localToWorld has on the length of the
                // forward vector, i.e., how much farther from the camera are things than with unit scale.
                var forward = transform.localToWorldMatrix.GetColumn(2);
                var scale = forward.magnitude;
                mat.SetFloat(m_CameraForwardScaleId, scale);
            }
        }
    }
}
