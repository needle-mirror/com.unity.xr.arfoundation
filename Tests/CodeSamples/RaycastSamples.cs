using System.Collections.Generic;
using NUnit.Framework;

namespace UnityEngine.XR.ARFoundation
{
    [TestFixture]
    class RaycastSamples
    {
        #region raycasthit_trackable
        void HandleRaycast(ARRaycastHit hit)
        {
            if (hit.trackable is ARPlane plane)
            {
                // Do something with 'plane':
                Debug.Log($"Hit a plane with alignment {plane.alignment}");
            }
            else
            {
                // What type of thing did we hit?
                Debug.Log($"Raycast hit a {hit.hitType}");
            }
        }
        #endregion

        #pragma warning disable CS0649

        class UsingTouch : MonoBehaviour
        {
            #region raycast_using_touch
            [SerializeField]
            ARRaycastManager m_RaycastManager;

            List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

            void OnEnable()
            {
                InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
            }

            void OnDisable()
            {
                InputSystem.EnhancedTouch.EnhancedTouchSupport.Disable();
            }

            void Update()
            {
                var activeTouches = InputSystem.EnhancedTouch.Touch.activeTouches;
                if (activeTouches.Count == 0)
                    return;

                if (m_RaycastManager.Raycast(activeTouches[0].screenPosition, m_Hits))
                {
                    // Only returns true if there is at least one hit
                }
            }
            #endregion
        }

        #pragma warning restore CS0649
    }
}
