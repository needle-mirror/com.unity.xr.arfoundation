using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;
#if OPENXR_1_13_OR_NEWER
using UnityEngine.XR.OpenXR.API;
#endif

namespace UnityEngine.XR.ARFoundation
{
    class UpdatableRenderTexture : IUpdatableTexture
    {
        XRTextureDescriptor IUpdatableTexture.descriptor => m_Descriptor;
        XRTextureDescriptor m_Descriptor;

        Texture IUpdatableTexture.texture => m_Texture;
        RenderTexture m_Texture;

        uint m_RenderTextureId;
        bool m_IsCreateRequested;
        bool m_IsCreated;

        internal UpdatableRenderTexture(XRTextureDescriptor descriptor)
        {
#if !OPENXR_1_13_OR_NEWER
            throw new NotSupportedException(
                "Creating a RenderTexture requires OpenXR Plug-in 1.13.0 or newer and only works on OpenXR devices.");
#else
            RequestCreateTexture(descriptor);
#endif // !OPENXR_1_13_OR_NEWER
        }

        void RequestCreateTexture(XRTextureDescriptor newDescriptor)
        {
#if OPENXR_1_13_OR_NEWER
            if (!SubsystemUtils.TryGetLoadedIntegratedSubsystem<XRDisplaySubsystem>(out _))
            {
                Debug.LogError("RenderTexture cannot be created because the XRDisplaySubsystem is not loaded.");
                return;
            }

            if (UnityXRDisplay.CreateTexture(ToUnityXRRenderTextureDesc(newDescriptor), out m_RenderTextureId))
            {
                m_IsCreateRequested = true;
                m_Descriptor = newDescriptor;
            }
            else
            {
                Debug.LogError($"Failed to create texture from descriptor {m_Descriptor}");
            }
#endif // OPENXR_1_13_OR_NEWER
        }

        bool TryRetrieveTexture()
        {
            if (!SubsystemUtils.TryGetLoadedIntegratedSubsystem<XRDisplaySubsystem>(out var displaySubsystem))
            {
                Debug.LogError("RenderTexture cannot be retrieved because the XRDisplaySubsystem is not loaded.");
                return false;
            }

            m_Texture = displaySubsystem.GetRenderTexture(m_RenderTextureId);
            if (m_Texture != null)
                m_IsCreated = true;

            return m_IsCreated;
        }

        bool IUpdatableTexture.TryUpdateFromDescriptor(XRTextureDescriptor newDescriptor)
        {
            if (m_IsCreated && m_Descriptor == newDescriptor)
            {
                return true;
            }
            if (m_IsCreated
                && m_Descriptor.propertyNameId != newDescriptor.propertyNameId
                && m_Descriptor.hasIdenticalTextureMetadata(newDescriptor))
            {
                m_Descriptor = newDescriptor;
                return true;
            }
            if (!m_Descriptor.hasIdenticalTextureMetadata(newDescriptor))
            {
                DestroyTexture();
                RequestCreateTexture(newDescriptor);
                return false;
            }
            if (!m_IsCreated && !m_IsCreateRequested)
            {
                RequestCreateTexture(newDescriptor);
                return false;
            }
            if (!m_IsCreated)
            {
                return TryRetrieveTexture();
            }
            return false;
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
                    throw new NotSupportedException(
                        $"Attempted to convert unsupported TextureFormat {textureFormat} to UnityXRDepthTextureFormat");
            }
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
                    throw new NotSupportedException(
                        $"Attempted to convert unsupported TextureFormat {textureFormat} to UnityXRRenderTextureFormat");
            }
        }
#endif // OPENXR_1_13_OR_NEWER

        public void DestroyTexture()
        {
            UnityObjectUtils.Destroy(m_Texture);
            m_IsCreated = false;
            m_IsCreateRequested = false;
        }

        void IDisposable.Dispose()
        {
            DestroyTexture();
        }
    }
}
