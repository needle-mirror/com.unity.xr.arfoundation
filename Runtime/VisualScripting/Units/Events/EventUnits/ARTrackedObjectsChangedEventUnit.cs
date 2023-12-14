#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARTrackedObjectManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Tracked Objects Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.ObjectTracking)]
    [TypeIcon(typeof(ARTrackedObjectManager))]
    public sealed class ARTrackedObjectsChangedEventUnit : ARTrackablesChangedEventUnit<
        ARTrackedObjectManager, ARTrackedObject, ARTrackedObjectsChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
