using UnityEngine.Rendering;
#if URP_7_OR_NEWER
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARSubsystems;
using System;
#if URP_17_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#else
using UnityEngine.Experimental.Rendering;
#endif // URP_17_OR_NEWER
#else
using ScriptableRendererFeature = UnityEngine.ScriptableObject;
#endif // URP_7_OR_NEWER

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A render feature for rendering the camera background for AR devices.
    /// </summary>
    public class ARBackgroundRendererFeature : ScriptableRendererFeature
    {
#if URP_7_OR_NEWER
        /// <summary>
        /// The scriptable render pass to be added to the renderer when the camera background is to be rendered.
        /// </summary>
        ARCameraBeforeOpaquesRenderPass beforeOpaquesScriptablePass => m_BeforeOpaquesScriptablePass ??= new ARCameraBeforeOpaquesRenderPass();
        ARCameraBeforeOpaquesRenderPass m_BeforeOpaquesScriptablePass;

        /// <summary>
        /// The scriptable render pass to be added to the renderer when the camera background is to be rendered.
        /// </summary>
        ARCameraAfterOpaquesRenderPass afterOpaquesScriptablePass => m_AfterOpaquesScriptablePass ??= new ARCameraAfterOpaquesRenderPass();
        ARCameraAfterOpaquesRenderPass m_AfterOpaquesScriptablePass;

        /// <summary>
        /// Create the scriptable render pass.
        /// </summary>
        public override void Create() { }

        /// <summary>
        /// Add the background rendering pass when rendering a game camera with an enabled AR camera background component.
        /// </summary>
        /// <param name="renderer">The scriptable renderer in which to enqueue the render pass.</param>
        /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var currentCamera = renderingData.cameraData.camera;
            if (currentCamera != null && currentCamera.cameraType == CameraType.Game)
            {
                var cameraBackground = currentCamera.gameObject.GetComponent<ARCameraBackground>();
                if (cameraBackground != null &&
                    cameraBackground.backgroundRenderingEnabled &&
                    cameraBackground.material != null &&
                    TrySelectRenderPassForBackgroundRenderMode(cameraBackground.currentRenderingMode, out var renderPass))
                {
                    var invertCulling = cameraBackground.GetComponent<ARCameraManager>()?.subsystem?.invertCulling ?? false;
                    renderPass.Setup(cameraBackground, invertCulling);
                    renderer.EnqueuePass(renderPass);
                }
            }
        }

        /// <summary>
        /// Selects the render pass for a given <see cref="UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode"/>
        /// </summary>
        /// <param name="renderingMode">The <see cref="UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode"/>
        /// that indicates which render pass to use.
        /// </param>
        /// <param name="renderPass">The <see cref="ARCameraBackgroundRenderPass"/> that corresponds
        /// to the given <paramref name="renderingMode"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="renderPass"/> was populated. Otherwise, <c>false</c>.
        /// </returns>
        bool TrySelectRenderPassForBackgroundRenderMode(XRCameraBackgroundRenderingMode renderingMode, out ARCameraBackgroundRenderPass renderPass)
        {
            switch (renderingMode)
            {
                case XRCameraBackgroundRenderingMode.AfterOpaques:
                    renderPass = afterOpaquesScriptablePass;
                    return true;

                case XRCameraBackgroundRenderingMode.BeforeOpaques:
                    renderPass = beforeOpaquesScriptablePass;
                    return true;

                case XRCameraBackgroundRenderingMode.None:
                default:
                    renderPass = null;
                    return false;
            }
        }

        /// <summary>
        /// An abstract <see cref="ScriptableRenderPass"/> that provides common utilities for rendering an AR Camera Background.
        /// </summary>
        abstract class ARCameraBackgroundRenderPass : ScriptableRenderPass
        {
            /// <summary>
            /// Data provided for the <see cref="ExecuteRasterRenderGraphPass"/> function.
            /// </summary>
            class PassData
            {
                internal Matrix4x4 worldToCameraMatrix;
                internal Matrix4x4 projectionMatrix;
                internal bool invertCulling;
                internal XRCameraBackgroundRenderingParams cameraBackgroundRenderingParams;
                internal Material backgroundMaterial;
            }

#if URP_17_OR_NEWER
            // Name of our RenderGraph render pass.
            const string k_RenderGraphPassName = "AR Background Render Pass (Render Graph Enabled)";
#endif  // URP_17_OR_NEWER

            /// <summary>
            /// The data that is passed to the render pass execute functions.
            /// </summary>
            PassData m_RenderPassData = new();

            /// <summary>
            /// The material used for rendering the device background using the camera video texture and potentially
            /// other device-specific properties and textures.
            /// </summary>
            Material m_BackgroundMaterial;

            /// <summary>
            /// The geometry and transform of the camera background for a given platform.
            /// </summary>
            XRCameraBackgroundRenderingParams m_CameraBackgroundRenderingParams;

            ARDefaultCameraBackgroundRenderingParams m_DefaultCameraBackgroundRenderingParams;

            /// <summary>
            /// Whether the culling mode should be inverted.
            /// ([CommandBuffer.SetInvertCulling](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.SetInvertCulling.html)).
            /// </summary>
            bool m_InvertCulling;

            /// <summary>
            /// The default platform geometry and transform for the camera background.
            /// </summary>
            XRCameraBackgroundRenderingParams defaultCameraBackgroundRenderingParams
                => m_DefaultCameraBackgroundRenderingParams.SelectDefaultBackgroundRenderParametersForRenderMode(renderingMode);

            /// <summary>
            /// The rendering mode for the camera background. Options are None, BeforeOpaques, and AfterOpaques.
            /// </summary>
            protected abstract XRCameraBackgroundRenderingMode renderingMode { get; }

            /// <summary>
            /// Set up the background render pass.
            /// </summary>
            /// <param name="cameraBackground">The <c>ARCameraBackground</c> component that provides the
            /// <see cref="Material"/> and any additional rendering information required by the render pass.</param>
            /// <param name="invertCulling">Whether the culling mode should be inverted.</param>
            public void Setup(ARCameraBackground cameraBackground, bool invertCulling)
            {
                SetupInternal(cameraBackground);

                m_DefaultCameraBackgroundRenderingParams = cameraBackground.defaultCameraBackgroundRenderingParams;
                if (!cameraBackground.TryGetRenderingParameters(out m_CameraBackgroundRenderingParams))
                    m_CameraBackgroundRenderingParams = defaultCameraBackgroundRenderingParams;

                m_BackgroundMaterial = cameraBackground.material;
                m_InvertCulling = invertCulling;
            }

            /// <summary>
            /// Provides inheritors an opportunity to perform any specialized setup during
            /// <see cref="ScriptableRenderPass.Setup"/>.
            /// </summary>
            /// <param name="cameraBackground">The <c>ARCameraBackground</c> component that provides the
            /// <see cref="Material"/> and any additional rendering information required by the render pass.</param>
            protected virtual void SetupInternal(ARCameraBackground cameraBackground) { }

            /// <summary>
            /// Execute the commands to render the camera background with RenderGraph disabled.
            /// </summary>
            /// <param name="context">The render context for executing the render commands.</param>
            /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
#pragma warning disable CS0672
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
#pragma warning restore CS0672
            {
                // Populate struct to send to ExecuteRenderPass
                m_RenderPassData.worldToCameraMatrix = renderingData.cameraData.camera.worldToCameraMatrix;
                m_RenderPassData.projectionMatrix = renderingData.cameraData.camera.projectionMatrix;
                m_RenderPassData.invertCulling = m_InvertCulling;
                m_RenderPassData.cameraBackgroundRenderingParams = m_CameraBackgroundRenderingParams;
                m_RenderPassData.backgroundMaterial = m_BackgroundMaterial;

                var cmd = CommandBufferPool.Get("AR Background Render Pass (Render Graph Disabled)");
                ExecuteRenderPass(CommandBufferHelpers.GetRasterCommandBuffer(cmd), m_RenderPassData);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            /// <summary>
            /// Execute the commands to render the camera background. This function is used for both RenderGraph and
            /// non-RenderGraph paths.
            /// </summary>
            /// <param name="rasterCommandBuffer">The <c>RasterCommandBuffer</c> object that allows us to enqueue
            /// rendering instructions to the native <see cref="CommandBuffer"/> for this render pass.</param>
            /// <param name="passData">The data that is passed to the function that executes this render pass.</param>
            static void ExecuteRenderPass(RasterCommandBuffer rasterCommandBuffer, PassData passData)
            {
                ARCameraBackground.AddBeforeBackgroundRenderHandler(rasterCommandBuffer);

                rasterCommandBuffer.SetInvertCulling(passData.invertCulling);
                rasterCommandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                rasterCommandBuffer.DrawMesh(
                    passData.cameraBackgroundRenderingParams.backgroundGeometry,
                    passData.cameraBackgroundRenderingParams.backgroundTransform,
                    passData.backgroundMaterial);
                rasterCommandBuffer.SetViewProjectionMatrices(
                    passData.worldToCameraMatrix,
                    passData.projectionMatrix);
            }

#if URP_17_OR_NEWER
            /// <summary>
            /// This is part of the RenderGraph path. It calls <see cref="ExecuteRenderPass"/>, which is shared by both the
            /// RenderGraph and non-RenderGraph paths.
            /// </summary>
            /// <param name="passData">The data that is passed to the function that executes this render pass.</param>
            /// <param name="rasterContext">The <c>RasterGraphContext</c> object that allows us to access the
            /// <see cref="RasterCommandBuffer"/> and native <see cref="CommandBuffer"/> to enqueue rendering instructions
            /// for this render pass.</param>
            static void ExecuteRasterRenderGraphPass(PassData passData, RasterGraphContext rasterContext)
            {
                ExecuteRenderPass(rasterContext.cmd, passData);
            }

            /// <summary>
            /// Create and add the raster pass to RenderGraph, as well as storing all the data needed to execute the pass.
            /// </summary>
            /// <param name="renderGraph">The <c>RenderGraph</c> object that we add the raster render pass to.</param>
            /// <param name="frameData">A <c>ContextContainer</c> object that gives us access to the camera and render
            /// texture data for this RenderGraph raster pass.</param>
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                using (var builder = renderGraph.AddRasterRenderPass<PassData>(
                    k_RenderGraphPassName,
                    out m_RenderPassData,
                    profilingSampler))
                {
                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                    // Populate struct to send to ExecuteRasterRenderGraphPass
                    m_RenderPassData.worldToCameraMatrix = cameraData.camera.worldToCameraMatrix;
                    m_RenderPassData.projectionMatrix = cameraData.camera.projectionMatrix;
                    m_RenderPassData.invertCulling = m_InvertCulling;
                    m_RenderPassData.cameraBackgroundRenderingParams = m_CameraBackgroundRenderingParams;
                    m_RenderPassData.backgroundMaterial = m_BackgroundMaterial;

                    // Shader keyword changes are considered global state modifications
                    builder.AllowGlobalStateModification(true);
                    builder.AllowPassCulling(false);

                    // The RenderGraph render target is the main camera's active color TextureHandle
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);

                    // The RenderGraph depth target is the main camera's active depth TextureHandle
                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

                    builder.SetRenderFunc<PassData>(ExecuteRasterRenderGraphPass);
                }
            }
