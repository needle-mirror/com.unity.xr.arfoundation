using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents a spatial buffer containing marker's encoded data, as detected by the XR marker subsystem if the marker
    /// supports encoded data.
    /// </summary>
    public readonly struct XRSpatialBuffer : IEquatable<XRSpatialBuffer>
    {
        /// <summary>
        /// The unique identifier for the buffer, assigned by the subsystem or provider.
        /// </summary>
        public ulong bufferId { get; }

        /// <summary>
        /// The type of data stored in this buffer.
        /// </summary>
        public XRSpatialBufferType bufferType { get; }

        /// <summary>
        /// Constructs a new <see cref="XRSpatialBuffer"/> with the given id and data type.
        /// </summary>
        /// <param name="bufferId">Unique identifier for this spatial buffer.</param>
        /// <param name="bufferType">The type of data in the buffer.</param>
        public XRSpatialBuffer(ulong bufferId, XRSpatialBufferType bufferType)
        {
            this.bufferId = bufferId;
            this.bufferType = bufferType;
        }

        /// <summary>
        /// Generates a hash code for this <see cref="XRSpatialBuffer"/>.
        /// </summary>
        /// <returns>A hash code suitable for use in data structures that use hashing.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(bufferId, bufferType);
        }

        /// <summary>
        /// Compares this <see cref="XRSpatialBuffer"/> with another for equality.
        /// </summary>
        /// <remarks>
        /// Two buffers are considered equal if they have the same <see cref="bufferId"/> and <see cref="bufferType"/>.
        /// </remarks>
        /// <param name="other">The other <see cref="XRSpatialBuffer"/> to compare against.</param>
        /// <returns>`true` if the two buffers are equal, otherwise `false`.</returns>
        public bool Equals(XRSpatialBuffer other)
            => bufferId == other.bufferId && bufferType == other.bufferType;

        /// <summary>
        /// Compares this <see cref="XRSpatialBuffer"/> with another object for equality.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is an <see cref="XRSpatialBuffer"/> and is equal to this
        /// instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
            => obj is XRSpatialBuffer other && Equals(other);

        /// <summary>
        /// Compares two <see cref="XRSpatialBuffer"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="XRSpatialBuffer"/> on the left-hand side of the operator.</param>
        /// <param name="right">The <see cref="XRSpatialBuffer"/> on the right-hand side of the operator.</param>
        /// <returns><see langword="true"/> if the two buffers are equal, otherwise <see langword="false"/>.</returns>
        public static bool operator ==(XRSpatialBuffer left, XRSpatialBuffer right)
            => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="XRSpatialBuffer"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="XRSpatialBuffer"/> on the left-hand side of the operator.</param>
        /// <param name="right">The <see cref="XRSpatialBuffer"/> on the right-hand side of the operator.</param>
        /// <returns><see langword="true"/> if the two buffers are not equal, otherwise <see langword="false"/>.</returns>
        public static bool operator !=(XRSpatialBuffer left, XRSpatialBuffer right)
            => !(left == right);
    }
}
