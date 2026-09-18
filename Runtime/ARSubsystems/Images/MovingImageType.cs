using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Indicates whether a reference image is expected to move in the real world, so that a provider can optimize
    /// tracking accordingly.
    /// </summary>
    public enum MovingImageType
    {
        /// <summary>
        /// The moving-image type is not specified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The image is expected to not move. Suitable for paintings, wall signs, etc.
        /// </summary>
        Static = 1,

        /// <summary>
        /// The image is expected to move. Suitable for stickers on portable objects, smaller objects a user may move, etc.
        /// </summary>
        Dynamic = 2,
    }
}
