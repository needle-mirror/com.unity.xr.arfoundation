using System;
using System.Collections;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// <para>
    /// Controls the lifecycle and configuration options for an AR session. There
    /// is only one active session. If you have multiple <see cref="ARSessionManager"/> components,
    /// they all communicate to the same session and will conflict with each other.
    /// </para><para>
    /// Enabling or disabling the <see cref="ARSessionManager"/> starts or stops the session,
    /// respectively.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Related information: <a href="xref:arfoundation-session">AR Session component</a>
    /// </remarks>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ARUpdateOrder.k_Session)]
    [AddComponentMenu("XR/AR Foundation/AR Session")]
    [HelpUrl("features/session")]
    [Icon(Constants.arvrIconPath)]
    public sealed partial class ARSessionManager :
        SubsystemLifecycleManager<XRSessionSubsystem, XRSessionSubsystemDescriptor, XRSessionSubsystem.Provider>
    {
        // Internal for tests
        [AutoStaticsCleanup]
        internal static ARSessionState s_State;

        /// <summary>
        /// The reason AR tracking was lost.
        /// </summary>
        public static NotTrackingReason notTrackingReason => s_NotTrackingReason;

        [AutoStaticsCleanup]
        static NotTrackingReason s_NotTrackingReason;

        int m_VSyncCount;
        int m_TargetFrameRate;
        bool m_WasFrameRateSet;

        [SerializeField]
        bool m_AttemptUpdate = true;

        /// <summary>
        /// If the device supports XR but does not have the necessary software, some platforms
        /// allow prompting the user to install or update the software. If <see cref="attemptUpdate"/>
        /// is <c>true</c>, a software update will be attempted. If the appropriate software is not installed
        /// or out of date, and <see cref="attemptUpdate"/> is <c>false</c>, then AR will not be available.
        /// </summary>
        public bool attemptUpdate
        {
            get => m_AttemptUpdate;
            set => m_AttemptUpdate = value;
        }

        [SerializeField]
        bool m_MatchFrameRate = true;

        /// <summary>
        /// If <c>true</c>, the underlying subsystem will attempt to synchronize the AR frame rate with Unity's.
        /// </summary>
        /// <seealso cref="matchFrameRateRequested"/>
        public bool matchFrameRateEnabled => descriptor?.supportsMatchFrameRate == true && subsystem.matchFrameRateEnabled;

        /// <summary>
        /// If <c>true</c>, the session will block execution until a new AR frame is available and set
        /// [Application.targetFrameRate](xref:UnityEngine.Application.targetFrameRate)
        /// to match the native update frequency of the AR session.
        /// Otherwise, the AR session is updated independently of the Unity frame.
        /// </summary>
        /// <remarks>
        /// If enabled with a simple scene, the `ARSessionManager.Update` might appear to take a long time.
        /// This is because your application is waiting for the next AR frame, similar to the way Unity will `WaitForTargetFPS` at the
        /// end of a frame. If the rest of the Unity frame takes non-trivial time, then the next `ARSessionManager.Update`
        /// will take a proportionally smaller amount of time.
        ///
        /// This option does three things:
        /// - Enables a [setting on the XRSessionSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.matchFrameRateRequested) which causes the update to block until the next AR frame is ready.
        /// - Sets [Application.targetFrameRate](xref:UnityEngine.Application.targetFrameRate) to the session's preferred update rate.
        /// - Sets [QualitySettings.vSyncCount](xref:UnityEngine.QualitySettings.vSyncCount) to zero.
        ///
        /// These settings are not reverted when the AR Session is disabled.
        /// </remarks>
        public bool matchFrameRateRequested
        {
            get => descriptor?.supportsMatchFrameRate == true ? subsystem.matchFrameRateRequested : m_MatchFrameRate;
            set => SetMatchFrameRateRequested(value);
        }

        [SerializeField]
        TrackingMode m_TrackingMode = TrackingMode.PositionAndRotation;

        /// <summary>
        /// Get or set the <c>TrackingMode</c> for the session.
        /// </summary>
        public TrackingMode requestedTrackingMode
        {
            get => subsystem?.requestedTrackingMode.ToTrackingMode() ?? m_TrackingMode;
            set
            {
                m_TrackingMode = value;
                if (enabled && subsystem != null)
                    subsystem.requestedTrackingMode = value.ToFeature();
            }
        }

        /// <summary>
        /// Get the current <c>TrackingMode</c> in use by the session.
        /// </summary>
        public TrackingMode currentTrackingMode => subsystem?.currentTrackingMode.ToTrackingMode() ?? TrackingMode.DontCare;

        /// <summary>
        /// Get the number of AR frames produced per second, or null if the frame rate cannot be determined.
        /// </summary>
        public int? frameRate => descriptor?.supportsMatchFrameRate ?? false ? new int?(subsystem.frameRate) : null;

        /// <summary>
        /// This event is invoked whenever the <see cref="state"/> changes.
        /// </summary>
        [AutoStaticsCleanup]
        public static event Action<ARSessionStateChangedEventArgs> stateChanged;

        /// <summary>
        /// The state of the entire system. Use this to determine the status of AR availability and installation.
        /// </summary>
        public static ARSessionState state
        {
            get => s_State;
            private set
            {
                if (s_State == value)
                    return;

                s_State = value;

                UpdateNotTrackingReason();

                if (stateChanged != null)
                    stateChanged(new ARSessionStateChangedEventArgs(state));
            }
        }

        /// <summary>
        /// Resets the AR Session.
        /// </summary>
        /// <remarks>
        /// Resetting the session destroys all trackables and resets device tracking (for example, the position of the session
        /// is reset to the origin).
        /// </remarks>
        public void Reset()
        {
            subsystem?.Reset();

            if (state > ARSessionState.Ready)
                state = ARSessionState.SessionInitializing;
        }

        void SetMatchFrameRateRequested(bool value)
        {
            m_MatchFrameRate = value;
            if (descriptor?.supportsMatchFrameRate == true)
                subsystem.matchFrameRateRequested = value;
        }

#if UNITY_ENABLE_CHECKS
        /// <summary>
        /// Prints a warning to the console if more than one <see cref="ARSessionManager"/>
        /// component is active. There is only a single, global AR Session; this
        /// component controls that session. If two or more <see cref="ARSessionManager"/>s are
        /// simultaneously active, then they both issue commands to the same session.
        /// Although this can cause unintended behavior, it is not expressly forbidden.
        ///
        /// This method is resource-intensive and should not be called frequently.
        /// </summary>
        static void WarnIfMultipleARSessions()
        {
            var sessions = FindObjectsByType<ARSessionManager>();
            if (sessions.Length > 1)
            {
                // Compile a list of session names
                string sessionNames = "";
                foreach (var session in sessions)
                {
                    sessionNames += $"\t{session.name}\n";
                }

                Debug.LogWarningFormat(
                    "Multiple active AR Sessions found. These will conflict with each other, so you should only have " +
                    "one active ARSessionManager at a time. Found these active sessions:\n{0}", sessionNames);
            }
        }
#endif

        static XRSessionSubsystem GetSubsystem()
        {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                var loader = XRGeneralSettings.Instance.Manager.activeLoader;
                if (loader != null)
                    return loader.GetLoadedSubsystem<XRSessionSubsystem>();
            }

            return null;
        }

        /// <summary>
        /// Start checking the availability of XR on the current device.
        /// </summary>
        /// <remarks>
        /// The availability check can be asynchronous, so this is implemented as a coroutine.
        /// It is safe to call this multiple times; if called a second time while an availability
        /// check is being made, it returns a new coroutine which waits on the first.
        ///
        /// Use <see cref="XRSubsystem{TSubsystem, TSubsystemDescriptor, TProvider}.TryStartAsync"/>
        /// with <see cref="XRSubsystemStartOptions.InstallSoftwareIfNeeded"/> instead.
        /// </remarks>
        /// <returns>An <c>IEnumerator</c> used for a coroutine.</returns>
        [Obsolete("CheckAvailability has been deprecated in AR Foundation version 6.7. Use XRSubsystemStartOptions.InstallSoftwareIfNeeded with TryStartAsync instead.", true)]
        public static IEnumerator CheckAvailability()
        {
            yield return null;
        }

        /// <summary>
        /// Begin installing AR software on the current device (if supported).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Installation can be asynchronous, so this is implemented as a coroutine.
        /// It is safe to call this multiple times, but you must first call <see cref="CheckAvailability"/>.
        /// </para><para>
        /// You must call <see cref="CheckAvailability"/> before trying to start the installation,
        /// and the <see cref="state"/> must not be <see cref="ARSessionState.Unsupported"/>
        /// or this method will throw <see cref="System.InvalidOperationException"/>.
        /// </para><para>
        /// Use <c>TryInstallAsync</c> on the platform-specific session subsystem, or pass
        /// <see cref="XRSubsystemStartOptions.InstallSoftwareIfNeeded"/> to
        /// <see cref="XRSubsystem{TSubsystem, TSubsystemDescriptor, TProvider}.TryStartAsync"/> instead.
        /// </para>
        /// </remarks>
        /// <returns>An <c>IEnumerator</c> used for a coroutine.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if <see cref="state"/> is
        ///     <see cref="ARSessionState.None"/> or <see cref="ARSessionState.Unsupported"/>.</exception>
        [Obsolete("Install has been deprecated in AR Foundation version 6.7. Pass XRSubsystemStartOptions.InstallSoftwareIfNeeded to TryStartAsync instead.", true)]
        public static IEnumerator Install()
        {
            yield return null;
        }

        /// <summary>
        /// Creates and initializes the session subsystem. Begins checking for availability.
        /// </summary>
        protected override void OnEnable()
        {
#if UNITY_ENABLE_CHECKS
            WarnIfMultipleARSessions();
#endif

            // Cache these values and restore them in OnDisable
            m_VSyncCount = QualitySettings.vSyncCount;
            m_TargetFrameRate = Application.targetFrameRate;
            m_WasFrameRateSet = false;

            base.OnEnable();
        }

        /// <inheritdoc/>
        protected override void OnBeforeStart()
        {
            if (subsystem != null)
            {
                // These values must be set before the subsystem is started by SubsystemLifecycleManager.
                SetMatchFrameRateRequested(m_MatchFrameRate);
                subsystem.requestedTrackingMode = m_TrackingMode.ToFeature();
            }
        }

        void Awake()
        {
            s_NotTrackingReason = NotTrackingReason.None;
        }

        void Update()
        {
            if (subsystem == null || !subsystem.running)
                return;

            m_TrackingMode = subsystem.requestedTrackingMode.ToTrackingMode();
            if (subsystem.matchFrameRateEnabled && m_MatchFrameRate)
            {
                m_WasFrameRateSet = true;
                Application.targetFrameRate = subsystem.frameRate;
                QualitySettings.vSyncCount = 0;
            }

            subsystem.Update(new XRSessionUpdateParams
            {
                screenOrientation = Screen.orientation,
                screenDimensions = new Vector2Int(Screen.width, Screen.height)
            });

            switch (subsystem.trackingState)
            {
                case TrackingState.None:
                case TrackingState.Limited:
                    state = ARSessionState.SessionInitializing;
                    break;
                case TrackingState.Tracking:
                    state = ARSessionState.SessionTracking;
                    break;
            }
        }

        void OnApplicationPause(bool paused)
        {
            if (subsystem == null)
                return;

            if (paused)
                subsystem.OnApplicationPause();
            else
                subsystem.OnApplicationResume();
        }

        /// <summary>
        /// Invoked when this `MonoBehaviour` is disabled. Affects the <see cref="state"/>.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // Only set back to ready if we were previously running
            if (state > ARSessionState.Ready)
            {
                state = ARSessionState.Ready;
            }

            // Restore values cached in OnEnable
            if (m_WasFrameRateSet)
            {
                QualitySettings.vSyncCount = m_VSyncCount;
                Application.targetFrameRate = m_TargetFrameRate;
            }
        }

        /// <summary>
        /// Invoked when this `MonoBehaviour` is destroyed. Affects the <see cref="state"/>.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Only set back to ready if we were previously running
            if (state > ARSessionState.Ready)
                state = ARSessionState.Ready;
        }

        static void UpdateNotTrackingReason()
        {
            switch (state)
            {
                case ARSessionState.None:
                case ARSessionState.SessionInitializing:
                    s_NotTrackingReason = GetSubsystem()?.notTrackingReason ?? NotTrackingReason.Unsupported;
                    break;
                case ARSessionState.Ready:
                case ARSessionState.SessionTracking:
                    s_NotTrackingReason = NotTrackingReason.None;
                    break;
                case ARSessionState.Unsupported:
                default:
                    s_NotTrackingReason = NotTrackingReason.Unsupported;
                    break;
            }
        }
    }
}
