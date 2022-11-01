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

        class SimulationProvider : Provider
        {
            SimulationCamera m_SimulationCamera;
            SimulationMeshSubsystem m_MeshSubsystem;
            SimulationEnvironmentScanner m_SimulationEnvironmentScanner;

            Camera m_XROriginCamera;
            int m_PreviousCullingMask;
            bool m_Initialized;

            public override TrackingState trackingState => TrackingState.Tracking;

            public override Promise<SessionAvailability> GetAvailabilityAsync() =>
                Promise<SessionAvailability>.CreateResolvedPromise(SessionAvailability.Installed | SessionAvailability.Supported);

            void Initialize()
            {
                s_SimulationSceneManager ??= new SimulationSceneManager();
                m_SimulationCamera = SimulationCamera.GetOrCreateSimulationCamera();

                if (SimulationMeshSubsystem.GetActiveSubsystemInstance() != null)
                    m_MeshSubsystem = new SimulationMeshSubsystem();

                SetupSimulation();

                var xrOrigin = Object.FindObjectOfType<XROrigin>();

                if (xrOrigin == null)
                    throw new NullReferenceException($"An XR Origin is required in the scene, none found.");

                m_XROriginCamera = xrOrigin.Camera;

                m_SimulationEnvironmentScanner = SimulationEnvironmentScanner.instance;
                m_SimulationEnvironmentScanner.Initialize(m_SimulationCamera,
                    s_SimulationSceneManager.environmentScene.GetPhysicsScene(),
                    s_SimulationSceneManager.simulationEnvironment.gameObject);

                m_Initialized = true;
            }

            public override void Start()
            {
                if (!m_Initialized)
                    Initialize();

#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif
                m_MeshSubsystem?.Start();
                m_SimulationEnvironmentScanner.Start();

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

                m_SimulationEnvironmentScanner.Stop();

#if UNITY_EDITOR
                AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
#endif
            }

            public override void Destroy()
            {
                if (m_SimulationCamera != null)
                {
                    Object.Destroy(m_SimulationCamera.gameObject);
                    m_SimulationCamera = null;
                }

                if (m_MeshSubsystem != null)
                {
                    m_MeshSubsystem.Dispose();
                    m_MeshSubsystem = null;
                }

                if (m_SimulationEnvironmentScanner != null)
                {
                    m_SimulationEnvironmentScanner.Dispose();
                    m_SimulationEnvironmentScanner = null;
                }

                if (s_SimulationSceneManager != null)
                {
                    s_SimulationSceneManager.TearDownEnvironment();
                    s_SimulationSceneManager = null;
                }

                m_Initialized = false;
            }

            public override void Update(XRSessionUpdateParams updateParams)
            {
                m_SimulationEnvironmentScanner.Update();
            }

            void SetupSimulation()
            {
                s_SimulationSceneManager.SetupEnvironment();
                m_SimulationCamera.SetSimulationEnvironment(s_SimulationSceneManager.simulationEnvironment);
            }

#if UNITY_EDITOR
            void OnBeforeAssemblyReload()
            {
#if UNITY_2022_2_OR_NEWER
                const string domainReloadOptions = "<b>Stop Playing and Recompile</b>";
#else
                const string domainReloadOptions = 
                    "either <b>Recompile After Finished Playing</b> or <b>Stop Playing and Recompile</b>";
#endif
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
            XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationSessionSubsystem),
                supportsInstall = false,
                supportsMatchFrameRate = false,
            });
        }
    }
}
