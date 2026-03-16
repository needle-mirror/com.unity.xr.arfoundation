using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    class UpdatableTexture2D : IUpdatableTexture
    {
        const bool k_TextureHasLinearColorSpace = false;

        XRTextureDescriptor IUpdatableTexture.descriptor => m_Descriptor;
        XRTextureDescriptor m_Descriptor;

        Texture IUpdatableTexture.texture => m_Texture;
        Texture2D m_Texture;

        internal UpdatableTexture2D(XRTextureDescriptor descriptor)
        {
            if (descriptor.textureType != XRTextureType.Texture2D)
                throw new ArgumentException($"Expected Texture2D but descriptor was of type {descriptor.textureType}");

            m_Descriptor = descriptor;
            m_Texture = CreateTextureFromDescriptor(descriptor);
        }

        static Texture2D CreateTextureFromDescriptor(XRTextureDescriptor descriptor)
        {
            return Texture2D.CreateExternalTexture(
                width: descriptor.width,
                height: descriptor.height,
                format: descriptor.format,
                mipChain: descriptor.mipmapCount > 1,
                linear: k_TextureHasLinearColorSpace,
                nativeTex: descriptor.nativeTexture);
        }

        // assumes newDescriptor.valid == true
        bool IUpdatableTexture.TryUpdateFromDescriptor(XRTextureDescriptor newDescriptor)
        {
            if (m_Descriptor == newDescriptor)
                return true;

            if (m_Descriptor.hasIdenticalTextureMetadata(newDescriptor))
            {
                m_Texture.UpdateExternalTexture(newDescriptor.nativeTexture);
                m_Descriptor = newDescriptor;
                return true;
            }

            UnityObjectUtils.Destroy(m_Texture);
            m_Texture = CreateTextureFromDescriptor(newDescriptor);

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
            m_Texture.wrapMode = TextureWrapMode.Clamp;
            m_Texture.filterMode = FilterMode.Bilinear;
            m_Texture.hideFlags = HideFlags.HideAndDontSave;

            m_Descriptor = newDescriptor;
            return true;
        }

        public void DestroyTexture()
        {
            UnityObjectUtils.Destroy(m_Texture);
            m_Descriptor = default;
        }

        void IDisposable.Dispose()
        {
            DestroyTexture();
        }
    }
}
