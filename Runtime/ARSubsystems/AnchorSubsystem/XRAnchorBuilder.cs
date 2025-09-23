using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines a fluent API for constructing an <see cref="XRAnchor"/>.
    /// </summary>
    public class XRAnchorBuilder
    {
        TrackableId m_TrackableId;
        Pose m_Pose;
        TrackingState m_TrackingState;
        IntPtr m_NativePtr;
        Guid m_SessionId;
        TrackableId m_ParentId;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public XRAnchorBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Reset the instance to default values.
        /// </summary>
        public void Reset()
        {
            m_TrackableId = TrackableId.invalidId;
            m_Pose = Pose.identity;
            m_TrackingState = TrackingState.None;
            m_NativePtr = IntPtr.Zero;
            m_SessionId = Guid.Empty;
            m_ParentId = TrackableId.invalidId;
        }

        /// <summary>
        /// Set the trackable ID for built `XRAnchor` instances.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <returns>This instance.</returns>
        public XRAnchorBuilder WithTrackableId(TrackableId trackableId)
        {
            m_TrackableId = trackableId;
            return this;
        }

        /// <summary>
        /// Set the pose for built `XRAnchor` instances.
        /// </summary>
        /// <param name="pose">The pose.</param>
        /// <returns>This instance.</returns>
        public XRAnchorBuilder WithPose(Pose pose)
        {
            m_Pose = pose;
            return this;
        }

        /// <summary>
        /// Set the tracking state for built `XRAnchor` instances.
        /// </summary>
        /// <param name="trackingState">The tracking state.</param>
        /// <returns>This instance.</returns>
        public XRAnchorBuilder WithTrackingState(TrackingState trackingState)
        {
            m_TrackingState = trackingState;
            return this;
        }

        /// <summary>
        /// Set the native pointer for built `XRAnchor` instances.
        /// </summary>
        /// <param name="nativePtr">The native pointer.</param>
        /// <returns>This instance.</returns>
        public XRAnchorBuilder WithNativePtr(IntPtr nativePtr)
        {
            m_NativePtr = nativePtr;
            return this;
        }

        /// <summary>
        /// Set the session ID for built `XRAnchor` instances.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        /// <returns>This instance.</returns>
        public XRAnchorBuilder WithSessionId(Guid sessionId)
        {
            m_SessionId = sessionId;
            return this;
        }

        /// <summary>
        /// Set the parent ID for built `XRAnchor` instances.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>This instance.</returns>
        public XRAnchorBuilder WithParentId(TrackableId parentId)
        {
            m_ParentId = parentId;
            return this;
        }

        /// <summary>
        /// Build the output `XRAnchor` using all this builder's input parameter values.
        /// </summary>
        /// <returns>The built `XRAnchor`.</returns>
        public XRAnchor Build()
        {
            return new XRAnchor(
                m_TrackableId,
                m_Pose,
                m_TrackingState,
                m_NativePtr,
                m_SessionId,
                m_ParentId);
        }
    }
}
