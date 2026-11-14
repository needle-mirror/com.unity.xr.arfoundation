using System;
using UnityEngine.Rendering;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Simulation
{
    [AddComponentMenu("")]
    [ExecuteAlways] // Required for callbacks in editor mode
    // ImageEffectAllowedInSceneView is obsolete in Unity 7000.0 but still copies this component to the Scene view
    // camera from the main camera each frame, including in URP. Keep it until it is replaced by checking for
    // CameraType.SceneView and reading the component from Camera.main.
#pragma warning disable CS0618
    [ImageEffectAllowedInSceneView]
#pragma warning restore CS0618
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    class XREnvironmentViewCamera : MonoBehaviour
    {
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        internal static event Action<Camera> preRender;

#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        internal static event Action<Camera> postRender;

        [SerializeField]
        [HideInInspector]
        Camera m_Camera;

#if UNITY_6000_5_OR_NEWER
        internal Camera camera => m_Camera;
#else
        internal new Camera camera => m_Camera;
#endif

        void OnEnable()
        {
            m_Camera = GetComponent<Camera>();

            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }

        // The render pipeline callbacks fire for every camera, so filter to the camera this component is attached to.
        // Compare GameObjects rather than m_Camera, because the copy of this component on the scene view camera
        // has its serialized m_Camera overwritten with the main camera's reference.
        void OnBeginCameraRendering(ScriptableRenderContext _, Camera renderingCamera)
        {
            if (renderingCamera == null || renderingCamera.gameObject != gameObject)
                return;

            preRender?.Invoke(renderingCamera);
        }

        void OnEndCameraRendering(ScriptableRenderContext _, Camera renderingCamera)
        {
            if (renderingCamera == null || renderingCamera.gameObject != gameObject)
                return;

            postRender?.Invoke(renderingCamera);
        }
    }
}
