using System;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    /// <summary>
    /// A utility class for checking loaded subsystems.
    /// </summary>
    static class SubsystemUtils
    {
        static Dictionary<Type, SubsystemWithProvider> s_SubsystemsByType = new();
        static Dictionary<Type, IntegratedSubsystem> s_IntegratedSubsystemsByType = new();

        /// <summary>
        /// Returns <see langword="true"/> if there is a loaded <see cref="SubsystemWithProvider"/> of
        /// type <typeparamref name="TSubsystem"/>, and outputs it.
        /// </summary>
        /// <typeparam name="TSubsystemBase">The base subsystem type, ie `XRSessionSubsystem`.</typeparam>
        /// <typeparam name="TSubsystem">The derived subsystem type to match.</typeparam>
        /// <param name="subsystem">The loaded subsystem, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if there exists a loaded <typeparamref name="TSubsystemBase"/> of type
        /// <typeparamref name="TSubsystem"/>. Otherwise, <see langword="false"/>.</returns>
        internal static bool TryGetLoadedSubsystem<TSubsystemBase, TSubsystem>(out TSubsystem subsystem)
            where TSubsystemBase : SubsystemWithProvider, new()
            where TSubsystem : TSubsystemBase
        {
            if (s_SubsystemsByType.TryGetValue(typeof(TSubsystem), out var subsystemWithProvider))
            {
                if (subsystemWithProvider != null)
                {
                    subsystem = subsystemWithProvider as TSubsystem;
                    return true;
                }

                s_SubsystemsByType.Remove(typeof(TSubsystem));
            }

            TryGetLoadedSubsystem<TSubsystemBase>(out var baseSubsystem);
            subsystem = baseSubsystem as TSubsystem;
            if (subsystem != null)
                s_SubsystemsByType.Add(typeof(TSubsystem), subsystem);

            return subsystem != null;
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a loaded <see cref="SubsystemWithProvider"/> of
        /// type <typeparamref name="TSubsystemBase"/> and outputs it. Otherwise, <see langword="false"/>.
        ///
        /// This method REQUIRES that `TSubsystemBase` is a base subsystem type, i.e. `XRSessionSubsystem`.
        /// </summary>
        /// <typeparam name="TSubsystemBase">The base subsystem type, ie `XRSessionSubsystem`.</typeparam>
        /// <param name="subsystem">The loaded subsystem, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if there exists a loaded <typeparamref name="TSubsystemBase"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        internal static bool TryGetLoadedSubsystem<TSubsystemBase>(out TSubsystemBase subsystem)
            where TSubsystemBase : SubsystemWithProvider, new()
        {
            if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null)
            {
                subsystem = null;
                return false;
            }

            var loader = XRGeneralSettings.Instance.Manager.activeLoader;
            subsystem = loader != null ? loader.GetLoadedSubsystem<TSubsystemBase>() : null;

            return subsystem != null;
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a loaded integrated subsystem of
        /// type <typeparamref name="TIntegratedSubsystem"/>. Otherwise, <see langword="false"/>.
        /// </summary>
        /// <typeparam name="TIntegratedSubsystem">The integrated subsystem type.</typeparam>
        /// <param name="subsystem">The loaded subsystem, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if there exists a loaded <typeparamref name="TIntegratedSubsystem"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        internal static bool TryGetLoadedIntegratedSubsystem<TIntegratedSubsystem>(out TIntegratedSubsystem subsystem)
            where TIntegratedSubsystem : IntegratedSubsystem, new()
        {
            if (s_IntegratedSubsystemsByType.TryGetValue(typeof(TIntegratedSubsystem), out var baseSubsystem))
            {
                if (baseSubsystem != null)
                {
                    subsystem = baseSubsystem as TIntegratedSubsystem;
                    return true;
                }

                s_IntegratedSubsystemsByType.Remove(typeof(TIntegratedSubsystem));
            }

            if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null)
            {
                subsystem = null;
                return false;
            }

            var loader = XRGeneralSettings.Instance.Manager.activeLoader;
            subsystem = loader != null ? loader.GetLoadedSubsystem<TIntegratedSubsystem>() : null;
            if (subsystem != null)
                s_IntegratedSubsystemsByType.Add(typeof(TIntegratedSubsystem), subsystem);

            return subsystem != null;
        }
    }
}
