#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Event unit for `ARPointCloudManager.trackablesChanged`.
    /// </summary>
    [UnitTitle("On Point Clouds Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.PointClouds)]
    [TypeIcon(typeof(ARPointCloudManager))]
    public sealed class ARPointCloudsChangedEventUnit : ARTrackablesChangedEventUnit<
        ARPointCloudManager, ARPointCloud, ARPointCloudsChangedListener>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
