// ReSharper disable ConvertToAutoPropertyWhenPossible
using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the near and far planes of a depth image.
    /// </summary>
    public readonly struct XRNearFarPlanes : IEquatable<XRNearFarPlanes>
    {
        /// <summary>
        /// The near plane, in meters.
        /// </summary>
        public float nearZ => m_NearZ;
        readonly float m_NearZ;

        /// <summary>
        /// The far plane, in meters.
        /// </summary>
        public float farZ => m_FarZ;
        readonly float m_FarZ;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nearZ">The near plane, in meters.</param>
        /// <param name="farZ">The far plane, in meters.</param>
        public XRNearFarPlanes(float nearZ, float farZ)
        {
            m_NearZ = nearZ;
            m_FarZ = farZ;
        }

        /// <summary>
        /// Indicates whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns><see langword="true"/> if this instance is equal to <paramref name="other"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRNearFarPlanes other) => m_NearZ.Equals(other.m_NearZ) && m_FarZ.Equals(other.m_FarZ);

        /// <summary>
        /// Indicates whether this instance is equal to another object.
        /// Casts the other object to `XRNearFarPlanes`, then returns <see cref="Equals(UnityEngine.XR.ARSubsystems.XRNearFarPlanes)"/>.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns><see langword="true"/> if this instance is equal to <paramref name="obj"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is XRNearFarPlanes other && Equals(other);

        /// <summary>
        /// Get a hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => HashCode.Combine(m_NearZ, m_FarZ);

        /// <summary>
        /// Generates a string representation of this instance suitable for debugging purposes.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => $"{{\n  nearZ: {m_NearZ}\n  farZ: {m_FarZ}\n}}";
    }
}
