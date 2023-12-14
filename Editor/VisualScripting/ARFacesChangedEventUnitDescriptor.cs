#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="ARFacesChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(ARFacesChangedEventUnit))]
    public class ARFacesChangedEventUnitDescriptor : ARTrackablesChangedEventUnitDescriptor<
        ARFaceManager, ARFace, ARFacesChangedListener>
    {
        /// <inheritdoc />
        public ARFacesChangedEventUnitDescriptor(
            ARTrackablesChangedEventUnit<ARFaceManager, ARFace, ARFacesChangedListener> target)
            : base(target)
        { }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
