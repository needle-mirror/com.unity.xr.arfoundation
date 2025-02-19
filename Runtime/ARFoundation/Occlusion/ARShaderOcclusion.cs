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
        const string k_SoftOcclusionShaderKeyword = "XR_SOFT_OCCLUSION";

        const string k_EnvironmentDepthProjectionMatricesPropertyName = "_EnvironmentDepthProjectionMatrices";
        const string k_NdcLinearConversionParametersPropertyName = "_NdcLinearConversionParameters";

        SoftOcclusionPreprocessor m_SoftOcclusionPreprocessor;

        Matrix4x4[] m_EnvironmentDepthReprojectionMatrices = new Matrix4x4[2];
        bool m_OcclusionShaderKeywordsForModeInitialized;

        internal static event Action<GameObject> shaderOcclusionComponentEnabled;
        internal static event Action<GameObject> shaderOcclusionComponentDisabled;

        [SerializeField, HideInInspector]
        AROcclusionManager m_OcclusionManager;

        [SerializeField]
        [Tooltip("The shader keywords to enable: hard occlusion, soft occlusion, or neither.")]
        AROcclusionShaderMode m_OcclusionShaderMode = AROcclusionShaderMode.HardOcclusion;

        [SerializeField]
        [Tooltip("The preprocessing shader, if any, to include in the build for soft occlusion.")]
        Shader m_SoftOcclusionPreprocessShader;

        /// <summary>
        /// Enables a global shader keyword to enable hard or soft occlusion. To implement occlusion in your app, use a
        /// provided URP shader or modify your custom shader to support <see cref="hardOcclusionShaderKeyword"/> and/or
        /// <see cref="softOcclusionShaderKeyword"/> keywords.
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

                if (Application.isPlaying
                    && value == AROcclusionShaderMode.SoftOcclusion
                    && m_SoftOcclusionPreprocessor == null
                    && m_SoftOcclusionPreprocessShader != null)
                    m_SoftOcclusionPreprocessor = new SoftOcclusionPreprocessor(m_SoftOcclusionPreprocessShader);
            }
        }

        /// <summary>
        /// The shader keyword to enable hard occlusion.
        /// </summary>
        public string hardOcclusionShaderKeyword => k_HardOcclusionShaderKeyword;

        /// <summary>
        /// The shader keyword to enable soft occlusions.
        /// </summary>
        public string softOcclusionShaderKeyword => k_SoftOcclusionShaderKeyword;

        /// <summary>
        /// Shader property ID for the depth view-projection matrix array.
        /// </summary>
        public int environmentDepthProjectionMatricesPropertyId { get; private set; }

        /// <summary>
        /// Shader property ID for Vector2 with parameters for conversion depth between NDC and linear.
        /// </summary>
        public int ndcLinearConversionParametersPropertyId { get; private set; }

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

            ndcLinearConversionParametersPropertyId = Shader.PropertyToID(k_NdcLinearConversionParametersPropertyName);

            if (occlusionShaderMode == AROcclusionShaderMode.SoftOcclusion
                && m_SoftOcclusionPreprocessShader != null)
                m_SoftOcclusionPreprocessor = new SoftOcclusionPreprocessor(m_SoftOcclusionPreprocessShader);
        }

        void OnDestroy()
        {
            m_SoftOcclusionPreprocessor?.Dispose();
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

            Shader.SetGlobalMatrixArray(environmentDepthProjectionMatricesPropertyId, m_EnvironmentDepthReprojectionMatrices);
            var ndcToLinearDepthParams = GetNdcToLinearDepthParameters(nearFarPlanes.nearZ, nearFarPlanes.farZ);
            Shader.SetGlobalVector(ndcLinearConversionParametersPropertyId, ndcToLinearDepthParams);

            if (!m_OcclusionShaderKeywordsForModeInitialized)
            {
                m_OcclusionShaderKeywordsForModeInitialized = true;
                SetOcclusionShaderKeywordsForMode(m_OcclusionShaderMode);
            }

            if (m_OcclusionShaderMode == AROcclusionShaderMode.SoftOcclusion)
                // assuming that the first texture in the list is depth texture
                m_SoftOcclusionPreprocessor.PreprocessDepthTexture(eventArgs.externalTextures[0]);
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

        static Vector4 GetNdcToLinearDepthParameters(float near, float far)
        {
            float invDepthFactor;
            float depthOffset;

            if (far < near || float.IsInfinity(far))
            {
                invDepthFactor = -2.0f * near;
                depthOffset = -1.0f;
            }
            else
            {
                invDepthFactor = -2.0f * far * near / (far - near);
                depthOffset = -(far + near) / (far - near);
            }

            return new Vector4(invDepthFactor, depthOffset, 0, 0);
        }

        static Matrix4x4 GetProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
        {
            float x = 2.0f * near / (right - left);
            float y = 2.0f * near / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            const float e = -1.0f;

            var ndcToLinearParams = GetNdcToLinearDepthParameters(near, far);

            var projMatrix = new Matrix4x4();
            projMatrix.SetRow(0, new Vector4(x, 0, a, 0));
            projMatrix.SetRow(1, new Vector4(0, y, b, 0));
            projMatrix.SetRow(2, new Vector4(0, 0, ndcToLinearParams.y, ndcToLinearParams.x));
            projMatrix.SetRow(3, new Vector4(0, 0, e, 0));

            return projMatrix;
        }

        static void SetOcclusionShaderKeywordsForMode(AROcclusionShaderMode mode)
        {
            switch (mode)
            {
                case AROcclusionShaderMode.HardOcclusion:
                    Shader.DisableKeyword(k_SoftOcclusionShaderKeyword);
                    Shader.EnableKeyword(k_HardOcclusionShaderKeyword);
                    break;
                case AROcclusionShaderMode.SoftOcclusion:
                    Shader.DisableKeyword(k_HardOcclusionShaderKeyword);
                    Shader.EnableKeyword(k_SoftOcclusionShaderKeyword);
                    break;
                default:
                case AROcclusionShaderMode.None:
                    Shader.DisableKeyword(k_HardOcclusionShaderKeyword);
                    Shader.DisableKeyword(k_SoftOcclusionShaderKeyword);
                    break;
            }
        }
    }
}
