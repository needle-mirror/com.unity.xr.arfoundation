using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class LoaderUtilitySamples
    {
        class CameraCheck
        {
            #region CheckIfCameraLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRCameraSubsystem>() != null)
                {
                    // XRCameraSubsystem was loaded. The platform supports the camera subsystem.
                }
            }
            #endregion
        }

        class PlanesCheck
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

        class BoundingBoxesCheck
        {
            #region CheckIfBoundingBoxesLoaded
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

        class AnchorsCheck
        {
            #region CheckIfAnchorsLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRAnchorSubsystem>() != null)
                {
                    // XRAnchorSubsystem was loaded. The platform supports anchors.
                }
            }
            #endregion
        }

        class OcclusionCheck
        {
            #region CheckIfOcclusionLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XROcclusionSubsystem>() != null)
                {
                    // XROcclusionSubsystem was loaded. The platform supports occlusion.
                }
            }
            #endregion
        }

        class FaceCheck
        {
            #region CheckIfFaceLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRFaceSubsystem>() != null)
                {
                    // XRFaceSubsystem was loaded. The platform supports face detection.
                }
            }
            #endregion
        }

        class RaycastCheck
        {
            #region CheckIfRaycastLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRRaycastSubsystem>() != null)
                {
                    // XRRaycastSubsystem was loaded. The platform supports ray casts.
                }
            }
            #endregion
        }
    }
}
