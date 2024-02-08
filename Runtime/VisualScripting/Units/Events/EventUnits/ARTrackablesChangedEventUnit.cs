#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Base class for units that expose data via the `ARTrackableManager.trackablesChanged` event.
    /// </summary>
    /// <typeparam name="TManager"></typeparam>
    /// <typeparam name="TTrackable"></typeparam>
    /// <typeparam name="TListener">The `MessageListener` type.</typeparam>
    public abstract class ARTrackablesChangedEventUnit<TManager, TTrackable, TListener> :
        GameObjectEventUnit<ARTrackablesChangedEventArgs<TTrackable>>
        where TManager : MonoBehaviour, ITrackablesChanged<TTrackable>
        where TTrackable : ARTrackable
        where TListener : MessageListener
    {
        static readonly Type k_HookNameKey = typeof(ARTrackablesChangedEventArgs<TTrackable>);

        /// <summary>
        /// Trackables added since the last frame.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput added { get; private set; }

        /// <summary>
        /// Trackables updated since the last frame.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput updated { get; private set; }

        /// <summary>
        /// Trackables removed since the last frame.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput removed { get; private set; }

        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames[k_HookNameKey];

        /// <summary>
        /// The <see cref="MessageListener"/> type.
        /// </summary>
        public override Type MessageListenerType => typeof(TListener);

        /// <summary>
        /// Unit definition.
        /// </summary>
        protected override void Definition()
        {
            base.Definition();

            added = ValueOutput<ReadOnlyList<TTrackable>>(nameof(added));
            updated = ValueOutput<ReadOnlyList<TTrackable>>(nameof(updated));
            removed = ValueOutput<KeyValuePair<TrackableId, TTrackable>>(nameof(removed));
        }

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARTrackablesChangedEventArgs<TTrackable> args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
