#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `AREnvironmentProbeManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On EnvironmentProbes Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.EnvironmentProbes)]
    [TypeIcon(typeof(AREnvironmentProbeManager))]
    public sealed class AREnvironmentProbesChangedEventUnit : ARTrackablesChangedEventUnit<
        AREnvironmentProbeManager, AREnvironmentProbe, AREnvironmentProbesChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
