using UnityEditor.XR.ARAnalytics;

namespace UnityEditor.XR.ARFoundation
{
    static class AREditorAnalytics
    {
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
