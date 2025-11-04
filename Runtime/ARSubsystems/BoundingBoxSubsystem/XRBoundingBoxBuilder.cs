using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines a fluent API for constructing an <see cref="XRBoundingBox"/>.
    /// </summary>
    class XRBoundingBoxBuilder
    {
        TrackableId m_TrackableId;
        Pose m_Pose;
        Vector3 m_Size;
        TrackingState m_TrackingState;
        IntPtr m_NativePtr;
        BoundingBoxClassifications m_Classifications;
        TrackableId m_ParentId;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public XRBoundingBoxBuilder()
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
            m_Size = Vector3.zero;
            m_TrackingState = TrackingState.None;
            m_NativePtr = IntPtr.Zero;
            m_Classifications = BoundingBoxClassifications.None;
            m_ParentId = TrackableId.invalidId;
        }

        /// <summary>
        /// Set the trackable ID for built `XRBoundingBox` instances.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <returns>This instance.</returns>
        public XRBoundingBoxBuilder WithTrackableId(TrackableId trackableId)
        {
            m_TrackableId = trackableId;
            return this;
        }

        /// <summary>
        /// Set the pose for built `XRBoundingBox` instances.
        /// </summary>
        /// <param name="pose">The pose.</param>
        /// <returns>This instance.</returns>
        public XRBoundingBoxBuilder WithPose(Pose pose)
        {
            m_Pose = pose;
            return this;
        }

        /// <summary>
        /// Set the size for build `XRBoundingBox` instances.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>This instance.</returns>
        public XRBoundingBoxBuilder WithSize(Vector3 size)
        {
            m_Size = size;
            return this;
        }

        /// <summary>
        /// Set the tracking state for built `XRBoundingBox` instances.
        /// </summary>
        /// <param name="trackingState">The tracking state.</param>
        /// <returns>This instance.</returns>
        public XRBoundingBoxBuilder WithTrackingState(TrackingState trackingState)
        {
            m_TrackingState = trackingState;
            return this;
        }

        /// <summary>
        /// Set the native pointer for built `XRBoundingBox` instances.
        /// </summary>
        /// <param name="nativePtr">The native pointer.</param>
        /// <returns>This instance.</returns>
        public XRBoundingBoxBuilder WithNativePtr(IntPtr nativePtr)
        {
            m_NativePtr = nativePtr;
            return this;
        }

        /// <summary>
        /// Set the classifications for built `XRBoundingBox` instances.
        /// </summary>
        /// <param name="classifications">The classifications.</param>
        /// <returns>This instance.</returns>
        public XRBoundingBoxBuilder WithClassifications(BoundingBoxClassifications classifications)
        {
            m_Classifications = classifications;
            return this;
        }

        /// <summary>
        /// Set the parent ID for built `XRBoundingBox` instances.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>This instance.</returns>
        public XRBoundingBoxBuilder WithParentId(TrackableId parentId)
        {
            m_ParentId = parentId;
            return this;
        }

        /// <summary>
        /// Build an output `XRBoundingBox` using all this builder's parameter values.
        /// </summary>
        /// <returns>The built `XRBoundingBox`.</returns>
        public XRBoundingBox Build()
        {
            return new XRBoundingBox(
                m_TrackableId,
                m_Pose,
                m_Size,
                m_TrackingState,
                m_Classifications,
                m_NativePtr,
                m_ParentId);
        }
    }
}
