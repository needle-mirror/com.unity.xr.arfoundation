using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A structure for occlusion information pertaining to a particular frame. This is used to communicate information
    /// in the <see cref="AROcclusionManager.frameReceived" /> event.
    /// </summary>
    public struct AROcclusionFrameEventArgs : IEquatable<AROcclusionFrameEventArgs>
    {
        /// <summary>
        /// The occlusion textures associated with this frame. These are generally external textures, which exist only
        /// on the GPU. To use them on the CPU (for example, for computer vision processing), you must read them back
        /// from the GPU.
        /// </summary>
        public List<Texture2D> textures { get; internal set; }

        /// <summary>
        /// Ids of the property name associated with each texture. This is a
        /// parallel <c>List</c> to the <see cref="textures"/> list.
        /// </summary>
        public List<int> propertyNameIds { get; internal set; }

        /// <summary>
        /// The list of keywords to be enabled for the material.
        /// </summary>
        [Obsolete("enabledMaterialKeywords has been deprecated in AR Foundation version 6.0. Use enabledShaderKeywords instead.")]
        public List<string> enabledMaterialKeywords { get; internal set; }

        /// <summary>
        /// The list of keywords to be disabled for the material.
        /// </summary>
        [Obsolete("disabledMaterialKeywords has been deprecated in AR Foundation version 6.0. Use disabledShaderKeywords instead.")]
        public List<string> disabledMaterialKeywords { get; internal set; }

        /// <summary>
        /// The enabled shader keywords.
        /// </summary>
        public ReadOnlyCollection<string> enabledShaderKeywords { get; internal set; }

        /// <summary>
        /// The disabled shader keywords.
        /// </summary>
        public ReadOnlyCollection<string> disabledShaderKeywords { get; internal set; }

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode() => HashCodeUtil.Combine(
            HashCodeUtil.ReferenceHash(textures),
            HashCodeUtil.ReferenceHash(propertyNameIds),
            HashCodeUtil.ReferenceHash(enabledShaderKeywords),
            HashCodeUtil.ReferenceHash(disabledShaderKeywords));

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="AROcclusionFrameEventArgs"/> and
        /// <see cref="Equals(AROcclusionFrameEventArgs)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(object obj)
            => (obj is AROcclusionFrameEventArgs) && Equals((AROcclusionFrameEventArgs)obj);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="AROcclusionFrameEventArgs"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="AROcclusionFrameEventArgs"/>, otherwise false.</returns>
        public bool Equals(AROcclusionFrameEventArgs other)
            => (((textures == null) ? (other.textures == null) : textures.Equals(other.textures))
                && ((propertyNameIds == null) ? (other.propertyNameIds == null)
                    : propertyNameIds.Equals(other.propertyNameIds))
                && ((enabledShaderKeywords == null) ? (other.enabledShaderKeywords == null)
                    : enabledShaderKeywords.Equals(other.enabledShaderKeywords))
                && ((disabledShaderKeywords == null) ? (other.disabledShaderKeywords == null)
                    : disabledShaderKeywords.Equals(other.disabledShaderKeywords)));

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(AROcclusionFrameEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(AROcclusionFrameEventArgs lhs, AROcclusionFrameEventArgs rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(AROcclusionFrameEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(AROcclusionFrameEventArgs lhs, AROcclusionFrameEventArgs rhs) => !lhs.Equals(rhs);
    }
}
