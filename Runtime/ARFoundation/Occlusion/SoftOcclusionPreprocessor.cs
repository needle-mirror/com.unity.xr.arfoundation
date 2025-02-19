using System;
using Unity.XR.CoreUtils;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation
{
    class SoftOcclusionPreprocessor : IDisposable
    {
        const string k_PreprocessedDepthTextureName = "_EnvironmentDepthTexturePreprocessed";

        RenderTexture m_PreprocessedDepthTexture;
        RenderTargetSetup m_PreprocessedDepthRenderTargetSetup;
        Material m_PreprocessMaterial;

        /// <remarks>
        /// Assumes that the shader is not null. Do not construct this object with a null shader.
        /// </remarks>
        internal SoftOcclusionPreprocessor(Shader preprocessShader)
        {
            m_PreprocessMaterial = new Material(preprocessShader);
        }

        public void Dispose()
        {
            if (m_PreprocessMaterial != null)
                UnityObjectUtils.Destroy(m_PreprocessMaterial);

            if (m_PreprocessedDepthTexture != null)
                UnityObjectUtils.Destroy(m_PreprocessedDepthTexture);
        }

        void InitializePreprocessedDepthTexture(Texture depthTexture)
        {
            m_PreprocessedDepthTexture =
                new RenderTexture(
                    depthTexture.width, depthTexture.height, GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormat.None)
            {
                dimension = TextureDimension.Tex2DArray,
                volumeDepth = GetViewCount(depthTexture),
                name = nameof(m_PreprocessedDepthTexture),
                depth = 0
            };
            m_PreprocessedDepthTexture.Create();

            m_PreprocessedDepthRenderTargetSetup = new RenderTargetSetup
            {
                color = new[] { m_PreprocessedDepthTexture.colorBuffer },
                depth = m_PreprocessedDepthTexture.depthBuffer,
                depthSlice = -1,
                colorLoad = new[] { RenderBufferLoadAction.DontCare },
                colorStore = new[] { RenderBufferStoreAction.Store },
                depthLoad = RenderBufferLoadAction.DontCare,
                depthStore = RenderBufferStoreAction.DontCare,
                mipLevel = 0,
                cubemapFace = CubemapFace.Unknown
            };

            var preprocessedDepthTexturePropertyId = Shader.PropertyToID(k_PreprocessedDepthTextureName);
            Shader.SetGlobalTexture(preprocessedDepthTexturePropertyId, m_PreprocessedDepthTexture);
        }

        internal void PreprocessDepthTexture(ARExternalTexture externalTexture)
        {
            if (m_PreprocessedDepthTexture == null)
                InitializePreprocessedDepthTexture(externalTexture.texture);

            var prevColorBuffer = Graphics.activeColorBuffer;
            var prevDepthBuffer = Graphics.activeDepthBuffer;

            Graphics.SetRenderTarget(m_PreprocessedDepthRenderTargetSetup);
            if (m_PreprocessMaterial.SetPass(0))
                Graphics.DrawProceduralNow(MeshTopology.Triangles, 3, GetViewCount(externalTexture.texture));

            Graphics.SetRenderTarget(prevColorBuffer, prevDepthBuffer);
        }

        static int GetViewCount(Texture texture)
        {
            if (texture is RenderTexture rt)
                return rt.volumeDepth;

            return 1;
        }
    }
}
