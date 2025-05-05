using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A utility class to spawn and manage trackables.
    /// </summary>
    class TrackableSpawner
    {
        TrackableSpawner() { }

        /// <summary>
        /// Singleton instance of the TrackableSpawner.
        /// </summary>
        internal static TrackableSpawner instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new TrackableSpawner();

                return m_Instance;
            }
        }

        static TrackableSpawner m_Instance;

        /// <summary>
        /// A Transform that spawned Trackables will default parent to.
        /// </summary>
        Transform m_TrackablesParent;

        /// <summary>
        /// A dictionary of parentIds to their respective transforms parent.
        /// </summary>
        Dictionary<TrackableId, Transform> m_ParentTransformByTrackableId = new();

        /// <summary>
        /// Sets the Transform that spawned Trackables will default parent to.
        /// </summary>
        /// <param name="trackablesParent">The Transform that spawned Trackables will default parent to.</param>
        public void SetTrackablesParent(Transform trackablesParent)
        {
            m_TrackablesParent = trackablesParent;

            ValidateParentTransforms();

            // In case of an OnTrackablesParentTransformChanged event from ARTrackableManager,
            // we need to reparent existing parent objects to the new parent
            foreach (var kvp in m_ParentTransformByTrackableId)
                kvp.Value.parent = m_TrackablesParent;
        }

        (GameObject gameObject, bool shouldBeActive) CreateGameObjectDeactivated(GameObject prefab)
        {
            if (prefab == null)
            {
                var newGameObject = new GameObject();
                newGameObject.SetActive(false);
                newGameObject.transform.parent = m_TrackablesParent;
                return (newGameObject, true);
            }

            var active = prefab.activeSelf;
            prefab.SetActive(false);
            var prefabInstance = Object.Instantiate(prefab, m_TrackablesParent);
            prefab.SetActive(active);
            return (prefabInstance, active);
        }

        (GameObject gameObject, bool shouldBeActive) CreateGameObjectDeactivated(GameObject prefab, string name)
        {
            var tuple = CreateGameObjectDeactivated(prefab);
            tuple.gameObject.name = name;
            return tuple;
        }

        /// <summary>
        /// Creates a new trackable GameObject based on the session relative data, prefab, and name.
        /// </summary>
        /// <param name="sessionRelativeData">The session relative data of the trackable.</param>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="name">The name the spawned trackable should have.</param>
        /// <typeparam name="TTrackable">The trackable type.</typeparam>
        /// <typeparam name="TSessionRelativeData">The session relative data type of the trackable.</typeparam>
        /// <returns>A tuple containing the spawned GameObject and a boolean indicating if it should be active.</returns>
        public (GameObject gameObject, bool shouldBeActive) CreateTrackable<TTrackable, TSessionRelativeData>(TSessionRelativeData sessionRelativeData, GameObject prefab, string name)
            where TSessionRelativeData : struct, ITrackable
            where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        {
            var (trackableGameObject, shouldBeActive) = CreateGameObjectDeactivated(prefab, name);

            if (!trackableGameObject.GetComponent<TTrackable>())
                trackableGameObject.AddComponent<TTrackable>();

            if (TryGetParentId(sessionRelativeData, out var parentId))
                trackableGameObject.transform.parent = GetOrCreateParentTransform(parentId);
            else
                trackableGameObject.transform.parent = m_TrackablesParent;

            return (trackableGameObject, shouldBeActive);
        }

        /// <summary>
        /// Called after a trackable is removed.
        /// This method will destroy the parent of the trackable if it has no children.
        /// </summary>
        /// <param name="trackable">The trackable that was removed.</param>
        /// <typeparam name="TTrackable">The trackable type.</typeparam>
        /// <typeparam name="TSessionRelativeData">The session relative data type of the trackable.</typeparam>
        public void AfterTrackableRemoved<TTrackable, TSessionRelativeData>(TTrackable trackable)
            where TSessionRelativeData : struct, ITrackable
            where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        {
            // Clean up the parent transform if it has no children remaining.
            if (TryGetParentId(trackable.sessionRelativeData, out var parentId)
                && m_ParentTransformByTrackableId.TryGetValue(parentId, out Transform parentTransform)
                && parentTransform.childCount == 0)
            {
                Object.Destroy(parentTransform.gameObject);
                m_ParentTransformByTrackableId.Remove(parentId);
            }
        }

        Transform CreateNewParentTransform(TrackableId parentId)
        {
            var parent = new GameObject($"Parent {parentId}").transform;
            parent.parent = m_TrackablesParent;

            if (m_ParentTransformByTrackableId.ContainsKey(parentId))
                m_ParentTransformByTrackableId[parentId] = parent;
            else
                m_ParentTransformByTrackableId.Add(parentId, parent.transform);

            return parent.transform;
        }

        Transform GetOrCreateParentTransform(TrackableId parentId)
        {
            if (m_ParentTransformByTrackableId.TryGetValue(parentId, out var parentTransform))
                return parentTransform;

            return CreateNewParentTransform(parentId);
        }

        void ValidateParentTransforms()
        {
            foreach (var parentId in m_ParentTransformByTrackableId.Keys.ToArray())
                if (m_ParentTransformByTrackableId[parentId] == null)
                    CreateNewParentTransform(parentId);
        }

        /// <summary>
        /// Gets the parent ID of the trackable (if it has one).
        /// </summary>
        /// <param name="sessionRelativeData">The session relative data of the trackable.</param>
        /// <param name="parentId">The parent ID of the trackable (if it has one).</param>
        /// <typeparam name="TSessionRelativeData">The session relative data type of the trackable.</typeparam>
        /// <returns><see langword="true"/> if the trackable has a parent ID, otherwise <see langword="false"/>.</returns>
        static bool TryGetParentId<TSessionRelativeData>(TSessionRelativeData sessionRelativeData, out TrackableId parentId)
            where TSessionRelativeData : struct, ITrackable
        {
            parentId = sessionRelativeData.parentId;
            return !parentId.Equals(TrackableId.invalidId);
        }
    }
}
