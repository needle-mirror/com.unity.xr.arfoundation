#if VISUALSCRIPTING_1_8_OR_NEWER

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Concrete listener type for the `ARPointCloudManager`.
    /// </summary>
    /// <remarks>
    /// Unity cannot serialize generic MonoBehaviour types, so we need this file to specify the type parameters.
    /// </remarks>
    public sealed class ARPointCloudsChangedListener : ARTrackableManagerListener<ARPointCloudManager, ARPointCloud>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
