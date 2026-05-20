using System;
using System.Threading;
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
        static readonly string k_NoSubsystemInstance =
            $"No active {typeof(TSubsystem).FullName} is available. This feature is either not supported on the current platform, or you may need to enable a provider in <b>Project Settings</b> > <b>XR Plug-in Management</b>.";

        CancellationTokenSource m_TokenSource;

        /// <summary>
        /// Get the <c>TSubsystem</c> whose lifetime this component manages.
        /// </summary>
        public TSubsystem subsystem { get; private set; }

        /// <summary>
        /// The descriptor for the subsystem.
        /// </summary>
        /// <value>The descriptor for the subsystem.</value>
        public TSubsystemDescriptor descriptor => subsystem?.subsystemDescriptor;

        /// <summary>
        /// Returns the active <c>TSubsystem</c> instance if present, otherwise returns null.
        /// </summary>
        /// <returns>The active subsystem instance, or `null` if there isn't one.</returns>
        protected static TSubsystem GetActiveSubsystemInstance()
        {
            var success = SubsystemUtils.TryGetLoadedSubsystem(out TSubsystem activeSubsystem);

            if (!success)
                Debug.LogWarning(k_NoSubsystemInstance);

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
        /// Starts the <c>TSubsystem</c>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_TokenSource = new CancellationTokenSource();
            EnsureSubsystemInstanceSet();
            if (subsystem != null)
                StartSubsystemAsync();
        }

        async void StartSubsystemAsync()
        {
            try
            {
                OnBeforeStart();

                // The derived class may disable this component if it has invalid state
                if (!enabled)
                    return;

                if (subsystem is XRSubsystem<TSubsystem, TSubsystemDescriptor, TProvider> xrSubsystem)
                {
                    var result = await xrSubsystem.TryStartAsync(m_TokenSource.Token);
                    if (result.IsError())
                    {
                        Debug.LogError($"{subsystem.GetType()} failed to start. Disabling {GetType()}.", this);
                        enabled = false;
                        return;
                    }
                }
                else
                {
                    subsystem.Start();
                }

                OnAfterStart();
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        /// <summary>
        /// Stops the <c>TSubsystem</c>.
        /// </summary>
        protected virtual void OnDisable()
        {
            m_TokenSource.Cancel();
            m_TokenSource.Dispose();
            m_TokenSource = null;
            if (subsystem?.running ?? false)
                subsystem.Stop();
        }

        /// <summary>
        /// Called when this instance is destroyed.
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
    }
}
