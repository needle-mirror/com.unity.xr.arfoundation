using UnityEngine.Events;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// An interface for the event when trackables have changed (been added, updated, or removed). Generally implemented by
    /// derived classes of <see cref="ARTrackableManager{TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable}"/>.
    /// </summary>
    /// <typeparam name="TTrackable">The trackable type.</typeparam>
    public interface ITrackablesChanged<TTrackable> where TTrackable : ARTrackable
    {
        /// <summary>
        /// Invoked when trackables have changed (been added, updated, or removed).
        /// </summary>
        UnityEvent<ARTrackablesChangedEventArgs<TTrackable>> trackablesChanged { get; }
    }
}
