using System;
using System.Linq;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Marks an object in a simulation environment as a source for a Bounding Box.
    /// This component is required by the <see cref="SimulationBoundingBoxSubsystem"/> on all GameObjects
    /// that represent bounding boxes in an environment.
    /// </summary>
    [DisallowMultipleComponent]
    public class SimulatedBoundingBox : MonoBehaviour
    {
        /// <summary>
        /// The size of the box, measured in the object's local space.
        /// </summary>
        public Vector3 size
        {
            get => m_Size;
            set => m_Size = value;
        }

        /// <summary>
        /// The center of the box, measured in the object's local space.
        /// </summary>
        public Vector3 center
        {
            get => m_Center;
            set => m_Center = value;
        }

        /// <summary>
        /// The classifications of the bounding box.
        /// </summary>
        public BoundingBoxClassifications classifications
        {
            get => m_Classifications;
            set => m_Classifications = value;
        }

        [SerializeField]
        Vector3 m_Size;

        [SerializeField]
        Vector3 m_Center;

        [SerializeField]
        BoundingBoxClassifications m_Classifications;

        Transform m_ThisTransform;

        /// <summary>
        /// The `TrackableId` for the bounding box.
        /// </summary>
        public TrackableId trackableId { get; private set; } = TrackableId.invalidId;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var matrix = GetPoseMatrix();

            var previousHandleMatrix = Handles.matrix;
            Handles.matrix = matrix;
            Handles.DrawWireCube(Vector3.zero, m_Size);
            Handles.matrix = previousHandleMatrix;
        }
#endif

        void Awake()
        {
            trackableId = GenerateTrackableID();
        }

        TrackableId GenerateTrackableID()
        {
            var unsignedInstanceId = (ulong)Convert.ToInt64(gameObject.GetInstanceID());
            return new TrackableId(unsignedInstanceId, 0);
        }

        void Reset()
        {
            // Calibrate to colliders first if any, then meshes, otherwise set to default values
            m_ThisTransform = transform;
            var rotStored = m_ThisTransform.rotation;

            // Bounds are better calculated when object is axis-aligned
            m_ThisTransform.rotation = Quaternion.identity;

            var colliders = GetComponents<Collider>().Where(x => x.enabled).ToArray();
            if (colliders.Length > 0)
            {
                ContainBounds(colliders.Select(x => x.bounds).ToArray());
                m_ThisTransform.rotation = rotStored;
                return;
            }

            var renderers = GetComponents<Renderer>().Where(x => x.enabled).ToArray();
            if (renderers.Length > 0)
            {
                ContainBounds((renderers.Select(x => x.bounds).ToArray()));
                m_ThisTransform.rotation = rotStored;
                return;
            }

            m_Size = Vector3.one;
            m_Center = Vector3.zero;
            m_ThisTransform.rotation = rotStored;
        }

        void ContainBounds(Bounds[] boundsToEncapsulate)
        {
            if (boundsToEncapsulate.Length > 0)
            {
                var bounds = new Bounds(
                    boundsToEncapsulate[0].center,
                    boundsToEncapsulate[0].size);
                foreach (var bound in boundsToEncapsulate)
                {
                    bounds.Encapsulate(bound);
                }

                var localScale = m_ThisTransform.localScale;
                var inverseScale = new Vector3(
                    1 / (localScale.x == 0 ? 1 : localScale.x),
                    1 / (localScale.y == 0 ? 1 : localScale.y),
                    1 / (localScale.z == 0 ? 1 : localScale.z));

                m_Size = Vector3.Scale(bounds.size, inverseScale);
                m_Center = Vector3.Scale(bounds.center - m_ThisTransform.position, inverseScale);
            }
        }

        /// <summary>
        /// Gets the world-space pose of the bounding box center.
        /// </summary>
        /// <returns>The pose.</returns>
        public Pose GetWorldSpaceCenterPose()
        {
            var matrix = GetPoseMatrix();
            var pos = matrix.GetPosition();
            var rot = matrix.rotation;
            return new Pose(pos, rot);
        }

        /// <summary>
        /// Gets the world-space size of the bounding box.
        /// </summary>
        /// <returns>The size.</returns>
        public Vector3 GetWorldSpaceSize()
        {
            return Vector3.Scale(m_Size, transform.lossyScale);
        }

        Matrix4x4 GetPoseMatrix()
        {
            if (m_ThisTransform == null)
                m_ThisTransform = transform;

            var matrix = m_ThisTransform.localToWorldMatrix;

            // Add position of center to matrix
            var column = matrix.GetColumn(3);
            column += (Vector4)(transform.TransformPoint(m_Center) - m_ThisTransform.position);
            matrix.SetColumn(3, column);

            return matrix;
        }

        void OnEnable()
        {
            if (SimulationUtils.IsInSimulationEnvironment(gameObject))
            {
                SimulationSessionSubsystem.simulationSceneManager.TrackBoundingBox(this);
            }
        }

        void OnDisable()
        {
            if (SimulationUtils.IsInSimulationEnvironment(gameObject))
            {
                SimulationSessionSubsystem.simulationSceneManager.UntrackBoundingBox(this);
            }
        }
    }
}
