using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A base class for subsystems whose lifetime is managed by a <c>MonoBehaviour</c>.
    /// </summary>
    /// <typeparam name="TSubsystem">The [Subsystem](xref:UnityEngine.Subsystem) which provides this manager data.</typeparam>
    /// <typeparam name="TProvider">The [provider](xref:UnityEngine.SubsystemsImplementation.SubsystemProvider) associated with this subsystem.</typeparam>
    /// <typeparam name="TSubsystemDescriptor">The <c>SubsystemDescriptor</c> required to create the Subsystem.</typeparam>
    public class SubsystemLifecycleManager<TSubsystem, TSubsystemDescriptor, TProvider> : MonoBehaviour
        where TSubsystem : SubsystemWithProvider<TSubsystem, TSubsystemDescriptor, TProvider>, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
        where TProvider : SubsystemProvider<TSubsystem>
    {
        /// <summary>
        /// Get the <c>TSubsystem</c> whose lifetime this component manages.
        /// </summary>
        public TSubsystem subsystem { get; private set; }

        /// <summary>
        /// The descriptor for the subsystem.
        /// </summary>
        /// <value>
        /// The descriptor for the subsystem.
        /// </value>
        public TSubsystemDescriptor descriptor => subsystem?.subsystemDescriptor;

        /// <summary>
        /// Returns the active <c>TSubsystem</c> instance if present, otherwise returns null.
        /// </summary>
        /// <returns>The active subsystem instance, or `null` if there isn't one.</returns>
        protected static TSubsystem GetActiveSubsystemInstance()
        {
            var success = SubsystemUtils.TryGetLoadedSubsystem(out TSubsystem activeSubsystem);

            if (!success)
                Debug.LogWarning($"No active {typeof(TSubsystem).FullName} is available. This feature is either not supported on the current platform, or you may need to enable a provider in <b>Project Settings</b> > <b>XR Plug-in Management</b>.");

            return activeSubsystem;
        }

        /// <summary>
        /// Called by derived classes to initialize the subsystem is initialized before use
        /// </summary>
        protected void EnsureSubsystemInstanceSet()
        {
            subsystem = GetActiveSubsystemInstance();
        }

        /// <summary>
        /// Creates the <c>TSubsystem</c>.
        /// </summary>
        protected virtual void OnEnable()
        {
            EnsureSubsystemInstanceSet();

            if (subsystem != null)
            {
                OnBeforeStart();

                // The derived class may disable the
                // component if it has invalid state
                if (enabled)
                {
                    subsystem.Start();
                    OnAfterStart();
                }
            }
        }

        /// <summary>
        /// Stops the <c>TSubsystem</c>.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (subsystem != null)
                subsystem.Stop();
        }

        /// <summary>
        /// Destroys the <c>TSubsystem</c>.
        /// </summary>
        protected virtual void OnDestroy()
        {
            subsystem = null;
        }

        /// <summary>
        /// Invoked after creating the subsystem and before calling Start on it.
        /// The <see cref="subsystem"/> is not <c>null</c>.
        /// </summary>
        protected virtual void OnBeforeStart()
        { }

        /// <summary>
        /// Invoked after calling Start on it the Subsystem.
        /// The <see cref="subsystem"/> is not <c>null</c>.
        /// </summary>
        protected virtual void OnAfterStart()
        { }

        static List<TSubsystemDescriptor> s_SubsystemDescriptors =
            new List<TSubsystemDescriptor>();

        static List<TSubsystem> s_SubsystemInstances =
            new List<TSubsystem>();
    }
}
