using System;

#if OPENXR_PLUGIN_1_16_0_PRE1_OR_NEWER
using UnityEngine.XR.OpenXR.NativeTypes;
#endif

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines a fluent API for constructing an <see cref="XRPlane"/>.
    /// </summary>
    public class XRPlaneBuilder
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
        public XRPlaneBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Copies values from an XRPlane to this instance.
        /// </summary>
        /// <param name="plane">The XRPlane values to copy to this instance.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder FromPlane(XRPlane plane)
        {
            m_TrackableId = plane.trackableId;
            m_Pose = plane.pose;
            m_TrackingState = plane.trackingState;
            m_NativePtr = plane.nativePtr;
            m_Size = plane.size;
            m_Alignment = plane.alignment;
            m_Center = plane.center;
            m_Classifications = plane.classifications;
            m_ParentId = plane.parentId;
            return this;
        }

        /// <summary>
        /// Copies values from an XRPlane to this instance.
        /// </summary>
        /// <param name="plane">The XRPlane values to copy to this instance.</param>
        /// <returns>This instance.</returns>
        [Obsolete("FromBoundedPlane has been renamed in AR Foundation version 7.0. Use FromPlane instead. (UnityUpgradable) -> FromPlane(*)", true)]
        public XRPlaneBuilder FromBoundedPlane(XRPlane plane) => FromPlane(plane);

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
        /// Set the trackable ID for built `XRPlane` instances.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithTrackableId(TrackableId trackableId)
        {
            m_TrackableId = trackableId;
            return this;
        }

        /// <summary>
        /// Set the subsumed by ID for built `XRPlane` instances.
        /// </summary>
        /// <param name="subsumedById">The subsumed by ID.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithSubsumedById(TrackableId subsumedById)
        {
            m_SubsumedById = subsumedById;
            return this;
        }

        /// <summary>
        /// Set the center for built `XRPlane` instances.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithCenter(Vector2 center)
        {
            m_Center = center;
            return this;
        }

        /// <summary>
        /// Set the pose for built `XRPlane` instances.
        /// </summary>
        /// <param name="pose">The pose.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithPose(Pose pose)
        {
            m_Pose = pose;
            return this;
        }

#if OPENXR_PLUGIN_1_16_0_PRE1_OR_NEWER
        /// <summary>
        /// Set the pose for built `XRPlane` instances.
        /// </summary>
        /// <param name="pose">The session space pose.</param>
        /// <remarks>
        /// The session space pose passed in will be transformed to Unity world space by flipping the Z component for
        /// the position and flipping the X and Y components of the rotation <see cref="Quaternion"/> and rotating it
        /// -90 degrees around the X-axis.
        /// </remarks>
        /// <returns>This instance</returns>
        public XRPlaneBuilder WithPose(XrPosef pose)
        {
            var position = pose.Position.AsVector3();
            var rotation = pose.Orientation.AsQuaternion();
            // Reorient so the Y-axis points out of the face of the bounded plane
            rotation *= Quaternion.Euler(-90f, 0f, 0f);
            m_Pose = new Pose(position, rotation);
            return this;
        }
#endif

        /// <summary>
        /// Set the size for built `XRPlane` instances.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithSize(Vector2 size)
        {
            m_Size = size;
            return this;
        }

        /// <summary>
        /// Set the plane alignment for built `XRPlane` instances.
        /// </summary>
        /// <param name="alignment">The plane alignment.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithAlignment(PlaneAlignment alignment)
        {
            m_Alignment = alignment;
            return this;
        }

        /// <summary>
        /// Set the tracking state for built `XRPlane` instances.
        /// </summary>
        /// <param name="trackingState">The tracking state.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithTrackingState(TrackingState trackingState)
        {
            m_TrackingState = trackingState;
            return this;
        }

        /// <summary>
        /// Set the native pointer for built `XRPlane` instances.
        /// </summary>
        /// <param name="nativePtr">The native pointer.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithNativePtr(IntPtr nativePtr)
        {
            m_NativePtr = nativePtr;
            return this;
        }

        /// <summary>
        /// Set the classifications for built `XRPlane` instances.
        /// </summary>
        /// <param name="classifications">The classifications.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithClassifications(PlaneClassifications classifications)
        {
            m_Classifications = classifications;
            return this;
        }

        /// <summary>
        /// Set the parent ID for built `XRPlane` instances.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>This instance.</returns>
        public XRPlaneBuilder WithParentId(TrackableId parentId)
        {
            m_ParentId = parentId;
            return this;
        }

        /// <summary>
        /// Build an output `XRPlane` using all this builder's parameter values.
        /// </summary>
        /// <returns>The built `XRPlane`.</returns>
        public XRPlane Build()
        {
            return new XRPlane(
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
