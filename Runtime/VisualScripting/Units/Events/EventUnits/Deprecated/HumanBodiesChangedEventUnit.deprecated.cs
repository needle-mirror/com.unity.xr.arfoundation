#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARHumanBodyManagerListener"/> to listen for a human-bodies-changed event on the visual
    /// scripting event bus and assign the <see cref="ARHumanBodiesChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Human Bodies Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.BodyTracking)]
    [TypeIcon(typeof(ARHumanBodyManager))]
    [Obsolete("HumanBodiesChangedEventUnit has been deprecated in AR Foundation version 6.0.", false)]
    public sealed class HumanBodiesChangedEventUnit : TrackablesChangedEventUnit<
        ARHumanBodyManager,
        XRHumanBodySubsystem,
        XRHumanBodySubsystemDescriptor,
        XRHumanBodySubsystem.Provider,
        XRHumanBody,
        ARHumanBody,
        ARHumanBodiesChangedEventArgs,
        ARHumanBodyManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.DeprecatedEventHookNames.humanBodiesChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARHumanBodiesChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
