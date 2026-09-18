using System.Threading;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARSessionSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport(ARSession manager)
        {
            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsMatchFrameRate)
            {
                // Match frame rate is supported.
            }
        }
        #endregion

        #region InstallAndStartSession
        async Awaitable InstallAndStartSession(XRSessionSubsystem sessionSubsystem, CancellationToken cancellationToken)
        {
            // Pass InstallSoftwareIfNeeded so the subsystem checks for, and installs,
            // any required AR software before it starts.
            var result = await sessionSubsystem.TryStartAsync(
                cancellationToken, XRSubsystemStartOptions.InstallSoftwareIfNeeded);

            if (result.statusCode == XRResultStatus.StatusCode.Unsupported)
            {
                // AR is not supported on this device. Start a fallback (non-AR) experience instead.
                return;
            }

            if (result.IsError())
            {
                // AR is supported, but the required software could not be installed
                // (for example, the user declined the installation).
                return;
            }

            // The session started successfully and AR is now running.
        }
        #endregion
    }
}
