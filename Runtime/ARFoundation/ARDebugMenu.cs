using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Menu that is added to a scene to surface tracking data and visualize trackables in order to aid in debugging.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class ARDebugMenu : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("A debug prefab that visualizes the position of the XROrigin.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_OriginAxisPrefab;

        /// <summary>
        /// Specifies a debug prefab that will be attached to the <see cref="XROrigin"/>.
        /// </summary>
        /// <value>
        /// A debug prefab that will be attached to the XR origin.
        /// </value>
        public GameObject originAxisPrefab
        {
            get => m_OriginAxisPrefab;
            set => m_OriginAxisPrefab = value;
        }

        [SerializeField]
        [Tooltip("A line renderer used to outline the ARPlanes in a scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        LineRenderer m_LineRendererPrefab;

        /// <summary>
        /// Specifies the line renderer that will be used to outline planes in the scene.
        /// </summary>
        /// <value>
        /// A line renderer used to outline planes in the scene.
        /// </value>
        public LineRenderer lineRendererPrefab
        {
            get => m_LineRendererPrefab;
            set => m_LineRendererPrefab = value;
        }

        [SerializeField]
        [Tooltip("The sub-menu that stores all the buttons.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_ButtonsMenu;

        /// <summary>
        /// The sub-menu that contains all the debug buttons.
        /// </summary>
        /// <value>
        /// The GameObject under which all the debug buttons will be parented.
        /// </value>
        public GameObject buttonsMenu
        {
            get => m_ButtonsMenu;
            set => m_ButtonsMenu = value;
        }

        [SerializeField]
        [Tooltip("The button that displays the AR Debug Menu.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_DisplayMenuButton;

        /// <summary>
        /// The button that displays the AR Debug Menu.
        /// </summary>
        /// <value>
        /// A button that will be used to display the AR Debug Menu.
        /// </value>
        public Button displayMenuButton
        {
            get => m_DisplayMenuButton;
            set => m_DisplayMenuButton = value;
        }

        [SerializeField, FormerlySerializedAs("m_ShowSessionOriginButton")]
        [Tooltip("The button that displays the XR origin prefab.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_ShowOriginButton;

        /// <summary>
        /// The button that displays the XR origin prefab.
        /// </summary>
        /// <value>
        /// A button that will be used to display the XR origin prefab.
        /// </value>
        public Button showOriginButton
        {
            get => m_ShowOriginButton;
            set => m_ShowOriginButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays detected AR planes in the scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_ShowPlanesButton;

        /// <summary>
        /// The button that displays detected AR planes in the scene.
        /// </summary>
        /// <value>
        /// A button that will be used to display detected AR planes in the scene.
        /// </value>
        public Button showPlanesButton
        {
            get => m_ShowPlanesButton;
            set => m_ShowPlanesButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays anchors in the scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_ShowAnchorsButton;

        /// <summary>
        /// The button that displays anchors in the scene.
        /// </summary>
        /// <value>
        /// A button that will be used to display anchors in the scene.
        /// </value>
        public Button showAnchorsButton
        {
            get => m_ShowAnchorsButton;
            set => m_ShowAnchorsButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays detected point clouds in the scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_ShowPointCloudsButton;

        /// <summary>
        /// The button that displays detected point clouds in the scene.
        /// </summary>
        /// <value>
        /// A button that will be used to display detected point clouds in the scene.
        /// </value>
        public Button showPointCloudsButton
        {
            get => m_ShowPointCloudsButton;
            set => m_ShowPointCloudsButton = value;
        }

        [SerializeField]
        [Tooltip("The text object that will display current FPS.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Text m_FpsLabel;

        /// <summary>
        /// The text object that will display current FPS.
        /// </summary>
        /// <value>
        /// A text object that will display current FPS.
        /// </value>
        public Text fpsLabel
        {
            get => m_FpsLabel;
            set => m_FpsLabel = value;
        }

        [SerializeField]
        [Tooltip("The text object that will display current tracking mode.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Text m_TrackingModeLabel;

        /// <summary>
        /// The text object that will display current tracking mode.
        /// </summary>
        /// <value>
        /// A text object that will display current tracking mode.
        /// </value>
        public Text trackingModeLabel
        {
            get => m_TrackingModeLabel;
            set => m_TrackingModeLabel = value;
        }

        void Start()
        {
            if(!CheckMenuConfigured())
            {
                enabled = false;
                Debug.LogError($"The menu has not been configured correctly and will currently be disabled. For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.");
            }
        }

        bool CheckMenuConfigured()
        {
            if(m_ButtonsMenu == null || m_DisplayMenuButton == null)
            {
                return false;
            }
            else if(m_ShowOriginButton == null || m_ShowPlanesButton == null || m_ShowAnchorsButton == null ||
                m_ShowPointCloudsButton == null || m_FpsLabel == null || m_TrackingModeLabel == null || m_OriginAxisPrefab == null || m_LineRendererPrefab == null)
            {
                Debug.LogWarning("The menu has not been fully configured so some functionality will be disabled. For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu");
            }

            return true;
        }

        void OnEnable()
        {
            InitMenu();
            ConfigureButtons();
        }

        void OnDisable()
        {
            if(m_Origin)
            {
                var planeManager = m_Origin.GetComponent<ARPlaneManager>();
                if(planeManager)
                {
                    planeManager.planesChanged -= OnPlaneChanged;
                }
            }
        }

        void Update()
        {
            int fps = (int)(1.0f / Time.unscaledDeltaTime);
            if(fps != m_PreviousFps)
            {
                m_FpsLabel.text = $"FPS: {fps}";
                m_PreviousFps = fps;
            }

            var state = (int)m_Session.currentTrackingMode;
            if(state != m_PreviousTrackingMode)
            {
                m_TrackingModeLabel.text = $"Mode: {m_Session.currentTrackingMode}";
                m_PreviousTrackingMode = state;
            }

            if(m_CameraFollow == true)
            {
                FollowCamera();
            }
        }

        void InitMenu()
        {
            var eventSystems = FindObjectsOfType<EventSystem>();
            if(eventSystems.Length == 0)
            {
                Debug.LogError($"Failed to find EventSystem in current scene. As a result, this component will be disabled.");
                enabled = false;
                return;
            }

            var sessions = FindObjectsOfType<ARSession>();
            if(sessions.Length == 0)
            {
                Debug.LogError($"Failed to find ARSession in current scene. As a result, this component will be disabled.");
                enabled = false;
                return;
            }
            m_Session = sessions[0];

            var origins = FindObjectsOfType<XROrigin>();
            if(origins.Length == 0)
            {
                Debug.LogError($"Failed to find XROrigin in current scene. As a result, this component will be disabled.");
                enabled = false;
                return;
            }
            m_Origin = origins[0];

            m_CameraAR = m_Origin.Camera;
#if !UNITY_IOS && !UNITY_ANDROID
            if(m_CameraAR == null)
            {
                Debug.LogError($"Failed to find camera attached to XROrigin. As a result, this component will be disabled.");
                enabled = false;
                return;
            }
#endif
            m_ButtonsMenu.SetActive(false);
            m_DisplayMenuButton.onClick.AddListener(ShowMenu);

            Canvas menu = GetComponent<Canvas>();
#if UNITY_IOS || UNITY_ANDROID
            menu.renderMode = RenderMode.ScreenSpaceOverlay;
#else
            var rectTransform = GetComponent<RectTransform>();
            menu.renderMode = RenderMode.WorldSpace;
            menu.worldCamera  = m_CameraAR;
            m_CameraFollow = true;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 575);
#endif
        }

        void ConfigureButtons()
        {
            if(m_ShowOriginButton && m_OriginAxisPrefab)
            {
                m_ShowOriginButton.interactable = true;
                m_ShowOriginButton.onClick.AddListener(ShowOrigin);
            }

            var planeManager = m_Origin.GetComponent<ARPlaneManager>();
            if(m_ShowPlanesButton && m_LineRendererPrefab && planeManager)
            {
                m_PlaneVisualizers = new GameObject("PlaneVisualizers");
                m_PlaneVisualizers.SetActive(false);
                m_ShowPlanesButton.interactable = true;
                m_ShowPlanesButton.onClick.AddListener(ShowPlanes);
                planeManager.planesChanged += OnPlaneChanged;
            }
        }

        void ShowMenu()
        {
            if(m_ButtonsMenu.activeSelf)
            {
                m_ButtonsMenu.SetActive(false);
                m_DisplayMenuButton.GetComponentInChildren<Text>().text = "Show Debug Menu";
            }
            else
            {
                m_ButtonsMenu.SetActive(true);
                m_DisplayMenuButton.GetComponentInChildren<Text>().text = "Hide Debug Menu";
            }
        }

        void ShowOrigin()
        {
            if(m_OriginAxis == null)
            {
                m_OriginAxis = Instantiate(m_OriginAxisPrefab, m_Origin.transform);
                m_ShowOriginButton.GetComponentInChildren<Text>().text = "Hide XR Origin";
            }
            else if(m_OriginAxis.activeSelf)
            {
               m_OriginAxis.SetActive(false);
               m_ShowOriginButton.GetComponentInChildren<Text>().text = "Show XR Origin";
            }
            else
            {
               m_OriginAxis.SetActive(true);
               m_ShowOriginButton.GetComponentInChildren<Text>().text = "Hide XR Origin";
            }
        }

        void ShowPlanes()
        {
            if(m_PlaneVisualizers.activeSelf)
            {
               m_PlaneVisualizers.SetActive(false);
               m_ShowPlanesButton.GetComponentInChildren<Text>().text = "Show Planes";
            }
            else
            {
               m_PlaneVisualizers.SetActive(true);
               m_ShowPlanesButton.GetComponentInChildren<Text>().text = "Hide Planes";
            }
        }

        void FollowCamera()
        {
            const float distance = 0.3f;
            const float smoothFactor = 0.1f;

            Vector3 targetPosition = m_CameraAR.transform.position + m_CameraAR.transform.forward * distance;
            Vector3 currentPosition = transform.position;

            if(Application.platform == RuntimePlatform.WSAPlayerX86)
            {
                transform.position = Vector3.Lerp(currentPosition, new Vector3(0, 0, targetPosition.z), smoothFactor);
                transform.rotation = Quaternion.LookRotation(currentPosition - m_CameraAR.transform.position);
            }
            else
            {
                transform.position = Vector3.Lerp(currentPosition, targetPosition, smoothFactor);
                transform.rotation = m_CameraAR.transform.rotation;
            }

            float height = 0;
            if (m_CameraAR.orthographic)
                height = m_CameraAR.orthographicSize * 2;
            else
                height = distance * Mathf.Tan(Mathf.Deg2Rad * (m_CameraAR.fieldOfView * 0.5f));

            float heightScale = height / m_CameraAR.scaledPixelHeight;
            transform.localScale = new Vector3(heightScale, heightScale, 1);
        }

        void OnPlaneChanged(ARPlanesChangedEventArgs eventArgs)
        {
            foreach(var plane in eventArgs.added)
            {
                var lineRenderer = GetOrCreateLineRenderer(plane);
                UpdateLine(plane, lineRenderer);
            }
            foreach(var plane in eventArgs.updated)
            {
                var lineRenderer = GetOrCreateLineRenderer(plane);
                UpdateLine(plane, lineRenderer);
            }
            foreach(var plane in eventArgs.removed)
            {
                if(m_PlaneLineRenderers.TryGetValue(plane, out var lineRenderer))
                {
                    m_PlaneLineRenderers.Remove(plane);
                    if(lineRenderer)
                    {
                        Destroy(lineRenderer.gameObject);
                    }
                }
            }
        }

        LineRenderer GetOrCreateLineRenderer(ARPlane plane)
        {
            if(m_PlaneLineRenderers.TryGetValue(plane, out var foundLineRenderer) && foundLineRenderer)
            {
                return foundLineRenderer;
            }

            var go = Instantiate(m_LineRendererPrefab, m_PlaneVisualizers.transform);
            var lineRenderer = go.GetComponent<LineRenderer>();
            m_PlaneLineRenderers[plane] = lineRenderer;

            return lineRenderer;
        }

        void UpdateLine(ARPlane plane, LineRenderer lineRenderer)
        {
            if(!lineRenderer)
            {
                return;
            }

            Transform planeTransform = plane.transform;
            bool useWorldSpace = lineRenderer.useWorldSpace;
            if(!useWorldSpace)
            {
                lineRenderer.transform.SetPositionAndRotation(planeTransform.position, planeTransform.rotation);
            }

            var boundary = plane.boundary;
            lineRenderer.positionCount = boundary.Length;
            for (int i = 0; i < boundary.Length; ++i)
            {
                var point2 = boundary[i];
                var localPoint = new Vector3(point2.x, 0, point2.y);
                if(useWorldSpace)
                {
                    lineRenderer.SetPosition(i, planeTransform.position + (planeTransform.rotation * localPoint));
                }
                else
                {
                    lineRenderer.SetPosition(i, new Vector3(point2.x, 0, point2.y));
                }
            }
        }

        //Managers
        XROrigin m_Origin;

        ARSession m_Session;

        //Visuals
        GameObject m_OriginAxis;

        GameObject m_PlaneVisualizers;

        //Labels
        int m_PreviousFps;

        int m_PreviousTrackingMode = -1;

        //Misc
        Camera m_CameraAR;

        bool m_CameraFollow;

        Dictionary<ARPlane, LineRenderer> m_PlaneLineRenderers = new Dictionary<ARPlane, LineRenderer>();
    }
}