#endif  // URP_17_OR_NEWER

            /// <summary>
            /// Called upon finish rendering a camera. Releases any resources created by this render pass or
            /// otherwise executes any cleanup code.
            /// </summary>
            /// <param name="cmd">Use this <c>CommandBuffer</c> to cleanup any generated data</param>
            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                if (cmd == null)
                    throw new ArgumentNullException(nameof(cmd));

                // If invert culling is true, we must make sure it is only enabled for the minimum amount of time
                // in code where it is needed, since other systems won't necessarily work when it is enabled.
                if (m_InvertCulling)
                    cmd.SetInvertCulling(false);
            }
        }

        /// <summary>
        /// The custom render pass to render the camera background before rendering opaques.
        /// </summary>
        class ARCameraBeforeOpaquesRenderPass : ARCameraBackgroundRenderPass
        {
            /// <summary>
            /// Constructs the background render pass.
            /// </summary>
            public ARCameraBeforeOpaquesRenderPass()
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            }

            /// <summary>
            /// Configure the render pass by setting the render target and clear values.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for configuration.</param>
            /// <param name="renderTextureDescriptor">The descriptor of the target render texture.</param>
#pragma warning disable CS0672
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
#pragma warning restore CS0672
            {
#pragma warning disable CS0618
                ConfigureClear(ClearFlag.Depth, Color.clear);
#pragma warning restore CS0618
            }

            protected override XRCameraBackgroundRenderingMode renderingMode
                => XRCameraBackgroundRenderingMode.BeforeOpaques;
        }

        /// <summary>
        /// The custom render pass to render the camera background after rendering opaques.
        /// </summary>
        class ARCameraAfterOpaquesRenderPass : ARCameraBackgroundRenderPass
        {
            /// <summary>
            /// Constructs the background render pass.
            /// </summary>
            public ARCameraAfterOpaquesRenderPass()
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }

            /// <summary>
            /// Configure the render pass by setting the render target and clear values.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for configuration.</param>
            /// <param name="renderTextureDescriptor">The descriptor of the target render texture.</param>
#pragma warning disable CS0672
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
#pragma warning restore CS0672
            {
#pragma warning disable CS0618
                ConfigureClear(ClearFlag.None, Color.clear);
#pragma warning restore CS0618
            }

            /// <inheritdoc />
            protected override void SetupInternal(ARCameraBackground cameraBackground)
            {
                if (cameraBackground.GetComponent<AROcclusionManager>()?.enabled ?? false)
                {
                    // If an occlusion texture is being provided, rendering will need
                    // to compare it against the depth texture created by the camera.
                    ConfigureInput(ScriptableRenderPassInput.Depth);
                }
            }

            protected override XRCameraBackgroundRenderingMode renderingMode
                => XRCameraBackgroundRenderingMode.AfterOpaques;
        }
#endif // URP_7_OR_NEWER
    }
}
