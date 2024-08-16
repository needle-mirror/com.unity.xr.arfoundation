using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines the interface for a _trackable_. A trackable represents anything that can be detected and tracked
    /// in the physical environment.
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        /// The `TrackableId` associated with this trackable.
        /// </summary>
        /// <value>The `TrackableId`.</value>
        TrackableId trackableId { get; }

        /// <summary>
        /// The `Pose`, in session space, associated with this trackable.
        /// </summary>
        /// <value>The pose.</value>
        /// <example>
        /// <para>Some `ITrackable` implementations are MonoBehaviours, and you can use the Transform component of their
        /// GameObjects to access the position and rotation of a trackable in Unity world space. For other trackables
        /// without associated GameObjects, you can convert a session-space pose to Unity world space if needed by
        /// using the example code below, assuming the XR Origin has a uniform scale of (1, 1, 1):</para>
        /// <code>
        /// var origin = FindAnyObjectByType&lt;XROrigin&gt;().transform;
        /// var originPose = new Pose(origin.position, origin.rotation);
        /// var worldSpacePose = pose.GetTransformedBy(xrOriginPose);
        /// </code>
        /// </example>
        Pose pose { get; }

        /// <summary>
        /// The `TrackingState` associated with this trackable.
        /// </summary>
        /// <value>The `TrackingState`.</value>
        TrackingState trackingState { get; }

        /// <summary>
        /// The native pointer associated with this trackable.
        /// </summary>
        /// <value>The native pointer.</value>
        /// <remarks>
        /// The data pointed to by this pointer is defined by the provider plug-in implementation.
        /// Refer to the provider documentation for each platform your app supports to learn more about the value of
        /// this pointer on your target platform(s).
        /// </remarks>
        IntPtr nativePtr { get; }
    }
}
