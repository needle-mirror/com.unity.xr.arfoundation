using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The session-relative data associated with a 3D bounding box.
    /// </summary>
    /// <seealso cref="XRBoundingBoxSubsystem"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct XRBoundingBox : ITrackable, IEquatable<XRBoundingBox>
    {
        /// <summary>
        /// The <see cref="TrackableId"/> associated with this bounding box.
        /// </summary>
        public TrackableId trackableId { get; }

        /// <summary>
        /// The `Pose`, in session space, of the bounding box.
        /// </summary>
        public Pose pose { get; }

        /// <summary>
        /// The size (dimensions) of the bounding box in meters.
        /// </summary>
        public Vector3 size { get; }

        /// <summary>
        /// The <see cref="TrackingState"/> of the bounding box.
        /// </summary>
        public TrackingState trackingState { get; }

        /// <summary>
        /// A native pointer associated with this bounding box.
        /// The data pointed to by this pointer is implementation defined.
        /// </summary>
        public IntPtr nativePtr { get; }

        /// <summary>
        /// The classifications of this bounding box.
        /// </summary>
        public BoundingBoxClassifications classifications { get; }

        /// <summary>
        /// Constructs a new instance. `XRBoundingBox` objects are typically created by
        /// <see cref="XRBoundingBoxSubsystem.GetChanges(Unity.Collections.Allocator)">XRBoundingBoxSubsystem.GetChanges</see>.
        /// </summary>
        /// <param name="trackableId">The `TrackableId` associated with the bounding box.</param>
        /// <param name="pose">The pose describing the position and orientation of the bounding box.</param>
        /// <param name="size">The dimensions of the bounding box.</param>
        /// <param name="trackingState">The `TrackingState` describing how well the XR device is tracking the bounding box.</param>
        /// <param name="classifications">The BoundingBoxClassification assigned to the bounding box by the XR device.</param>
        /// <param name="nativePtr">The native pointer associated with the bounding box.</param>
        public XRBoundingBox(
            TrackableId trackableId,
            Pose pose,
            Vector3 size,
            TrackingState trackingState,
            BoundingBoxClassifications classifications,
            IntPtr nativePtr)
        {
            this.trackableId = trackableId;
            this.pose = pose;
            this.size = size;
            this.trackingState = trackingState;
            this.classifications = classifications;
            this.nativePtr = nativePtr;
        }

        /// <summary>
        /// Generates a new string that describes the bounding box's properties, suitable for debugging purposes.
        /// </summary>
        /// <returns>A string that describes the bounding box's properties.</returns>
        public override string ToString()
        {
            SharedStringBuilder.stringBuilder.AppendLine("Bounding Box:");
            SharedStringBuilder.stringBuilder.AppendLine("\ttrackableId: " + trackableId);
            SharedStringBuilder.stringBuilder.AppendLine("\tpose: " + pose);
            SharedStringBuilder.stringBuilder.AppendLine("\tsize: " + size);
            SharedStringBuilder.stringBuilder.AppendLine("\tclassifications: " + classifications);
            SharedStringBuilder.stringBuilder.AppendLine("\ttrackingState: " + trackingState);
            SharedStringBuilder.stringBuilder.Append("\tnativePtr: ");
            SharedStringBuilder.stringBuilder.Append("" + nativePtr.ToInt64(), 0, 16);
            SharedStringBuilder.stringBuilder.Append("\n");
            string tempString = SharedStringBuilder.stringBuilder.ToString();
            SharedStringBuilder.stringBuilder.Clear();
            return tempString;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="XRBoundingBox"/> and
        /// <see cref="Equals(XRBoundingBox)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(object obj) => (obj is XRBoundingBox other) && Equals(other);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = trackableId.GetHashCode();
                hashCode = (hashCode * 486187739) + pose.GetHashCode();
                hashCode = (hashCode * 486187739) + size.GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)classifications).GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)trackingState).GetHashCode();
                hashCode = (hashCode * 486187739) + nativePtr.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(XRBoundingBox)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>. Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XRBoundingBox lhs, XRBoundingBox rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(XRBoundingBox)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator !=(XRBoundingBox lhs, XRBoundingBox rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRBoundingBox"/> to compare against.</param>
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="XRBoundingBox"/>, otherwise <see langword="false"/>.</returns>
        public bool Equals(XRBoundingBox other)
        {
            return
                trackableId.Equals(other.trackableId) &&
                pose.Equals(other.pose) &&
                size.Equals(other.size) &&
                (trackingState == other.trackingState) &&
                (nativePtr == other.nativePtr) &&
                (classifications == other.classifications);
        }
    }
}
