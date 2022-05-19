using UnityEditor.XR.ARAnalytics;

namespace UnityEditor.XR.ARFoundation
{
    static class AREditorAnalytics
    {
        const string k_UsageTableName = "arfoundation_usage";

        public static readonly AREditorAnalyticsEvent<ARUsageAnalyticsArgs> arUsageAnalyticEvent = new(k_UsageTableName);

        [InitializeOnLoadMethod]
        static void SetupAndRegister()
        {
            // The check for whether analytics is enabled or not is already done
            // by the Editor Analytics API.
            arUsageAnalyticEvent.Register();
        }
    }
}
