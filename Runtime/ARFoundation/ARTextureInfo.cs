using System;
using Unity.XR.CoreUtils;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;
#if OPENXR_1_13_OR_NEWER
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.OpenXR.API;
#endif

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Container that pairs a <see cref="UnityEngine.XR.ARSubsystems.XRTextureDescriptor"/> wrapping a native texture
    /// object with a <c>Texture</c> that is created for the native texture object.
    /// </summary>
    struct ARTextureInfo : IEquatable<ARTextureInfo>, IDisposable
    {
        /// <summary>
        /// Indicates whether the texture is in linear color space.
        /// </summary>
        const bool k_TextureHasLinearColorSpace = false;

        /// <summary>
        /// The texture descriptor describing the metadata for the native texture object.
        /// </summary>
        /// <value>The texture descriptor.</value>
        internal XRTextureDescriptor descriptor => m_Descriptor;
        XRTextureDescriptor m_Descriptor;

        /// <summary>
        /// The Unity <c>Texture</c> object for the native texture.
        /// </summary>
        /// <value>The texture.</value>
        internal Texture texture => m_Texture;
        Texture m_Texture;

        /// <summary>
        /// Constructs the texture info with the given descriptor and material.
        /// </summary>
        /// <param name="descriptor">The texture descriptor wrapping a native texture object.</param>
        internal ARTextureInfo(XRTextureDescriptor descriptor)
        {
            m_Descriptor = descriptor;
            m_Texture = CreateTexture(descriptor);
        }

        /// <summary>
        /// Resets the texture info back to the default state destroying the texture GameObject, if one exists.
        /// </summary>
        internal void Reset()
        {
            m_Descriptor.Reset();
            DestroyTexture();
        }

        /// <summary>
        /// Destroys the texture and sets the property to <c>null</c>.
        /// </summary>
        void DestroyTexture()
        {
            if (m_Texture != null)
            {
                UnityObjectUtils.Destroy(m_Texture);
                m_Texture = null;
            }
        }

        /// <summary>
        /// Sets the current descriptor and creates/updates the associated texture as appropriate.
        /// </summary>
        /// <param name="textureInfo">The texture information to update.</param>
        /// <param name="descriptor">The updated texture descriptor from which to copy updated data.</param>
        /// <returns>The updated texture information.</returns>
        internal static ARTextureInfo GetUpdatedTextureInfo(ARTextureInfo textureInfo, XRTextureDescriptor descriptor)
        {
            if (textureInfo.m_Descriptor.Equals(descriptor))
                return textureInfo;

            if (!descriptor.valid)
            {
                textureInfo.DestroyTexture();
                return default;
            }

            DebugWarn.WhenFalse(textureInfo.m_Descriptor.textureType == descriptor.textureType)?.
                WithMessage($"Texture type should not change from {textureInfo.m_Descriptor.textureType} to {descriptor.textureType}.");

            textureInfo.m_Descriptor = descriptor;

            // If there is a texture already and if the descriptors have identical texture metadata, we only need
            // to update the existing texture with the given native texture object.
            if (textureInfo.m_Texture != null &&
                textureInfo.m_Descriptor.textureType is not (XRTextureType.ColorRenderTexture or XRTextureType.DepthRenderTexture) &&
                textureInfo.m_Descriptor.hasIdenticalTextureMetadata(descriptor))
            {
                switch (descriptor.textureType)
                {
                    case XRTextureType.Texture3D:
                        ((Texture3D)textureInfo.m_Texture).UpdateExternalTexture(textureInfo.m_Descriptor.nativeTexture);
                        break;
                    case XRTextureType.Texture2D:
                        ((Texture2D)textureInfo.m_Texture).UpdateExternalTexture(textureInfo.m_Descriptor.nativeTexture);
                        break;
                    case XRTextureType.Cube:
                        ((Cubemap)textureInfo.m_Texture).UpdateExternalTexture(textureInfo.m_Descriptor.nativeTexture);
                        break;
                    default:
                        throw new NotSupportedException($"'{descriptor.textureType.ToString()}' is not a supported texture type.");
                }
            }
            // Else, we need to destroy the existing texture object and create a new texture object.
            else
            {
                textureInfo.DestroyTexture();
                textureInfo.m_Texture = CreateTexture(textureInfo.m_Descriptor);
            }

            return textureInfo;
        }

        /// <summary>
        /// Create the texture object for the native texture wrapped by the valid descriptor.
        /// </summary>
        /// <param name="descriptor">The texture descriptor wrapping a native texture object.</param>
        /// <returns>If the descriptor is valid, the <c>Texture</c> object created from the texture descriptor.
        /// Otherwise, <c>null</c>.</returns>
        static Texture CreateTexture(XRTextureDescriptor descriptor)
        {
            if (!descriptor.valid)
                return null;

            switch (descriptor.textureType)
            {
                case XRTextureType.Texture3D:
                    return Texture3D.CreateExternalTexture(
                        width: descriptor.width,
                        height: descriptor.height,
                        depth: descriptor.depth,
                        format: descriptor.format,
                        mipChain: descriptor.mipmapCount > 1,
                        nativeTex: descriptor.nativeTexture);
                case XRTextureType.Texture2D:
                    var texture = Texture2D.CreateExternalTexture(
                        width: descriptor.width,
                        height: descriptor.height,
                        format: descriptor.format,
                        mipChain: descriptor.mipmapCount > 1,
                        linear: k_TextureHasLinearColorSpace,
                        nativeTex: descriptor.nativeTexture);

                    // NB: SetWrapMode needs to be the first call here, and the value passed
                    //     needs to be kTexWrapClamp - this is due to limitations of what
                    //     wrap modes are allowed for external textures in OpenGL (which are
                    //     used for ARCore), as Texture::ApplySettings will eventually hit
                    //     an assert about an invalid enum (see calls to glTexParameteri
                    //     towards the top of ApiGLES::TextureSampler)
                    // reference: "3.7.14 External Textures" section of
                    // https://www.khronos.org/registry/OpenGL/extensions/OES/OES_EGL_image_external.txt
                    // (it shouldn't ever matter what the wrap mode is set to normally, since
                    // this is for a pass-through video texture, so we shouldn't ever need to
                    // worry about the wrap mode as textures should never "wrap")
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Bilinear;
                    texture.hideFlags = HideFlags.HideAndDontSave;
                    return texture;
                case XRTextureType.Cube:
                    return Cubemap.CreateExternalTexture(
                        width: descriptor.width,
                        format: descriptor.format,
                        mipmap: descriptor.mipmapCount > 1,
                        descriptor.nativeTexture);
                case XRTextureType.ColorRenderTexture:
                case XRTextureType.DepthRenderTexture:
                    return CreateRenderTexture(descriptor);
                default:
                    return null;
            }
        }

        static RenderTexture CreateRenderTexture(XRTextureDescriptor descriptor)
        {
#if OPENXR_1_13_OR_NEWER
            if (!SubsystemUtils.TryGetLoadedIntegratedSubsystem<XRDisplaySubsystem>(out var displaySubsystem))
            {
                Debug.LogError("RenderTexture cannot be created because the XRDisplaySubsystem is not loaded.");
                return null;
            }

            if (!UnityXRDisplay.CreateTexture(ToUnityXRRenderTextureDesc(descriptor), out var renderTextureID))
            {
                Debug.LogError($"Failed to create texture from descriptor {descriptor}");
                return null;
            }

            return displaySubsystem.GetRenderTexture(renderTextureID);
#else
            Debug.LogError("Creating a RenderTexture requires OpenXR Plug-in 1.13.0 or newer. Add the OpenXR Plug-in package to your project.");
            return null;
#endif // OPENXR_1_13_OR_NEWER
        }

#if OPENXR_1_13_OR_NEWER
        /// <summary>
        /// Creates and returns an OpenXR `UnityXRRenderTextureDesc` with the data of the given texture descriptor.
        /// Because the data types are not identical, the conversion may be imprecise and the texture formats may
        /// not match exactly.
        /// </summary>
        /// <returns>An <see cref="UnityXRRenderTextureDesc"/> matching this object as closely as possible.</returns>
        static UnityXRRenderTextureDesc ToUnityXRRenderTextureDesc(XRTextureDescriptor descriptor)
        {
            var renderTextureDescriptor = new UnityXRRenderTextureDesc
            {
                shadingRateFormat = UnityXRShadingRateFormat.kUnityXRShadingRateFormatNone,
                shadingRate = new UnityXRTextureData(),
                width = (uint)descriptor.width,
                height = (uint)descriptor.height,
                textureArrayLength = (uint)descriptor.depth,
                flags = 0,
                colorFormat = UnityXRRenderTextureFormat.kUnityXRRenderTextureFormatNone,
                depthFormat = UnityXRDepthTextureFormat.kUnityXRDepthTextureFormatNone
            };

            switch (descriptor.textureType)
            {
                case XRTextureType.DepthRenderTexture:
                    renderTextureDescriptor.depthFormat = ToUnityXRDepthTextureFormat(descriptor.format);
                    renderTextureDescriptor.depth = new UnityXRTextureData
                    {
                        nativePtr = descriptor.nativeTexture
                    };
                    break;
                case XRTextureType.ColorRenderTexture:
                    renderTextureDescriptor.colorFormat = ToUnityXRRenderTextureFormat(descriptor.format);
                    renderTextureDescriptor.color = new UnityXRTextureData
                    {
                        nativePtr = descriptor.nativeTexture
                    };
                    break;
            }

            return renderTextureDescriptor;
        }

        static UnityXRRenderTextureFormat ToUnityXRRenderTextureFormat(TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.RGBA32:
                    return UnityXRRenderTextureFormat.kUnityXRRenderTextureFormatRGBA32;
                case TextureFormat.BGRA32:
                    return UnityXRRenderTextureFormat.kUnityXRRenderTextureFormatBGRA32;
                case TextureFormat.RGB565:
                    return UnityXRRenderTextureFormat.kUnityXRRenderTextureFormatRGB565;
                case TextureFormat.RGBAHalf:
                    return UnityXRRenderTextureFormat.kUnityXRRenderTextureFormatR16G16B16A16_SFloat;
                default:
                    Debug.LogWarning($"Attempted to convert unsupported TextureFormat {textureFormat} to UnityXRRenderTextureFormat");
                    return UnityXRRenderTextureFormat.kUnityXRRenderTextureFormatNone;
            }
        }

        static UnityXRDepthTextureFormat ToUnityXRDepthTextureFormat(TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.RFloat:
                    return UnityXRDepthTextureFormat.kUnityXRDepthTextureFormat24bitOrGreater;
                case TextureFormat.R16:
                case TextureFormat.RHalf:
                    return UnityXRDepthTextureFormat.kUnityXRDepthTextureFormat16bit;
                default:
                    Debug.LogWarning($"Attempted to convert unsupported TextureFormat {textureFormat} to UnityXRDepthTextureFormat");
                    return UnityXRDepthTextureFormat.kUnityXRDepthTextureFormatNone;
            }
        }
