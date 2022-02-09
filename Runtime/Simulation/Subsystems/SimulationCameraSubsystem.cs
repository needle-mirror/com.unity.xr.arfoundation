using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of
    /// [`XRCameraSubsystem`](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem).
    /// </summary>
    public sealed class SimulationCameraSubsystem : XRCameraSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Camera";

        class SimulationProvider : Provider
        {
            public override bool permissionGranted => true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            var cInfo = new XRCameraSubsystemCinfo {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationCameraSubsystem),
                supportsCameraConfigurations = true,
                supportsCameraImage = true,
            };

            if (!XRCameraSubsystem.Register(cInfo))
                Debug.LogError("Cannot register the camera subsystem");
        }
    }

}
