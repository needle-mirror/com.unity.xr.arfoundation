using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// An enum used to determine the current status of the software install.
    /// </summary>
    /// <remarks>
    /// Some devices might support AR but require a software update. Some platforms allow
    /// prompting a user to install the required software. This enum is used to indicate
    /// the result of an installation request.
    ///
    /// Use <see cref="XRResultStatus"/> returned from a platform-specific <c>TryInstallAsync</c> instead.
    /// </remarks>
    [Obsolete("SessionInstallationStatus has been deprecated in AR Foundation version 6.7. Refer to the Google ARCore XR Plug-in user manual for replacement API information.")]
    public enum SessionInstallationStatus
    {
        /// <summary>
        /// Default value. The installation status is not known.
        /// </summary>
        None = 0,

        /// <summary>
        /// The installation was successful.
        /// </summary>
        Success,

        /// <summary>
        /// The installation was not successful because the user declined the installation.
        /// </summary>
        ErrorUserDeclined,

        /// <summary>
        /// The installation was not successful because the device is not compatible.
        /// </summary>
        ErrorDeviceNotCompatible,

        /// <summary>
        /// The installation was not successful because installation is not supported.
        /// </summary>
        ErrorInstallNotSupported,

        /// <summary>
        /// An unknown error occurred during installation.
        /// </summary>
        Error
    }
}
