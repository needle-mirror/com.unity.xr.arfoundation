using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents a tracked image in the physical environment.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ARUpdateOrder.k_TrackedImage)]
    [HelpURL(typeof(ARTrackedImage))]
    public class ARTrackedImage : ARTrackable<XRTrackedImage, ARTrackedImage>
    {
        /// <summary>
        /// The 2D extents of the image. This is half the <see cref="size"/>.
        /// </summary>
        public Vector2 extents
        {
            get { return sessionRelativeData.size * 0.5f; }
        }

        /// <summary>
        /// The 2D size of the image. This is the dimensions of the image.
        /// </summary>
        public Vector2 size
        {
            get { return sessionRelativeData.size; }
        }

        /// <summary>
        /// The reference image which was used to detect this image in the environment.
        /// </summary>
        public XRReferenceImage referenceImage { get; internal set; }
    }
}
