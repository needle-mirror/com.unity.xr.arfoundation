using System.Runtime.InteropServices;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Takes mouse and keyboard input and uses it to compute a new camera transform
    /// which is passed to our InputSubsystem in native code.
    /// </summary>
    class CameraPoseProvider : MonoBehaviour
    {
        CameraFPSModeHandler m_FPSModeHandler;

        void OnEnable()
        {
            if (Application.isPlaying)
                m_FPSModeHandler = new CameraFPSModeHandler();
        }

        void Update()
        {
            if (Application.isPlaying)
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
            SetCameraPose(localPose.position.x, localPose.position.y, localPose.position.z,
                localPose.rotation.x, localPose.rotation.y, localPose.rotation.z, localPose.rotation.w);
        }

        public void SetSimulationEnvironment(SimulationEnvironment simulationEnvironment)
        {
            if (simulationEnvironment != null)
            {
                m_FPSModeHandler.movementBounds = simulationEnvironment.cameraMovementBounds;
                m_FPSModeHandler.useMovementBounds = true;

                UpdatePose(simulationEnvironment.cameraStartingPose);
            }
        }

        internal static CameraPoseProvider AddPoseProviderToScene()
        {
            var go = GameObjectUtils.Create("CameraPoseProvider");
            var cameraPoseProvider = go.AddComponent<CameraPoseProvider>();

            return cameraPoseProvider;
        }

        [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_SetCameraPose")]
        public static extern void SetCameraPose(float pos_x, float pos_y, float pos_z,
        float rot_x, float rot_y, float rot_z, float rot_w);
    }
}
