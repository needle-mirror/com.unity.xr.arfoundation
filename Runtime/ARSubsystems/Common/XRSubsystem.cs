using System;
using System.Threading;
using Unity.XR.CoreUtils;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the possible input arguments to
    /// [XRSubsystem.TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*).
    /// </summary>
    [Flags]
    public enum XRSubsystemStartOptions
    {
        /// <summary>
        /// No options are requested.
        /// </summary>
        None = 0,

        /// <summary>
        /// The subsystem should request any necessary permissions from the platform before starting,
        /// if they aren't already granted.
        /// </summary>
        RequestPermissionsIfNeeded = 1 << 0,
    }

    /// <summary>
    /// Base class for subsystems that enables asynchronous start and the ability to automatically
    /// request required permissions on any platform.
    /// </summary>
    /// <typeparam name="TSubsystem">The subsystem type.</typeparam>
    /// <typeparam name="TSubsystemDescriptor">The subsystem descriptor type.</typeparam>
    /// <typeparam name="TProvider">The subsystem provider type.</typeparam>
    public class XRSubsystem<TSubsystem, TSubsystemDescriptor, TProvider>
        : SubsystemWithProvider<TSubsystem, TSubsystemDescriptor, TProvider>
        where TSubsystem : SubsystemWithProvider, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider
        where TProvider : SubsystemProvider<TSubsystem>
    {
        /// <summary>
        /// Get the state of any required permissions for this subsystem to run on the current device.
        /// </summary>
        /// <returns>The permission state.</returns>
        public virtual XRPermissionState GetPermissionState() => XRPermissionState.Unknown;

        /// <summary>
        /// Attempts to start the subsystem with the given options.
        /// </summary>
        /// <param name="token">An optional cancellation token, which you can use to cancel the operation in progress.</param>
        /// <param name="options">The start options.</param>
        /// <returns>A status representing whether the subsystem was successfully started, and
        /// any applicable error codes from the runtime.</returns>
        public virtual Awaitable<XRResultStatus> TryStartAsync(
            CancellationToken token = default, XRSubsystemStartOptions options = XRSubsystemStartOptions.None)
        {
            Start();
            return AwaitableUtils<XRResultStatus>.FromResult(XRResultStatus.unqualifiedSuccess);
        }
    }
}
