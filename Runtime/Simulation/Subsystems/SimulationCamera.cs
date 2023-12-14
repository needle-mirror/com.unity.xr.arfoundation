using System.Runtime.InteropServices;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Takes updates from InputSystem bindings and uses it to compute a new camera transform
    /// which is passed to our InputSubsystem in native code.
    /// </summary>
    class SimulationCamera : MonoBehaviour
    {
        static SimulationCamera s_Instance;

        bool m_InputHandlerEnabled;
        CameraFPSModeHandler m_FPSModeHandler;

        void OnEnable()
        {
            ToggleControls(true);
        }

        void OnDisable()
        {
            ToggleControls(false);
        }

        void Awake()
        {
            m_FPSModeHandler = new CameraFPSModeHandler();
        }

        void ToggleControls(bool active)
        {
            if (m_FPSModeHandler == null || active == m_InputHandlerEnabled)
                return;

            if (active)
            {
                InputSystem.InputSystem.onAfterUpdate += OnInputUpdate;
                m_FPSModeHandler.OnEnable();
            }
            else
            {
                InputSystem.InputSystem.onAfterUpdate -= OnInputUpdate;
                m_FPSModeHandler.OnDisable();
            }

            m_InputHandlerEnabled = active;
        }

        void OnInputUpdate()
        {
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

        internal void SetSimulationEnvironment(SimulationEnvironment simulationEnvironment)
        {
            if (simulationEnvironment != null)
            {
                m_FPSModeHandler.movementBounds = simulationEnvironment.cameraMovementBounds;
                m_FPSModeHandler.useMovementBounds = true;

                UpdatePose(simulationEnvironment.cameraStartingPose);
            }
        }

        internal static SimulationCamera GetOrCreateSimulationCamera()
        {
            if (!s_Instance)
            {
                var go = GameObjectUtils.Create("SimulationCamera");
                s_Instance = go.AddComponent<SimulationCamera>();
                var camera = go.AddComponent<Camera>();
                camera.enabled = false;
            }

            return s_Instance;
        }

        [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_SetCameraPose")]
        static extern void SetCameraPose(float pos_x, float pos_y, float pos_z,
        float rot_x, float rot_y, float rot_z, float rot_w);
    }
}
