using UnityEngine;
using UnityEngine.Rendering;
#if MODULE_URP_ENABLED
using UnityEngine.Rendering.Universal;
#else
using ScriptableRendererFeature = UnityEngine.ScriptableObject;
#endif

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A render feature for rendering the camera background for AR devices.
    /// </summary>
    public class ARBackgroundRendererFeature : ScriptableRendererFeature
    {
#if MODULE_URP_ENABLED
        /// <summary>
        /// The scriptable render pass to be added to the renderer when the camera background is to be rendered.
        /// </summary>
        ARCameraBeforeOpaquesRenderPass m_BeforeOpaquesScriptablePass;
        /// <summary>
        /// The scriptable render pass to be added to the renderer when the camera background is to be rendered.
        /// </summary>
        ARCameraAfterOpaquesRenderPass m_AfterOpaquesScriptablePass;

        /// <summary>
        /// Create the scriptable render pass.
        /// </summary>
        public override void Create()
        {
        }

        /// <summary>
        /// Add the background rendering pass when rendering a game camera with an enabled AR camera background component.
        /// </summary>
        /// <param name="renderer">The scriptable renderer in which to enqueue the render pass.</param>
        /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Camera currentCamera = renderingData.cameraData.camera;
            if ((currentCamera != null) && (currentCamera.cameraType == CameraType.Game))
            {
                ARCameraBackground cameraBackground = currentCamera.gameObject.GetComponent<ARCameraBackground>();
                if ((cameraBackground != null) && cameraBackground.backgroundRenderingEnabled
                    && (cameraBackground.material != null))
                {
                    bool invertCulling = cameraBackground.GetComponent<ARCameraManager>()?.subsystem?.invertCulling ?? false;

                    ARCameraBackgroundRenderPass renderPass;
                    switch (cameraBackground.currentRenderingMode)
                    {
                        case XRCameraBackgroundRenderingMode.AfterOpaques:
                            m_AfterOpaquesScriptablePass ??= new ARCameraAfterOpaquesRenderPass();
                            renderPass = m_AfterOpaquesScriptablePass;
                            break;

                        case XRCameraBackgroundRenderingMode.BeforeOpaques:
                            m_BeforeOpaquesScriptablePass ??= new ARCameraBeforeOpaquesRenderPass();
                            renderPass = m_BeforeOpaquesScriptablePass;
                            break;

                        default:
                            return;
                    }

                    renderPass.Setup(cameraBackground.material, invertCulling);
                    renderer.EnqueuePass(renderPass);
                }
            }
        }

        abstract class ARCameraBackgroundRenderPass : ScriptableRenderPass
        {
            /// <summary>
            /// The name for the custom render pass which will display in graphics debugging tools.
            /// </summary>
            const string k_CustomRenderPassName = "AR Background Pass (URP)";

            /// <summary>
            /// The mesh for rendering the background material.
            /// </summary>
            protected Mesh m_BackgroundMesh;

            /// <summary>
            /// The material used for rendering the device background using the camera video texture and potentially
            /// other device-specific properties and textures.
            /// </summary>
            Material m_BackgroundMaterial;

            /// <summary>
            /// Whether the culling mode should be inverted.
            /// ([CommandBuffer.SetInvertCulling](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.SetInvertCulling.html)).
            /// </summary>
            bool m_InvertCulling;
            
            /// <summary>
            /// The projection matrix used to render the <see cref="mesh"/>.
            /// </summary>
            protected abstract Matrix4x4 projectionMatrix { get; }

            /// <summary>
            /// The (xref: UnityEngine.Mesh) used in this custom render pass.
            /// </summary>
            protected abstract Mesh mesh { get; }

            /// <summary>
            /// Set up the background render pass.
            /// </summary>
            /// <param name="backgroundMesh">The mesh used for rendering the device background.</param>
            /// <param name="backgroundMaterial">The material used for rendering the device background.</param>
            /// <param name="invertCulling">Whether the culling mode should be inverted.</param>
            public void Setup(Material backgroundMaterial, bool invertCulling)
            {
                m_BackgroundMaterial = backgroundMaterial;
                m_InvertCulling = invertCulling;
            }

            /// <summary>
            /// Execute the commands to render the camera background.
            /// </summary>
            /// <param name="context">The render context for executing the render commands.</param>
            /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(k_CustomRenderPassName);
                cmd.BeginSample(k_CustomRenderPassName);

                ARCameraBackground.AddBeforeBackgroundRenderHandler(cmd);
                cmd.SetInvertCulling(m_InvertCulling);

                cmd.SetViewProjectionMatrices(Matrix4x4.identity, projectionMatrix);

                cmd.DrawMesh(mesh, Matrix4x4.identity, m_BackgroundMaterial);

                cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix,
                                              renderingData.cameraData.camera.projectionMatrix);

                cmd.EndSample(k_CustomRenderPassName);
                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

            /// <summary>
            /// Clean up any resources for the render pass.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for frame cleanup.</param>
            public override void FrameCleanup(CommandBuffer commandBuffer)
            {
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
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
            {
                ConfigureClear(ClearFlag.Depth, Color.clear);
            }

            /// <inheritdoc />
            protected override Matrix4x4 projectionMatrix => ARCameraBackgroundRenderingUtils.beforeOpaquesOrthoProjection;

            /// <inheritdoc />
            protected override Mesh mesh => ARCameraBackgroundRenderingUtils.fullScreenNearClipMesh;
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
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
            {
                ConfigureClear(ClearFlag.None, Color.clear);
            }

            /// <inheritdoc />
            protected override Matrix4x4 projectionMatrix => ARCameraBackgroundRenderingUtils.afterOpaquesOrthoProjection;

            /// <inheritdoc />
            protected override Mesh mesh => ARCameraBackgroundRenderingUtils.fullScreenFarClipMesh;
        }
#endif // MODULE_URP_ENABLED
    }
}
