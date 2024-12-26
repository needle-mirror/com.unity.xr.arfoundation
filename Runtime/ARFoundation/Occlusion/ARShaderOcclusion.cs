using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Add this component alongside <c>AROcclusionManager</c> to copy the depth camera's texture into shader memory.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AROcclusionManager))]
    [AddComponentMenu("XR/AR Foundation/AR Shader Occlusion")]
    [HelpURL("features/occlusion")]
    public class ARShaderOcclusion : MonoBehaviour
    {
        // Depth data is expected to be provided in a right-handed coordinate system.
        // In a right-handed system, the forward vector points away from the viewer, which is opposite
        // to a left-handed system where it points towards the viewer. Therefore, to interpret the depth data
        // correctly in a left-handed system, we must invert the z component.
        static readonly Vector3 k_CoordSystemScale = new(1, 1, -1);

        const string k_HardOcclusionShaderKeyword = "XR_HARD_OCCLUSION";

        const string k_EnvironmentDepthProjectionMatricesPropertyName = "_EnvironmentDepthProjectionMatrices";
        const string k_EnvironmentDepthNearFarPlanesPropertyName = "_EnvironmentDepthNearFarPlanes";

        // Using a large threshold value to approximate infinity when comparing the far clip plane in the shader.
        // The isinf function may not work reliably in Vulkan, potentially due to different handling or representation
        // of infinite values.
        const float k_InfinityThreshold = 1e+30f;

        Matrix4x4[] m_EnvironmentDepthReprojectionMatrices = new Matrix4x4[2];
        bool m_OcclusionShaderKeywordsForModeInitialized;

        internal static event Action<GameObject> shaderOcclusionComponentEnabled;
        internal static event Action<GameObject> shaderOcclusionComponentDisabled;

        [SerializeField, HideInInspector]
        AROcclusionManager m_OcclusionManager;

        [SerializeField]
        AROcclusionShaderMode m_OcclusionShaderMode = AROcclusionShaderMode.HardOcclusion;

        /// <summary>
        /// <para>Enables a global shader keyword to enable hard occlusion or default.</para>
        /// <para>To enable per-object occlusion, use a provided cross-platform occlusion URP shader or modify your
        /// custom shader to support <see cref="hardOcclusionShaderKeyword"/> keyword.</para>
        /// </summary>
        public AROcclusionShaderMode occlusionShaderMode
        {
            get => m_OcclusionShaderMode;
            set
            {
                if (m_OcclusionShaderMode == value)
                    return;

                m_OcclusionShaderMode = value;
                SetOcclusionShaderKeywordsForMode(value);
            }
        }

        /// <summary>
        /// The shader keyword to enable hard occlusion.
        /// </summary>
        public string hardOcclusionShaderKeyword => k_HardOcclusionShaderKeyword;

        /// <summary>
        /// Shader property ID for the depth view-projection matrix array.
        /// </summary>
        public int environmentDepthProjectionMatricesPropertyId { get; private set; }

        /// <summary>
        /// Shader property ID for near and far clip planes.
        /// </summary>
        public int environmentDepthNearFarPlanesPropertyId { get; private set; }

        void Reset()
        {
            m_OcclusionManager = GetComponent<AROcclusionManager>();
        }

        void Awake()
        {
            if (m_OcclusionManager == null)
                m_OcclusionManager = GetComponent<AROcclusionManager>();

            environmentDepthProjectionMatricesPropertyId =
                Shader.PropertyToID(k_EnvironmentDepthProjectionMatricesPropertyName);

            environmentDepthNearFarPlanesPropertyId =
                Shader.PropertyToID(k_EnvironmentDepthNearFarPlanesPropertyName);
        }

        void OnEnable()
        {
            shaderOcclusionComponentEnabled?.Invoke(gameObject);
            m_OcclusionManager.frameReceived += OnOcclusionFrameReceived;
        }

        void OnDisable()
        {
            m_OcclusionManager.frameReceived -= OnOcclusionFrameReceived;
            shaderOcclusionComponentDisabled?.Invoke(gameObject);
        }

        void OnOcclusionFrameReceived(AROcclusionFrameEventArgs eventArgs)
        {
            // always send textures and set keywords so that ARCore, ARKit, and Simulation can be supported by this
            // component in the future.
            foreach (var tex in eventArgs.externalTextures)
            {
                Shader.SetGlobalTexture(tex.propertyId, tex.texture);
            }

            RenderingUtility.SetShaderKeywordsGlobal(eventArgs.shaderKeywords);

            if (!eventArgs.TryGetPoses(out var poses)
                || !eventArgs.TryGetFovs(out var fovs)
                || !eventArgs.TryGetNearFarPlanes(out var nearFarPlanes))
            {
                // ARCore, ARKit, and XR Simulation return here.
                return;
            }

            var numPoses = poses.Count;
            if (m_EnvironmentDepthReprojectionMatrices.Length != numPoses)
                m_EnvironmentDepthReprojectionMatrices = new Matrix4x4[numPoses];

            for (int i = 0; i < numPoses; ++i)
            {
                var viewMatrix = Matrix4x4.TRS(poses[i].position, poses[i].rotation, k_CoordSystemScale).inverse;
                m_EnvironmentDepthReprojectionMatrices[i] = GetViewProjectionMatrix(fovs[i], nearFarPlanes, viewMatrix);
            }

            var far = float.IsInfinity(nearFarPlanes.farZ) ? k_InfinityThreshold : nearFarPlanes.farZ;
            var environmentDepthProjectionParams = new Vector3(nearFarPlanes.nearZ, far, k_InfinityThreshold);
            Shader.SetGlobalVector(environmentDepthNearFarPlanesPropertyId, environmentDepthProjectionParams);
            Shader.SetGlobalMatrixArray(
                environmentDepthProjectionMatricesPropertyId, m_EnvironmentDepthReprojectionMatrices);

            if (!m_OcclusionShaderKeywordsForModeInitialized)
            {
                m_OcclusionShaderKeywordsForModeInitialized = true;
                if (m_OcclusionShaderMode != AROcclusionShaderMode.None)
                    SetOcclusionShaderKeywordsForMode(m_OcclusionShaderMode);
            }
        }

        static Matrix4x4 GetViewProjectionMatrix(XRFov fov, XRNearFarPlanes planes, Matrix4x4 trackingSpaceViewMatrix)
        {
            var near = planes.nearZ;
            var far = planes.farZ;
            var l = Mathf.Tan(-Mathf.Abs(fov.angleLeft)) * near;
            var r = Mathf.Tan(Mathf.Abs(fov.angleRight)) * near;
            var b = Mathf.Tan(-Mathf.Abs(fov.angleDown)) * near;
            var t = Mathf.Tan(Mathf.Abs(fov.angleUp)) * near;

            var projectionMatrix = GetProjectionMatrix(l, r, b, t, near, far);

            return projectionMatrix * trackingSpaceViewMatrix;
        }

        static Matrix4x4 GetProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
        {
            float x = 2.0f * near / (right - left);
            float y = 2.0f * near / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            const float e = -1.0f;
            float c, d;

            if (float.IsInfinity(far))
            {
                c = -1.0f;
                d = -2.0f * near;
            }
            else
            {
                c = -(far + near) / (far - near);
                d = -(2.0f * far * near) / (far - near);
            }

            var projMatrix = new Matrix4x4();
            projMatrix.SetRow(0, new Vector4(x, 0, a, 0));
            projMatrix.SetRow(1, new Vector4(0, y, b, 0));
            projMatrix.SetRow(2, new Vector4(0, 0, c, d));
            projMatrix.SetRow(3, new Vector4(0, 0, e, 0));

            return projMatrix;
        }

        static void SetOcclusionShaderKeywordsForMode(AROcclusionShaderMode mode)
        {
            switch (mode)
            {
                case AROcclusionShaderMode.HardOcclusion:
                    Shader.EnableKeyword(k_HardOcclusionShaderKeyword);
                    break;
                default:
                case AROcclusionShaderMode.None:
                    Shader.DisableKeyword(k_HardOcclusionShaderKeyword);
                    break;
            }
        }
    }
}
