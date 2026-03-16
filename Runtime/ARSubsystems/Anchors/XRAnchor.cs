using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Describes session-relative data for an anchor.
    /// </summary>
    /// <seealso cref="XRAnchorSubsystem"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct XRAnchor : ITrackable, IEquatable<XRAnchor>
    {
        TrackableId m_Id;
        Pose m_Pose;
        TrackingState m_TrackingState;
        IntPtr m_NativePtr;
        Guid m_SessionId;
        TrackableId m_ParentId;

        /// <summary>
        /// Get this anchor's trackable ID.
        /// </summary>
        public TrackableId trackableId => m_Id;

        /// <summary>
        /// Get this anchor's pose in session space.
        /// </summary>
        public Pose pose => m_Pose;

        /// <summary>
        /// Get this anchor's tracking state.
        /// </summary>
        public TrackingState trackingState => m_TrackingState;

        /// <summary>
        /// Get this anchor's native pointer.
        /// The data pointed to by this pointer is implementation-specific.
        /// </summary>
        public IntPtr nativePtr => m_NativePtr;

        /// <summary>
        /// Get the ID of the session from which this anchor originated.
        /// </summary>
        public Guid sessionId => m_SessionId;

        /// <summary>
        /// Get the trackable ID of this anchor's parent trackable.
        /// </summary>
        public TrackableId parentId => m_ParentId;

        /// <summary>
        /// Get a default-initialized <see cref="XRAnchor"/>, which is distinct from C# <see langword="default"/>
        /// as `Pose.identity` is not equal to `0`.
        /// </summary>
        public static XRAnchor defaultValue => s_Default;

        static readonly XRAnchor s_Default = new()
        {
            m_Id = TrackableId.invalidId,
            m_Pose = Pose.identity,
            m_SessionId = Guid.Empty,
            m_ParentId = TrackableId.invalidId
        };

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <param name="pose">The pose in session space.</param>
        /// <param name="trackingState">The tracking state.</param>
        /// <param name="nativePtr">The native pointer.</param>
        public XRAnchor(
            TrackableId trackableId,
            Pose pose,
            TrackingState trackingState,
            IntPtr nativePtr)
        {
            m_Id = trackableId;
            m_Pose = pose;
            m_TrackingState = trackingState;
            m_NativePtr = nativePtr;
            m_SessionId = Guid.Empty;
            m_ParentId = TrackableId.invalidId;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <param name="pose">The pose in session space.</param>
        /// <param name="trackingState">The tracking state.</param>
        /// <param name="nativePtr">The native pointer.</param>
        /// <param name="parentId">The trackable ID of the parent trackable.</param>
        public XRAnchor(
            TrackableId trackableId,
            Pose pose,
            TrackingState trackingState,
            IntPtr nativePtr,
            TrackableId parentId)
            : this(trackableId, pose, trackingState, nativePtr)
        {
            m_ParentId = parentId;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <param name="pose">The pose in session space.</param>
        /// <param name="trackingState">The tracking state.</param>
        /// <param name="nativePtr">The native pointer.</param>
        /// <param name="sessionId">The ID of the session from which this anchor originated.</param>
        public XRAnchor(
            TrackableId trackableId,
            Pose pose,
            TrackingState trackingState,
            IntPtr nativePtr,
            Guid sessionId)
            : this(trackableId, pose, trackingState, nativePtr)
        {
            m_SessionId = sessionId;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="trackableId">The trackable ID.</param>
        /// <param name="pose">The pose in session space.</param>
        /// <param name="trackingState">The tracking state.</param>
        /// <param name="nativePtr">The native pointer.</param>
        /// <param name="sessionId">The ID of the session from which this anchor originated.</param>
        /// <param name="parentId">The trackable ID of the parent trackable.</param>
        public XRAnchor(
            TrackableId trackableId,
            Pose pose,
            TrackingState trackingState,
            IntPtr nativePtr,
            Guid sessionId,
            TrackableId parentId)
            : this(trackableId, pose, trackingState, nativePtr, parentId)
        {
            m_SessionId = sessionId;
        }

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = m_Id.GetHashCode();
                hashCode = hashCode * 486187739 + m_Pose.GetHashCode();
                hashCode = hashCode * 486187739 + ((int)m_TrackingState).GetHashCode();
                hashCode = hashCode * 486187739 + m_NativePtr.GetHashCode();
                hashCode = hashCode * 486187739 + m_SessionId.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRAnchor"/> to compare against.</param>
        /// <returns>`true` if every field in <paramref name="other"/> is equal to this <see cref="XRAnchor"/>.
        /// Otherwise, `false`.</returns>
        public bool Equals(XRAnchor other)
        {
            return
                m_Id.Equals(other.m_Id)
                && m_Pose.Equals(other.m_Pose)
                && m_TrackingState == other.m_TrackingState
                && m_NativePtr == other.m_NativePtr
                && m_SessionId.Equals(other.m_SessionId)
                && m_ParentId.Equals(other.m_ParentId);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`true` if <paramref name="obj"/> is of type <see cref="XRAnchor"/> and
        /// <see cref="Equals(XRAnchor)"/> also returns `true`. Otherwise, `false`.</returns>
        public override bool Equals(object obj) => obj is XRAnchor anchor && Equals(anchor);

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(XRAnchor)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`true` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>. Otherwise, `false`.</returns>
        public static bool operator==(XRAnchor lhs, XRAnchor rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to `!`<see cref="Equals(XRAnchor)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`true` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>. Otherwise, `false`.</returns>
        public static bool operator!=(XRAnchor lhs, XRAnchor rhs) => !lhs.Equals(rhs);
    }
}
