using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Stores the enabled and disabled shader keywords for a material.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Manual/shader-keywords.html"/>
    public struct ShaderKeywords
    {
        /// <summary>
        /// The enabled shader keywords.
        /// </summary>
        public ReadOnlyCollection<string> enabledKeywords { get; }

        /// <summary>
        /// The disabled shader keywords.
        /// </summary>
        public ReadOnlyCollection<string> disabledKeywords { get; }

        /// <summary>
        /// Constructs a <see cref="ShaderKeywords"/>.
        /// </summary>
        /// <param name="enabledKeywords">The enabled shader keywords.</param>
        /// <param name="disabledKeywords">The disabled shader keywords.</param>
        public ShaderKeywords(ReadOnlyCollection<string> enabledKeywords = null, ReadOnlyCollection<string> disabledKeywords = null)
        {
            this.enabledKeywords = enabledKeywords;
            this.disabledKeywords = disabledKeywords;
        }
    }
}
