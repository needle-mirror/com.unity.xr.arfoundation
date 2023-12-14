#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARFaceManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Faces Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.FaceTracking)]
    [TypeIcon(typeof(ARFaceManager))]
    public sealed class ARFacesChangedEventUnit : ARTrackablesChangedEventUnit<
        ARFaceManager, ARFace, ARFacesChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
