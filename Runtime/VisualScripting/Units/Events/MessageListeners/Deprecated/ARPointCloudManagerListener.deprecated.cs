#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARPointCloudManager.pointCloudsChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="PointCloudsChangedEventUnit"/>
    [Obsolete("ARPointCloudManagerListener has been deprecated in AR Foundation version 6.0.", false)]
    public sealed class ARPointCloudManagerListener : TrackableManagerListener<ARPointCloudManager>
    {
        void OnTrackablesChanged(ARPointCloudChangedEventArgs args)
            => EventBus.Trigger(Constants.DeprecatedEventHookNames.pointCloudsChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARPointCloudManager manager)
            => manager.pointCloudsChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARPointCloudManager manager)
            => manager.pointCloudsChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
