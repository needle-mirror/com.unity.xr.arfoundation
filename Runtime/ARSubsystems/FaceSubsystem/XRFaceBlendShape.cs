using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents a blend shape ID and its weight. For more information about blend shapes in Unity, refer to the tutorial
    /// [Setting up Blendshapes in Unity](https://learn.unity.com/tutorial/setting-up-blendshapes-in-unity#).
    /// </summary>
    public struct XRFaceBlendShape : IEquatable<XRFaceBlendShape>
    {
        int m_BlendShapeId;
        float m_Weight;

        /// <summary>
        /// The ID of the blend shape, as defined by the platform.
        /// Refer to [Face tracking platform support](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.2/manual/features/face-tracking/platform-support.html)
        /// to learn if your target platform supports blend shapes, and if so, refer the provider plug-in manual
        /// for that platform to understand the meaning of each ID value.
        /// </summary>
        public int blendShapeId => m_BlendShapeId;

        /// <summary>
        /// A value from 0.0 (no influence) to 1.0 (maximum influence) that specifies the influence of this blend shape in the facial expression.
        /// The value may describe, for example, how closed is the left eye, how open is the mouth, etc.
        /// </summary>
        public float weight => m_Weight;

        /// <summary>
        /// Constructs an instance with the given blend shape identifier and weight.
        /// </summary>
        /// <param name="blendShapeId">The blend shape ID.</param>
        /// <param name="weight">The weight, between 0.0 and 1.0.</param>
        public XRFaceBlendShape(int blendShapeId, float weight)
        {
            m_BlendShapeId = blendShapeId;
            m_Weight = weight;
        }

        /// <summary>
        /// Tests for equality, using `float.Equals` when comparing weights.
        /// </summary>
        /// <param name="other">The other face blend shape to compare against.</param>
        /// <returns><see langword="true"/> if this face blend shape has the same ID and weight as <paramref name="other"/>.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public bool Equals(XRFaceBlendShape other)
        {
            return (blendShapeId == other.blendShapeId) &&
            weight.Equals(other.weight);
        }

        /// <summary>
        /// Tests for equality, using `float.Equals` when comparing weights.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns><see langword="true"/> if this face blend shape is equal to <paramref name="obj"/> using
        /// <see cref="Equals(XRFaceBlendShape)"/>. Otherwise, returns <see langword="false"/>.</returns>
        public override bool Equals(object obj) => (obj is XRFaceBlendShape other) && Equals(other);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>This face blend shape's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = blendShapeId.GetHashCode();
                hash = hash * 486187739 + weight.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(XRFaceBlendShape)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(XRFaceBlendShape)"/>. Otherwise, returns <see langword="false"/>.</returns>
        public static bool operator ==(XRFaceBlendShape lhs, XRFaceBlendShape rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to the negation of <see cref="Equals(XRFaceBlendShape)"/>.
        /// </summary>
        /// <param name="lhs">The instance to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The instance to compare with <paramref name="lhs"/>.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        /// <see cref="Equals(XRFaceBlendShape)"/>. Otherwise, returns <see langword="true"/>.</returns>
        public static bool operator !=(XRFaceBlendShape lhs, XRFaceBlendShape rhs) => !lhs.Equals(rhs);
    }
}
