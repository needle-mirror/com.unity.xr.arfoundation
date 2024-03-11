using UnityEngine.Rendering;
#if URP_7_OR_NEWER
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARSubsystems;
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
            if ((currentCamera != null) && (currentCamera.cameraType == CameraType.Game))
            {
                var cameraBackground = currentCamera.gameObject.GetComponent<ARCameraBackground>();
                if ((cameraBackground != null) && cameraBackground.backgroundRenderingEnabled
                    && (cameraBackground.material != null)
                    && TrySelectRenderPassForBackgroundRenderMode(cameraBackground.currentRenderingMode, out var renderPass))
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
            // Data provided for the static ExecuteRenderPass function
            private class PassData
            {
                internal Matrix4x4 worldToCameraMatrix;
                internal Matrix4x4 projectionMatrix;
                internal bool invertCulling;
                internal XRCameraBackgroundRenderingParams cameraBackgroundRenderingParams;
                internal Material backgroundMaterial;
            }

#if URP_17_OR_NEWER
            // Name of our Render Graph render pass
            const string k_RenderPassName = "AR Background Render Pass (Render Graph Enabled)";
#else
            // Name of our non-Render Graph render pass
            const string k_RenderPassName = "AR Background Render Pass (Render Graph Disabled)";
#endif  // URP_17_OR_NEWER

            /// <summary>
            /// The data that is passed to the render pass execute functions
            /// </summary>
            PassData m_RenderPassData = new();

            /// <summary>
            /// The material used for rendering the device background using the camera video texture and potentially
            /// other device-specific properties and textures.
            /// </summary>
            Material m_BackgroundMaterial;

            XRCameraBackgroundRenderingParams m_CameraBackgroundRenderingParams;

            /// <summary>
            /// Whether the culling mode should be inverted.
            /// ([CommandBuffer.SetInvertCulling](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.SetInvertCulling.html)).
            /// </summary>
            bool m_InvertCulling;

            /// <summary>
            /// The default platform rendering parameters for the camera background.
            /// </summary>
            XRCameraBackgroundRenderingParams defaultCameraBackgroundRenderingParams
                => ARCameraBackgroundRenderingUtils.SelectDefaultBackgroundRenderParametersForRenderMode(renderingMode);

            /// <summary>
            /// The rendering mode for the camera background.
            /// </summary>
            protected abstract XRCameraBackgroundRenderingMode renderingMode { get; }

            /// <summary>
            /// Set up the background render pass.
            /// </summary>
            /// <param name="cameraBackground">The <see cref="ARCameraBackground"/> component that provides the <see cref="Material"/>
            /// and any additional rendering information required by the render pass.</param>
            /// <param name="invertCulling">Whether the culling mode should be inverted.</param>
            public void Setup(ARCameraBackground cameraBackground, bool invertCulling)
            {
                SetupInternal(cameraBackground);

                if (!cameraBackground.TryGetRenderingParameters(out m_CameraBackgroundRenderingParams))
                    m_CameraBackgroundRenderingParams = defaultCameraBackgroundRenderingParams;

                m_BackgroundMaterial = cameraBackground.material;
                m_InvertCulling = invertCulling;
            }

            /// <summary>
            /// Provides inheritors an opportunity to perform any specialized setup during <see cref="ScriptableRenderPass.Setup"/>.
            /// </summary>
            /// <param name="cameraBackground">The <see cref="ARCameraBackground"/> component that provides the <see cref="Material"/>
            /// and any additional rendering information required by the render pass.</param>
            protected virtual void SetupInternal(ARCameraBackground cameraBackground) { }

            /// <summary>
            /// Execute the commands to render the camera background with Render Graph disabled.
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

                var cmd = CommandBufferPool.Get(k_RenderPassName);
                ExecuteRenderPass(CommandBufferHelpers.GetRasterCommandBuffer(cmd), m_RenderPassData);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            // Execute the commands to render the camera background. This function is used for both Render Graph and non Render Graph
            // paths. 
            static void ExecuteRenderPass(RasterCommandBuffer rasterCommandBuffer, PassData passData)
            {
                ARCameraBackground.AddBeforeBackgroundRenderHandler(rasterCommandBuffer);

                rasterCommandBuffer.SetInvertCulling(passData.invertCulling);
                rasterCommandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                rasterCommandBuffer.DrawMesh(
                    passData.cameraBackgroundRenderingParams.backgroundGeometry,
                    passData.cameraBackgroundRenderingParams.backgroundTransform,
                    passData.backgroundMaterial);
                rasterCommandBuffer.SetViewProjectionMatrices(passData.worldToCameraMatrix,
                    passData.projectionMatrix);
            }

#if URP_17_OR_NEWER
            static void ExecuteRenderGraphPass(PassData data, RasterGraphContext rasterContext)
            {
                ExecuteRenderPass(rasterContext.cmd, data);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                using (var builder = renderGraph.AddRasterRenderPass<PassData>(k_RenderPassName, out m_RenderPassData, profilingSampler))
                {
                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                    // Populate struct to send to ExecuteRenderGraphPass
                    m_RenderPassData.worldToCameraMatrix = cameraData.camera.worldToCameraMatrix;
                    m_RenderPassData.projectionMatrix = cameraData.camera.projectionMatrix;
                    m_RenderPassData.invertCulling = m_InvertCulling;
                    m_RenderPassData.cameraBackgroundRenderingParams = m_CameraBackgroundRenderingParams;
                    m_RenderPassData.backgroundMaterial = m_BackgroundMaterial;

                    // Shader keyword changes are considered global state modifications
                    builder.AllowGlobalStateModification(true);
                    builder.AllowPassCulling(false);

                    // The render graph render target is the main camera's active color buffer and
                    // the render graph depth target is the main camera's active depth buffer.
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

                    builder.SetRenderFunc<PassData>(ExecuteRenderGraphPass);
                }
            }
#endif  // URP_17_OR_NEWER

            /// <summary>
            /// Clean up any resources for the render pass.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for frame cleanup.</param>
            public override void FrameCleanup(CommandBuffer commandBuffer) { }
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
