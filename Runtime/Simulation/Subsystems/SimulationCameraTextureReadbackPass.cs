#if URP_7_OR_NEWER
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if URP_17_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif // URP_17_OR_NEWER

namespace UnityEngine.XR.Simulation
{
    class SimulationCameraTextureReadbackPass : ScriptableRenderPass
    {
        /// <summary>
        /// The CameraTextureProvider class object that will execute the async readback logic.
        /// </summary>
        CameraTextureProvider m_Provider;

        const string k_PassName = "Copy Simulation Camera";

#if URP_17_OR_NEWER
        /// <summary>
        /// Data provided for the <see cref="ExecuteRenderGraphReadbackPass"/> function.
        /// </summary>
        class PassData
        {
            internal CameraTextureProvider cameraTextureProvider;
        }
#endif  // URP_17_OR_NEWER

        /// <summary>
        /// Constructs a <c>SimulationCameraTextureReadbackPass</c> class instance.
        /// </summary>
        /// <param name="cameraTextureProvider">The <c>CameraTextureProvider</c> <c>MonoBehaviour</c> object that
        /// implements the asynchronous readback logic.</param>
        public SimulationCameraTextureReadbackPass(CameraTextureProvider cameraTextureProvider)
        {
            // Initialize the CameraTextureProvider object that will execute the async readback logic.
            m_Provider = cameraTextureProvider;
            // Specify that the async readback pass will occur after all effects are rendered.
            renderPassEvent = RenderPassEvent.AfterRendering;
            profilingSampler = new ProfilingSampler(k_PassName);
            // Configure the camera's depth texture input for both the RenderGraph and non-RenderGraph async readback
            // passes. Color is not needed because the pass reads from its own m_SimulationCameraRenderTexture.
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        /// <summary>
        /// Called by the renderer before rendering a camera. Configures the camera's depth texture input type
        /// for the non-RenderGraph async readback pass.
        /// </summary>
        /// <param name="cmd">The <see cref="CommandBuffer"/> object to enqueue rendering commands.</param>
        /// <param name="renderingData">Current rendering state information.</param>
        /// <seealso cref="ScriptableRenderPass.ConfigureInput"/>
#pragma warning disable CS0672
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        /// <summary>
        /// Queries the <see cref="CameraTextureProvider"/> object to perform Simulation camera texture async readback
        /// with RenderGraph disabled.
        /// </summary>
        /// <param name="context">The <c>ScriptableRenderContext</c> object that lets us execute the rendering commands on
        /// the <see cref="CommandBuffer"/> object for this render pass.</param>
        /// <param name="renderingData">Current rendering state information. Unused for this render pass.</param>
        /// <seealso cref="CameraTextureProvider.TryConfigureReadbackCommandBuffer"/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
#pragma warning restore CS0672
        {
            var commandBuffer = CommandBufferPool.Get(k_PassName);
            if (m_Provider.TryConfigureReadbackCommandBuffer(commandBuffer))
            {
                context.ExecuteCommandBuffer(commandBuffer);
            }
            CommandBufferPool.Release(commandBuffer);
        }

#if URP_17_OR_NEWER
        /// <summary>
        /// Queries the <see cref="CameraTextureProvider"/> object to perform Simulation camera texture async readback
        /// with RenderGraph enabled.
        /// </summary>
        /// <param name="passData">The data that is passed to the function that executes this RenderGraph pass.</param>
        /// <param name="unsafeContext">The <c>UnsafeGraphContext</c> object that gives us access to the native
        /// <see cref="CommandBuffer"/> object to enqueue rendering instructions for this RenderGraph pass.</param>
        /// <seealso cref="CameraTextureProvider.TryConfigureRenderGraphReadbackCommandBuffer"/>
        static void ExecuteRenderGraphReadbackPass(PassData passData, UnsafeGraphContext unsafeContext)
        {
            var nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(unsafeContext.cmd);
            passData.cameraTextureProvider.TryConfigureRenderGraphReadbackCommandBuffer(nativeCommandBuffer);
        }

        /// <summary>
        /// Add the unsafe pass to the RenderGraph object in order to perform async readback on the camera's color and/or
        /// depth render textures.
        /// </summary>
        /// <remarks>
        /// This function needs to add an unsafe render pass to RenderGraph because a raster render pass, which is typically
        /// used for rendering with RenderGraph, cannot perform the texture readback operations performed with the
        /// <see cref="CommandBuffer"/> in <see cref="CameraTextureProvider"/>. Rendering Simulation camera textures is a
        /// special case. Unsafe passes can do certain operations that raster render passes cannot do and have access to
        /// the full command buffer API.
        /// </remarks>
        /// <param name="renderGraph">The RenderGraph object that we add the unsafe render pass to.</param>
        /// <param name="frameData">A <c>ContextContainer</c> object that is unused for this RenderGraph pass.</param>
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddUnsafePass<PassData>(
                k_PassName,
                out PassData passData,
                profilingSampler))
            {
                passData.cameraTextureProvider = m_Provider;

                builder.AllowPassCulling(false);
                builder.SetRenderFunc<PassData>(ExecuteRenderGraphReadbackPass);
            }
        }
#endif // URP_17_OR_NEWER
    }
}
#endif // URP_7_OR_NEWER
