using System;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace UnityEngine.XR.Simulation
{
    [AddComponentMenu("")]
    [ExecuteAlways] // Required for callbacks in editor mode
    [ImageEffectAllowedInSceneView] // This copies the component to the scene view camera from main camera per frame
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
