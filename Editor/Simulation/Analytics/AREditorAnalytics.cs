using UnityEditor.XR.ARAnalytics;

namespace UnityEditor.XR.Simulation
{
    static class AREditorAnalytics
    {
        const string k_UITableName = "xrsimulation_ui";
        const string k_SessionTableName = "xrsimulation_session";

        public static readonly AREditorAnalyticsEvent<SimulationUIAnalyticsArgs> simulationUIAnalyticsEvent = new(k_UITableName);
        public static readonly AREditorAnalyticsEvent<SimulationSessionAnalyticsArgs> simulationSessionAnalyticsEvent = new(k_SessionTableName);

        [InitializeOnLoadMethod]
        static void SetupAndRegister()
        {
            // The check for whether analytics is enabled or not is already done
            // by the Editor Analytics API.
            simulationUIAnalyticsEvent.Register();
            simulationSessionAnalyticsEvent.Register();
        }
    }
}
