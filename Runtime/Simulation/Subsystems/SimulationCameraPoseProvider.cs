using System.Runtime.InteropServices;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// This component takes updates from mouse and keyboard input and uses it to compute a new camera pose,
    /// which it then passes to the XR Input subsystem in native code.
    /// </summary>
    /// <remarks>
    /// The <see cref="SimulationCameraSubsystem"/> is responsible to create this component at runtime.
    /// </remarks>
    [AddComponentMenu("")]
    public class SimulationCameraPoseProvider : MonoBehaviour
    {
        static SimulationCameraPoseProvider s_Instance;

        CameraFPSModeHandler m_FPSModeHandler;

        void OnEnable()
        {
            if (Application.isPlaying)
                m_FPSModeHandler = new CameraFPSModeHandler();
        }

        void Update()
        {
            if (Application.isPlaying && XRSimulationPreferences.Instance.enableNavigation)
                m_FPSModeHandler.HandleGameInput();

            if (!m_FPSModeHandler.moveActive)
                return;

            var pose = m_FPSModeHandler.CalculateMovement(transform.GetWorldPose(), true);
            UpdatePose(pose);
        }

        void UpdatePose(Pose pose)
        {
            transform.SetWorldPose(pose);

            var localPose = transform.GetLocalPose();
            NativeApi.SetCameraPose(localPose.position.x, localPose.position.y, localPose.position.z,
                localPose.rotation.x, localPose.rotation.y, localPose.rotation.z, localPose.rotation.w);
        }

        internal void SetSimulationEnvironment(SimulationEnvironment simulationEnvironment)
        {
            if (simulationEnvironment != null)
            {
                m_FPSModeHandler.movementBounds = simulationEnvironment.cameraMovementBounds;
                m_FPSModeHandler.useMovementBounds = true;

                UpdatePose(simulationEnvironment.cameraStartingPose);
            }
        }

        internal static SimulationCameraPoseProvider GetOrCreateSimulationCameraPoseProvider()
        {
            if (!s_Instance)
            {
                var go = GameObjectUtils.Create("SimulationCamera");
                s_Instance = go.AddComponent<SimulationCameraPoseProvider>();
                var camera = go.AddComponent<Camera>();
                camera.enabled = false;
            }

            return s_Instance;
        }

        static class NativeApi
        {
            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_SetCameraPose")]
            public static extern void SetCameraPose(float pos_x, float pos_y, float pos_z,
            float rot_x, float rot_y, float rot_z, float rot_w);
        }
    }
}
