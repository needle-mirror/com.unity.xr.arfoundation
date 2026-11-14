using System;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// A utilities class for containing universally useful Rendering operations and information
    /// for rendering AR.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    public static partial class ARRenderingUtils
#else
    public static class ARRenderingUtils
#endif
    {
        /// <summary>
        /// (Obsolete) Whether to use the legacy rendering pipeline. The Built-in Render Pipeline is not supported in
        /// Unity 7000.0, so AR Foundation no longer uses this property.
        /// </summary>
        /// <value>
        /// <c>true</c> if no Scriptable Render Pipeline asset is active. Otherwise, <c>false</c>.
        /// </value>
        [Obsolete("The Built-in Render Pipeline is not supported in Unity 7000.0, and AR Foundation no longer uses this property. It will be removed in a future release. To check whether a Scriptable Render Pipeline is active, use GraphicsSettings.currentRenderPipeline instead.", true)]
        public static bool useLegacyRenderPipeline => GraphicsSettings.currentRenderPipeline == null;
    }
}
