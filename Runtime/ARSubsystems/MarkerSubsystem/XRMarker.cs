using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents a marker detected by the marker subsystem.
    /// </summary>
    public struct XRMarker : ITrackable, IEquatable<XRMarker>
    {
        /// <summary>
        /// The unique identifier for this marker.
        /// </summary>
        public TrackableId trackableId { get; }

        /// <summary>
        /// The pose of the marker in the session space.
        /// </summary>
        /// <remarks>
        /// The `Pose` is oriented so that its local positive Y-axis (up) points directly away from the front face of
        /// the marker, perpendicular to its surface.
        /// </remarks>
        public Pose pose { get; }

        /// <summary>
        /// The current tracking state of the marker.
        /// </summary>
        public TrackingState trackingState { get; }

        /// <summary>
        /// A native pointer to additional provider-specific data. May be <see cref="IntPtr.Zero"/>.
        /// </summary>
        public IntPtr nativePtr { get; }

        /// <summary>
        /// The logical size of the detected marker, in meters.
        /// </summary>
        /// <remarks>
        /// This value represents the width and height of the marker. The X component (size.x) represents the width and
        /// corresponds to the marker's local X-axis. The Y component (size.y) represents the height and corresponds to the
        /// marker's local Z-axis.
        /// </remarks>
        public Vector2 size { get; }

        /// <summary>
        /// The type of marker that was detected.
        /// </summary>
        public XRMarkerType markerType { get; }

        /// <summary>
        /// If <see cref="markerType"/> is <see cref="XRMarkerType.ArUco"/> or <see cref="XRMarkerType.AprilTag"/>,
        /// this property gets the integer ID encoded by the marker. Otherwise, `0`.
        /// </summary>
        public int markerId { get; }

        /// <summary>
        /// The marker's data buffer.
        /// </summary>
        /// <remarks>
        /// Only valid if <see cref="markerType"/> is <see cref="XRMarkerType.QRCode"/> or <see cref="XRMarkerType.MicroQRCode"/>.
        ///
        /// To safely access the encoded data, you should first check the <see cref="XRSpatialBuffer.bufferType"/> of this buffer.
        /// When the runtime successfully decodes a marker's encoded data, it sets the `bufferType` to either
        /// <see cref="XRSpatialBufferType.String"/> or <see cref="XRSpatialBufferType.Uint8"/> and assigns a valid
        /// <see cref="XRSpatialBuffer.bufferId"/>. If the `bufferType` is one of these values, you can then use
        /// <see cref="XRMarkerSubsystem.TryGetStringData"/> or
        /// <see cref="XRMarkerSubsystem.TryGetBytesData(UnityEngine.XR.ARSubsystems.XRSpatialBuffer)"/> to retrieve the data.
        /// </remarks>
        public XRSpatialBuffer dataBuffer { get; }

        /// <summary>
        /// The <see cref="TrackableId"/> of the parent of this tracked object.
        /// </summary>
        public TrackableId parentId { get; }

        /// <summary>
        /// Gets a default-initialized <see cref="XRMarker"/>.
        /// </summary>
        public static XRMarker defaultValue => new
        (
            TrackableId.invalidId,
            Pose.identity,
            TrackingState.None,
            IntPtr.Zero,
            Vector2.zero,
            XRMarkerType.None,
            0,
            default,
            TrackableId.invalidId
        );

        /// <summary>
        /// Constructs a new <see cref="XRMarker"/>.
        /// </summary>
        /// <param name="trackableId">The unique identifier for the marker.</param>
        /// <param name="pose">The marker's pose in session space.</param>
        /// <param name="trackingState">Tracking status.</param>
        /// <param name="nativePtr">Provider-specific native pointer, or <see cref="IntPtr.Zero"/>.</param>
        /// <param name="size">The detected marker size.</param>
        /// <param name="markerType">Type/classification of marker.</param>
        /// <param name="markerId">Integer identifier.</param>
        /// <param name="dataBuffer">Optional buffer for marker-specific data encoded data.</param>
        /// <param name="parentId">The <see cref="TrackableId"/> of the parent of this tracked object.</param>
        public XRMarker(
            TrackableId trackableId,
            Pose pose,
            TrackingState trackingState,
            IntPtr nativePtr,
            Vector2 size,
            XRMarkerType markerType,
            int markerId,
            XRSpatialBuffer dataBuffer,
            TrackableId parentId)
        {
            this.trackableId = trackableId;
            this.pose = pose;
            this.trackingState = trackingState;
            this.nativePtr = nativePtr;
            this.size = size;
            this.markerType = markerType;
            this.markerId = markerId;
            this.dataBuffer = dataBuffer;
            this.parentId = parentId;
        }

        /// <summary>
        /// Indicates whether every field in this <see cref="XRMarker"/> is equal to the corresponding field in
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="XRMarker"/> to compare with.</param>
        /// <returns>
        /// `true` if all properties match; otherwise, `false`.
        /// </returns>
        public bool Equals(XRMarker other)
        {
            return
                trackableId.Equals(other.trackableId) &&
                parentId.Equals(other.parentId) &&
                pose.Equals(other.pose) &&
                trackingState == other.trackingState &&
                nativePtr == other.nativePtr &&
                size == other.size &&
                markerType == other.markerType &&
                markerId == other.markerId &&
                dataBuffer.Equals(other.dataBuffer);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// `true` if the <paramref name="obj"/> is an `XRMarker` and all properties match; otherwise, `false`.
        /// </returns>
        public override bool Equals(object obj) => obj is XRMarker other && Equals(other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>The hash value of this marker.</returns>
        public override int GetHashCode()
        {
            var hashCode = HashCode.Combine(trackableId, parentId, pose, trackingState, nativePtr, size, markerType, markerId);
            return HashCode.Combine(hashCode, dataBuffer);
        }

        /// <summary>
        /// Determines whether two <see cref="XRMarker"/> instances have exactly the same property values.
        /// </summary>
        /// <param name="left">The first marker to compare.</param>
        /// <param name="right">The second marker to compare.</param>
        /// <returns>`true` if all properties match; otherwise, `false`.</returns>
        public static bool operator ==(XRMarker left, XRMarker right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="XRMarker"/> instances have different property values.
        /// </summary>
        /// <param name="left">The first marker to compare.</param>
        /// <param name="right">The second marker to compare.</param>
        /// <returns>`true` if any property differs; otherwise, `false`.</returns>
        public static bool operator !=(XRMarker left, XRMarker right) => !(left == right);
    }
}
