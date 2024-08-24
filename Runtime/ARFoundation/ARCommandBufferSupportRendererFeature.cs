using UnityEngine.Rendering;
using UnityEngine.SubsystemsImplementation.Extensions;
using UnityEngine.XR.ARFoundation.InternalUtils;

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
    /// A renderer feature that gives support to XRSessionSubsystem providers that need access to URP
    /// command buffers.
    /// </summary>
    public class ARCommandBufferSupportRendererFeature : ScriptableRendererFeature
    {
#if URP_7_OR_NEWER
        /// <summary>
        /// A non-render pass that only injects the plugin event callback into command buffer to support session subsystem
        /// rendering modification.
        /// </summary>
        class EventInjectionRenderPass : ScriptableRenderPass
        {
            public EventInjectionRenderPass()
            {
                // Configure the event to be invoked before rendering started.
                // This ensures that necessary resources are ready when required during rendering.
                renderPassEvent = RenderPassEvent.BeforeRendering;
            }

            static readonly string k_NonRenderGraphPassName = "XRSessionSubsystem Command Buffer Event Injection Pass (Render Graph Disabled)";

#if URP_17_OR_NEWER
            /// <summary>
            /// No data is passed during this pass.
            /// </summary>
            private class PassData { }

            /// <summary>
            /// Name of our RenderGraph render pass.
            /// </summary>
            static readonly string k_RenderGraphPassName = "XRSessionSubsystem Command Buffer Event Injection Pass (Render Graph Enabled)";

            /// <summary>
            /// Execute the commands to inject the plugin event callback with RenderGraph enabled.
            /// </summary>
            /// <param name="data">PassData is unused for this render pass.</param>
            /// <param name="unsafeContext">The UnsafeGraphContext object that gives us access to the native
            /// CommandBuffer object to enqueue rendering instructions for this RenderGraph pass.</param>
            static void ExecutePass(PassData data, UnsafeGraphContext unsafeContext)
            {
                var nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(unsafeContext.cmd);
                ExecuteRenderPass(nativeCommandBuffer);
            }

            /// <summary>
            /// Add the unsafe pass to the RenderGraph object in order to inject the plugin event callback.
            /// </summary>
            /// <param name="renderGraph">The RenderGraph object that we add the unsafe render pass to.</param>
            /// <param name="frameData">A ContextContainer object that is unused for this RenderGraph pass.</param>
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (RequiresCommandBufferSupport(out var sessionSubsystem))
                {
                    using (var builder = renderGraph.AddUnsafePass<PassData>(k_RenderGraphPassName, out PassData passData))
                    {
                        builder.AllowPassCulling(false);
                        builder.SetRenderFunc<PassData>(ExecutePass);
                    }

                    sessionSubsystem.OnCommandBufferSupportEnabled();
                }
            }
#endif  // URP_17_OR_NEWER

            /// <summary>
            /// Execute the commands to inject the plugin event callback. This function is used for both RenderGraph and
            /// non-RenderGraph paths.
            /// </summary>
            /// <param name="commandBuffer">The CommandBuffer object that allows us to enqueue rendering instructions for this render pass.</param>
            /// <param name="passData">The data that is passed to the function that executes this render pass.</param>
            static void ExecuteRenderPass(CommandBuffer commandBuffer)
            {
                if (RequiresCommandBufferSupport(out var sessionSubsystem))
                    sessionSubsystem.OnCommandBufferExecute(commandBuffer);
            }

            /// <summary>
            /// Execute the commands to inject the plugin event callback with RenderGraph disabled.
            /// </summary>
            /// <param name="context">The render context for executing the render commands.</param>
            /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
#pragma warning disable CS0672
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
#pragma warning restore CS0672
            {
                var commandBuffer = CommandBufferPool.Get(k_NonRenderGraphPassName);
                ExecuteRenderPass(commandBuffer);
                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }

        EventInjectionRenderPass m_EventInjectionPass;

        /// <summary>
        /// Create the scriptable render pass.
        /// </summary>
        public override void Create()
        {
            m_EventInjectionPass = new EventInjectionRenderPass();
            if (RequiresCommandBufferSupport(out var sessionSubsystem))
                sessionSubsystem.GetProvider().OnCommandBufferSupportEnabled();
        }

        /// <summary>
        /// Add the render pass to inject the plugin event callback.
        /// </summary>
        /// <param name="renderer">The scriptable renderer in which to enqueue the render pass.</param>
        /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (RequiresCommandBufferSupport(out var _))
            {
                renderer.EnqueuePass(m_EventInjectionPass);
            }
        }

        static bool RequiresCommandBufferSupport(out XRSessionSubsystem sessionSubsystem)
        {
            return SubsystemUtils.TryGetLoadedSubsystem<XRSessionSubsystem>(out sessionSubsystem) &&
                (sessionSubsystem?.requiresCommandBuffer ?? false);
        }
#endif // URP_7_OR_NEWER
    }
}
