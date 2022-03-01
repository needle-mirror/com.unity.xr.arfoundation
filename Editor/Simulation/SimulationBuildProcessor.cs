using UnityEditor.XR.Management;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// Build processor for XR Simulation.
    /// </summary>
    public class SimulationBuildProcessor : XRBuildHelper<SimulationSettings>
    {
        /// <summary>
        /// Settings key for <see cref="SimulationSettings"/>.
        /// </summary>
        /// <returns>A string specifying the key to be used to set/get settigns in EditorBuildSettings.</returns>
        public override string BuildSettingsKey => SimulationSettings.k_SettingsKey;
    }
}
