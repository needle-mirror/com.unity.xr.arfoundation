using System;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [InitializeOnLoad]
    static class AREnvironmentViewUtilities
    {
        static bool s_HasEnvironmentSettings;
        static BaseSimulationSceneManager s_SimulationSceneManager;
        static SimulationRenderSettings s_SimulationRenderSettings;

        // Used to lock out the use of temp render settings and lighting when switching scenes.
        static bool s_RenderingOverrideEnabled;
        static bool s_LightingOverrideActive;

        internal static SimulationRenderSettings simulationRenderSettings => s_SimulationRenderSettings;
        internal static bool lightingOverrideActive => s_LightingOverrideActive;

        internal static bool renderingOverrideEnabled
        {
            get => s_RenderingOverrideEnabled;
            set => s_RenderingOverrideEnabled = value;
        }

        static AREnvironmentViewUtilities()
        {
            s_SimulationRenderSettings = new SimulationRenderSettings();
            s_SimulationRenderSettings.UseSceneRenderSettings();

            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            CameraTextureProvider.preRenderCamera += PreRenderCameraTextureProvider;
            CameraTextureProvider.postRenderCamera += PostRenderCameraTextureProvider;
#if !UNITY_2022_1_OR_NEWER
            AREnvironmentViewCamera.preRender += PreRenderEnvironmentViewCamera;
            AREnvironmentViewCamera.postRender += PostRenderEnvironmentViewCamera;
#endif
            // Environment View Overlay state can be shared between all scene views after a domain reload.
            // Need to make sure to cache what views are using the overlay before a domain reload so the overlay
            // usage can be restored after the reload.
            AssemblyReloadEvents.beforeAssemblyReload += BeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += AfterAssemblyReload;
            SimulationEditorUtilities.CheckIsSimulationSubsystemEnabled();

            EditorApplication.delayCall += () =>
            {
                // Need to delay a frame since overlays are not fully re-initialized when opening the editor
                AREnvironmentView.instance.FindAllSceneAndEnvironmentViews();
            };

            BaseSimulationSceneManager.environmentTeardownStarted += StartTearDownEnvironment;
            // Do this last since attaching to the event also calls it if initialized
            BaseSimulationSceneManager.environmentSetupFinished += FinishSetupEnvironment;
        }

        static void PlayModeStateChanged(PlayModeStateChange mode)
        {
            var environmentView = AREnvironmentView.instance;
            switch (mode)
            {
                case PlayModeStateChange.EnteredEditMode:
                    environmentView.OnEnable();
                    environmentView.SetUpOrChangeEnvironment();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    environmentView.CleanUpEnvironment();
                    environmentView.OnDisable();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    s_SimulationRenderSettings.UseSceneRenderSettings();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    PostRenderCameraTextureProvider(null);
                    environmentView.OverrideCameraSceneMask(EditorSceneManager.DefaultSceneCullingMask);
                    break;
            }
        }

        static void FinishSetupEnvironment()
        {
            s_SimulationSceneManager = Application.isPlaying ?
                SimulationSessionSubsystem.simulationSceneManager :
                AREnvironmentView.instance.activeSceneManager;
            s_HasEnvironmentSettings = s_SimulationSceneManager.simulationEnvironment != null;

            s_RenderingOverrideEnabled = true;
        }

        static void StartTearDownEnvironment()
        {
            RestoreBaseLighting();
            // Stop using the environment render settings before tearing down the environment
            s_RenderingOverrideEnabled = false;

            s_SimulationSceneManager = null;
            s_HasEnvironmentSettings = false;
        }

        static void PreRenderCameraTextureProvider(Camera camera)
        {
            if (!Application.isPlaying ||!s_HasEnvironmentSettings || s_SimulationRenderSettings == null)
                return;

            SetOverrideLighting();
        }

        static void PostRenderCameraTextureProvider(Camera camera)
        {
            if (!Application.isPlaying)
                return;

            RestoreBaseLighting();
        }

        static void PreRenderEnvironmentViewCamera(Camera camera)
        {
#if !UNITY_2022_1_OR_NEWER
            if (!s_LightingOverrideActive)
                s_SimulationRenderSettings.UseSceneRenderSettings();

            // Due to the way AREnvironmentViewCamera is copied, passing the camera in the callback is the wrong camera.
            // But scene view camera is already the current camera when we reach this step.
            if (!AREnvironmentView.instance.environmentCameras.Contains(Camera.current))
                return;

            if (!s_HasEnvironmentSettings || s_SimulationRenderSettings == null)
                return;

            SetOverrideLighting();
#endif
        }

        static void PostRenderEnvironmentViewCamera(Camera camera)
        {
#if !UNITY_2022_1_OR_NEWER
            // Due to the way AREnvironmentViewCamera is copied, passing the camera in the callback is the wrong camera.
            // But scene view camera is already the current camera when we reach this step.
            if (!AREnvironmentView.instance.environmentCameras.Contains(Camera.current))
                return;

            RestoreBaseLighting();
#endif
        }

        static void SetOverrideLighting()
        {
            if (!s_RenderingOverrideEnabled || s_SimulationSceneManager == null)
                return;

            if (s_SimulationSceneManager.environmentScene.IsValid())
            {
                Unsupported.SetOverrideLightingSettings(s_SimulationSceneManager.environmentScene);

                var simulationEnvironment = s_SimulationSceneManager.simulationEnvironment;
                if (simulationEnvironment != null)
                    simulationEnvironment.renderSettings.ApplyTempRenderSettings();

                s_LightingOverrideActive = true;
            }
        }

        internal static void RestoreBaseLighting()
        {
            if (!s_RenderingOverrideEnabled && s_LightingOverrideActive)
                Debug.Break();

            if (!s_RenderingOverrideEnabled)
                return;

            if (s_LightingOverrideActive)
            {
                s_LightingOverrideActive = false;
                Unsupported.RestoreOverrideLightingSettings();
                s_SimulationRenderSettings?.ApplyTempRenderSettings();
            }
        }

        /// <summary>
        /// Track the changes in XR plugin management for the XR Simulation loader being enabled or disabled.
        /// </summary>
        /// <returns>The hidden visual element that is tracking changes in the xr loader.</returns>
        internal static VisualElement TrackStandAloneXRSettingsChange()
        {
            var trackerElement = new VisualElement();

            var standAloneSettingsSerializedObject = new SerializedObject(XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone));
            var managerInstanceProp = standAloneSettingsSerializedObject.FindProperty("m_LoaderManagerInstance");
            var managerInstanceSerializedObject = new SerializedObject(managerInstanceProp.objectReferenceValue);
            var property = managerInstanceSerializedObject.FindProperty("m_Loaders.Array");

            property.Next(true);
            trackerElement.TrackPropertyValue(property, _ => SetupListTracker(trackerElement) );

            return trackerElement;
        }

        /// <summary>
        /// Creates elements to track changes in every loader in the loaders list.
        /// </summary>
        /// <param name="trackerElement">Parent tracker visual element.</param>
        static void SetupListTracker(VisualElement trackerElement)
        {
            var standAloneSettingsSerializedObject = new SerializedObject(XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone));
            var managerInstanceProp = standAloneSettingsSerializedObject.FindProperty("m_LoaderManagerInstance");
            var managerInstanceSerializedObject = new SerializedObject(managerInstanceProp.objectReferenceValue);
            var property = managerInstanceSerializedObject.FindProperty("m_Loaders.Array");

            var endProperty = property.GetEndProperty();

            property.NextVisible(true); // Expand the first child.
            var childIndex = 0;
            // Iterate each property under the array and populate the container with preview elements
            do
            {
                // Stop if we've reached the end of the array
                if (SerializedProperty.EqualContents(property, endProperty))
                    break;

                // Skip the array size property
                if (property.propertyType == SerializedPropertyType.ArraySize)
                    continue;

                VisualElement element;

                if (childIndex < trackerElement.childCount)
                {
                    element = trackerElement[childIndex];
                }
                else
                {
                    element = new VisualElement();
                    trackerElement.Add(element);
                }

                element.TrackPropertyValue(property, _ => SimulationEditorUtilities.CheckIsSimulationSubsystemEnabled());

                ++childIndex;
            }
            while (property.NextVisible(false));   // Never expand children.

            // Remove excess elements if the array is now smaller
            while (childIndex < trackerElement.childCount)
            {
                trackerElement.RemoveAt(trackerElement.childCount - 1);
            }

            SimulationEditorUtilities.CheckIsSimulationSubsystemEnabled();
        }

         internal static bool IsBaseSceneView(object obj)
         {
             if (obj is SceneView sceneView)
                 return !sceneView.GetType().IsSubclassOf(typeof(SceneView));

             return false;
         }

         static void BeforeAssemblyReload()
         {
             s_RenderingOverrideEnabled = false;
             s_SimulationSceneManager = null;
             s_HasEnvironmentSettings = false;
             AREnvironmentView.instance.CacheEnvironmentViewsBeforeReload();
         }

         static void AfterAssemblyReload()
         {
             s_RenderingOverrideEnabled = false;
             AREnvironmentView.instance.RestoreEnvironmentViewsAfterReload();
         }
    }
}
