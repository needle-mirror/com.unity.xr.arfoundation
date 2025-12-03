using System;
using UnityEngine;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines a fluent API for constructing a <see cref="BoundedPlane"/>.
    /// </summary>
    public class BoundedPlaneBuilder
    {
        TrackableId m_TrackableId;
        TrackableId m_SubsumedById;
        Vector2 m_Center;
        Pose m_Pose;
        Vector2 m_Size;
        PlaneAlignment m_Alignment;
        TrackingState m_TrackingState;
        IntPtr m_NativePtr;
        PlaneClassifications m_Classifications;
        TrackableId m_ParentId;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public BoundedPlaneBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Reset the instance to default values.
        /// </summary>
        public void Reset()
        {
            m_TrackableId = TrackableId.invalidId;
            m_SubsumedById = TrackableId.invalidId;
            m_Center = Vector2.zero;
            m_Pose = Pose.identity;
            m_Size = Vector2.zero;
            m_Alignment = PlaneAlignment.NotAxisAligned;
            m_TrackingState = TrackingState.Tracking;
            m_NativePtr = IntPtr.Zero;
            m_Classifications = PlaneClassifications.None;
            m_ParentId = TrackableId.invalidId;
        }

        /// <summary>
        /// Set the trackable ID for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithTrackableId(TrackableId trackableId)
        {
            m_TrackableId = trackableId;
            return this;
        }

        /// <summary>
        /// Set the subsumed by ID for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="subsumedById">The subsumed by ID.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithSubsumedById(TrackableId subsumedById)
        {
            m_SubsumedById = subsumedById;
            return this;
        }

        /// <summary>
        /// Set the center for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithCenter(Vector2 center)
        {
            m_Center = center;
            return this;
        }

        /// <summary>
        /// Set the pose for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="pose">The pose.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithPose(Pose pose)
        {
            m_Pose = pose;
            return this;
        }

        /// <summary>
        /// Set the size for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithSize(Vector2 size)
        {
            m_Size = size;
            return this;
        }

        /// <summary>
        /// Set the plane alignment for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="alignment">The plane alignment.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithAlignment(PlaneAlignment alignment)
        {
            m_Alignment = alignment;
            return this;
        }

        /// <summary>
        /// Set the tracking state for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="trackingState">The tracking state.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithTrackingState(TrackingState trackingState)
        {
            m_TrackingState = trackingState;
            return this;
        }

        /// <summary>
        /// Set the native pointer for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="nativePtr">The native pointer.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithNativePtr(IntPtr nativePtr)
        {
            m_NativePtr = nativePtr;
            return this;
        }

        /// <summary>
        /// Set the classifications for build `BoundedPlane` instances.
        /// </summary>
        /// <param name="classifications">The classifications.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithClassifications(PlaneClassifications classifications)
        {
            m_Classifications = classifications;
            return this;
        }

        /// <summary>
        /// Set the parent ID for built `BoundedPlane` instances.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>This instance.</returns>
        public BoundedPlaneBuilder WithParentId(TrackableId parentId)
        {
            m_ParentId = parentId;
            return this;
        }

        /// <summary>
        /// Build an output `BoundedPlane` using all this builder's parameter values.
        /// </summary>
        /// <returns>The built `BoundedPlane`.</returns>
        public BoundedPlane Build()
        {
            return new BoundedPlane(
                m_TrackableId,
                m_SubsumedById,
                m_Pose,
                m_Center,
                m_Size,
                m_Alignment,
                m_TrackingState,
                m_NativePtr,
                m_Classifications,
                m_ParentId);
        }
    }
}
