namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the state of any system permissions that a subsystem needs to be able to run on the current device.
    /// </summary>
    public enum XRPermissionState
    {
        /// <summary>
        /// A required permission is not granted, and the subsystem is unable to function.
        /// </summary>
        NotGranted = -1,

        /// <summary>
        /// The subsystem hasn't reported any information about required permissions.
        /// </summary>
        /// <remarks>
        /// This is the default value of this `enum`. Refer to the documentation for your provider plug-in(s) to
        /// understand if any permissions are required for its subsystems.
        /// </remarks>
        Unknown = 0,

        /// <summary>
        /// The subsystem doesn't require any system permissions on this platform, and is able to run successfully.
        /// </summary>
        NotRequired = 1,

        /// <summary>
        /// The subsystem requires permissions on this platform, and the required permissions are granted.
        /// The subsystem is able to run successfully.
        /// </summary>
        Granted = 2,

        /// <summary>
        /// The subsystem has requested required permissions on this platform, and is awaiting a system callback
        /// with the results of this request.
        /// </summary>
        RequestPending = 3
    }
}
