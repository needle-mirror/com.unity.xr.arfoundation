using System;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents the current state of the AR system.
    /// </summary>
    public enum ARSessionState
    {
        /// <summary>
        /// AR has not been initialized and availability is unknown.
        /// </summary>
        None,

        /// <summary>
        /// AR is not supported on the device.
        /// </summary>
        Unsupported,

        /// <summary>
        /// (Deprecated.) The session subsystem is currently checking availability of AR on the device.
        /// </summary>
        /// <remarks>
        /// This state is only set by the deprecated <see cref="ARSession.CheckAvailability"/> API. Use
        /// <see cref="UnityEngine.XR.ARSubsystems.XRSubsystemStartOptions.InstallSoftwareIfNeeded"/> with
        /// <see cref="UnityEngine.XR.ARSubsystems.XRSubsystem{TSubsystem, TSubsystemDescriptor, TProvider}.TryStartAsync"/> instead.
        /// </remarks>
        [Obsolete("CheckingAvailability has been deprecated in AR Foundation version 6.7. It is only set by the deprecated ARSession.CheckAvailability API.")]
        CheckingAvailability,

        /// <summary>
        /// (Deprecated.) The device supports AR, but requires additional software to be installed.
        /// </summary>
        /// <remarks>
        /// This state is only set by the deprecated <see cref="ARSession.CheckAvailability"/> API. Use
        /// <c>TryInstallAsync</c> on the platform-specific session subsystem instead.
        /// </remarks>
        [Obsolete("NeedsInstall has been deprecated in AR Foundation version 6.7. It is only set by the deprecated ARSession.CheckAvailability API. Await TryStartAsync with XRSubsystemStartOptions.InstallSoftwareIfNeeded instead")]
        NeedsInstall,

        /// <summary>
        /// (Deprecated.) AR software is currently installing.
        /// </summary>
        /// <remarks>
        /// This state is only set by the deprecated <see cref="ARSession.Install"/> API. Use
        /// <c>TryInstallAsync</c> on the platform-specific session subsystem instead.
        /// </remarks>
        [Obsolete("Installing has been deprecated in AR Foundation version 6.7. It is only set by the deprecated ARSession.Install API. Await TryStartAsync with XRSubsystemStartOptions.InstallSoftwareIfNeeded instead.")]
        Installing,

        /// <summary>
        /// The device supports AR, and any necessary software is installed.
        /// This state will automatically change to either <c>SessionInitializing</c> or <c>SessionTracking</c>.
        /// </summary>
        Ready,

        /// <summary>
        /// The AR session is currently initializing. This usually means AR is running, but not yet tracking successfully.
        /// </summary>
        SessionInitializing,

        /// <summary>
        /// The AR session is running and tracking successfully. The device is able to determine its position and orientation in the world.
        /// If tracking is lost during a session, this state may change to <c>SessionInitializing</c>.
        /// </summary>
        SessionTracking
    }
}
