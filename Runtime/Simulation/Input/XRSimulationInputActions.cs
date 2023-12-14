using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Simulation
{
    class XRSimulationInputActions : ScriptableObject
    {
        [SerializeField, Tooltip("Action for toggling navigation actions on/off. If not set, actions will be active by default")]
        InputActionReference m_UnlockInputActionReference;

        [SerializeField, Tooltip("Action for controlling movement")]
        InputActionReference m_MoveInputActionReference;

        [SerializeField, Tooltip("Action for controlling rotation")]
        InputActionReference m_LookInputActionReference;

        [SerializeField, Tooltip("Action for activating fast movement")]
        InputActionReference m_SprintInputActionReference;

        internal InputActionReference unlockInputActionReference => m_UnlockInputActionReference;

        internal InputActionReference moveInputActionReference => m_MoveInputActionReference;

        internal InputActionReference lookInputActionReference => m_LookInputActionReference;

        internal InputActionReference sprintInputActionReference => m_SprintInputActionReference;
    }
}
