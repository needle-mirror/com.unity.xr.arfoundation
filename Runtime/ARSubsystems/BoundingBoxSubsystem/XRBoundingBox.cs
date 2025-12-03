using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The session-relative data associated with a 3D bounding box.
    /// </summary>
    /// <seealso cref="XRBoundingBoxSubsystem"/>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct XRBoundingBox : ITrackable, IEquatable<XRBoundingBox>
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
        /// The parent ID of the bounding box.
        /// </summary>
        public TrackableId parentId{ get; }

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
        /// <param name="parentId">The <see cref="TrackableId"/> of the parent of this tracked object.</param>
        public XRBoundingBox(
           TrackableId trackableId,
           Pose pose,
           Vector3 size,
           TrackingState trackingState,
           BoundingBoxClassifications classifications,
           IntPtr nativePtr,
           TrackableId parentId)
        {
            this.trackableId = trackableId;
            this.pose = pose;
            this.size = size;
            this.trackingState = trackingState;
            this.classifications = classifications;
            this.nativePtr = nativePtr;
            this.parentId = parentId;
        }

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
            parentId = TrackableId.invalidId;
        }

        /// <summary>
        /// Generates a hash suitable for use with containers such as
        /// <see cref="System.Collections.Generic.HashSet{T}">HashSet</see>
        /// and <see cref="System.Collections.Generic.Dictionary{T1, T2}">Dictionary</see>.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(trackableId, pose, size, (int)trackingState, nativePtr, (int)classifications, parentId);
        }

        /// <summary>
        /// Generates a new string that describes the bounding box's properties, suitable for debugging purposes.
        /// </summary>
        /// <returns>A string that describes the bounding box's properties.</returns>
        public override string ToString()
        {
            var sb = SharedStringBuilder.instance;
            sb.AppendLine("Bounding Box:");
            sb.AppendLine("\ttrackableId: " + trackableId);
            sb.AppendLine("\tpose: " + pose);
            sb.AppendLine("\tsize: " + size);
            sb.AppendLine("\ttrackingState: " + trackingState);
            sb.AppendLine($"\tnativePtr: {nativePtr.ToInt64()}");
            sb.AppendLine("\tclassifications: " + classifications);
            sb.AppendLine("\tparentId: " + parentId);
            var result = SharedStringBuilder.instance.ToString();
            SharedStringBuilder.instance.Clear();
            return result;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`true` if <paramref name="obj"/> is of type <see cref="XRBoundingBox"/> and
        /// <see cref="Equals(XRBoundingBox)"/> also returns `true`. Otherwise, `false`.</returns>
        public override bool Equals(object obj) => (obj is XRBoundingBox other) && Equals(other);

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(XRBoundingBox)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`true` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, `false`.</returns>
        public static bool operator ==(XRBoundingBox lhs, XRBoundingBox rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to `!`<see cref="Equals(XRBoundingBox)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`true` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
        /// Otherwise, `false`.</returns>
        public static bool operator !=(XRBoundingBox lhs, XRBoundingBox rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRBoundingBox"/> to compare against.</param>
        /// <returns>`true` if every field in <paramref name="other"/> is equal to this <see cref="XRBoundingBox"/>.
        /// Otherwise, `false`.</returns>
        public bool Equals(XRBoundingBox other)
        {
            return
                trackableId.Equals(other.trackableId)
                && pose.Equals(other.pose)
                && size.Equals(other.size)
                && trackingState == other.trackingState
                && nativePtr == other.nativePtr
                && classifications == other.classifications
                && parentId == other.parentId;
        }
    }
}
