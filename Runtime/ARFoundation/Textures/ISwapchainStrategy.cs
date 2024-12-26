using System;
using Unity.Collections;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    interface ISwapchainStrategy : IDisposable
    {
        /// <summary>
        /// Given an array of texture descriptors, returns <see langword="true"/> if the textures for all descriptors
        /// were successfully updated, and returns the corresponding <see cref="ARTextureInfo"/> objects.
        ///
        /// If any texture was not successfully updated, returns <see langword="false"/>.
        /// </summary>
        internal bool TryUpdateTextureInfosForFrame(
            NativeArray<XRTextureDescriptor> textureDescriptors, out ReadOnlyListSpan<ARTextureInfo> textureInfos);

        /// <summary>
        /// Destroy all textures to free memory.
        /// </summary>
        /// <remarks>
        /// It is expected that <see cref="TryUpdateTextureInfosForFrame"/> can be called again after this method,
        /// in which case textures should be recreated.
        /// </remarks>
        public void DestroyTextures();
    }
}
