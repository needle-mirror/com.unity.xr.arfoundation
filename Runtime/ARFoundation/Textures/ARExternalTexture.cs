using System;
using System.Text;
using UnityEngine.XR.ARFoundation.InternalUtils;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents an external texture on the GPU.
    /// </summary>
    public readonly struct ARExternalTexture : IEquatable<ARExternalTexture>
    {
        /// <summary>
        /// The external texture.
        /// </summary>
        /// <remarks>
        /// This texture may only exist on the GPU. Refer to your provider documentation for more information.
        ///
        /// If this is a GPU texture, to use the texture on the CPU, you must read it back from
        /// the GPU using [Texture2D.ReadPixels](xref:UnityEngine.Texture2D.ReadPixels(UnityEngine.Rect,System.Int32,System.Int32,System.Boolean)).
        /// </remarks>
        public Texture texture { get; }

        /// <summary>
        /// ID of the shader property associated with this texture.
        /// </summary>
        /// <seealso href="xref:UnityEngine.Shader.PropertyToID(System.String)"/>
        public int propertyId { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="propertyId">The texture's property ID.</param>
        public ARExternalTexture(Texture texture, int propertyId)
        {
            this.texture = texture;
            this.propertyId = propertyId;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type. Textures are compared
        /// using reference equality.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to <paramref name="other"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(ARExternalTexture other)
            => ReferenceEquals(texture, other.texture) && propertyId == other.propertyId;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type. Textures are compared
        /// using reference equality.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to <paramref name="obj"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is ARExternalTexture other && Equals(other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => HashCode.Combine(texture, propertyId);

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(ARExternalTexture)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(ARExternalTexture lhs, ARExternalTexture rhs) => Equals(lhs, rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to `!`<see cref="Equals(AROcclusionFrameEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(ARExternalTexture lhs, ARExternalTexture rhs) => !Equals(lhs, rhs);

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  propertyId: {propertyId},");
            sb.AppendLine($"  texture: {texture.ToDebugString()}");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