#endif // OPENXR_1_13_OR_NEWER

        internal static bool IsSupported(XRTextureDescriptor descriptor)
        {
            switch (descriptor.textureType)
            {
                case XRTextureType.ColorRenderTexture:
                case XRTextureType.DepthRenderTexture:
#if OPENXR_1_13_OR_NEWER
                    return true;
#else
                    return false;
#endif // OPENXR_1_13_OR_NEWER
                case XRTextureType.Texture3D:
                case XRTextureType.Texture2D:
                case XRTextureType.Cube:
                    return true;
                case XRTextureType.Unknown:
                default:
                    return false;
            }
        }

        public void Dispose() => DestroyTexture();

        public override int GetHashCode()
        {
            var hash = 486187739;
            unchecked
            {
                hash = hash * 486187739 + m_Descriptor.GetHashCode();
                hash = hash * 486187739 + (m_Texture == null ? 0 : m_Texture.GetHashCode());
            }

            return hash;
        }

        public bool Equals(ARTextureInfo other) => m_Descriptor.Equals(other.descriptor) && m_Texture == other.m_Texture;

        public override bool Equals(object obj) => obj is ARTextureInfo info && Equals(info);

        public static bool operator ==(ARTextureInfo lhs, ARTextureInfo rhs) => lhs.Equals(rhs);

        public static bool operator !=(ARTextureInfo lhs, ARTextureInfo rhs) => !lhs.Equals(rhs);
    }
}
