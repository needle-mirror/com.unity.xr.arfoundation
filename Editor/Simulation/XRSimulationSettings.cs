using System;
using UnityEngine;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// Build settings for XR Simulation.
    /// </summary>
    [Serializable]
    [Obsolete("XRSimulationSettings has been deprecated in AR Foundation 6.5. Use XRSimulationRuntimeSettingsInstead")]
    public class XRSimulationSettings : ScriptableObject
    {
        /// <summary>
        /// Configuration key for the settings.
        /// </summary>
        public const string k_SettingsKey = "com.unity.xr.arfoundation.simulation_settings";

        /// <summary>
        /// Get the instance of the <see cref="XRSimulationSettings"/>.
        /// </summary>
        public static XRSimulationSettings currentSettings => EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out XRSimulationSettings settings) ? settings : null;

        void Awake()
        {
            EditorApplication.delayCall += XREnvironmentViewUtilities.ToggleXREnvironmentOverlays;
        }
    }
}
