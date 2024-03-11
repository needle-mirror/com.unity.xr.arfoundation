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

        class BoundingBoxCheck
        {
            #region CheckIfBoundingBoxLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRBoundingBoxSubsystem>() != null)
                {
                    // XRBoundingBoxSubsystem was loaded. The platform supports bounding box detection.
                }
            }
            #endregion
        }
    }
}
