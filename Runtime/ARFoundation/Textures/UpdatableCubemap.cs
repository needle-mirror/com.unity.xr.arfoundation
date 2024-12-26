using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    class UpdatableCubemap : IUpdatableTexture
    {
        XRTextureDescriptor IUpdatableTexture.descriptor => m_Descriptor;
        XRTextureDescriptor m_Descriptor;

        Texture IUpdatableTexture.texture => m_Texture;
        Cubemap m_Texture;

        internal UpdatableCubemap(XRTextureDescriptor descriptor)
        {
            if (descriptor.textureType != XRTextureType.Cube)
                throw new ArgumentException($"Expected Cubemap but descriptor was of type {descriptor.textureType}");

            m_Descriptor = descriptor;
            m_Texture = CreateTextureFromDescriptor(descriptor);
        }

        static Cubemap CreateTextureFromDescriptor(XRTextureDescriptor descriptor)
        {
            return Cubemap.CreateExternalTexture(
                width: descriptor.width,
                format: descriptor.format,
                mipmap: descriptor.mipmapCount > 1,
                nativeTex: descriptor.nativeTexture);
        }

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
