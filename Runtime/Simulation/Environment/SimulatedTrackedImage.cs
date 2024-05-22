using System;
using Unity.XR.CoreUtils;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation.InternalUtils;
using GuidUtil = UnityEngine.XR.ARSubsystems.GuidUtil;
using SerializableGuid = UnityEngine.XR.ARSubsystems.SerializableGuid;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Marks an object in a simulation environment as a source from which to provide a tracked image.
    /// This component is required by the <see cref="SimulationImageTrackingSubsystem"/> on all GameObjects
    /// which represent tracked images in an environment.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [DisallowMultipleComponent]
    public class SimulatedTrackedImage : MonoBehaviour
    {
        const float k_MinSideLengthMeters = .01f;
        const string k_QuadShader = "Unlit/Texture";

        [SerializeField, Tooltip("The image to track.")]
        Texture2D m_Image;

        [SerializeField, Tooltip("The world-space size of the image, in meters.")]
        Vector2 m_ImagePhysicalSizeMeters = new(1f, 1f);

        [SerializeField, HideInInspector]
        Mesh m_QuadMesh;

        [SerializeField, HideInInspector]
        Material m_QuadMaterial;

        [SerializeField, HideInInspector]
        MeshFilter m_QuadMeshFilter;

        [SerializeField, HideInInspector]
        MeshRenderer m_QuadMeshRenderer;

        [ReadOnly, ShowInDebugInspectorOnly]
        SerializableGuid m_SerializableImageAssetGuid;

        Material s_QuadBaseMaterial;

        /// <summary>
        /// The tracked image's texture. If the user does not provide a texture at edit time, a new texture will
        /// be generated at runtime.
        /// </summary>
        public Texture2D texture
        {
            get
            {
                if (!Application.isPlaying)
                    return m_Image;

                if (m_Image == null)
                    m_Image = new Texture2D(0, 0);

                return m_Image;
            }
        }

        /// <summary>
        /// The world-space width and height of the tracked image.
        /// </summary>
        public Vector2 size => m_ImagePhysicalSizeMeters;

        /// <summary>
        /// The <see cref="TrackableId"/> for the tracked image.
        /// </summary>
        public TrackableId trackableId { get; private set; } = TrackableId.invalidId;

        /// <summary>
        /// The unique 128-bit ID associated with the asset file of the above texture.
        /// </summary>
        public Guid imageAssetGuid => m_SerializableImageAssetGuid.guid;

        /// <summary>
        /// A unique 128-bit ID associated with the content of the tracked image.
        /// </summary>
        /// <remarks>
        /// This method should only be used as a fallback strategy to generate a GUID,
        /// in the event that the <see cref="SimulationImageTrackingSubsystem"/>'s
        /// runtime reference image library does not contain a reference image matching our <c>image</c>.
        /// </remarks>
        public Guid fallbackSourceImageId { get; private set; } = Guid.Empty;

        TrackableId GenerateTrackableID()
        {
            var unsignedInstanceId = (ulong)Math.Abs(Convert.ToInt64(gameObject.GetInstanceID()));
            return new TrackableId(unsignedInstanceId, 0);
        }

        Guid GenerateSourceImageId()
        {
            var unsignedInstanceId = (ulong)Math.Abs(Convert.ToInt64(texture.GetInstanceID()));
            return GuidUtil.Compose(unsignedInstanceId, 0);
        }

        void Awake()
        {
            if (!Application.isPlaying)
                return;

            fallbackSourceImageId = GenerateSourceImageId();
            trackableId = GenerateTrackableID();
        }

        /// <summary>
        /// Prevent users from entering an invalid value for the image's physical size.
        /// </summary>
        void OnValidate()
        {
            if (m_ImagePhysicalSizeMeters.x <= k_MinSideLengthMeters || m_ImagePhysicalSizeMeters.y <= k_MinSideLengthMeters)
            {
                m_ImagePhysicalSizeMeters = new Vector2(
                    m_ImagePhysicalSizeMeters.x < k_MinSideLengthMeters ? k_MinSideLengthMeters : m_ImagePhysicalSizeMeters.x,
                    m_ImagePhysicalSizeMeters.y < k_MinSideLengthMeters ? k_MinSideLengthMeters : m_ImagePhysicalSizeMeters.y);
            }

            UpdateQuadMesh();

#if UNITY_EDITOR
            SetSerializedGuid();
#endif
        }

        /// <remarks>
        /// Because the Quad Mesh and Quad Material are not saved to disk as their own asset files, their state cannot
        /// be saved as part of a prefab.
        ///
        /// Therefore whenever we enter or exit Prefab Mode in the Editor, we must regenerate the Mesh.
        /// ExecuteInEditMode OnEnable is a suitable trigger for loading a scene or entering/exiting Prefab Mode.
        /// </remarks>
        void OnEnable()
        {
            if (m_QuadMesh == null)
                CreateQuadMesh();

#if UNITY_EDITOR
            if (m_SerializableImageAssetGuid.guid == Guid.Empty)
                SetSerializedGuid();
#endif
        }

        void CreateQuadMesh()
        {
            m_QuadMeshFilter = GetComponent<MeshFilter>();
            m_QuadMeshRenderer = GetComponent<MeshRenderer>();

            var x = m_ImagePhysicalSizeMeters.x / 2;
            var y = m_ImagePhysicalSizeMeters.y / 2;

            m_QuadMesh = new Mesh
            {
                name = "Tracked Image Mesh Visualizer",
                vertices = new Vector3[]
                {
                    new(-x, 0f, -y),
                    new(x, 0f, -y),
                    new(x, 0f, y),
                    new(-x, 0f, y)
                },
                triangles = new[] { 3, 1, 0, 3, 2, 1 },
                normals = new[] { -Vector3.up, -Vector3.up, -Vector3.up, -Vector3.up },
                uv = new Vector2[] { new(0f, 0f), new(1f, 0f), new(1f, 1f), new (0f, 1f) }
            };

            m_QuadMesh.UploadMeshData(false);
            m_QuadMeshFilter.mesh = m_QuadMesh;

            // we lazy create the base material to use for the tracking image's material
            // as needed. if play-mode is re-run without a domain reload, the base material
            // will get nullified and recreated here.
            if (s_QuadBaseMaterial == null)
                s_QuadBaseMaterial = new Material(Shader.Find(k_QuadShader));

            m_QuadMaterial = new Material(s_QuadBaseMaterial)
            {
                name = name,
                hideFlags = HideFlags.NotEditable,
                mainTexture = m_Image,
            };

            m_QuadMeshRenderer.sharedMaterial = m_QuadMaterial;
        }

        void UpdateQuadMesh()
        {
            if (m_QuadMesh == null || m_QuadMaterial == null)
                return;

            var x = m_ImagePhysicalSizeMeters.x / 2;
            var y = m_ImagePhysicalSizeMeters.y / 2;

            m_QuadMesh.vertices = new Vector3[]
            {
                new(-x, 0f, -y),
                new(x, 0f, -y),
                new(x, 0f, y),
                new(-x, 0f, y)
            };

            m_QuadMesh.UploadMeshData(false);
            m_QuadMaterial.mainTexture = m_Image;

            if (m_Image != null)
                m_QuadMaterial.name = m_Image.name;
        }

        void SetSerializedGuid()
        {
#if UNITY_EDITOR
            if (m_Image == null)
                return;

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_Image.GetInstanceID(), out string assetGuidString, out var _);
            var guid = new Guid(assetGuidString);
            guid.Decompose(out var low, out var high);
            m_SerializableImageAssetGuid = new SerializableGuid(low, high);
#endif
        }
    }
}
