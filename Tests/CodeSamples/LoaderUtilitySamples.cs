using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class LoaderUtilitySamples
    {
        class PlaneCheck
        {
            #region CheckIfPlanesLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRPlaneSubsystem>() != null)
                {
                    // XRPlaneSubsystem was loaded. The platform supports plane detection.
                }
            }
            #endregion
        }

    }
}
