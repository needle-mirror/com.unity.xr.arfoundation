#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARParticipantManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Participants Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Participants)]
    [TypeIcon(typeof(ARParticipantManager))]
    public sealed class ARParticipantsChangedEventUnit : ARTrackablesChangedEventUnit<
        ARParticipantManager, ARParticipant, ARParticipantsChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
