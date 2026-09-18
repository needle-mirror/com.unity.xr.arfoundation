using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines a fluent API for constructing an <see cref="XRTrackedImage"/>.
    /// </summary>
    public class XRTrackedImageBuilder
    {
        TrackableId m_TrackableId;
        Guid m_SourceImageId;
        Pose m_Pose;
        Vector2 m_Size;
        TrackingState m_TrackingState;
        IntPtr m_NativePtr;
        TrackableId m_ParentId;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public XRTrackedImageBuilder()
        {
            Reset();
        }

        /// <summary>
        /// Initialize all fields using data from the given tracked image.
        /// </summary>
        /// <param name="trackedImage">The tracked image.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder FromTrackedImage(XRTrackedImage trackedImage)
        {
            m_TrackableId = trackedImage.trackableId;
            m_SourceImageId = trackedImage.sourceImageId;
            m_Pose = trackedImage.pose;
            m_Size = trackedImage.size;
            m_TrackingState = trackedImage.trackingState;
            m_NativePtr = trackedImage.nativePtr;
            m_ParentId = trackedImage.parentId;
            return this;
        }

        /// <summary>
        /// Reset the instance to default values.
        /// </summary>
        public void Reset()
        {
            m_TrackableId = TrackableId.invalidId;
            m_SourceImageId = Guid.Empty;
            m_Pose = Pose.identity;
            m_Size = Vector2.zero;
            m_TrackingState = TrackingState.None;
            m_NativePtr = IntPtr.Zero;
            m_ParentId = TrackableId.invalidId;
        }

        /// <summary>
        /// Set the trackable ID for built `XRTrackedImage` instances.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder WithTrackableId(TrackableId trackableId)
        {
            m_TrackableId = trackableId;
            return this;
        }

        /// <summary>
        /// Set the source image ID for built `XRTrackedImage` instances.
        /// </summary>
        /// <param name="sourceImageId">The GUID of the reference image that this tracked image was detected from.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder WithSourceImageId(Guid sourceImageId)
        {
            m_SourceImageId = sourceImageId;
            return this;
        }

        /// <summary>
        /// Set the pose for built `XRTrackedImage` instances.
        /// </summary>
        /// <param name="pose">The pose.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder WithPose(Pose pose)
        {
            m_Pose = pose;
            return this;
        }

        /// <summary>
        /// Set the size for built `XRTrackedImage` instances.
        /// </summary>
        /// <param name="size">The size, in meters, of the detected image.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder WithSize(Vector2 size)
        {
            m_Size = size;
            return this;
        }

        /// <summary>
        /// Set the tracking state for built `XRTrackedImage` instances.
        /// </summary>
        /// <param name="trackingState">The tracking state.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder WithTrackingState(TrackingState trackingState)
        {
            m_TrackingState = trackingState;
            return this;
        }

        /// <summary>
        /// Set the native pointer for built `XRTrackedImage` instances.
        /// </summary>
        /// <param name="nativePtr">The native pointer.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder WithNativePtr(IntPtr nativePtr)
        {
            m_NativePtr = nativePtr;
            return this;
        }

        /// <summary>
        /// Set the parent ID for built `XRTrackedImage` instances.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <returns>This instance.</returns>
        public XRTrackedImageBuilder WithParentId(TrackableId parentId)
        {
            m_ParentId = parentId;
            return this;
        }

        /// <summary>
        /// Build an output `XRTrackedImage` using all this builder's parameter values.
        /// </summary>
        /// <returns>The built `XRTrackedImage`.</returns>
        public XRTrackedImage Build()
        {
            return new XRTrackedImage(
                m_TrackableId,
                m_SourceImageId,
                m_Pose,
                m_Size,
                m_TrackingState,
                m_NativePtr,
                m_ParentId);
        }
    }
}
