#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARTrackedImageManager.trackedImagesChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="TrackedImagesChangedEventUnit"/>
    [Obsolete("ARTrackedImageManagerListener has been deprecated in AR Foundation version 6.0.", false)]
    public sealed class ARTrackedImageManagerListener : TrackableManagerListener<ARTrackedImageManager>
    {
        void OnTrackablesChanged(ARTrackedImagesChangedEventArgs args)
            => EventBus.Trigger(Constants.DeprecatedEventHookNames.trackedImagesChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARTrackedImageManager manager)
            => manager.trackedImagesChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARTrackedImageManager manager)
            => manager.trackedImagesChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
