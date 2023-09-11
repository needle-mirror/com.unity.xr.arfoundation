using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    /// <summary>
    /// A utilities class for checking loaded subsystems.
    /// </summary>
    static class SubsystemUtils
    {
        /// <summary>
        /// Returns <see langword="true"/> if there is a loaded subsystem <see cref="SubsystemWithProvider"/> of
        /// type <typeparamref name="TSubsystemBase"/> and cast its type as <typeparamref name="TSubsystem"/>.
        /// Otherwise, <see langword="false"/>.
        /// </summary>
        /// <typeparam name="TSubsystemBase">The <see cref="SubsystemWithProvider"/> type to check the loaded subsystem.</typeparam>
        /// <typeparam name="TSubsystem">The subsystem type.</typeparam>
        /// <param name="subsystem">The loaded subsystem.</param>
        /// <returns><see langword="true"/> if <c>TSubsystemBase</c> is loaded. Otherwise, <see langword="false"/>.</returns>
        public static bool TryGetLoadedSubsystem<TSubsystemBase, TSubsystem>(out TSubsystem subsystem)
            where TSubsystemBase : SubsystemWithProvider, new()
            where TSubsystem : TSubsystemBase
        {
            TryGetLoadedSubsystem<TSubsystemBase>(out var baseSubsystem);
            subsystem = baseSubsystem as TSubsystem;
            return subsystem != null;
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a loaded subsystem <see cref="SubsystemWithProvider"/> of
        /// type <typeparamref name="TSubsystemBase"/>. Otherwise, <see langword="false"/>.
        /// </summary>
        /// <typeparam name="TSubsystemBase">The <see cref="SubsystemWithProvider"/> type to check the loaded subsystem.</typeparam>
        /// <param name="subsystem">The loaded subsystem.</param>
        /// <returns><see langword="true"/> if <c>TSubsystemBase</c> is loaded. Otherwise, <see langword="false"/>.</returns>
        public static bool TryGetLoadedSubsystem<TSubsystemBase>(out TSubsystemBase subsystem)
            where TSubsystemBase : SubsystemWithProvider, new()
        {
            if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null)
            {
                subsystem = null;
                return false;
            }

            var loader = XRGeneralSettings.Instance.Manager.activeLoader;

            // Query the currently active loader for the created subsystem, if one exists.
            subsystem = loader != null ? loader.GetLoadedSubsystem<TSubsystemBase>() : null;

            return subsystem != null;
        }
    }
}
