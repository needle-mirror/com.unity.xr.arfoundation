using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Simulation
{
    [ScriptableSettingsPath(SimulationConstants.userSettingsPath)]
    class XRSimulationPreferences : ScriptableSettings<XRSimulationPreferences>
    {
        static readonly string k_BasePath = "Packages/com.unity.xr.arfoundation/";
        static readonly string k_DefaultInputActionsPath = k_BasePath + "Assets/InputActions/DefaultXRSimulationInputActions.asset";

        [SerializeField]
        bool m_HasInputActionUpgrade;

        [SerializeField, Tooltip("Simulation environment prefab")]
        GameObject m_EnvironmentPrefab;

        [SerializeField, Tooltip("Fallback simulation environment prefab")]
        GameObject m_FallbackEnvironmentPrefab;

        [SerializeField, Tooltip("Button type action for toggling navigation actions on/off. If not set, actions will be active by default")]
        InputActionReference m_UnlockInputActionReference;

        [SerializeField, Tooltip("Value (Vector 3) type action for controlling movement")]
        InputActionReference m_MoveInputActionReference;

        [SerializeField, Tooltip("Value (Delta/Vector 2) type action for controlling rotation")]
        InputActionReference m_LookInputActionReference;

        [SerializeField, Tooltip("Button type action for activating fast movement")]
        InputActionReference m_SprintInputActionReference;

        [SerializeField, Tooltip("Camera rotation speed")]
        [Range(0.5f, 3)]
        float m_LookSpeed = 1;

        [SerializeField, Tooltip("Base camera movement speed")]
        [Range(0.5f, 3)]
        float m_MoveSpeed = 1;

        [SerializeField, Tooltip("Modifier applied to base camera movement speed for faster movement")]
        [Range(2, 5)]
        int m_MoveSpeedModifier = 3;

        /// <summary>
        /// The prefab for the simulation environment.
        /// </summary>
        internal GameObject environmentPrefab
        {
            get => m_EnvironmentPrefab;
            set => m_EnvironmentPrefab = value;
        }

        internal GameObject fallbackEnvironmentPrefab => m_FallbackEnvironmentPrefab;

        /// <summary>
        /// The current simulation environment prefab, or a fallback environment prefab if no environment is set.
        /// </summary>
        internal GameObject activeEnvironmentPrefab =>
            m_EnvironmentPrefab != null ? m_EnvironmentPrefab : m_FallbackEnvironmentPrefab;

        internal InputActionReference unlockInputActionReference
        {
            get => m_UnlockInputActionReference;
            private set => m_UnlockInputActionReference = value;
        }

        internal InputActionReference moveInputActionReference
        {
            get => m_MoveInputActionReference;
            private set => m_MoveInputActionReference = value;
        }

        internal InputActionReference lookInputActionReference
        {
            get => m_LookInputActionReference;
            private set => m_LookInputActionReference = value;
        }

        internal InputActionReference sprintInputActionReference
        {
            get => m_SprintInputActionReference;
            private set => m_SprintInputActionReference = value;
        }

        internal float lookSpeed => m_LookSpeed;
        internal float moveSpeed => m_MoveSpeed;
        internal int moveSpeedModifier => m_MoveSpeedModifier;

#if UNITY_EDITOR
        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (BaseInstance != null && !BaseInstance.m_HasInputActionUpgrade) SetupInputActions();
        }

        // Sets up input actions in existing scriptable object asset if it has not already been upgraded
        void SetupInputActions()
        {
            if (BaseInstance == null)
                return;

            var defaultInputActions = GetDefaultInputActions();

            if (defaultInputActions == null)
            {
                Debug.LogWarning("Missing default input actions asset. Unable to setup simulation input actions.");
                return;
            }

            BaseInstance.unlockInputActionReference = defaultInputActions.unlockInputActionReference;
            BaseInstance.moveInputActionReference = defaultInputActions.moveInputActionReference;
            BaseInstance.lookInputActionReference = defaultInputActions.lookInputActionReference;
            BaseInstance.sprintInputActionReference = defaultInputActions.sprintInputActionReference;

            BaseInstance.m_HasInputActionUpgrade = true;

            EditorUtility.SetDirty(BaseInstance);
            AssetDatabase.SaveAssetIfDirty(BaseInstance);
        }

        internal static XRSimulationInputActions GetDefaultInputActions()
        {
            return AssetDatabase.LoadAssetAtPath<XRSimulationInputActions>(k_DefaultInputActionsPath);
        }
#endif
    }
}
