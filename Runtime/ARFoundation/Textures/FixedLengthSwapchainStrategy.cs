using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Swapchain strategy for providers that expose a fixed-length swapchain of fixed-size textures.
    /// </summary>
    /// <remarks>
    /// This strategy is especially useful for providers that represent textures using `RenderTexture`, as it
    /// allows you to create all the RenderTextures when the swapchain is initialized and re-use them for the duration
    /// of the app's lifecycle.
    /// </remarks>
    class FixedLengthSwapchainStrategy : ISwapchainStrategy
    {
        ARTextureInfo[][] m_TextureInfos;
        readonly Dictionary<IntPtr, int> m_TextureInfoIndicesByNativePtr = new();
        readonly int m_NumTexturesPerFrame;

        internal FixedLengthSwapchainStrategy(NativeArray<NativeArray<XRTextureDescriptor>> swapchainDescriptors)
        {
            if (swapchainDescriptors.Length == 0
                || swapchainDescriptors[0].Length == 0)
                throw new ArgumentException("Swapchain must contain at least one texture", nameof(swapchainDescriptors));

            int swapchainSize = swapchainDescriptors.Length;
            m_NumTexturesPerFrame = swapchainDescriptors[0].Length;
            m_TextureInfos = new ARTextureInfo[swapchainSize][];

            for (var i = 0; i < swapchainSize; ++i)
            {
                if (swapchainDescriptors[i].Length != m_NumTexturesPerFrame)
                    throw new ArgumentException(
                        "Swapchain must use the same number of textures per frame", nameof(swapchainDescriptors));

                m_TextureInfos[i] = new ARTextureInfo[m_NumTexturesPerFrame];
                for (var j = 0; j < m_NumTexturesPerFrame; ++j)
                {
                    if (swapchainDescriptors[i][j].nativeTexture == IntPtr.Zero)
                        throw new ArgumentException("Swapchain textures must have non-null native texture pointer.", nameof(swapchainDescriptors));

                    // Create all render textures if necessary
                    m_TextureInfos[i][j] = new ARTextureInfo(swapchainDescriptors[i][j]);

                    m_TextureInfoIndicesByNativePtr.Add(swapchainDescriptors[i][j].nativeTexture, i);
                }
            }
        }

        bool ISwapchainStrategy.TryUpdateTextureInfosForFrame(
            NativeArray<XRTextureDescriptor> descriptors, out ReadOnlyListSpan<ARTextureInfo> textureInfos)
        {
            if (descriptors.Length == 0)
            {
                textureInfos = ReadOnlyListSpan<ARTextureInfo>.Empty();
                return true;
            }

            if (!m_TextureInfoIndicesByNativePtr.TryGetValue(descriptors[0].nativeTexture, out var frameIndex))
                throw new InvalidOperationException(
                    "Fixed-length swapchain strategy was given a texture that is not in the swapchain.");

            if (descriptors.Length != m_NumTexturesPerFrame)
                throw new InvalidOperationException(
                    $"Fixed-length swapchain uses {m_NumTexturesPerFrame} textures per frame, but was given {descriptors.Length} textures this frame.");

            bool allRequiredTexturesDoExist = true;
            for (var j = 0; j < m_NumTexturesPerFrame; j++)
            {
                if (!m_TextureInfos[frameIndex][j].TryUpdateTextureInfo(descriptors[j]))
                    allRequiredTexturesDoExist = false;
            }

            textureInfos = new ReadOnlyListSpan<ARTextureInfo>(m_TextureInfos[frameIndex]);
            return allRequiredTexturesDoExist;
        }

        public void DestroyTextures()
        {
            foreach (var frameArray in m_TextureInfos)
            {
                foreach (var textureInfo in frameArray)
                {
                    textureInfo.DestroyTexture();
                }
            }
        }

        void IDisposable.Dispose()
        {
            DestroyTextures();
        }
    }
}
