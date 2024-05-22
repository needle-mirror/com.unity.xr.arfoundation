using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers) that detects and tracks
    /// 3D bounding boxes in the physical environment. Add this component to your <see cref="XROrigin"/> GameObject to enable 3D bounding box
    /// detection in your app.
    /// </summary>
    [DefaultExecutionOrder(ARUpdateOrder.k_BoundingBoxManager)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XROrigin))]
    [HelpURL(typeof(ARBoundingBoxManager))]
    public class ARBoundingBoxManager : ARTrackableManager<
        XRBoundingBoxSubsystem,
        XRBoundingBoxSubsystemDescriptor,
        XRBoundingBoxSubsystem.Provider,
        XRBoundingBox,
        ARBoundingBox>, IRaycaster
    {
        [SerializeField]
        [Tooltip(
            "If not null, this prefab is instantiated for each detected 3D bounding box. " +
            "If the prefab does not contain an ARBoundingBox component, ARBoundingBoxManager will add one.")]
        GameObject m_BoundingBoxPrefab;

        /// <summary>
        /// Get or set the prefab to instantiate for each detected 3D bounding box. Can be <see langword="null"/>.
        /// </summary>
        public GameObject boundingBoxPrefab
        {
            get => m_BoundingBoxPrefab;
            set => m_BoundingBoxPrefab = value;
        }

        /// <summary>
        /// The name to be used for the instantiated GameObject whenever a new 3D bounding box is detected.
        /// </summary>
        protected override string gameObjectName => "ARBoundingBox";

        BoundingBoxRaycaster m_BoundingBoxRaycaster;

        /// <summary>
        /// Saves a reference to the XR Origin component and initializes the `BoundingBoxRaycaster`.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            m_BoundingBoxRaycaster = new(trackables);
        }

        /// <summary>
        /// Attempt to retrieve an existing <see cref="ARBoundingBox"/> by <paramref name="trackableId"/>.
        /// </summary>
        /// <param name="trackableId">The `TrackableId` of the bounding box to retrieve.</param>
        /// <param name="arBoundingBox">The output bounding box, if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if a bounding box exists with the given <paramref name="trackableId"/>. Otherwise, <see langword="false"/>.</returns>
        public bool TryGetBoundingBox(TrackableId trackableId, out ARBoundingBox arBoundingBox)
        {
            return m_Trackables.TryGetValue(trackableId, out arBoundingBox);
        }

        /// <summary>
        /// Performs a raycast against all currently tracked 3D bounding boxes.
        /// </summary>
        /// <param name="ray">The ray, in Unity world space, to cast.</param>
        /// <param name="trackableTypeMask">A mask of raycast types to perform.</param>
        /// <param name="allocator">The <c>Allocator</c> to use when creating the returned <c>NativeArray</c>.</param>
        /// <returns>
        /// A new <c>NativeArray</c> of raycast results allocated with <paramref name="allocator"/>.
        /// The caller owns the memory and is responsible for calling <c>Dispose</c> on the <c>NativeArray</c>.
        /// </returns>
        public NativeArray<XRRaycastHit> Raycast(
            Ray ray,
            TrackableType trackableTypeMask,
            Allocator allocator)
        {
            return m_BoundingBoxRaycaster.Raycast(ray, trackableTypeMask, allocator);
        }

        /// <summary>
        /// Get the prefab to instantiate for each detected 3D bounding box. Can be <see langword="null"/>.
        /// </summary>
        /// <returns>The Prefab which will be instantiated for each <see cref="ARBoundingBox"/>.</returns>
        protected override GameObject GetPrefab() => m_BoundingBoxPrefab;

        /// <summary>
        /// Invoked when Unity enables this `MonoBehaviour`. Used to register with the <see cref="ARRaycastManager"/>.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (subsystem != null)
            {
                var raycastManager = GetComponent<ARRaycastManager>();
                if (raycastManager != null)
                    raycastManager.RegisterRaycaster(this);
            }
        }

        /// <summary>
        /// Invoked when this component is disabled. Used to unregister with the <see cref="ARRaycastManager"/>.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            var raycastManager = GetComponent<ARRaycastManager>();
            if (raycastManager != null)
                raycastManager.UnregisterRaycaster(this);
        }
    }
}
