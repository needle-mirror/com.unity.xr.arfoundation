#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARAnchorManager.anchorsChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="AnchorsChangedEventUnit"/>
    [Obsolete("ARAnchorManagerListener has been deprecated in AR Foundation version 6.0.", false)]
    public sealed class ARAnchorManagerListener : TrackableManagerListener<ARAnchorManager>
    {
        void OnTrackablesChanged(ARAnchorsChangedEventArgs args)
            => EventBus.Trigger(Constants.DeprecatedEventHookNames.anchorsChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARAnchorManager manager)
            => manager.anchorsChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARAnchorManager manager)
            => manager.anchorsChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
