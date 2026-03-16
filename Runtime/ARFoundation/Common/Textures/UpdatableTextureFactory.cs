using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    static class UpdatableTextureFactory
    {
        internal static IUpdatableTexture Create(XRTextureDescriptor descriptor)
        {
            return descriptor.textureType switch
            {
                XRTextureType.Texture2D => new UpdatableTexture2D(descriptor),
                XRTextureType.Texture3D => new UpdatableTexture3D(descriptor),
                XRTextureType.Cube => new UpdatableCubemap(descriptor),
                XRTextureType.ColorRenderTexture or XRTextureType.DepthRenderTexture => new UpdatableRenderTexture(descriptor),
                XRTextureType.ColorRenderTextureRef or XRTextureType.DepthRenderTextureRef => new UpdatableRenderTextureRef(descriptor),
                _ => throw new NotSupportedException($"Unsupported texture type: {descriptor.textureType}")
            };
        }
    }
}
