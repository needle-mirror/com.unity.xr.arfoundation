using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Container that pairs a <see cref="UnityEngine.XR.ARSubsystems.XRTextureDescriptor"/> wrapping a native texture
    /// object with a <c>Texture</c> that is created for the native texture object.
    /// </summary>
    class ARTextureInfo : IEquatable<ARTextureInfo>, IDisposable
    {
        IUpdatableTexture m_UpdatableTexture;

        internal XRTextureDescriptor descriptor => m_UpdatableTexture.descriptor;
        internal Texture texture => m_UpdatableTexture.texture;

        internal ARTextureInfo(XRTextureDescriptor descriptor)
        {
            switch (descriptor.textureType)
            {
                case XRTextureType.Texture2D:
                    m_UpdatableTexture = new UpdatableTexture2D(descriptor);
                    return;
                case XRTextureType.Texture3D:
                    m_UpdatableTexture = new UpdatableTexture3D(descriptor);
                    return;
                case XRTextureType.Cube:
                    m_UpdatableTexture = new UpdatableCubemap(descriptor);
                    return;
                case XRTextureType.ColorRenderTexture:
                case XRTextureType.DepthRenderTexture:
                    m_UpdatableTexture = new UpdatableRenderTexture(descriptor);
                    return;
                case XRTextureType.ColorRenderTextureRef:
                case XRTextureType.DepthRenderTextureRef:
                    m_UpdatableTexture = new UpdatableRenderTextureRef(descriptor);
                    return;
                default:
                    throw new NotSupportedException($"Unsupported texture type: {descriptor.textureType}");
            }
        }

        internal void DestroyTexture() => m_UpdatableTexture.DestroyTexture();

        internal bool TryUpdateTextureInfo(XRTextureDescriptor newDescriptor)
            => m_UpdatableTexture.TryUpdateFromDescriptor(newDescriptor);

        public void Dispose() => m_UpdatableTexture.Dispose();

        public bool Equals(ARTextureInfo other) => m_UpdatableTexture.Equals(other.m_UpdatableTexture);
    }
}
