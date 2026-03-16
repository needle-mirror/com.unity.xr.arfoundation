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
        /// <summary>
        /// A 2-dimensional array of texture info objects.
        /// Each row represents a frame. Each column represents a texture that is used that frame.
        /// </summary>
        IUpdatableTexture[][] m_UpdatableTextures;

        /// <summary>
        /// Enables a constant-time lookup operation. Given the native pointer of any texture in the swapchain,
        /// look up the index of the frame where that texture is used.
        /// </summary>
        readonly Dictionary<IntPtr, int> m_FrameIndicesByTexturePtr = new();

        readonly int m_NumTexturesPerFrame;

        internal FixedLengthSwapchainStrategy(NativeArray<NativeArray<XRTextureDescriptor>> swapchainDescriptors)
        {
            if (swapchainDescriptors.Length == 0
                || swapchainDescriptors[0].Length == 0)
                throw new ArgumentException("Swapchain must contain at least one texture", nameof(swapchainDescriptors));

            int numFramesInSwapchain = swapchainDescriptors.Length;
            m_NumTexturesPerFrame = swapchainDescriptors[0].Length;
            m_UpdatableTextures = new IUpdatableTexture[numFramesInSwapchain][];

            for (var i = 0; i < numFramesInSwapchain; ++i)
            {
                if (swapchainDescriptors[i].Length != m_NumTexturesPerFrame)
                    throw new ArgumentException(
                        "Swapchain must use the same number of textures per frame", nameof(swapchainDescriptors));

                m_UpdatableTextures[i] = new IUpdatableTexture[m_NumTexturesPerFrame];
                for (var j = 0; j < m_NumTexturesPerFrame; ++j)
                {
                    if (swapchainDescriptors[i][j].nativeTexture == IntPtr.Zero)
                        throw new ArgumentException("Swapchain textures must have non-null native texture pointer.", nameof(swapchainDescriptors));

                    if (m_FrameIndicesByTexturePtr.ContainsKey(swapchainDescriptors[i][j].nativeTexture))
                        throw new ArgumentException("Swapchain native texture pointers must be unique.", nameof(swapchainDescriptors));

                    m_UpdatableTextures[i][j] = UpdatableTextureFactory.Create(swapchainDescriptors[i][j]);
                    m_FrameIndicesByTexturePtr.Add(swapchainDescriptors[i][j].nativeTexture, i);
                }
            }
        }

        bool ISwapchainStrategy.TryUpdateTexturesForFrame(
            NativeArray<XRTextureDescriptor> descriptors, out ReadOnlyListSpan<IUpdatableTexture> textureInfos)
        {
            if (descriptors.Length == 0)
            {
                textureInfos = ReadOnlyListSpan<IUpdatableTexture>.Empty();
                return true;
            }

            if (!m_FrameIndicesByTexturePtr.TryGetValue(descriptors[0].nativeTexture, out var frameIndex))
                throw new InvalidOperationException(
                    "Fixed-length swapchain strategy was given a texture that is not in the swapchain.");

            if (descriptors.Length != m_NumTexturesPerFrame)
                throw new InvalidOperationException(
                    $"Fixed-length swapchain uses {m_NumTexturesPerFrame} textures per frame, but was given {descriptors.Length} textures this frame.");

            bool allRequiredTexturesDoExist = true;
            for (var j = 0; j < m_NumTexturesPerFrame; j++)
            {
                if (!m_UpdatableTextures[frameIndex][j].TryUpdateFromDescriptor(descriptors[j]))
                    allRequiredTexturesDoExist = false;
            }

            textureInfos = new ReadOnlyListSpan<IUpdatableTexture>(m_UpdatableTextures[frameIndex]);
            return allRequiredTexturesDoExist;
        }

        public void DestroyTextures()
        {
            foreach (var frame in m_UpdatableTextures)
            {
                foreach (var textureInfo in frame)
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
