using System;
using UnityEditor.XR.ARAnalytics;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.XR.ARFoundation
{
    [AnalyticInfo(k_EventName, ARAnalyticsConstants.arFoundationVendorKey, k_EventVersion, ARAnalyticsConstants.defaultMaxEventsPerHour, ARAnalyticsConstants.defaultMaxElementCount)]
    class ARUsageAnalyticsEvent : AREditorAnalyticsEvent<ARUsageAnalyticsEvent.Context>
    {
        const string k_EventName = "arfoundation_usage";
        const int k_EventVersion = 2;

        internal ARUsageAnalyticsEvent() : base(k_EventName, k_EventVersion) { }

        [Serializable]
        internal class EventPayload : Payload
        {
            /// <summary>
            /// A string containing the <see cref="GUID"/> of the build.
            /// </summary>
            [SerializeField]
            private string buildGuid;

            /// <summary>
            /// A string containing the <see cref="GUID"/> of the scene.
            /// </summary>
            [SerializeField]
            private string sceneGuid;

            /// <summary>
            /// The current target build platform.
            /// </summary>
            [SerializeField]
            private string targetPlatform;

            /// <summary>
            /// List of AR Managers in the scene specified by <see cref="sceneGuid"/>.
            /// </summary>
            [SerializeField]
            private ARManagerInfo[] arManagersInfo;

            internal EventPayload(
                Context eventName,
                GUID sceneGuid,
                ARManagerInfo[] arManagersInfo,
                GUID? buildGuid = null,
                BuildTarget? targetPlatform = null) : base(eventName)
            {
                this.buildGuid = buildGuid.ToString();
                this.sceneGuid = sceneGuid.ToString();
                this.targetPlatform = targetPlatform.ToString();
                this.arManagersInfo = arManagersInfo;
            }
        }

        [Serializable]
        internal struct ARManagerInfo
        {
            /// <summary>
            /// Name of the AR Manager.
            /// </summary>
            internal string name;

            /// <summary>
            ///  <see langword="true"/> if the AR Manager is active in the scene. Otherwise, <see langword="false"/>.
            /// </summary>
            internal bool active;
        }

        internal enum Context
        {
            SceneSave,
            SceneOpen,
            BuildPlayer
        }
    }
}
