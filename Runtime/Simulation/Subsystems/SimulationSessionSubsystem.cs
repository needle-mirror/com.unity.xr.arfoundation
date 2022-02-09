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
            GameObject m_SimulationEnvironment;

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
                m_SimulationEnvironment = new GameObject("Simulation Environment Root");
                m_SimulationEnvironment.layer = 30; // TODO: Use settings

                // Couple of dummy objects to be our simulation environment
                var cap1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                cap1.transform.SetParent(m_SimulationEnvironment.transform);
                cap1.transform.localPosition = new Vector3(5, 0, 10);
                var cap2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                cap2.transform.SetParent(m_SimulationEnvironment.transform);
                cap2.transform.localPosition = new Vector3(-5, 0, 10);
            }

            void ShutdownSimulation()
            {
                Object.Destroy(m_SimulationEnvironment);
                m_SimulationEnvironment = null;
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
