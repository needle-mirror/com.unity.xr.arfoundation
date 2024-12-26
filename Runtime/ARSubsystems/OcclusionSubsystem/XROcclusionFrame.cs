using System;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the properties included in the camera frame.
    /// </summary>
    [Flags]
    public enum XROcclusionFrameProperties
    {
        /// <summary>
        /// No occlusion frame properties are included.
        /// </summary>
        None = 0,

        /// <summary>
        /// The timestamp of the frame is included.
        /// </summary>
        Timestamp = 1 << 0,

        /// <summary>
        /// The near and far planes are included.
        /// </summary>
        NearFarPlanes = 1 << 1,

        /// <summary>
        /// The poses for the frame are included.
        /// </summary>
        Poses = 1 << 2,

        /// <summary>
        /// The fields of view for the frame are included.
        /// </summary>
        Fovs = 1 << 3,
    }

    /// <summary>
    /// Represents a frame captured by the device camera with included metadata.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XROcclusionFrame : IEquatable<XROcclusionFrame>, IDisposable
    {
        /// <summary>
        /// The properties that are included in the frame.
        /// </summary>
        /// <value>The included properties.</value>
        public XROcclusionFrameProperties properties => m_Properties;
        XROcclusionFrameProperties m_Properties;

        /// <summary>
        /// The timestamp of the frame, in nanoseconds.
        /// </summary>
        long m_TimestampNs;

        XRNearFarPlanes m_NearFarPlanes;

        /// <remarks>
        /// Indices are correlated with <see cref="m_Fovs"/>.
        /// </remarks>
        NativeArray<Pose> m_Poses;

        /// <remarks>
        /// Indices are correlated with <see cref="m_Poses"/>.
        /// </remarks>
        NativeArray<XRFov> m_Fovs;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="properties">The properties that are included in the frame.</param>
        /// <param name="timestamp">The timestamp of the frame, in nanoseconds.</param>
        /// <param name="nearFarPlanes">The near and far planes.</param>
        /// <param name="poses">The poses from which the frame was rendered.</param>
        /// <param name="fovs">The fields of view for the frame.</param>
        public XROcclusionFrame(
            XROcclusionFrameProperties properties,
            long timestamp,
            XRNearFarPlanes nearFarPlanes,
            NativeArray<Pose> poses,
            NativeArray<XRFov> fovs)
        {
            m_Properties = properties;
            m_TimestampNs = timestamp;
            m_NearFarPlanes = nearFarPlanes;
            m_Poses = poses;
            m_Fovs = fovs;
        }

        /// <summary>
        /// Get the timestamp of the frame, if possible.
        /// </summary>
        /// <param name="timestampNs">The timestamp of the camera frame, in nanoseconds.</param>
        /// <returns><see langword="true"/> if the frame has a timestamp that was output to <paramref name="timestampNs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetTimestamp(out long timestampNs)
        {
            if ((m_Properties & XROcclusionFrameProperties.Timestamp) != 0)
            {
                timestampNs = m_TimestampNs;
                return true;
            }

            timestampNs = default;
            return false;
        }

        /// <summary>
        /// Get the near and far planes for the frame, if possible.
        /// </summary>
        /// <param name="nearFarPlanes">The near and far planes, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the frame has near and far planes that were output to <paramref name="nearFarPlanes"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetNearFarPlanes(out XRNearFarPlanes nearFarPlanes)
        {
            if ((m_Properties & XROcclusionFrameProperties.NearFarPlanes) != 0)
            {
                nearFarPlanes = m_NearFarPlanes;
                return true;
            }

            nearFarPlanes = default;
            return false;
        }

        /// <summary>
        /// Get an array of poses from which the frame was rendered, if possible.
        /// Poses are in Unity world space.
        /// </summary>
        /// <param name="poses">The output array of poses, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the frame has poses that were output to <paramref name="poses"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetPoses(out NativeArray<Pose> poses)
        {
            if ((m_Properties & XROcclusionFrameProperties.Poses) != 0)
            {
                poses = m_Poses;
                return true;
            }

            poses = default;
            return false;
        }

        /// <summary>
        /// Get an array of fields of view for the frame if possible.
        /// </summary>
        /// <param name="fovs">The output array of fields of view, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the frame has fields of view that were output to <paramref name="fovs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetFovs(out NativeArray<XRFov> fovs)
        {
            if ((m_Properties & XROcclusionFrameProperties.Fovs) != 0)
            {
                fovs = m_Fovs;
                return true;
            }

            fovs = default;
            return false;
        }

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XROcclusionFrame"/> to compare against.</param>
        /// <returns><see langword="true"/> if the <see cref="XROcclusionFrame"/> represents the same object.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XROcclusionFrame other)
        {
            return m_Properties == other.m_Properties &&
                m_TimestampNs.Equals(other.m_TimestampNs) &&
                m_NearFarPlanes.Equals(other.m_NearFarPlanes) &&
                m_Poses.Equals(other.m_Poses) &&
                m_Fovs.Equals(other.m_Fovs);
        }

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="obj">An <c>object</c> to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an <see cref="XROcclusionFrame"/> and
        /// <see cref="Equals(XROcclusionFrame)"/> is also <see langword="true"/>. Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is XROcclusionFrame frame && Equals(frame);
        }

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for equality using <see cref="Equals(XROcclusionFrame)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XROcclusionFrame"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XROcclusionFrame"/> of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XROcclusionFrame lhs, XROcclusionFrame rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for inequality using <see cref="Equals(XROcclusionFrame)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XROcclusionFrame"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XROcclusionFrame"/> of the comparison.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="true"/>.</returns>
        public static bool operator !=(XROcclusionFrame lhs, XROcclusionFrame rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Generates a hash code suitable for use in <c>HashSet</c> and <c>Dictionary</c>.
        /// </summary>
        /// <returns>A hash of this <see cref="XROcclusionFrame"/>.</returns>
        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = hashCode * 486187739 + m_TimestampNs.GetHashCode();
                hashCode = hashCode * 486187739 + ((int)m_Properties).GetHashCode();
                hashCode = hashCode * 486187739 + m_Poses.GetHashCode();
                hashCode = hashCode * 486187739 + m_Fovs.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Generates a string representation of this instance suitable for debugging purposes.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            if ((m_Properties & XROcclusionFrameProperties.Timestamp) != 0)
                sb.AppendLine($"  TimestampNs: {m_TimestampNs}");
            if ((m_Properties & XROcclusionFrameProperties.NearFarPlanes) != 0)
                sb.AppendLine($"  NearZ: {m_NearFarPlanes.nearZ}\n  FarZ: {m_NearFarPlanes.farZ}");
            if ((m_Properties & XROcclusionFrameProperties.Poses) != 0)
            {
                for (var i = 0; i < m_Poses.Length; i++)
                {
                    sb.AppendLine($"  Poses[{i}]: {m_Poses[i]}");
                }
            }
            if ((m_Properties & XROcclusionFrameProperties.Fovs) != 0)
            {
                for (var i = 0; i < m_Fovs.Length; i++)
                {
                    sb.AppendLine($"  Fovs[{i}]: {m_Fovs[i]}");
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Dispose native resources associated with this frame, including the native array of display matrices.
        /// </summary>
        public void Dispose()
        {
            if (m_Poses.IsCreated)
                m_Poses.Dispose();
            if (m_Fovs.IsCreated)
                m_Fovs.Dispose();
        }
    }
}
