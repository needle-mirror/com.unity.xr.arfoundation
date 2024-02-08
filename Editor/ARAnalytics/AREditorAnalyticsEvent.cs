using System;
using Unity.XR.CoreUtils.Editor.Analytics;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.XR.ARAnalytics
{
    /// <typeparam name="TContext">The <see cref="Enum"/> type used to define options for the <see cref="AREditorAnalyticsEvent{TContext}.Payload.eventName"/> field within the data payload.</typeparam>
    abstract class AREditorAnalyticsEvent<TContext> : EditorAnalyticsEvent<AREditorAnalyticsEvent<TContext>.Payload> where TContext : Enum
    {
        internal AREditorAnalyticsEvent(string eventName, int eventVersion = ARAnalyticsConstants.defaultVersion) :
            base(eventName, eventVersion) { }

        protected override AnalyticsResult SendToAnalyticsServer(Payload payload) => EditorAnalytics.SendAnalytic(this);
        protected override AnalyticsResult RegisterWithAnalyticsServer() => AnalyticsResult.Ok;

        /// <summary>
        /// Maintains common data properties sent with AR analytics events.
        /// </summary>
        internal abstract class Payload : IAnalytic.IData
        {
            /// <summary>
            /// The contextual event name used to further classify the event within its analytics data table.
            /// Not to be confused with <see cref="AREditorAnalyticsEvent{TContext}.EventName"/>.
            /// </summary>
            [SerializeField]
            protected string eventName;

            internal Payload(TContext eventContext)
            {
                eventName = eventContext.ToString();
            }
        }
    }
}
