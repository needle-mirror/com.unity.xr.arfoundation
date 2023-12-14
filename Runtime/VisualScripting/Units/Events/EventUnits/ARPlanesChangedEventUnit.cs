#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARPlaneManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Planes Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.PlaneDetection)]
    [TypeIcon(typeof(ARPlaneManager))]
    public sealed class ARPlanesChangedEventUnit : ARTrackablesChangedEventUnit<
        ARPlaneManager, ARPlane, ARPlanesChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
