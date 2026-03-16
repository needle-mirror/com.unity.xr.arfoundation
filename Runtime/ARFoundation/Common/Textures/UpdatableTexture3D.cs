using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    class UpdatableTexture3D : IUpdatableTexture
    {
        XRTextureDescriptor IUpdatableTexture.descriptor => m_Descriptor;
        XRTextureDescriptor m_Descriptor;

        Texture IUpdatableTexture.texture => m_Texture;
        Texture3D m_Texture;

        internal UpdatableTexture3D(XRTextureDescriptor descriptor)
        {
            if (descriptor.textureType != XRTextureType.Texture3D)
                throw new ArgumentException($"Expected Texture3D but descriptor was of type {descriptor.textureType}");

            m_Descriptor = descriptor;
            m_Texture = CreateTextureFromDescriptor(descriptor);
        }

        static Texture3D CreateTextureFromDescriptor(XRTextureDescriptor descriptor)
        {
            return Texture3D.CreateExternalTexture(
                width: descriptor.width,
                height: descriptor.height,
                depth: descriptor.depth,
                format: descriptor.format,
                mipChain: descriptor.mipmapCount > 1,
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
