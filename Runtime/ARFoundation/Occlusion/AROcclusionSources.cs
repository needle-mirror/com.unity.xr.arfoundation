using System;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Flags to form a mask of enabled occlusion sources
    /// </summary>
    [Flags]
    public enum AROcclusionSources
    {
        /// <summary>
        /// No occlusion source is selected
        /// </summary>
        None = 0,

        /// <summary>
        /// Use depth data provided by the runtime
        /// </summary>
        EnvironmentDepth = 1 << 0,

        /// <summary>
        /// Hand meshes are constructed and used to occlude in passthrough rendering.
        /// </summary>
        HandMesh = 1 << 1
    }
}
