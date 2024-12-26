using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

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
        /// on the GPU. To use them on the CPU (for example, for computer vision processing), you must read them back
        /// from the GPU.
        /// </summary>
        [Obsolete("textures has been deprecated in AR Foundation version 6.1. Use externalTextures instead.")]
        public List<Texture2D> textures
        {
            get
            {
                var list = new List<Texture2D>();
                foreach (var tex in externalTextures)
                {
                    if (tex.texture is Texture2D tex2D)
                        list.Add(tex2D);
                }

                return list;
            }
        }

        /// <summary>
        /// Ids of the property name associated with each texture. This is a parallel list to `textures`.
        /// </summary>
        [Obsolete("propertyNameIds has been deprecated in AR Foundation version 6.1. Use externalTextures instead.")]
        public List<int> propertyNameIds
        {
            get
            {
                var list = new List<int>();
                foreach (var tex in externalTextures)
                {
                    if (tex.texture is Texture2D)
                        list.Add(tex.propertyId);
                }

                return list;
            }
        }

        /// <summary>
        /// The list of keywords to be enabled for the material.
        /// </summary>
        [Obsolete("enabledMaterialKeywords has been deprecated in AR Foundation version 6.0. Use shaderKeywords instead.")]
        public List<string> enabledMaterialKeywords
            => shaderKeywords.enabledKeywords != null ? new(shaderKeywords.enabledKeywords.ToArray()) : null;

        /// <summary>
        /// The list of keywords to be disabled for the material.
        /// </summary>
        [Obsolete("disabledMaterialKeywords has been deprecated in AR Foundation version 6.0. Use shaderKeywords instead.")]
        public List<string> disabledMaterialKeywords
            => shaderKeywords.disabledKeywords != null ? new(shaderKeywords.disabledKeywords.ToArray()) : null;

        /// <summary>
        /// The enabled shader keywords.
        /// </summary>
        [Obsolete("enabledShaderKeywords is deprecated as of AR Foundation 6.1. Use shaderKeywords instead.")]
        public ReadOnlyCollection<string> enabledShaderKeywords
            => shaderKeywords.enabledKeywords != null ? new(shaderKeywords.enabledKeywords.ToArray()) : null;

        /// <summary>
        /// The disabled shader keywords.
        /// </summary>
        [Obsolete("disabledShaderKeywords is deprecated as of AR Foundation 6.1. Use shaderKeywords instead")]
        public ReadOnlyCollection<string> disabledShaderKeywords
            => shaderKeywords.disabledKeywords != null ? new(shaderKeywords.disabledKeywords.ToArray()) : null;

        /// <summary>
        /// The enabled and disabled shader keywords.
        /// </summary>
        public XRShaderKeywords shaderKeywords { get; internal set; }

        /// <summary>
        /// All external textures associated with occlusion for this frame.
        /// </summary>
        public ReadOnlyList<ARExternalTexture> externalTextures { get; internal set; }

        internal XROcclusionFrameProperties properties { private get; set; }

        /// <summary>
        /// The timestamp of the frame, in nanoseconds.
        /// </summary>
        internal long timestamp { private get; set; }

        internal XRNearFarPlanes nearFarPlanes { private get; set; }

        internal ReadOnlyList<Pose> poses { private get; set; }

        internal ReadOnlyList<XRFov> fovs { private get; set; }

        /// <summary>
        /// Get the timestamp of the frame, if possible.
        /// </summary>
        /// <param name="timestampOut">The timestamp of the camera frame, in nanoseconds.</param>
        /// <returns><see langword="true"/> if the frame has a timestamp that was output to <paramref name="timestampOut"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetTimestamp(out long timestampOut)
        {
            if ((properties & XROcclusionFrameProperties.Timestamp) != 0)
            {
                timestampOut = timestamp;
                return true;
            }

            timestampOut = default;
            return false;
        }

        /// <summary>
        /// Get the near and far planes for the frame, if possible.
        /// </summary>
        /// <param name="planesOut">The near and far planes.</param>
        /// <returns><see langword="true"/> if the frame has near and far planes that were output to <paramref name="planesOut"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetNearFarPlanes(out XRNearFarPlanes planesOut)
        {
            if ((properties & XROcclusionFrameProperties.NearFarPlanes) != 0)
            {
                planesOut = nearFarPlanes;
                return true;
            }

            planesOut = default;
            return false;
        }

        /// <summary>
        /// Get an array of poses from which the frame was rendered, if possible.
        /// Poses are in Unity world space.
        /// </summary>
        /// <param name="posesOut">The output array of poses, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the frame has poses that were output to <paramref name="posesOut"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetPoses(out ReadOnlyList<Pose> posesOut)
        {
            if ((properties & XROcclusionFrameProperties.Poses) != 0)
            {
                posesOut = poses;
                return true;
            }

            posesOut = default;
            return false;
        }

        /// <summary>
        /// Get an array of fields of view for the frame if possible.
        /// </summary>
        /// <param name="fovsOut">The output array of fields of view, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the frame has fields of view that were output to <paramref name="fovsOut"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool TryGetFovs(out ReadOnlyList<XRFov> fovsOut)
        {
            if ((properties & XROcclusionFrameProperties.Fovs) != 0)
            {
                fovsOut = fovs;
                return true;
            }

            fovsOut = default;
            return false;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is of type <see cref="AROcclusionFrameEventArgs"/> and
        /// <see cref="Equals(AROcclusionFrameEventArgs)"/> also returns <see langword="true"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is AROcclusionFrameEventArgs args && Equals(args);

        /// <summary>
        /// Tests for equality. Equivalent to <see cref="Equals(AROcclusionFrameEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(AROcclusionFrameEventArgs lhs, AROcclusionFrameEventArgs rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Equivalent to `!`<see cref="Equals(AROcclusionFrameEventArgs)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(AROcclusionFrameEventArgs lhs, AROcclusionFrameEventArgs rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(shaderKeywords);
            hashCode.Add((int)properties);
            hashCode.Add(timestamp);
            hashCode.Add(nearFarPlanes);
            hashCode.Add(poses);
            hashCode.Add(fovs);
            hashCode.Add(externalTextures);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type. Collections are compared
        /// using reference equality.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to <paramref name="other"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(AROcclusionFrameEventArgs other)
        {
            return Equals(shaderKeywords, other.shaderKeywords) &&
                properties == other.properties &&
                timestamp == other.timestamp &&
                nearFarPlanes.Equals(other.nearFarPlanes) &&
                Equals(poses, other.poses) &&
                Equals(fovs, other.fovs) &&
                Equals(externalTextures, other.externalTextures);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  shaderKeywords: {shaderKeywords.ToString()},");
            sb.AppendLine($"  properties: {properties.ToString()},");
            sb.AppendLine($"  timestamp: {timestamp.ToString()},");
            sb.AppendLine($"  nearFarPlanes: {nearFarPlanes.ToString()},");
            sb.AppendLine($"  poses: {poses},");
            sb.AppendLine($"  fovs: {fovs},");
            sb.AppendLine($"  externalTextures: {externalTextures},");
            sb.AppendLine("}");
            var result = sb.ToString();
            sb.Clear();
            return result;
        }
    }
}
