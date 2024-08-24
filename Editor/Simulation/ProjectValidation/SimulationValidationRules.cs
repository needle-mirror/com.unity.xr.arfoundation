using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    static class SimulationProjectValidationRules
    {
        const string k_Category = "XR Simulation";
        const string k_EditorPreferencesScriptCompilationKey = "ScriptCompilationDuringPlay";

        // note: these correspond with the Unity Editor's internal enumerated values
        internal enum ScriptCompilationDuringPlayOptions
        {
            RecompileAndContinuePlaying = 0,
            RecompileAfterFinishedPlaying = 1,
            StopPlayingAndRecompile = 2
        }

        [InitializeOnLoadMethod]
        static void AddValidationRules()
        {
            var SimulationGlobalRules = new[]
            {
                new BuildValidationRule
                {
                    Category = k_Category,
                    Message = "XR Simulation is only compatible with the <b>Recompile After Finished Playing</b> " +
                        "option for the <b>Script Changes While Playing</b> preference.",
                    IsRuleEnabled = IsXRSimulationPluginEnabled,
                    CheckPredicate = IsCurrentScriptCompilationPreferenceValid,
                    FixItMessage = "Open the <b>Preferences</b> window, then go to <b>General</b> > <b>Script " +
                        "Changes While Playing</b>, and set <b>Recompile After Finished Playing</b>.",
                    FixIt = FixScriptCompilationPreference,
                    FixItAutomatic = true,
                    Error = true
                }
            };

            BuildValidator.AddRules(BuildTargetGroup.Standalone, SimulationGlobalRules);
        }

        static bool IsCurrentScriptCompilationPreferenceValid()
        {
            var currentPreference = EditorPrefs.GetInt(k_EditorPreferencesScriptCompilationKey);
            return currentPreference == (int)ScriptCompilationDuringPlayOptions.RecompileAfterFinishedPlaying;
        }

        static void FixScriptCompilationPreference()
        {
            EditorPrefs.SetInt(k_EditorPreferencesScriptCompilationKey, (int)ScriptCompilationDuringPlayOptions.RecompileAfterFinishedPlaying);
        }

        static bool IsXRSimulationPluginEnabled()
        {
            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(
                BuildTargetGroup.Standalone);

            if (generalSettings == null)
                return false;

            var managerSettings = generalSettings.AssignedSettings;

            return managerSettings != null && managerSettings.activeLoaders.Any(loader => loader is SimulationLoader);
        }
    }
}
