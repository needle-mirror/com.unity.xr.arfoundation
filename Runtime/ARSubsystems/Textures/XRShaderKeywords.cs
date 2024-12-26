using System;
using Unity.XR.CoreUtils.Collections;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Stores the enabled and disabled shader keywords for a material.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Manual/shader-keywords.html"/>
    public readonly struct XRShaderKeywords : IEquatable<XRShaderKeywords>
    {
        /// <summary>
        /// The enabled shader keywords.
        /// </summary>
        public ReadOnlyList<string> enabledKeywords { get; }

        /// <summary>
        /// The disabled shader keywords.
        /// </summary>
        public ReadOnlyList<string> disabledKeywords { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="enabledKeywords">The enabled shader keywords.</param>
        /// <param name="disabledKeywords">The disabled shader keywords.</param>
        public XRShaderKeywords(ReadOnlyList<string> enabledKeywords, ReadOnlyList<string> disabledKeywords)
        {
            this.enabledKeywords = enabledKeywords;
            this.disabledKeywords = disabledKeywords;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to <paramref name="other"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRShaderKeywords other)
            => Equals(enabledKeywords, other.enabledKeywords) && Equals(disabledKeywords, other.disabledKeywords);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to <paramref name="obj"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is XRShaderKeywords other && Equals(other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => HashCode.Combine(enabledKeywords, disabledKeywords);

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(XRShaderKeywords)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XRShaderKeywords lhs, XRShaderKeywords rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to `!`<see cref="Equals(XRShaderKeywords)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(XRShaderKeywords lhs, XRShaderKeywords rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
            => $"{{\n  enabledKeywords: {enabledKeywords},\n  disabledKeywords: {disabledKeywords}\n}}";
    }
}
