using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Contains a native texture object and includes various metadata about the texture.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XRTextureDescriptor : IEquatable<XRTextureDescriptor>
    {
        /// <summary>
        /// A pointer to the native texture object.
        /// </summary>
        /// <value>A pointer to the native texture object.</value>
        public IntPtr nativeTexture => m_NativeTexture;
        IntPtr m_NativeTexture;

        /// <summary>
        /// Specifies the width dimension of the native texture object.
        /// </summary>
        /// <value>The width of the native texture object.</value>
        public int width => m_Width;
        int m_Width;

        /// <summary>
        /// Specifies the height dimension of the native texture object.
        /// </summary>
        /// <value>The height of the native texture object.</value>
        public int height => m_Height;
        int m_Height;

        /// <summary>
        /// Specifies the number of mipmap levels in the native texture object.
        /// </summary>
        /// <value>The number of mipmap levels in the native texture object.</value>
        public int mipmapCount => m_MipmapCount;
        int m_MipmapCount;

        /// <summary>
        /// Specifies the texture format of the native texture object.
        /// </summary>
        /// <value>The format of the native texture object.</value>
        public TextureFormat format => m_Format;
        TextureFormat m_Format;

        /// <summary>
        /// Specifies the unique shader property name ID for the material shader texture.
        /// </summary>
        /// <value>The unique shader property name ID for the material shader texture.</value>
        /// <remarks>
        /// Use the static method <c>Shader.PropertyToID(string name)</c> to get the unique identifier.
        /// </remarks>
        public int propertyNameId => m_PropertyNameId;
        int m_PropertyNameId;

        /// <summary>
        /// Determines whether the texture data references a valid texture object with positive width and height.
        /// </summary>
        /// <value><see langword="true"/> if the texture data references a valid texture object with positive width and height.
        /// Otherwise, <see langword="false"/>.</value>
        public bool valid => m_NativeTexture != IntPtr.Zero && m_Width > 0 && m_Height > 0;

        /// <summary>
        /// This specifies the depth dimension of the native texture. For a 3D texture, depth is greater than zero.
        /// For any other kind of valid texture, depth is one.
        /// </summary>
        /// <value>The depth dimension of the native texture object.</value>
        public int depth => m_Depth;
        int m_Depth;

        /// <summary>
        /// Get the dimension of the native texture object.
        /// </summary>
        /// <value>The texture dimension of the native texture object.</value>
        [Obsolete("dimension has been deprecated in AR Foundation version 6.1. Use textureType instead.")]
        public TextureDimension dimension => m_Dimension;
        TextureDimension m_Dimension;

        /// <summary>
        /// Get the texture type.
        /// </summary>
        /// <value>The texture type.</value>
        /// <remarks>
        /// This property can use data from the deprecated <see cref="dimension"/> property as a fallback mechanism
        /// for backwards compatibility if the `m_TextureType` field is uninitialized.
        /// </remarks>
        public XRTextureType textureType
        {
            get
            {
                if (m_TextureType == XRTextureType.None)
                    return m_Dimension.ToXRTextureType();

                return m_TextureType;
            }
        }
        XRTextureType m_TextureType;

        /// <summary>
        /// Creates a <see cref="XRTextureDescriptor"/>.
        /// </summary>
        /// <param name="nativeTexture">Pointer to the native texture.</param>
        /// <param name="width">Width of the native texture.</param>
        /// <param name="height">Height of the native texture.</param>
        /// <param name="mipmapCount">Number of mipmaps in the native texture.</param>
        /// <param name="format">Format of the native texture.</param>
        /// <param name="propertyNameId">The unique shader property name ID for the material shader texture.</param>
        /// <param name="depth">Depth dimension of the native texture. Should be one for all except 3D textures.</param>
        /// <param name="dimension">[Texture dimension](https://docs.unity3d.com/ScriptReference/Rendering.TextureDimension.html) of the native texture object.</param>
        [Obsolete("This constructor has been deprecated and replaced with a new constructor in AR Foundation 6.1.")]
        public XRTextureDescriptor(IntPtr nativeTexture, int width, int height, int mipmapCount, TextureFormat format,
            int propertyNameId, int depth, TextureDimension dimension)
            : this(nativeTexture, width, height, mipmapCount, format, propertyNameId, depth, dimension.ToXRTextureType())
        { }

        /// <summary>
        /// Creates a <see cref="XRTextureDescriptor"/>.
        /// </summary>
        /// <param name="nativeTexture">Pointer to the native texture.</param>
        /// <param name="width">Width of the native texture.</param>
        /// <param name="height">Height of the native texture.</param>
        /// <param name="mipmapCount">Number of mipmaps in the native texture.</param>
        /// <param name="format">Format of the native texture.</param>
        /// <param name="propertyNameId">The unique shader property name ID for the material shader texture.</param>
        /// <param name="depth">Depth dimension of the native texture. Should be one for all except 3D textures.</param>
        /// <param name="textureType">Whether the native texture object is a texture2D, cubemap, render texture, etc.</param>
        public XRTextureDescriptor(IntPtr nativeTexture, int width, int height, int mipmapCount, TextureFormat format,
            int propertyNameId, int depth, XRTextureType textureType)
        {
            m_NativeTexture = nativeTexture;
            m_Width = width;
            m_Height = height;
            m_MipmapCount = mipmapCount;
            m_Format = format;
            m_PropertyNameId = propertyNameId;
            m_Depth = depth;
            m_Dimension = textureType.ToTextureDimension();
            m_TextureType = textureType;
        }

        /// <summary>
        /// Construct an instance with the given property name ID and default data otherwise.
        /// </summary>
        /// <param name="propertyNameId">The shader property ID to assign the texture.</param>
        public XRTextureDescriptor(int propertyNameId)
        {
            m_NativeTexture = default;
            m_Width = default;
            m_Height = default;
            m_MipmapCount = default;
            m_Format = default;
            m_PropertyNameId = propertyNameId;
            m_Depth = default;
            m_Dimension = default;
            m_TextureType = default;
        }

        /// <summary>
        /// Determines whether the given texture descriptor has identical texture metadata (dimension, mipmap count,
        /// and format).
        /// </summary>
        /// <param name="other">The given texture descriptor with which to compare.</param>
        /// <returns><see langword="true"/> if the texture metadata (dimension, mipmap count, and format) are identical
        /// between the current and other texture descriptors. Otherwise, <see langword="false"/>.
        /// </returns>
        public bool hasIdenticalTextureMetadata(XRTextureDescriptor other)
        {
            return m_Width.Equals(other.m_Width) &&
                m_Height.Equals(other.m_Height) &&
                m_Depth.Equals(other.m_Depth) &&
                m_Dimension == other.m_Dimension &&
                m_MipmapCount.Equals(other.m_MipmapCount) &&
                m_Format == other.m_Format &&
                m_TextureType == other.m_TextureType;
        }

        /// <summary>
        /// Reset the texture descriptor back to default values.
        /// </summary>
        public void Reset()
        {
            m_NativeTexture = IntPtr.Zero;
            m_Width = 0;
            m_Height = 0;
            m_Depth = 0;
            m_Dimension = TextureDimension.None;
            m_MipmapCount = 0;
            m_Format = 0; // undefined TextureFormat
            m_PropertyNameId = 0;
            m_TextureType = XRTextureType.None;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other texture descriptor to compare against.</param>
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this instance.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRTextureDescriptor other)
        {
            return hasIdenticalTextureMetadata(other) &&
                m_PropertyNameId.Equals(other.m_PropertyNameId) &&
                m_NativeTexture == other.m_NativeTexture;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is of type <see cref="XRTextureDescriptor"/> and
        /// <see cref="Equals(XRTextureDescriptor)"/> also returns <see langword="true"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is XRTextureDescriptor descriptor && Equals(descriptor);

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(XRTextureDescriptor)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XRTextureDescriptor lhs, XRTextureDescriptor rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to `!`<see cref="Equals(XRTextureDescriptor)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(XRTextureDescriptor lhs, XRTextureDescriptor rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = (hashCode * 486187739) + m_NativeTexture.GetHashCode();
                hashCode = (hashCode * 486187739) + m_Width.GetHashCode();
                hashCode = (hashCode * 486187739) + m_Height.GetHashCode();
                hashCode = (hashCode * 486187739) + m_Depth.GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_Dimension).GetHashCode();
                hashCode = (hashCode * 486187739) + m_MipmapCount.GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_Format).GetHashCode();
                hashCode = (hashCode * 486187739) + m_PropertyNameId.GetHashCode();
                hashCode = (hashCode * 486187739) + m_TextureType.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Generates a string suitable for debugging purposes.
        /// </summary>
        /// <returns>A string suitable for debug logging.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"nativeTexture\": 0x{m_NativeTexture.ToString("X16")},");
            sb.AppendLine($"  \"width\": {m_Width.ToString()},");
            sb.AppendLine($"  \"height\": {m_Height.ToString()},");
            sb.AppendLine($"  \"depth\": {m_Depth.ToString()},");
            sb.AppendLine($"  \"mipmapCount\": {m_MipmapCount.ToString()},");
            sb.AppendLine($"  \"format\": {m_Format.ToString()},");
            sb.AppendLine($"  \"propertyNameId:\" {m_PropertyNameId.ToString()},");
            sb.AppendLine($"  \"dimension\": {m_Dimension.ToString()},");
            sb.AppendLine($"  \"textureType\": {m_TextureType.ToString()}");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
