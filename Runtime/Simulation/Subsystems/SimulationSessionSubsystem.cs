using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of
    /// [XRSessionSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem).
    /// </summary>
    public sealed class SimulationSessionSubsystem : XRSessionSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Session";

        class SimulationProvider : Provider
        {
            CameraPoseProvider m_CameraPoseProvider;
            EnvironmentManager m_EnvironmentManager = new EnvironmentManager();

            public override TrackingState trackingState => TrackingState.Tracking;

            public override Promise<SessionAvailability> GetAvailabilityAsync() =>
                Promise<SessionAvailability>.CreateResolvedPromise(SessionAvailability.Installed | SessionAvailability.Supported);

            public override void Start()
            {
                m_CameraPoseProvider = CameraPoseProvider.AddPoseProviderToScene();

                SetupSimulation();
            }

            public override void Stop()
            {
                ShutdownSimulation();

                Object.Destroy(m_CameraPoseProvider.gameObject);
                m_CameraPoseProvider = null;
            }

            void SetupSimulation()
            {
                m_EnvironmentManager.SetupEnvironment();
            }

            void ShutdownSimulation()
            {
                m_EnvironmentManager.TearDownEnvironment();
            }
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
