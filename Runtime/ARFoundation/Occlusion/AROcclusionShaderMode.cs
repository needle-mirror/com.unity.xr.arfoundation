namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Specifies which global shader keyword should be enabled. It is used
    /// in the shader to select the type of occlusion.
    /// </summary>
    public enum AROcclusionShaderMode
    {
        /// <summary>
        /// Disable global shader keywords for both soft and hard occlusion.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enable a global shader keyword for hard occlusion.
        /// </summary>
        HardOcclusion = 1,

        /// <summary>
        /// Enable a global shader keyword for soft occlusion.
        /// </summary>
        SoftOcclusion = 2
    }
}
