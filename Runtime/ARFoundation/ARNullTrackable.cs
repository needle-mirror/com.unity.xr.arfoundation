using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A null implementation of `ARTrackable`, used if a trackable reports a parent ID that doesn't correspond to
    /// any other trackable.
    /// </summary>
    public class ARNullTrackable : ARTrackable
    {
        internal TrackableId trackableIdInternal;

        /// <inheritdoc/>
        public override TrackableId trackableId => trackableIdInternal;

        /// <inheritdoc/>
        public override Pose pose => Pose.identity;

        /// <inheritdoc/>
        public override TrackingState trackingState => TrackingState.None;

        /// <inheritdoc/>
        public override IntPtr nativePtr => IntPtr.Zero;
    }
}
