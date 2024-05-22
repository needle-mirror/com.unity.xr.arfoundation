using System;
using System.Runtime.InteropServices;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// This component takes updates from Unity Input System bindings and uses it to compute a new camera pose,
    /// which it then passes to the XR Input subsystem in native code.
    /// </summary>
    /// <remarks>
    /// The <see cref="SimulationCameraSubsystem"/> is responsible to create this component at runtime.
    /// </remarks>
    [AddComponentMenu("")]
    public class SimulationCameraPoseProvider : MonoBehaviour
    {
        static SimulationCameraPoseProvider s_Instance;

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

            if (s_Instance == null)
                s_Instance = this;
        }

        void OnDestroy()
        {
            if (s_Instance == this)
                s_Instance = null;
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

                if (m_InputHandlerEnabled)
                {
                    UpdatePose(simulationEnvironment.cameraStartingPose);
                }
            }
        }

        internal static SimulationCameraPoseProvider GetOrCreateSimulationCameraPoseProvider()
        {
            if (s_Instance == null)
            {
                var go = GameObjectUtils.Create("SimulationCamera");
                // s_Instance gets assigned in the Awake() event handler.  This way whether the component
                // is added through some other means, or added as a result of this method call, then the
                // s_Instance static member will get set.
                go.AddComponent<SimulationCameraPoseProvider>();
                go.AddComponent<Camera>().enabled = false;
            }

            return s_Instance;
        }

        [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_SetCameraPose")]
        static extern void SetCameraPose(float pos_x, float pos_y, float pos_z,
        float rot_x, float rot_y, float rot_z, float rot_w);
    }
}
