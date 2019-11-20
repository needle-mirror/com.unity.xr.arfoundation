using System;
using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A structure for occlusion information pertaining to a particular frame. This is used to communicate information
    /// in the <see cref="AROcclusionManager.frameReceived" /> event.
    /// </summary>
    public struct AROcclusionFrameEventArgs : IEquatable<AROcclusionFrameEventArgs>
    {
        /// <summary>
        /// The occlusion textures associated with this frame. These are generally external textures, which exist only
        /// on the GPU. To use them on the CPU, e.g., for computer vision processing, you will need to read them back
        /// from the GPU.
        /// </summary>
        public List<Texture2D> textures { get; internal set; }

        /// <summary>
        /// Ids of the property name associated with each texture. This is a
        /// parallel <c>List</c> to the <see cref="textures"/> list.
        /// </summary>
        public List<int> propertyNameIds { get; internal set; }

        /// <summary>
        /// The list of keywords to be enabled for the material.
        /// </summary>
        public List<string> enabledMaterialKeywords { get; internal set; }

        /// <summary>
        /// The list of keywords to be disabled for the material.
        /// </summary>
        public List<string> disabledMaterialKeywords { get; internal set; }

        public override int GetHashCode()
        {
            int hash = 486187739;
            unchecked
            {
                hash = hash * 486187739 + (textures == null ? 0 : textures.GetHashCode());
                hash = hash * 486187739 + (propertyNameIds == null ? 0 : propertyNameIds.GetHashCode());
                hash = hash * 486187739 + (enabledMaterialKeywords == null ? 0 : enabledMaterialKeywords.GetHashCode());
                hash = hash * 486187739 + (disabledMaterialKeywords == null ? 0 : disabledMaterialKeywords.GetHashCode());
            }
            return hash;
        }

        public override bool Equals(object obj)
            => (obj is AROcclusionFrameEventArgs) && Equals((AROcclusionFrameEventArgs)obj);

        public bool Equals(AROcclusionFrameEventArgs other)
            => (((textures == null) ? (other.textures == null) : textures.Equals(other.textures))
                && ((propertyNameIds == null) ? (other.propertyNameIds == null)
                    : propertyNameIds.Equals(other.propertyNameIds))
                && ((enabledMaterialKeywords == null) ? (other.enabledMaterialKeywords == null)
                    : enabledMaterialKeywords.Equals(other.enabledMaterialKeywords))
                && ((disabledMaterialKeywords == null) ? (other.disabledMaterialKeywords == null)
                    : disabledMaterialKeywords.Equals(other.disabledMaterialKeywords)));

        public static bool operator ==(AROcclusionFrameEventArgs lhs, AROcclusionFrameEventArgs rhs) => lhs.Equals(rhs);

        public static bool operator !=(AROcclusionFrameEventArgs lhs, AROcclusionFrameEventArgs rhs) => !lhs.Equals(rhs);
    }
}
