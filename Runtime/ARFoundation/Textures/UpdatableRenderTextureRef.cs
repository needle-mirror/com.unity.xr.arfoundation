using System;
using System.Runtime.InteropServices;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    class UpdatableRenderTextureRef : IUpdatableTexture
    {
        XRTextureDescriptor IUpdatableTexture.descriptor => m_Descriptor;
        XRTextureDescriptor m_Descriptor;

        Texture IUpdatableTexture.texture => m_Texture;
        RenderTexture m_Texture;

        bool m_IsCreated;

        internal UpdatableRenderTextureRef(XRTextureDescriptor descriptor)
        {
            AdoptTexture(descriptor);
        }

        void AdoptTexture(XRTextureDescriptor newDescriptor)
        {
            if (
                newDescriptor.textureType
                is not XRTextureType.ColorRenderTextureRef
                    or XRTextureType.DepthRenderTextureRef
            )
            {
                Debug.LogError(
                    $"Failed to adopt texture from descriptor {m_Descriptor}: not a reference TextureType"
                );
            }

            var textureHandle = GCHandle.FromIntPtr(newDescriptor.nativeTexture);
            m_Texture = (RenderTexture)textureHandle.Target;
            textureHandle.Free();

            // must update the nativeTexture pointer in the descriptor because that is what is
            // sent to the shader.
            var nativeTexturePtr =
                newDescriptor.textureType == XRTextureType.ColorRenderTextureRef
                    ? m_Texture.GetNativeTexturePtr()
                    : m_Texture.GetNativeDepthBufferPtr();
            m_Descriptor = new XRTextureDescriptor(
                nativeTexture: nativeTexturePtr,
                width: newDescriptor.width,
                height: newDescriptor.height,
                mipmapCount: newDescriptor.mipmapCount,
                format: newDescriptor.format,
                propertyNameId: newDescriptor.propertyNameId,
                depth: newDescriptor.depth,
                textureType: newDescriptor.textureType
            );

            m_IsCreated = true;

            return;
        }

        bool IUpdatableTexture.TryUpdateFromDescriptor(XRTextureDescriptor newDescriptor)
        {
            if (!m_IsCreated)
            {
                return false;
            }

            if (
                m_Descriptor.textureType == XRTextureType.ColorRenderTextureRef
                && m_Descriptor.nativeTexture == m_Texture.GetNativeTexturePtr()
            )
            {
                return true;
            }
            if (
                m_Descriptor.textureType == XRTextureType.DepthRenderTextureRef
                && m_Descriptor.nativeTexture == m_Texture.GetNativeDepthBufferPtr()
            )
            {
                return true;
            }

            AdoptTexture(newDescriptor);
            return true;
        }

        public void DestroyTexture()
        {
            m_IsCreated = false;
        }

        void IDisposable.Dispose()
        {
            DestroyTexture();
        }
    }
}
