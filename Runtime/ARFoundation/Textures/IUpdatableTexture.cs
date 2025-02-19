using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    interface IUpdatableTexture : IEquatable<IUpdatableTexture>, IDisposable
    {
        internal XRTextureDescriptor descriptor { get; }

        internal Texture texture { get; }

        public bool TryUpdateFromDescriptor(XRTextureDescriptor descriptor);

        public void DestroyTexture();

        bool IEquatable<IUpdatableTexture>.Equals(IUpdatableTexture other)
        {
            return other != null
                && descriptor == other.descriptor
                && texture == other.texture;
        }
    }
}
