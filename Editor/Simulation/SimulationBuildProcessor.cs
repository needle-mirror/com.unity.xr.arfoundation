using UnityEditor.Build.Reporting;
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


        // Note: this class was incorrectly implemented and is deprecated in AR Foundation 6.5.
        // We can't backport the deprecation into an older version, so instead we override the base class to
        // nullify its implementation.

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
