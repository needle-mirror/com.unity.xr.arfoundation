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
        static CameraTextureProvider s_Provider;

#if URP_17_OR_NEWER
        // Data provided for the Render Graph render pass. 
        class PassData {}

        // Name of our Render Graph render pass
        static readonly string k_UnsafePassName = "SimulationCameraTextureReadbackPass (Render Graph Unsafe Pass)";
#endif  // URP_17_OR_NEWER

        public SimulationCameraTextureReadbackPass(CameraTextureProvider cameraTextureProvider)
        {
            s_Provider = cameraTextureProvider;
            renderPassEvent = RenderPassEvent.AfterRendering;
        }

#pragma warning disable CS0672
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
#pragma warning restore CS0672
        {
            using var commandBuffer = CommandBufferPool.Get("SimulationCameraTextureReadbackPass");
            if (s_Provider.TryConfigureReadbackCommandBuffer(commandBuffer))
            {
                context.ExecuteCommandBuffer(commandBuffer);
            }
        }

#if URP_17_OR_NEWER
        // The render function that Render Graph uses as a callback for RecordRenderGraph below. It needs to be static
        // because passing any non-static functions that rely on instance data or on local variables would cause the
        // Render Graph’s render function lambda to capture those, which will cause GC allocations.
        static void ExecuteRenderGraphPass(PassData passData, UnsafeGraphContext unsafeContext)
        {
            var nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(unsafeContext.cmd);

            if (!s_Provider.TryConfigureRenderGraphReadbackCommandBuffer(nativeCommandBuffer))
            {
                Debug.LogError("SimulationCameraTextureReadbackPass.ExecuteRenderGraphPass() - " +
                    "TryConfigureRenderGraphReadbackCommandBuffer() failed");
            }
        }

        // This function needs to add an unsafe render pass to Render Graph because a raster render pass, which is typically
        // used for rendering with Render Graph, cannot perform the texture readback operations performed with the command
        // buffer in CameraTextureProvider. Rendering Simulation camera textures is a special case. Unsafe passes can do
        // certain operations that raster render passes cannot do and have access to the full command buffer API.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddUnsafePass<PassData>(k_UnsafePassName, out PassData passData, profilingSampler))
            {
                builder.AllowPassCulling(false);

                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                builder.UseTexture(resourceData.activeColorTexture);
                builder.UseTexture(resourceData.activeDepthTexture);

                builder.SetRenderFunc<PassData>(ExecuteRenderGraphPass);
            }
        }
#endif // URP_17_OR_NEWER
    }
}
#endif // URP_7_OR_NEWER
