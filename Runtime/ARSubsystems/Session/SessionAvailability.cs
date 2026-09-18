using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Flags used to determine session availability.
    /// </summary>
    /// <remarks>
    /// Pass <see cref="XRSubsystemStartOptions.InstallSoftwareIfNeeded"/> to
    /// [XRSubsystem.TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*) instead.
    /// </remarks>
    [Flags]
    [Obsolete("SessionAvailability has been deprecated in AR Foundation version 6.7. Call XRResultStatus.IsSuccess on the result of TryStartAsync instead.")]
    public enum SessionAvailability
    {
        /// <summary>
        /// Default value. The availability is unknown.
        /// </summary>
        None = 0,

        /// <summary>
        /// The current device is AR capable (but might require a software update).
        /// </summary>
        Supported = 1 << 1,

        /// <summary>
        /// The required AR software is installed on the device.
        /// </summary>
        Installed = 1 << 2
    }

    /// <summary>
    /// Extensions to the <see cref="SessionAvailability"/> and <see cref="SessionInstallationStatus"/> enums.
    /// </summary>
    /// <remarks>
    /// Use <see cref="XRResultStatus.IsSuccess"/> from
    /// [XRSubsystem.TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*) instead.
    /// </remarks>
    [Obsolete("SessionAvailabilityExtensions has been deprecated in AR Foundation version 6.7. Use XRResultStatus.IsSuccess() from TryStartAsync instead.")]
    public static class SessionAvailabilityExtensions
    {
        /// <summary>
        /// A helper method for <see cref="SessionAvailability"/> flags.
        /// </summary>
        /// <param name="availability">A <see cref="SessionAvailability"/> enum.</param>
        /// <returns>`True` if the <see cref="SessionAvailability.Supported"/> flag is set.</returns>
        public static bool IsSupported(this SessionAvailability availability)
        {
            return (availability & SessionAvailability.Supported) != SessionAvailability.None;
        }

        /// <summary>
        /// A helper method for <see cref="SessionAvailability"/> flags.
        /// </summary>
        /// <param name="availability">A <see cref="SessionAvailability"/> enum.</param>
        /// <returns>`True` if the <see cref="SessionAvailability.Installed"/> flag is set.</returns>
        public static bool IsInstalled(this SessionAvailability availability)
        {
            return (availability & SessionAvailability.Installed) != SessionAvailability.None;
        }
    }
}
