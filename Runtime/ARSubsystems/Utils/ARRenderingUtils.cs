#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
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
#if UNITY_6000_5_OR_NEWER
        [AutoStaticsCleanup]
#endif
        static bool? s_UseLegacyRenderPipeline;

        /// <summary>
        /// Whether to use the legacy rendering pipeline.
        /// </summary>
        /// <value>
        /// <c>true</c> if the legacy render pipeline is in use. Otherwise, <c>false</c>.
        /// </value>
        public static bool useLegacyRenderPipeline => s_UseLegacyRenderPipeline ??= GraphicsSettings.currentRenderPipeline == null;
    }
}
