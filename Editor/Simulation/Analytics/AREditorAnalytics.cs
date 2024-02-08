using UnityEditor.XR.ARAnalytics;
using UnityEngine.Analytics;

namespace UnityEditor.XR.Simulation
{
    static class AREditorAnalytics
    {
        public static readonly SimulationUIAnalyticsEvent simulationUIAnalyticsEvent = new();
        public static readonly SimulationSessionAnalyticsEvent simulationSessionAnalyticsEvent = new();

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
