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
        Matrix4x4[] m_DepthVPMatrices = new Matrix4x4[2];

        internal static event Action<GameObject> shaderOcclusionComponentEnabled;
        internal static event Action<GameObject> shaderOcclusionComponentDisabled;

        Texture2DArray m_TextureArray;

        [SerializeField, HideInInspector]
        AROcclusionManager m_OcclusionManager;

        [SerializeField, HideInInspector]
        Camera m_XRCamera;

        void Reset()
        {
            m_OcclusionManager = GetComponent<AROcclusionManager>();
            m_XRCamera = GetComponent<Camera>();
        }

        void Awake()
        {
            if (m_OcclusionManager == null)
                m_OcclusionManager = GetComponent<AROcclusionManager>();

            if (m_XRCamera == null)
                m_XRCamera = GetComponent<Camera>();
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

        /// <summary>
        /// Transfers occlusion data to shader memory.
        /// </summary>
        void OnOcclusionFrameReceived(AROcclusionFrameEventArgs eventArgs)
        {
            if (eventArgs.gpuTextures == null || eventArgs.gpuTextures.Count == 0)
                return;

            var gpuTextures = eventArgs.gpuTextures;
            var sampleTexture = gpuTextures[0].texture;
            if (m_TextureArray == null ||
                sampleTexture.width != m_TextureArray.width ||
                sampleTexture.height != m_TextureArray.height ||
                gpuTextures.Count != m_TextureArray.depth ||
                sampleTexture.graphicsFormat != m_TextureArray.graphicsFormat)
            {
                m_TextureArray = new Texture2DArray(
                    width: sampleTexture.width,
                    height: sampleTexture.height,
                    depth: gpuTextures.Count,
                    format: sampleTexture.graphicsFormat,
                    flags: Experimental.Rendering.TextureCreationFlags.None);

                // Assign the Texture2DArray to a global texture with ID of the first texture in the array
                Shader.SetGlobalTexture(gpuTextures[0].propertyId, m_TextureArray);
            }

            if (m_DepthVPMatrices.Length != gpuTextures.Count)
                m_DepthVPMatrices = new Matrix4x4[gpuTextures.Count];

            for (int i = 0; i < gpuTextures.Count; ++i)
            {
                Graphics.CopyTexture(gpuTextures[i].texture, 0, m_TextureArray, i);
                m_DepthVPMatrices[i] = GetViewProjectionMatrix(eventArgs.fovs[i], eventArgs.nearFarPlanes, m_XRCamera.worldToCameraMatrix);
            }

            Shader.SetGlobalMatrixArray(eventArgs.depthViewProjectionMatricesPropertyId, m_DepthVPMatrices);

            if (m_OcclusionManager.TryGetEnvironmentDepthConfidenceTexture(out var depthConfidenceTexture))
                Shader.SetGlobalTexture(depthConfidenceTexture.propertyId, depthConfidenceTexture.texture);
        }

        static Matrix4x4 GetViewProjectionMatrix(XRFov fov, XRNearFarPlanes planes, Matrix4x4 trackingSpaceViewMatrix)
        {
            var near = planes.nearZ;
            var far = planes.farZ;
            var l = Mathf.Tan(fov.angleLeft) * near;
            var r = Mathf.Tan(fov.angleRight) * near;
            var t = Mathf.Tan(fov.angleUp) * near;
            var b = Mathf.Tan(fov.angleDown) * near;
            var projectionMatrix = Matrix4x4.Frustum(l, r, b, t, near, far);

            return projectionMatrix * trackingSpaceViewMatrix;
        }
    }
}
