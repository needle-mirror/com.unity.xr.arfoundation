using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of
    /// [XRSessionSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem).
    /// </summary>
    public sealed class SimulationSessionSubsystem : XRSessionSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Session";

        static SimulationSceneManager s_SimulationSceneManager;

        internal static SimulationSceneManager simulationSceneManager => s_SimulationSceneManager;

        internal static event Action s_SimulationSessionReset;

        class SimulationProvider : Provider
        {
            SimulationCameraPoseProvider m_SimulationCameraPoseProvider;
            SimulationMeshSubsystem m_MeshSubsystem;

            Camera m_XROriginCamera;
            int m_PreviousCullingMask;
            bool m_Initialized;
            Guid m_SessionId;

            public override TrackingState trackingState => TrackingState.Tracking;

            public override Promise<SessionAvailability> GetAvailabilityAsync() =>
                Promise<SessionAvailability>.CreateResolvedPromise(SessionAvailability.Installed | SessionAvailability.Supported);

            public override Guid sessionId => m_SessionId;

            protected override bool TryInitialize()
            {
                m_SessionId = Guid.NewGuid();
                return true;
            }

            bool Initialize()
            {
                s_SimulationSceneManager ??= new SimulationSceneManager();
                m_SimulationCameraPoseProvider = SimulationCameraPoseProvider.GetOrCreateSimulationCameraPoseProvider();

                if (SimulationMeshSubsystem.GetActiveSubsystemInstance() != null)
                {
                    m_MeshSubsystem?.Dispose();
                    m_MeshSubsystem = new SimulationMeshSubsystem();
                }

                SetupSimulation();

                var xrOrigin = Object.FindAnyObjectByType<XROrigin>();

                if (xrOrigin == null)
                {
                    Debug.LogError($"An XR Origin is required in the scene, none found.");
                    return false;
                }

                m_XROriginCamera = xrOrigin.Camera;

                SimulationEnvironmentScanner.GetOrCreate().Initialize(
                    m_SimulationCameraPoseProvider,
                    s_SimulationSceneManager.environmentScene.GetPhysicsScene(),
                    s_SimulationSceneManager.simulationEnvironment.gameObject);

                m_Initialized = true;
                return true;
            }

            public override void Start()
            {
                if (!m_Initialized && !Initialize())
                    return;

#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif
                m_MeshSubsystem?.Start();
                SimulationEnvironmentScanner.GetOrCreate().Start();

                m_PreviousCullingMask = m_XROriginCamera.cullingMask;
                m_XROriginCamera.cullingMask &= ~(1 << XRSimulationRuntimeSettings.Instance.environmentLayer);

#if UNITY_EDITOR
                AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif
            }

            public override void Stop()
            {
                if (m_XROriginCamera)
                    m_XROriginCamera.cullingMask = m_PreviousCullingMask;

                SimulationEnvironmentScanner.GetOrCreate().Stop();

#if UNITY_EDITOR
                AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
#endif
            }

            public override void Destroy()
            {
                if (m_SimulationCameraPoseProvider != null)
                {
                    Object.Destroy(m_SimulationCameraPoseProvider.gameObject);
                    m_SimulationCameraPoseProvider = null;
                }

                if (m_MeshSubsystem != null)
                {
                    m_MeshSubsystem.Dispose();
                    m_MeshSubsystem = null;
                }

                SimulationEnvironmentScanner.GetOrCreate().Dispose();

                if (s_SimulationSceneManager != null)
                {
                    s_SimulationSceneManager.TearDownEnvironment();
                    s_SimulationSceneManager = null;
                }

                m_Initialized = false;
            }

            public override void Reset()
            {
                if (!m_Initialized)
                    return;

                m_SessionId = Guid.NewGuid();
                s_SimulationSessionReset?.Invoke();
            }

            public override void Update(XRSessionUpdateParams updateParams)
            {
                SimulationEnvironmentScanner.GetOrCreate().Update();
            }

            void SetupSimulation()
            {
                s_SimulationSceneManager.SetupEnvironment();
                m_SimulationCameraPoseProvider.SetSimulationEnvironment(s_SimulationSceneManager.simulationEnvironment);
            }

#if UNITY_EDITOR
            static void OnBeforeAssemblyReload()
            {
                const string domainReloadOptions =
                    "either <b>Recompile After Finished Playing</b> or <b>Stop Playing and Recompile</b>";

                Debug.LogError(
                    "XR Simulation does not support script recompilation while playing. To disable script compilation"+
                    " while playing, in the Preferences window under <b>General > Script Changes While Playing</b>,"+
                    $" select {domainReloadOptions}.");
            }
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRSessionSubsystemDescriptor.Register(new XRSessionSubsystemDescriptor.Cinfo {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationSessionSubsystem),
                supportsInstall = false,
                supportsMatchFrameRate = false,
            });
        }
    }
}
