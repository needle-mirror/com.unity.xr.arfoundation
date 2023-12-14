#if VISUALSCRIPTING_1_8_OR_NEWER

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Concrete listener type for the `ARAnchorManager`.
    /// </summary>
    /// <remarks>
    /// Unity cannot serialize generic MonoBehaviour types, so we need this file to specify the type parameters.
    /// </remarks>
    public sealed class ARAnchorsChangedListener : ARTrackableManagerListener<ARAnchorManager, ARAnchor>
    { }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
