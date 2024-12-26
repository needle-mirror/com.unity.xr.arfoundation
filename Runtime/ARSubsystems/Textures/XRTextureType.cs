using UnityEngine.Rendering;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents a texture's dimension and whether it was created as a `RenderTexture`.
    ///
    /// This enum extends [TextureDimension](xref:UnityEngine.Rendering.TextureDimension) by removing values that are
    /// not supported by AR Foundation and adding values to represent render textures.
    /// </summary>
    public enum XRTextureType
    {
        /// <summary>
        /// The texture type is unknown.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// There is no texture type. This enum may be uninitialized.
        /// </summary>
        None = 0,

        /// <summary>
        /// The texture is of type [Texture2D](xref:UnityEngine.Texture2D).
        /// </summary>
        Texture2D = 1,

        /// <summary>
        /// The texture is of type [Texture3D](xref:UnityEngine.Texture3D).
        /// </summary>
        Texture3D = 2,

        /// <summary>
        /// The texture is of type [Cubemap](xref:UnityEngine.Cubemap).
        /// </summary>
        Cube = 3,

        /// <summary>
        /// The texture is of type [RenderTexture](xref:UnityEngine.RenderTexture) with texture data assigned to
        /// [colorBuffer](xref:UnityEngine.RenderTexture.colorBuffer).
        /// </summary>
        ColorRenderTexture = 4,

        /// <summary>
        /// The texture is of type [RenderTexture](xref:UnityEngine.RenderTexture) with texture data assigned to
        /// [depthBuffer](xref:UnityEngine.RenderTexture.depthBuffer).
        /// </summary>
        DepthRenderTexture = 5,

        /// <summary>
        /// A reference to an existing [RenderTexture](xref:UnityEngine.RenderTexture) with texture data assigned to
        /// [colorBuffer](xref:UnityEngine.RenderTexture.colorBuffer).
        /// </summary>
        ColorRenderTextureRef = 6,

        /// <summary>
        /// A reference to an existing [RenderTexture](xref:UnityEngine.RenderTexture) with texture data assigned to
        /// [depthBuffer](xref:UnityEngine.RenderTexture.depthBuffer).
        /// </summary>
        DepthRenderTextureRef = 7,
    }

    /// <summary>
    /// Extension methods for <see cref="XRTextureType"/>.
    /// </summary>
    public static class XRTextureTypeExtensions
    {
        /// <summary>
        /// Indicates whether the texture type is represented in Unity as a [RenderTexture](UnityEngine.RenderTexture).
        /// </summary>
        /// <param name="textureType">The texture type.</param>
        /// <returns><see langword="true"/>> if the texture type is represented in Unity as a render texture.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool IsRenderTexture(this XRTextureType textureType)
        {
            return textureType
                is XRTextureType.ColorRenderTexture
                    or XRTextureType.DepthRenderTexture
                    or XRTextureType.ColorRenderTextureRef
                    or XRTextureType.DepthRenderTextureRef;
        }

        /// <summary>
        /// Converts a given `XRTextureType` to a `TextureDimension`.
        /// </summary>
        /// <param name="textureType">A texture type to be converted.</param>
        /// <returns>An equivalent `TextureDimension`.</returns>
        public static TextureDimension ToTextureDimension(this XRTextureType textureType)
        {
            switch (textureType)
            {
                case XRTextureType.None:
                    return TextureDimension.None;
                case XRTextureType.Texture2D:
                    return TextureDimension.Tex2D;
                case XRTextureType.Texture3D:
                    return TextureDimension.Tex3D;
                case XRTextureType.Cube:
                    return TextureDimension.Cube;
                case XRTextureType.ColorRenderTexture:
                case XRTextureType.DepthRenderTexture:
                case XRTextureType.ColorRenderTextureRef:
                case XRTextureType.DepthRenderTextureRef:
                    return TextureDimension.Any;
                default:
                    return TextureDimension.Unknown;
            }
        }

        /// <summary>
        /// Converts a given `TextureDimension` to an `XRTextureType`.
        /// </summary>
        /// <param name="dimension">A texture dimension to be converted.</param>
        /// <returns>An equivalent `XRTextureType`.</returns>
        /// <remarks>
        /// AR Foundation supports the following texture dimensions:
        /// * `Tex2D`
        /// * `Tex3D`
        /// * `Cube`.
        /// </remarks>
        public static XRTextureType ToXRTextureType(this TextureDimension dimension)
        {
            switch (dimension)
            {
                case TextureDimension.None:
                    return XRTextureType.None;
                case TextureDimension.Tex2D:
                    return XRTextureType.Texture2D;
                case TextureDimension.Tex3D:
                    return XRTextureType.Texture3D;
                case TextureDimension.Cube:
                    return XRTextureType.Cube;
                default:
                    return XRTextureType.Unknown;
            }
        }
    }
}
