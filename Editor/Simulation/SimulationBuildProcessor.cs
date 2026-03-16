using System;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// Build processor for XR Simulation.
    /// </summary>
    [Obsolete("SimulationBuildProcessor is deprecated in AR Foundation 6.5. This class was never used.")]
    public class SimulationBuildProcessor : XRBuildHelper<XRSimulationSettings>
    {
        /// <summary>
        /// Settings key for <see cref="XRSimulationSettings"/>.
        /// </summary>
        /// <value>A string specifying the key to be used to set/get settings in EditorBuildSettings.</value>
        public override string BuildSettingsKey => XRSimulationSettings.k_SettingsKey;

        /// <summary>
        /// Override of base IPreprocessBuildWithReport
        /// </summary>
        /// <param name="report">BuildReport instance passed in from build pipeline.</param>
        public override void OnPreprocessBuild(BuildReport report)
        {
            // Do nothing. You can't load an Editor-only scriptable object into a build!
        }
    }
}
