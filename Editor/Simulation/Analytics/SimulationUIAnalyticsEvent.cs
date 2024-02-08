using System;
using UnityEditor.XR.ARAnalytics;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.XR.Simulation
{
    [AnalyticInfo(k_EventName, ARAnalyticsConstants.arFoundationVendorKey, k_EventVersion, ARAnalyticsConstants.defaultMaxEventsPerHour, ARAnalyticsConstants.defaultMaxElementCount)]
    class SimulationUIAnalyticsEvent : AREditorAnalyticsEvent<SimulationUIAnalyticsEvent.Context>
    {
        const string k_EventName = "xrsimulation_ui";
        const int k_EventVersion = 1;

        internal SimulationUIAnalyticsEvent() : base(k_EventName, k_EventVersion) { }

        [Serializable]
        internal class EventPayload : Payload
        {
            [SerializeField]
            private WindowUsed windowUsed;

            [SerializeField]
            private string environmentGuid;

            internal EventPayload(
                Context eventName,
                WindowUsed windowUsed = null,
                GUID? environmentGuid = null) : base(eventName)
            {
                this.windowUsed = windowUsed;
                this.environmentGuid = environmentGuid.ToString();
            }
        }

        internal enum Context
        {
            WindowUsed,
            EnvironmentCycle
        }

        [Serializable]
        internal class WindowUsed
        {
            internal string name;
            internal bool isActive;
        }
    }
}
