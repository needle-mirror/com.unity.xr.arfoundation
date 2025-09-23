using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class BoundedPlaneBuilder
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

        internal BoundedPlaneBuilder()
        {
            Reset();
        }

        internal void Reset()
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

        internal BoundedPlaneBuilder WithTrackableId(TrackableId trackableId)
        {
            m_TrackableId = trackableId;
            return this;
        }

        internal BoundedPlaneBuilder WithSubsumedById(TrackableId subsumedById)
        {
            m_SubsumedById = subsumedById;
            return this;
        }

        internal BoundedPlaneBuilder WithCenter(Vector2 center)
        {
            m_Center = center;
            return this;
        }

        internal BoundedPlaneBuilder WithPose(Pose pose)
        {
            m_Pose = pose;
            return this;
        }

        internal BoundedPlaneBuilder WithSize(Vector2 size)
        {
            m_Size = size;
            return this;
        }

        internal BoundedPlaneBuilder WithAlignment(PlaneAlignment alignment)
        {
            m_Alignment = alignment;
            return this;
        }

        internal BoundedPlaneBuilder WithTrackingState(TrackingState trackingState)
        {
            m_TrackingState = trackingState;
            return this;
        }

        internal BoundedPlaneBuilder WithNativePtr(IntPtr nativePtr)
        {
            m_NativePtr = nativePtr;
            return this;
        }

        internal BoundedPlaneBuilder WithClassifications(PlaneClassifications classifications)
        {
            m_Classifications = classifications;
            return this;
        }

        internal BoundedPlaneBuilder WithParentId(TrackableId parentId)
        {
            m_ParentId = parentId;
            return this;
        }

        internal BoundedPlane Build()
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
