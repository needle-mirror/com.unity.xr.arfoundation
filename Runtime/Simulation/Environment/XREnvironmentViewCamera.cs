using System;

namespace UnityEngine.XR.Simulation
{
    [AddComponentMenu("")]
    [ExecuteAlways] // Required for callbacks in editor mode
    [ImageEffectAllowedInSceneView] // This copies the component to the scene view camera from main camera per frame
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    class XREnvironmentViewCamera : MonoBehaviour
    {
        internal static event Action<Camera> preRender;
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
        }

        void OnPreRender()
        {
            preRender?.Invoke(m_Camera);
        }

        void OnPostRender()
        {
            postRender?.Invoke(m_Camera);
        }
    }
}
