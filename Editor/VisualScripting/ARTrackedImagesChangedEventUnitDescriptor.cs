#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="ARTrackedImagesChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(ARTrackedImagesChangedEventUnit))]
    public class ARTrackedImagesChangedEventUnitDescriptor : ARTrackablesChangedEventUnitDescriptor<
        ARTrackedImageManager, ARTrackedImage, ARTrackedImagesChangedListener>
    {
        /// <inheritdoc />
        public ARTrackedImagesChangedEventUnitDescriptor(
            ARTrackablesChangedEventUnit<ARTrackedImageManager, ARTrackedImage, ARTrackedImagesChangedListener> target)
            : base(target)
        { }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
