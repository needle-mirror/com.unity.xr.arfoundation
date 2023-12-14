#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARTrackedImageManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Tracked Images Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.ImageTracking)]
    [TypeIcon(typeof(ARTrackedImageManager))]
    public sealed class ARTrackedImagesChangedEventUnit : ARTrackablesChangedEventUnit<
        ARTrackedImageManager, ARTrackedImage, ARTrackedImagesChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
