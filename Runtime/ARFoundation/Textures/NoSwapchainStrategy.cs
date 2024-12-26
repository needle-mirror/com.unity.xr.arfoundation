using System;
using Unity.Collections;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Swapchain strategy for providers that do not expose a swapchain interface. Only the most recent frame of
    /// texture descriptors is cached. On a new frame, texture infos are updated in place.
    /// </summary>
    class NoSwapchainStrategy : ISwapchainStrategy
    {
        ARTextureInfo[] m_TextureInfos;

        bool ISwapchainStrategy.TryUpdateTextureInfosForFrame(
            NativeArray<XRTextureDescriptor> textureDescriptors, out ReadOnlyListSpan<ARTextureInfo> textureInfos)
        {
            if (m_TextureInfos is null || m_TextureInfos.Length != textureDescriptors.Length)
                ResizeTextureInfos(textureDescriptors);

            bool allRequiredTexturesDoExist = true;

            for (int i = 0; i < m_TextureInfos!.Length; ++i)
            {
                if (!m_TextureInfos[i].TryUpdateTextureInfo(textureDescriptors[i]))
                    allRequiredTexturesDoExist = false;
            }

            textureInfos = new ReadOnlyListSpan<ARTextureInfo>(m_TextureInfos);
            return allRequiredTexturesDoExist;
        }

        void ResizeTextureInfos(NativeArray<XRTextureDescriptor> descriptors)
        {
            var newInfos = new ARTextureInfo[descriptors.Length];
            int numInfosToCopy = 0;

            if (m_TextureInfos != null)
            {
                numInfosToCopy = Mathf.Min(m_TextureInfos.Length, descriptors.Length);
                for (var i = 0; i < numInfosToCopy; ++i)
                {
                    newInfos[i] = m_TextureInfos[i];
                }

                // If we are downsizing, dispose infos that aren't copied to the new array
                for (var i = newInfos.Length; i < numInfosToCopy; ++i)
                {
                    m_TextureInfos[i].Dispose();
                }
            }

            // If we are upsizing or creating a new array, create new infos
            for (var i = numInfosToCopy; i < newInfos.Length; ++i)
            {
                newInfos[i] = new ARTextureInfo(descriptors[i]);
            }

            m_TextureInfos = newInfos;
        }

        public void DestroyTextures()
        {
            if (m_TextureInfos == null)
                return;

            foreach (ARTextureInfo textureInfo in m_TextureInfos)
            {
                textureInfo.Dispose();
            }
        }

        void IDisposable.Dispose()
        {
            DestroyTextures();
        }
    }
}
