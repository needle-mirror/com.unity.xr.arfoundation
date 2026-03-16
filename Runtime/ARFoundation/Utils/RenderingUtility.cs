using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    static class RenderingUtility
    {
        /// <summary>
        /// Name of the shader parameter for the camera forward scale.
        /// </summary>
        internal const string k_CameraForwardScaleName = "_UnityCameraForwardScale";

        /// <summary>
        /// Name of the shader parameter for the display transform matrix.
        /// </summary>
        internal const string k_DisplayTransformName = "_UnityDisplayTransform";

        internal static void SetShaderKeywords(Material material, XRShaderKeywords keywords)
        {
            if (keywords.enabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.enabledKeywords)
                    material.EnableKeyword(shaderKeyword);
            }

            if (keywords.disabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.disabledKeywords)
                {
                    if (material.IsKeywordEnabled(shaderKeyword))
                        material.DisableKeyword(shaderKeyword);
                }
            }
        }


        internal static void SetShaderKeywordsGlobal(XRShaderKeywords keywords)
        {
            if (keywords.enabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.enabledKeywords)
                    Shader.EnableKeyword(shaderKeyword);
            }

            if (keywords.disabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.disabledKeywords)
                {
                    if (Shader.IsKeywordEnabled(shaderKeyword))
                        Shader.DisableKeyword(shaderKeyword);
                }
            }
        }

        internal class BuiltInRendering
        {
            /// <summary>
            /// The list of [CameraEvent](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.html)s
            /// to add to the [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html)
            /// when rendering before opaques.
            /// </summary>
            static readonly CameraEvent[] s_DefaultBeforeOpaqueCameraEvents = {
                CameraEvent.BeforeForwardOpaque,
                CameraEvent.BeforeGBuffer
            };

            /// <summary>
            /// The list of [CameraEvent](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.html)s
            /// to add to the [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html)
            /// when rendering after opaques.
            /// </summary>
            static readonly CameraEvent[] s_DefaultAfterOpaqueCameraEvents = {
                CameraEvent.AfterForwardOpaque,
                CameraEvent.AfterGBuffer
            };

            /// <summary>
            /// The list of [CameraEvent](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.html)s
            /// to add to the [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html).
            /// By default, it will select either <see cref="s_DefaultBeforeOpaqueCameraEvents"/> or <see cref="s_DefaultAfterOpaqueCameraEvents"/>
            /// depending on the value of <paramref name="commandBufferRenderOrderState"/>.
            ///
            /// In the case where Before Opaques rendering has been selected it will return:
            ///
            /// [BeforeForwardOpaque](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.BeforeForwardOpaque.html)
            /// and
            /// [BeforeGBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.BeforeGBuffer.html)}.
            ///
            /// In the case where After Opaques rendering has been selected it will return:
            ///
            /// [AfterForwardOpaque](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.AfterForwardOpaque.html)
            /// and
            /// [AfterGBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.AfterGBuffer.html)}.
            ///
            /// Override to use different camera events.
            /// </summary>
            internal static IEnumerable<CameraEvent> GetBuiltInRenderingCameraEvents(XRCameraBackgroundRenderingMode commandBufferRenderOrderState)
            {
                return commandBufferRenderOrderState switch
                {
                    XRCameraBackgroundRenderingMode.BeforeOpaques => s_DefaultBeforeOpaqueCameraEvents,
                    XRCameraBackgroundRenderingMode.AfterOpaques => s_DefaultAfterOpaqueCameraEvents,
                    _ => default(IEnumerable<CameraEvent>)
                };
            }

            /// <summary>
            /// Adds the AR Camera Background <see cref="CommandBuffer"/> to the <see cref="builtInRenderingCameraEvents"/>.
            /// </summary>
            /// <param name="camera">The camera to which command buffer should be added.</param>
            /// <param name="cameraEvents">Camera events for which to add the command buffer.</param>
            /// <param name="commandBuffer">The command buffer to add to the camera events.</param>
            internal static void AddCommandBufferToCameraEvent(Camera camera, IEnumerable<CameraEvent> cameraEvents, CommandBuffer commandBuffer)
            {
                foreach (var cameraEvent in cameraEvents)
                {
                    camera.AddCommandBuffer(cameraEvent, commandBuffer);
                }
            }

            /// <summary>
            /// Removes the AR Camera Background <see cref="CommandBuffer"/> from the camera rendering events.
            /// </summary>
            /// <param name="camera">The camera from which command buffer should be removed.</param>
            /// <param name="cameraEvents">Camera events for which to remove the command buffer.</param>
            /// <param name="commandBuffer">The command buffer to remove from the camera events.</param>
            internal static void RemoveCommandBufferFromCameraEvents(Camera camera, IEnumerable<CameraEvent> cameraEvents, CommandBuffer commandBuffer)
            {
                foreach (var cameraEvent in cameraEvents)
                {
                    camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
                }
            }
        }
    }
}
