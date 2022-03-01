using System;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Settings for AR Simulation.
    /// </summary>
    [Serializable]
    [XRConfigurationData("XR Simulation", SimulationSettings.k_SettingsKey)]
    public class SimulationSettings : ScriptableObject
    {
        /// <summary>
        /// Configuration key for the settings.
        /// </summary>
        public const string k_SettingsKey = "com.unity.xr.arfoundation.simulation_settings";

        [SerializeField, Tooltip("Simulation environment prefab")]
        GameObject m_EnvironmentPrefab;

        [SerializeField, Tooltip("Fallback simulation environment prefab")]
        GameObject m_FallbackEnvironmentPrefab;

#if !UNITY_EDITOR
        static SimulationSettings s_RuntimeInstance = null;
#endif

        /// <summary>
        /// Get the instance of the <see cref="SimulationSettings"/>.
        /// </summary>
        public static SimulationSettings currentSettings
        {
#if UNITY_EDITOR
                get => EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out SimulationSettings settings) ? settings : null;
#else
                get => s_RuntimeInstance;
#endif
        }

        /// <summary>
        /// The prefab for the simulation environment.
        /// </summary>
        public GameObject environmentPrefab
        {
            get => m_EnvironmentPrefab;
            set { m_EnvironmentPrefab = value; }
        }

        internal GameObject fallbackEnvironmentPrefab => m_FallbackEnvironmentPrefab;

#if !UNITY_EDITOR
        void Awake()
        {
            s_RuntimeInstance = this;
        }
#endif
    }
}
