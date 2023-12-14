#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARHumanBodyManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Human Bodies Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.BodyTracking)]
    [TypeIcon(typeof(ARHumanBodyManager))]
    public sealed class ARHumanBodiesChangedEventUnit : ARTrackablesChangedEventUnit<
        ARHumanBodyManager, ARHumanBody, ARHumanBodiesChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
