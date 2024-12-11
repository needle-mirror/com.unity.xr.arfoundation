using UnityEditor.XR.Management;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// Build processor for XR Simulation.
    /// </summary>
    public class SimulationBuildProcessor : XRBuildHelper<XRSimulationSettings>
    {
        /// <summary>
        /// Settings key for <see cref="XRSimulationSettings"/>.
        /// </summary>
        /// <value>A string specifying the key to be used to set/get settings in EditorBuildSettings.</value>
        public override string BuildSettingsKey => XRSimulationSettings.k_SettingsKey;
    }
}
