using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    class UpdatableRenderTextureRef : IUpdatableTexture
    {
        XRTextureDescriptor IUpdatableTexture.descriptor => m_Descriptor;
        XRTextureDescriptor m_Descriptor;

        Texture IUpdatableTexture.texture => m_Texture;
        RenderTexture m_Texture;

        IntPtr m_RenderTexturePtr;

        internal UpdatableRenderTextureRef(XRTextureDescriptor descriptor)
        {
            if (!TryUpdateFromDescriptor(descriptor))
                throw new ArgumentException(nameof(descriptor));
        }

        public bool TryUpdateFromDescriptor(XRTextureDescriptor newDescriptor)
        {
            if (newDescriptor.textureType is not (XRTextureType.ColorRenderTextureRef or XRTextureType.DepthRenderTextureRef))
            {
                Debug.LogError($"UpdatableRenderTextureRef cannot be created with invalid texture type: {m_Descriptor.textureType}");
                return false;
            }

            if (newDescriptor.textureType == XRTextureType.ColorRenderTextureRef
                && newDescriptor.hasIdenticalTextureMetadata(m_Descriptor)
                && newDescriptor.nativeTexture == m_RenderTexturePtr)
                return true;

            if (newDescriptor.textureType == XRTextureType.DepthRenderTextureRef
                && newDescriptor.hasIdenticalTextureMetadata(m_Descriptor)
                && newDescriptor.nativeTexture == m_RenderTexturePtr)
                return true;

            // NOTE: newDescriptor.nativeTexture is not actually a pointer to a native texture in this class.
            // It's a pointer to a RenderTexture, from which we extract the real native texture pointer via GCHandle.
            // Thus, for equality comparison with texture descriptors on future frames, we separately save the
            // RenderTexture pointer so that we don't need to create a GCHandle to check for equality.
            m_RenderTexturePtr = newDescriptor.nativeTexture;
            var textureHandle = GCHandle.FromIntPtr(m_RenderTexturePtr);
            m_Texture = (RenderTexture)textureHandle.Target;
            textureHandle.Free();

            var nativeTexturePtr = newDescriptor.textureType == XRTextureType.ColorRenderTextureRef
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

            return true;
        }

        public void DestroyTexture()
        {
            // Do nothing. This class cannot destroy the RenderTexture that it references.
        }

        void IDisposable.Dispose()
        {
            // Do nothing.
        }
    }
}
