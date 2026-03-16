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
        IUpdatableTexture[] m_UpdatableTextures;

        bool ISwapchainStrategy.TryUpdateTexturesForFrame(
            NativeArray<XRTextureDescriptor> textureDescriptors, out ReadOnlyListSpan<IUpdatableTexture> updatableTextures)
        {
            if (m_UpdatableTextures is null || m_UpdatableTextures.Length != textureDescriptors.Length)
                ResizeTextureInfos(textureDescriptors);

            bool allRequiredTexturesDoExist = true;

            for (int i = 0; i < m_UpdatableTextures!.Length; ++i)
            {
                if (!m_UpdatableTextures[i].TryUpdateFromDescriptor(textureDescriptors[i]))
                    allRequiredTexturesDoExist = false;
            }

            updatableTextures = new ReadOnlyListSpan<IUpdatableTexture>(m_UpdatableTextures);
            return allRequiredTexturesDoExist;
        }

        void ResizeTextureInfos(NativeArray<XRTextureDescriptor> descriptors)
        {
            var newInfos = new IUpdatableTexture[descriptors.Length];
            int numInfosToCopy = 0;

            if (m_UpdatableTextures != null)
            {
                numInfosToCopy = Mathf.Min(m_UpdatableTextures.Length, descriptors.Length);
                for (var i = 0; i < numInfosToCopy; ++i)
                {
                    newInfos[i] = m_UpdatableTextures[i];
                }

                // If we are downsizing, dispose infos that aren't copied to the new array
                for (var i = newInfos.Length; i < numInfosToCopy; ++i)
                {
                    m_UpdatableTextures[i].Dispose();
                }
            }

            // If we are upsizing or creating a new array, create new infos
            for (var i = numInfosToCopy; i < newInfos.Length; ++i)
            {
                newInfos[i] = UpdatableTextureFactory.Create(descriptors[i]);
            }

            m_UpdatableTextures = newInfos;
        }

        public void DestroyTextures()
        {
            if (m_UpdatableTextures == null)
                return;

            foreach (IUpdatableTexture updatableTexture in m_UpdatableTextures)
            {
                updatableTexture.Dispose();
            }
        }

        void IDisposable.Dispose()
        {
            DestroyTextures();
        }
    }
}
