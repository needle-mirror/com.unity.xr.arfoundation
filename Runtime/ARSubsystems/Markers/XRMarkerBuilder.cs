using System;

#if OPENXR_PLUGIN_1_16_0_PRE1_OR_NEWER
using UnityEngine.XR.OpenXR.NativeTypes;
#endif

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines a fluent API for constructing an <see cref="XRMarker"/>.
    /// </summary>
    public class XRMarkerBuilder
    {
        TrackableId m_TrackableId;
        Pose m_Pose;
        TrackingState m_TrackingState;
        IntPtr m_NativePtr;
        Vector2 m_Size;
        XRMarkerType m_MarkerType;
        uint m_MarkerId;
        XRSpatialBuffer m_DataBuffer;
        TrackableId m_ParentId;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public XRMarkerBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Copies values from another marker to this instance.
        /// </summary>
        /// <param name="marker">The marker values to copy to this instance.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder FromMarker(XRMarker marker)
        {
            m_TrackableId = marker.trackableId;
            m_Pose = marker.pose;
            m_TrackingState = marker.trackingState;
            m_NativePtr =  marker.nativePtr;
            m_Size =  marker.size;
            m_MarkerType = marker.markerType;
            m_MarkerId =  marker.markerId;
            m_DataBuffer = marker.dataBuffer;
            m_ParentId  = marker.parentId;
            return this;
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
            m_Size = Vector2.zero;
            m_MarkerType = XRMarkerType.None;
            m_MarkerId = 0;
            m_DataBuffer = default;
            m_ParentId = TrackableId.invalidId;
        }

        /// <summary>
        /// Set the trackable ID for built `XRMarker` instances.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithTrackableId(TrackableId trackableId)
        {
            m_TrackableId = trackableId;
            return this;
        }

        /// <summary>
        /// Set the pose for built `XRMarker` instances.
        /// </summary>
        /// <param name="pose">The Unity world space pose.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithPose(Pose pose)
        {
            m_Pose = pose;
            return this;
        }

#if OPENXR_PLUGIN_1_16_0_PRE1_OR_NEWER
        /// <summary>
        /// Set the pose for built `XRMarker` instances.
        /// </summary>
        /// <param name="pose">The session space pose.</param>
        /// <remarks>
        /// The session space pose passed in will be transformed to Unity world space by flipping the Z component for
        /// the position and flipping the X and Y components of the rotation <see cref="Quaternion"/> and rotating it
        /// -90 degrees around the X-axis.
        /// </remarks>
        /// <returns>This instance</returns>
        public XRMarkerBuilder WithPose(XrPosef pose)
        {
            var position = pose.Position.AsVector3();
            var rotation = pose.Orientation.AsQuaternion();
            // Reorient so the Y-axis points out of the face of the marker
            rotation *= Quaternion.Euler(-90f, 0f, 0f);
            m_Pose = new Pose(position, rotation);
            return this;
        }
#endif

        /// <summary>
        /// Set the tracking state for built `XRMarker` instances.
        /// </summary>
        /// <param name="trackingState">The tracking state.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithTrackingState(TrackingState trackingState)
        {
            m_TrackingState = trackingState;
            return this;
        }

        /// <summary>
        /// Set the native pointer for built `XRMarker` instances.
        /// </summary>
        /// <param name="nativePtr">The native pointer.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithNativePtr(IntPtr nativePtr)
        {
            m_NativePtr = nativePtr;
            return this;
        }

        /// <summary>
        /// Set the size for built `XRMarker` instances.
        /// </summary>
        /// <param name="size">The size of the marker.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithSize(Vector2 size)
        {
            m_Size = size;
            return this;
        }

        /// <summary>
        /// Set the marker type for built `XRMarker` instances.
        /// </summary>
        /// <param name="markerType">The marker type of the marker.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithMarkerType(XRMarkerType markerType)
        {
            m_MarkerType = markerType;
            return this;
        }

        /// <summary>
        /// Set the marker ID for built `XRMarker` instances.
        /// </summary>
        /// <param name="markerId">The ID of the marker.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithMarkerId(uint markerId)
        {
            m_MarkerId = markerId;
            return this;
        }

        /// <summary>
        /// Set the data buffer for built `XrMarker` instances.
        /// </summary>
        /// <param name="dataBuffer">The data buffer of the marker.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithDataBuffer(XRSpatialBuffer dataBuffer)
        {
            m_DataBuffer = dataBuffer;
            return this;
        }

        /// <summary>
        /// Set the parent ID for built `XRMarker` instances.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>This instance.</returns>
        public XRMarkerBuilder WithParentId(TrackableId parentId)
        {
            m_ParentId = parentId;
            return this;
        }

        /// <summary>
        /// Build the output `XRMarker` using all this builder's input parameter values.
        /// </summary>
        /// <returns>The built `XRMarker`.</returns>
        public XRMarker Build()
        {
            return new XRMarker(
                m_TrackableId,
                m_Pose,
                m_TrackingState,
                m_NativePtr,
                m_Size,
                m_MarkerType,
                m_MarkerId,
                m_DataBuffer,
                m_ParentId);
        }
    }
}
