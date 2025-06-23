using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARSessionSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport(ARSession manager)
        {
            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsInstall)
            {
                // Install is supported.
            }
        }
        #endregion

        #region CheckForARSupport
        public class MyComponent
        {
            [SerializeField]
            ARSession m_Session;

            IEnumerator Start()
            {
                if ((ARSession.state == ARSessionState.None) ||
                    (ARSession.state == ARSessionState.CheckingAvailability))
                {
                    yield return ARSession.CheckAvailability();
                }

                if (ARSession.state == ARSessionState.Unsupported)
                {
                    // Start some fallback experience for unsupported devices
                }
                else
                {
                    // Start the AR session
                    m_Session.enabled = true;
                }
            }
        }
        #endregion
    }
}

