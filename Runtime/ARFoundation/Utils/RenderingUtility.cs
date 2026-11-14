using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    static class RenderingUtility
    {
        /// <summary>
        /// Name of the shader parameter for the camera forward scale.
        /// </summary>
        internal const string k_CameraForwardScaleName = "_UnityCameraForwardScale";

        /// <summary>
        /// Name of the shader parameter for the display transform matrix.
        /// </summary>
        internal const string k_DisplayTransformName = "_UnityDisplayTransform";

        internal static void SetShaderKeywords(Material material, XRShaderKeywords keywords)
        {
            if (keywords.enabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.enabledKeywords)
                    material.EnableKeyword(shaderKeyword);
            }

            if (keywords.disabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.disabledKeywords)
                {
                    if (material.IsKeywordEnabled(shaderKeyword))
                        material.DisableKeyword(shaderKeyword);
                }
            }
        }

        internal static void SetShaderKeywordsGlobal(XRShaderKeywords keywords)
        {
            if (keywords.enabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.enabledKeywords)
                    Shader.EnableKeyword(shaderKeyword);
            }

            if (keywords.disabledKeywords != null)
            {
                foreach (var shaderKeyword in keywords.disabledKeywords)
                {
                    if (Shader.IsKeywordEnabled(shaderKeyword))
                        Shader.DisableKeyword(shaderKeyword);
                }
            }
        }
    }
}
