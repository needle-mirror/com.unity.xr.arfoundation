namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Arguments <see cref="AROcclusionSourceEventArgs"/> that are passed along event when occlusion source was set;
    /// </summary>
    public struct AROcclusionSourceEventArgs
    {
        /// <summary>
        /// Flags to form a mask of enabled occlusion sources
        /// </summary>
        public AROcclusionSources occlusionSources { get; }

        /// <summary>
        /// A material which is being used on hands, when hands occlusion is enabled.
        /// </summary>
        public Material handsMaterial { get; }

        /// <summary>
        /// Constructor invoked by the <see cref="ARShaderOcclusion"/> which triggered the event.
        /// </summary>
        /// <param name="occlusionSources">Flags to form a mask <see cref="AROcclusionSources"/> of enabled
        /// occlusion sources.</param>
        /// <param name="handsMaterial">A material used for hands occlusion.</param>
        public AROcclusionSourceEventArgs(AROcclusionSources occlusionSources, Material handsMaterial)
        {
            this.occlusionSources = occlusionSources;
            this.handsMaterial = handsMaterial;
        }
    }
}
