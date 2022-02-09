using System;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Settings for AR Simulation.
    /// </summary>
    [XRConfigurationData("Simulation", SimulationSettings.k_SettingsKey)]
    [Serializable]
    public class SimulationSettings : ScriptableObject
    {
        const string k_SettingsKey = "com.unity.xr.arfoundation.simulation_settings";
    }
}
