#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEditor.XR.ARFoundation
{
    static class AREditorAnalytics
    {
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        public static readonly ARUsageAnalyticsEvent arUsageAnalyticsEvent = new();

        [InitializeOnLoadMethod]
        static void SetupAndRegister()
        {
            // The check for whether analytics is enabled or not is already done
            // by the Editor Analytics API.
            arUsageAnalyticsEvent.Register();
        }
    }
}
