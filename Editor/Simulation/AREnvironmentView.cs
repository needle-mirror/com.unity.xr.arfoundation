using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [Serializable]
    [ExecuteAlways]
    class AREnvironmentView : ScriptableObject
    {
        const string k_ViewName = "AR Environment";
        static GUIContent s_SimulationSubsystemNotLoadedContent = new GUIContent($"{k_ViewName} is not Available.\nEnable XR Simulation in Project Settings > XR Plug-in Management.");
        static GUIContent s_BaseSceneViewContent = new GUIContent($"{k_ViewName} is not Available.\nUse a Scene View window.");
        static GUIContent s_AREnvironmentViewTitleContent;

        static AREnvironmentView s_Instance;

        [SerializeField]
        GUIContent m_SceneViewTitleContent;

        [SerializeField]
        EditorSimulationSceneManager m_EditorSimulationSceneManager;

        [SerializeField]
        AREnvironmentViewCamera m_EnvironmentViewCamera;

        [SerializeField]
        SceneView[] m_ActiveEnvironmentViewsAtReload;

        [SerializeField]
        bool m_Initialized;

        ulong m_CurrentSceneMask;

        SimulationXRayManager m_XRayManager;

        HashSet<SceneView> m_AllSceneViews = new HashSet<SceneView>();
        HashSet<SceneView> m_EnvironmentViews = new HashSet<SceneView>();
        HashSet<Camera> m_EnvironmentCameras = new HashSet<Camera>();

        static GUIContent arEnvironmentViewTitleContent
        {
            get
            {
                if (s_AREnvironmentViewTitleContent == null)
                {
                    s_AREnvironmentViewTitleContent = EditorGUIUtility.TrIconContent(AREnvironmentToolbarOverlay.arEnvironmentIconPath);
                    s_AREnvironmentViewTitleContent.text = k_ViewName;
                }

                return s_AREnvironmentViewTitleContent;
            }
        }

        bool useEditorSceneManager => SimulationEditorUtilities.simulationSubsystemEnabled
            && !EditorApplication.isPlayingOrWillChangePlaymode;

        internal BaseSimulationSceneManager activeSceneManager => !EditorApplication.isPlayingOrWillChangePlaymode
            ? m_EditorSimulationSceneManager
            : SimulationSessionSubsystem.simulationSceneManager;

        internal HashSet<Camera> environmentCameras => m_EnvironmentCameras;

        internal static AREnvironmentView instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<AREnvironmentView>(true);
                    if (s_Instance == null)
                    {
                        s_Instance = CreateInstance<AREnvironmentView>();
                        s_Instance.hideFlags = HideFlags.HideAndDontSave;
                    }
                }

                return s_Instance;
            }
        }

        [MenuItem("Window/XR/AR Foundation/" + k_ViewName)]
        static void GetAREnvironmentView()
        {
            var environmentView = instance;
            SceneView sceneView = null;
            if (environmentView.m_EnvironmentViews.Count > 0)
            {
                var lastActiveView = SceneView.lastActiveSceneView;
                sceneView = environmentView.m_EnvironmentViews.Contains(lastActiveView) ? lastActiveView : instance.m_EnvironmentViews.First();
            }

            if (sceneView == null)
            {
                sceneView = EditorWindow.CreateWindow<SceneView>();
                environmentView.m_AllSceneViews.Add(sceneView);
                if (sceneView.TryGetOverlay(AREnvironmentToolbarOverlay.overlayId, out var environmentOverlay))
                    environmentOverlay.displayed = true;
            }

            sceneView.Show();
            sceneView.Focus();
        }

        internal void OnEnable()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else if (s_Instance != this)
            {
                UnityObjectUtils.Destroy(this);
                return;
            }

            SceneView.beforeSceneGui += BeforeSceneGui;
            SceneView.duringSceneGui += DuringSceneGui;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved += SetUpOrChangeEnvironment;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved += SimulationSubsystemDisabledMessage;
            PrefabStage.prefabStageOpened += PrefabStageOpened;
            PrefabStage.prefabStageClosing += PrefabStageClosing;

            if (useEditorSceneManager)
                m_EditorSimulationSceneManager = new EditorSimulationSceneManager();

            m_XRayManager = new SimulationXRayManager();

            var environmentAssetsManager = SimulationEnvironmentAssetsManager.Instance;
            environmentAssetsManager.activeEnvironmentChanged += SetUpOrChangeEnvironment;

            foreach (var sceneViewObj in SceneView.sceneViews)
            {
                var setView = useEditorSceneManager && m_EnvironmentViews.Count > 0;

                if (sceneViewObj is SceneView sceneView)
                {
                    CacheTitleContent(sceneView);
                    m_CurrentSceneMask = useEditorSceneManager
                        ? EditorSceneManager.GetSceneCullingMask(m_EditorSimulationSceneManager.environmentScene)
                        : EditorSceneManager.DefaultSceneCullingMask;

                    if (setView && m_EnvironmentViews.Contains(sceneView))
                        SetViewToEnvironment(sceneView);
                }
            }
        }

        internal void OnDisable()
        {
            SceneView.beforeSceneGui -= BeforeSceneGui;
            SceneView.duringSceneGui -= DuringSceneGui;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved -= SetUpOrChangeEnvironment;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved -= SimulationSubsystemDisabledMessage;
            PrefabStage.prefabStageOpened -= PrefabStageOpened;
            PrefabStage.prefabStageClosing -= PrefabStageClosing;

            var environmentAssetsManager = SimulationEnvironmentAssetsManager.Instance;
            environmentAssetsManager.activeEnvironmentChanged -= SetUpOrChangeEnvironment;

            CleanUpEnvironmentViews();
            CleanUpEnvironment();

            m_EditorSimulationSceneManager = null;
            m_XRayManager = null;
        }

        internal void AddEnvironmentView(SceneView sceneView)
        {
            if (!AREnvironmentViewUtilities.IsBaseSceneView(sceneView))
            {
                BaseSceneViewMessage(sceneView);
                m_EnvironmentViews.Remove(sceneView);
                m_EnvironmentCameras.Remove(sceneView.camera);
                return;
            }

            // When a new scene view is opened from add tab when an AR Environment View is open or if an
            // AR Environment View was the last Scene View open when opening a new scene view window, the new scene view
            // can open with the AR Environment Overlay already open. This will catch that case but will not effect
            // opening a new AR Environment View from the main toolbar Window > AR Foundation > AR Environment.
            if (!m_AllSceneViews.Contains(sceneView))
            {
                if (sceneView.TryGetOverlay(AREnvironmentToolbarOverlay.overlayId, out var environmentOverlay))
                    environmentOverlay.displayed = false;

                m_AllSceneViews.Add(sceneView);
                return;
            }

            CacheTitleContent(sceneView);

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;

            var previousCount = m_EnvironmentViews.Count;
            m_EnvironmentViews.Add(sceneView);
            m_EnvironmentCameras.Add(sceneView.camera);

            sceneView.titleContent = arEnvironmentViewTitleContent;

            if (!SimulationEditorUtilities.simulationSubsystemEnabled)
            {
                sceneView.ShowNotification(s_SimulationSubsystemNotLoadedContent);
                return;
            }

            if (CheckRemoveNotifications(sceneView))
                sceneView.RemoveNotification();

            if (previousCount == 0)
                SetUpOrChangeEnvironment();

            if (activeSceneManager == null)
                return;

            var simulationEnvironment = activeSceneManager.simulationEnvironment;
            if (simulationEnvironment != null)
            {
                var pivot = simulationEnvironment.defaultViewPivot;
                var rotation = simulationEnvironment.defaultViewPose.rotation;
                var size = simulationEnvironment.defaultViewSize;

                sceneView.LookAt(pivot, rotation, size, sceneView.orthographic, true);
            }
        }

        internal void RemoveEnvironmentView(SceneView sceneView)
        {
            if (!AREnvironmentViewUtilities.IsBaseSceneView(sceneView))
                return;

            m_EnvironmentViews.Remove(sceneView);
            m_EnvironmentCameras.Remove(sceneView.camera);

            if (sceneView == null || sceneView.camera == null)
                return;

            sceneView.camera.overrideSceneCullingMask = EditorSceneManager.DefaultSceneCullingMask;

            if (sceneView.titleContent == arEnvironmentViewTitleContent)
                sceneView.titleContent = m_SceneViewTitleContent;

            if (useEditorSceneManager && m_EnvironmentViews.Count == 0)
                CleanUpEnvironment();
        }

        void BeforeSceneGui(SceneView sceneView)
        {
            if (!m_Initialized)
                return;

            // Cache the render settings in case they are being modified by the user.
            if (!AREnvironmentViewUtilities.lightingOverrideActive)
                AREnvironmentViewUtilities.simulationRenderSettings.UseSceneRenderSettings();
        }

        void DuringSceneGui(SceneView sceneView)
        {
            if (!m_Initialized)
                return;

            if (!m_AllSceneViews.Contains(sceneView))
            {
                var displayOverlay = m_EnvironmentViews.Contains(sceneView);
                if (sceneView.TryGetOverlay(AREnvironmentToolbarOverlay.overlayId, out var environmentOverlay))
                    environmentOverlay.displayed = displayOverlay;

                m_AllSceneViews.Add(sceneView);
                DoSceneViewXRay();
                return;
            }

            if (m_EnvironmentViews.Contains(sceneView))
                sceneView.camera.overrideSceneCullingMask = m_CurrentSceneMask;

            if (m_EnvironmentViews.Contains(sceneView) && activeSceneManager != null)
            {
                CheckARCamera();

                var scene = activeSceneManager.environmentScene;
                var hasXRayRegion = XRayRuntimeUtils.xRayRegions.TryGetValue(scene, out var xRayRegion);
                m_XRayManager?.UpdateXRayShader(hasXRayRegion, xRayRegion);

                var simEnv = activeSceneManager.simulationEnvironment;
                if (simEnv != null)
                {
                    Handles.color = Color.cyan;
                    SimulationEnvironment.DrawWireCamera(Handles.DrawLine, simEnv.cameraStartingPose, sceneView.size * 0.06f);
                }
            }
            else
            {
                DoSceneViewXRay();
            }
        }

        internal void SetUpOrChangeEnvironment()
        {
            CleanUpEnvironment();

            if (useEditorSceneManager && m_EnvironmentViews.Count > 0 && m_EditorSimulationSceneManager != null)
            {
                m_EditorSimulationSceneManager.SetupEnvironment();
                m_CurrentSceneMask = EditorSceneManager.GetSceneCullingMask(m_EditorSimulationSceneManager.environmentScene);

                foreach (var sceneView in m_EnvironmentViews)
                {
                    SetViewToEnvironment(sceneView);
                }
            }
        }

        void SetViewToEnvironment(SceneView sceneView)
        {
            sceneView.camera.overrideSceneCullingMask = m_CurrentSceneMask;
            var simulationEnvironment = activeSceneManager.simulationEnvironment;
            if (simulationEnvironment != null)
            {
                var pivot = simulationEnvironment.defaultViewPivot;
                var rotation = simulationEnvironment.defaultViewPose.rotation;
                var size = simulationEnvironment.defaultViewSize;

                sceneView.LookAt(pivot, rotation, size, sceneView.orthographic, true);
            }
        }

        internal void CleanUpEnvironment()
        {
            m_CurrentSceneMask = EditorSceneManager.DefaultSceneCullingMask;
            AREnvironmentViewUtilities.RestoreBaseLighting();
            AREnvironmentViewUtilities.renderingOverrideEnabled = false;
            m_EditorSimulationSceneManager?.TearDownEnvironment();
        }

        internal void CleanUpEnvironmentViews()
        {
            m_CurrentSceneMask = EditorSceneManager.DefaultSceneCullingMask;
            OverrideCameraSceneMask(EditorSceneManager.DefaultSceneCullingMask);
        }

        /// <summary>
        /// Checks the the 'main' camera has the <see cref="AREnvironmentViewCamera"/> component added to it.
        /// This component is used to set render overrides when rendering the scene view camera. It is done this way
        /// since rendering components are copied over from the main camera to the scene view camera before every render.
        /// Directly trying to add the <see cref="AREnvironmentViewCamera"/> to the scene view camera will not work
        /// since it will be removed before rendering.
        /// </summary>
        void CheckARCamera()
        {
            if (m_EnvironmentViewCamera != null && m_EnvironmentViewCamera.camera == Camera.main)
                return;

            var camera = Camera.main;
            if (camera == null)
            {
                var xrOrigin = FindObjectOfType<XROrigin>();
                if (xrOrigin != null)
                    camera = xrOrigin.Camera;
            }

            if (camera == null)
                return;

            if (m_EnvironmentViewCamera != null && m_EnvironmentViewCamera.camera != camera)
            {
                UnityObjectUtils.Destroy(m_EnvironmentViewCamera);
            }
            else
            {
                var environmentViewCamera = camera.GetComponent<AREnvironmentViewCamera>();
                m_EnvironmentViewCamera = environmentViewCamera != null ? environmentViewCamera
                    : camera.gameObject.AddComponent<AREnvironmentViewCamera>();

                m_EnvironmentViewCamera.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
            }
        }

        void DoSceneViewXRay()
        {
            if (m_XRayManager == null)
                return;

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                m_XRayManager.UpdateXRayShader(false, null);
                return;
            }

            var useXRay = XRayRuntimeUtils.xRayRegions.TryGetValue(prefabStage.scene, out var xRayRegion);
            m_XRayManager.UpdateXRayShader(useXRay, xRayRegion);
        }

        internal void OverrideCameraSceneMask(ulong sceneMask)
        {
            m_CurrentSceneMask = sceneMask;

            foreach (var sceneView in m_EnvironmentViews)
            {
                if (sceneView.camera != null)
                    sceneView.camera.overrideSceneCullingMask = sceneMask;
            }
        }

        void SimulationSubsystemDisabledMessage()
        {
            foreach (var sceneView in m_EnvironmentViews)
            {
                if (!SimulationEditorUtilities.simulationSubsystemEnabled)
                    sceneView.ShowNotification(s_SimulationSubsystemNotLoadedContent);
                else if (CheckRemoveNotifications(sceneView))
                    sceneView.RemoveNotification();
            }
        }

        void PrefabStageOpened(PrefabStage stage)
        {
            foreach (var sceneViewObj in SceneView.sceneViews)
            {
                if (sceneViewObj is SceneView sceneView)
                    RemoveEnvironmentView(sceneView);
            }

            m_EnvironmentViews.Clear();
        }

        void PrefabStageClosing(PrefabStage stage)
        {
            FindAllSceneAndEnvironmentViews();
        }

        void BaseSceneViewMessage(SceneView sceneView)
        {
            if (!AREnvironmentViewUtilities.IsBaseSceneView(sceneView))
                sceneView.ShowNotification(s_BaseSceneViewContent);
            else if (CheckRemoveNotifications(sceneView))
                sceneView.RemoveNotification();
        }

        bool CheckRemoveNotifications(SceneView sceneView)
        {
            return SimulationEditorUtilities.simulationSubsystemEnabled
                && AREnvironmentViewUtilities.IsBaseSceneView(sceneView);
        }

        internal void FindAllSceneAndEnvironmentViews()
        {
            foreach (var sceneViewObj in SceneView.sceneViews)
            {
                if (sceneViewObj is SceneView sceneView)
                {
                    CacheTitleContent(sceneView);
                    m_AllSceneViews.Add(sceneView);
                    if (sceneView.TryGetOverlay(AREnvironmentToolbarOverlay.overlayId, out var environmentOverlay) && environmentOverlay.displayed)
                    {
                        AddEnvironmentView(sceneView);
                    }
                }
            }

            m_Initialized = true;
        }

        internal void CacheEnvironmentViewsBeforeReload()
        {
            if (m_EnvironmentViews != null && m_EnvironmentViews.Count > 0)
                m_ActiveEnvironmentViewsAtReload = m_EnvironmentViews.ToArray();
            else
                m_ActiveEnvironmentViewsAtReload = Array.Empty<SceneView>();
        }

        internal void RestoreEnvironmentViewsAfterReload()
        {
            if (m_ActiveEnvironmentViewsAtReload == null || m_ActiveEnvironmentViewsAtReload.Length < 1)
                return;

            m_EnvironmentViews.Clear();
            m_EnvironmentViews = new HashSet<SceneView>(m_ActiveEnvironmentViewsAtReload);
            SetUpOrChangeEnvironment();

            foreach (var sceneViewObj in SceneView.sceneViews)
            {
                if (sceneViewObj is SceneView sceneView)
                {
                    CacheTitleContent(sceneView);
                    m_AllSceneViews.Add(sceneView);
                    var displayOverlay = m_EnvironmentViews.Contains(sceneView);
                    if (sceneView.TryGetOverlay(AREnvironmentToolbarOverlay.overlayId, out var environmentOverlay))
                        environmentOverlay.displayed = displayOverlay;

                    if (displayOverlay)
                        AddEnvironmentView(sceneView);
                    else
                        RemoveEnvironmentView(sceneView);
                }
            }
        }

        void CacheTitleContent(SceneView sceneView)
        {
            // Only cache the title contents if we know the view is not a environment view.
            if (m_SceneViewTitleContent == null
                && AREnvironmentViewUtilities.IsBaseSceneView(sceneView)
                && sceneView.titleContent != arEnvironmentViewTitleContent)
            {
                m_SceneViewTitleContent = sceneView.titleContent;
            }
        }
    }
}
