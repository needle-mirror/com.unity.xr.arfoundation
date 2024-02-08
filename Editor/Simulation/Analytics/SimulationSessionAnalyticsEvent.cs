using System;
using UnityEditor.XR.ARAnalytics;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.XR.Simulation
{
    [AnalyticInfo(k_EventName, ARAnalyticsConstants.arFoundationVendorKey, k_EventVersion, ARAnalyticsConstants.defaultMaxEventsPerHour, ARAnalyticsConstants.defaultMaxElementCount)]
    class SimulationSessionAnalyticsEvent : AREditorAnalyticsEvent<SimulationSessionAnalyticsEvent.Context>
    {
        const string k_EventName = "xrsimulation_session";
        const int k_EventVersion = 2;

        internal SimulationSessionAnalyticsEvent() : base(k_EventName, k_EventVersion) { }

        [Serializable]
        internal class EventPayload : Payload
        {
            [SerializeField]
            private string environmentGuid;

            [SerializeField]
            private string[] arSubsystemsInfo;

            [SerializeField]
            private double duration;

            internal EventPayload(
                Context eventName,
                GUID environmentGuid,
                string[] arSubsystemsInfo,
                double duration) : base(eventName)
            {
                this.environmentGuid = environmentGuid.ToString();
                this.arSubsystemsInfo = arSubsystemsInfo;
                this.duration = duration;
            }
        }

        internal enum Context
        {
            SimulationEnded
        }
    }
}
