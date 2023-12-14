#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARAnchorManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Anchors Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Anchors)]
    [TypeIcon(typeof(ARAnchorManager))]
    public sealed class ARAnchorsChangedEventUnit : ARTrackablesChangedEventUnit<
        ARAnchorManager, ARAnchor, ARAnchorsChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
