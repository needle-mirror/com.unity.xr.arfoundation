using System.ComponentModel;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the preference for how to occlude.
    /// </summary>
    public enum OcclusionPreferenceMode
    {
        /// <summary>
        /// The preference is to occlude using environment depth.
        /// </summary>
        PreferEnvironmentOcclusion = 0,

        /// <summary>
        /// The preference is to occlude using human segmentation stencil and depth.
        /// </summary>
        PreferHumanOcclusion = 1,

        /// <summary>
        /// The preference is to not occlude.
        /// </summary>
        NoOcclusion = 2,
    }
}
