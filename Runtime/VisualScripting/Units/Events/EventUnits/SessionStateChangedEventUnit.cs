#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses <see cref="ARSessionManager.stateChanged"/> to listen for an AR Session state changed event, then fires the same event
    /// on the visual scripting event bus.
    /// </summary>
    [UnitTitle("On Session State Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Session)]
    [TypeIcon(typeof(ARSessionManager))]
    public sealed class SessionStateChangedEventUnit : GlobalEventUnit<ARSessionState>
    {
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static bool s_CallbackIsRegistered;
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static readonly Type k_HookNameKey = typeof(ARSessionStateChangedEventArgs);

        /// <summary>
        /// <see cref="ARSessionState"/> output.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("AR Session State")]
        public ValueOutput sessionStateOut { get; private set; }

        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames[k_HookNameKey];

        /// <summary>
        /// Fired once on startup for each instance of this unit in the scene.
        /// </summary>
        /// <param name="stack">The graph stack.</param>
        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            if (s_CallbackIsRegistered)
                return;

            ARSessionManager.stateChanged += HandleStateChange;
            s_CallbackIsRegistered = true;
        }

        /// <summary>
        /// Fired once on teardown for each instance of this unit in the scene.
        /// </summary>
        /// <param name="stack">The graph stack.</param>
        public override void StopListening(GraphStack stack)
        {
            ARSessionManager.stateChanged -= HandleStateChange;
            s_CallbackIsRegistered = false;
            base.StopListening(stack);
        }

        static void HandleStateChange(ARSessionStateChangedEventArgs args)
        {
            EventBus.Trigger(Constants.EventHookNames[k_HookNameKey], args.state);
        }

        /// <summary>
        /// Unit definition.
        /// </summary>
        protected override void Definition()
        {
            base.Definition();
            sessionStateOut = ValueOutput<ARSessionState>(nameof(sessionStateOut));
        }

        /// <summary>
        /// Assigns the <paramref name="state"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="state">The session state.</param>
        protected override void AssignArguments(Flow flow, ARSessionState state)
        {
            flow.SetValue(sessionStateOut, state);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ResetStaticsOnLoad()
        {
            if (s_CallbackIsRegistered)
            {
                ARSessionManager.stateChanged -= HandleStateChange;
                s_CallbackIsRegistered = false;
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
