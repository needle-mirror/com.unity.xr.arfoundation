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
            {
                m_FPSModeHandler = new CameraFPSModeHandler();
            }
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                m_FPSModeHandler.HandleGameInput();
            }

            var pose = m_FPSModeHandler.CalculateMovement(transform.GetWorldPose(), true);
            transform.SetWorldPose(pose);

            var newPose = transform.GetLocalPose();
            SetCameraPose(newPose.position.x, newPose.position.y, newPose.position.z,
                newPose.rotation.x, newPose.rotation.y, newPose.rotation.z, newPose.rotation.w);
        }

        internal static CameraPoseProvider AddPoseProviderToScene()
        {
            // TODO: Centralize management of playmode game objects somewhere
            var go = GameObjectUtils.Create("CameraPoseProvider");
            var cameraPoseProvider = go.AddComponent<CameraPoseProvider>();

            return cameraPoseProvider;
        }

        [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_SetCameraPose")]
        public static extern void SetCameraPose(float pos_x, float pos_y, float pos_z,
        float rot_x, float rot_y, float rot_z, float rot_w);
    }
}
