using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Assertions;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    class TrackableSpawner
    {
        internal class TrackableEntry : IReleasable
        {
            internal ARTrackable trackable { get; set; }
            internal HashSet<TrackableId> childIds { get; } = new();

            [SuppressMessage("ReSharper", "ParameterHidesMember")]
            internal void Initialize(ARTrackable trackable)
            {
                this.trackable = trackable;
            }

            void IReleasable.Release()
            {
                trackable = null;
                childIds.Clear();
            }
        }

        internal static TrackableSpawner instance
        {
            get
            {
                s_Instance ??= new TrackableSpawner();
                return s_Instance;
            }
        }
        static TrackableSpawner s_Instance;

        XROrigin m_Origin;

        // Internal access granted for automated testing validation only.
        // DON'T modify internal state from other classes; use the internal methods instead.
        internal readonly Pool.ObjectPool<TrackableEntry> m_EntryPool;
        internal readonly Dictionary<TrackableId, TrackableEntry> m_EntriesByTrackableId = new();
        internal readonly Pool.ObjectPool<List<TrackableEntry>> m_OrphanPool;
        internal readonly Dictionary<TrackableId, List<TrackableEntry>> m_OrphanedEntriesByMissingParentIds = new();

        TrackableSpawner()
        {
            m_EntryPool = ObjectPoolCreateUtil.CreateWithReleaseTrigger<TrackableEntry>();
            m_OrphanPool = ObjectPoolCreateUtil.Create<List<TrackableEntry>>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void ResetInstance()
        {
            s_Instance = null;
        }

        internal void CreateOrUpdateTrackables<TTrackable, TSessionRelativeData>(
            NativeArray<TSessionRelativeData> sessionRelativeDataArray, GameObject prefab, string namePrefix)
            where TSessionRelativeData : struct, ITrackable
            where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        {
            foreach (var sessionRelativeData in sessionRelativeDataArray)
            {
                CreateOrUpdateTrackable<TTrackable, TSessionRelativeData>(sessionRelativeData, prefab, namePrefix);
            }

            CreatePlaceholderParentsForRemainingOrphans();
        }

        internal TTrackable CreateOrUpdateTrackable<TTrackable, TSessionRelativeData>(
            TSessionRelativeData sessionRelativeData, GameObject prefab, string namePrefix)
            where TSessionRelativeData : struct, ITrackable
            where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        {
            // If trackable is already created, update it and return.
            if (m_EntriesByTrackableId.TryGetValue(sessionRelativeData.trackableId, out var preexistingEntry)
                && preexistingEntry.trackable.GetType() == typeof(TTrackable))
            {
                var typedTrackable = preexistingEntry.trackable as TTrackable;
                SetSessionRelativeDataAndPose(typedTrackable, sessionRelativeData);
                return typedTrackable;
            }

            TTrackable trackable;
            var name = GenerateTrackableName(namePrefix, sessionRelativeData.trackableId);

            if (prefab == null)
            {
                var go = new GameObject(name);
                go.SetActive(false);
                go.transform.parent = m_Origin.TrackablesParent;
                trackable = GetOrAddTrackableComponent<TTrackable, TSessionRelativeData>(go, sessionRelativeData);
                trackable.gameObject.SetActive(true);
            }
            else
            {
                var active = prefab.activeSelf;
                prefab.SetActive(false);
                var prefabInstance = Object.Instantiate(prefab, m_Origin.TrackablesParent);
                prefabInstance.name = name;
                prefab.SetActive(active);
                trackable = GetOrAddTrackableComponent<TTrackable, TSessionRelativeData>(
                    prefabInstance, sessionRelativeData);
                prefabInstance.SetActive(active);
            }

            var entry = RegisterCreatedTrackable(trackable);
            ResolveOrphans(entry);
            trackable.transform.parent = GetOrCreateParentTransform(entry);
            return trackable;
        }

        TTrackable GetOrAddTrackableComponent<TTrackable, TSessionRelativeData>(
            GameObject go, TSessionRelativeData sessionRelativeData)
            where TSessionRelativeData : struct, ITrackable
            where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        {
            var trackable = go.GetComponent<TTrackable>();
            if (trackable == null)
                trackable = go.AddComponent<TTrackable>();

            SetSessionRelativeDataAndPose(trackable, sessionRelativeData);
            return trackable;
        }

        /// <summary>
        /// Creates an entry for a trackable.
        /// </summary>
        TrackableEntry RegisterCreatedTrackable(ARTrackable trackable)
        {
            Assert.IsNotNull(trackable);
            var transform = trackable.transform;
            var trackableId = trackable.trackableId;

            if (!m_EntriesByTrackableId.TryGetValue(trackableId, out var placeholderEntry))
            {
                var entry = m_EntryPool.Get();
                entry.Initialize(trackable);
                m_EntriesByTrackableId.Add(trackableId, entry);
                return entry;
            }

            // The only way we reach this path given valid data is if a trackable with the same trackable ID as a
            // parent that didn't exist on a previous frame now exists.
            // This should never happen in practice, but a malicious runtime could theoretically be implemented this way.

            Assert.AreEqual(typeof(ARNullTrackable), placeholderEntry.trackable.GetType());
            var placeholderGameObject = placeholderEntry.trackable.gameObject;
            placeholderEntry.Initialize(trackable);

            foreach (var childId in placeholderEntry.childIds)
            {
                Assert.IsTrue(m_EntriesByTrackableId.ContainsKey(childId));
                m_EntriesByTrackableId[childId].trackable.transform.SetParent(transform);
            }

            UnityObjectUtils.Destroy(placeholderGameObject);
            return placeholderEntry;
        }

        /// <summary>
        /// Given a trackable entry, if there are any other trackable entries waiting for this trackable to be
        /// created as their parent, apply all the relevant state to establish the parent-child relationship.
        ///
        /// This process prevents us from creating placeholder GameObjects and then immediately destroying them if
        /// `changes.added` lists children before parents.
        /// </summary>
        void ResolveOrphans(TrackableEntry entry)
        {
            if (!m_OrphanedEntriesByMissingParentIds.TryGetValue(entry.trackable.trackableId, out var orphans))
                return;

            foreach (var orphanEntry in orphans)
            {
                orphanEntry.trackable.transform.SetParent(entry.trackable.transform);
                entry.childIds.Add(orphanEntry.trackable.trackableId);
            }

            orphans.Clear();
            m_OrphanPool.Release(orphans);
            m_OrphanedEntriesByMissingParentIds.Remove(entry.trackable.trackableId);
        }

        Transform GetOrCreateParentTransform(TrackableEntry entry)
        {
            Assert.IsNotNull(entry);
            Assert.IsNotNull(entry.trackable);
            var transform = entry.trackable.transform;
            var trackableId = entry.trackable.trackableId;
            var parentId = entry.trackable.parentId;

            if (m_EntriesByTrackableId.TryGetValue(parentId, out var parentEntry))
            {
                var parentTransform = parentEntry.trackable.transform;
                transform.SetParent(parentTransform);
                parentEntry.childIds.Add(trackableId);

                m_EntriesByTrackableId[trackableId] = entry;
                m_EntriesByTrackableId[parentId] = parentEntry;

                return parentTransform;
            }

            // Parent does not exist yet, but might be created later this frame.
            // For now, this trackable is an orphan.
            if (parentId != TrackableId.invalidId)
            {
                if (!m_OrphanedEntriesByMissingParentIds.ContainsKey(parentId))
                    m_OrphanedEntriesByMissingParentIds.Add(parentId, m_OrphanPool.Get());

                m_OrphanedEntriesByMissingParentIds[parentId].Add(entry);
            }

            return m_Origin.TrackablesParent;
        }

        void CreatePlaceholderParentsForRemainingOrphans()
        {
            foreach (var (parentId, orphans) in m_OrphanedEntriesByMissingParentIds)
            {
                var parentGameObject = new GameObject($"Parent {parentId}");
                var parentTrackable = parentGameObject.AddComponent<ARNullTrackable>();
                parentTrackable.trackableIdInternal = parentId;
                parentGameObject.transform.SetParent(m_Origin.TrackablesParent);

                var parentEntry = m_EntryPool.Get();
                parentEntry.Initialize(parentTrackable);
                m_EntriesByTrackableId.Add(parentId, parentEntry);

                foreach (var orphan in orphans)
                {
                    orphan.trackable.transform.SetParent(parentGameObject.transform);
                    parentEntry.childIds.Add(orphan.trackable.trackableId);
                }

                orphans.Clear();
                m_OrphanPool.Release(orphans);
            }

            m_OrphanedEntriesByMissingParentIds.Clear();
        }

        internal void OnTrackableDestroyed(TrackableId id)
        {
            if (!m_EntriesByTrackableId.Remove(id, out var destroyedEntry))
                return;

            if (m_EntriesByTrackableId.TryGetValue(destroyedEntry.trackable.parentId, out var parentEntry))
                parentEntry.childIds.Remove(id);

            m_EntryPool.Release(destroyedEntry);
        }

        internal void SetSessionRelativeDataAndPose<TTrackable, TSessionRelativeData>(
            TTrackable trackable, TSessionRelativeData data)
            where TSessionRelativeData : struct, ITrackable
            where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        {
            trackable.SetSessionRelativeData(data);

            var trackableIsRegistered = m_EntriesByTrackableId.TryGetValue(data.trackableId, out var entry);
            if (!trackableIsRegistered)
            {
                var worldspacePose = GetWorldspacePose(data.pose);
                trackable.transform.SetPositionAndRotation(worldspacePose.position, worldspacePose.rotation);
                return;
            }

            SetPose(entry, data.pose);
        }

        void SetPose(TrackableEntry entry, Pose sessionPose)
        {
            // Each trackable created by AR Foundation has a pose relative to the XR Origin.
            // Therefore if a parent moves, to avoid moving the children relative to the parent, we must:
            // 1) Unparent the children, 2) Move the trackable, then 3) Reparent the children
            foreach (var childId in entry.childIds)
            {
                if (m_EntriesByTrackableId.TryGetValue(childId, out var childData))
                    childData.trackable.transform.SetParent(m_Origin.TrackablesParent);
            }

            var worldspacePose = GetWorldspacePose(sessionPose);
            entry.trackable.transform.SetPositionAndRotation(worldspacePose.position, worldspacePose.rotation);

            foreach (var childId in entry.childIds)
            {
                if (m_EntriesByTrackableId.TryGetValue(childId, out var childData))
                    childData.trackable.transform.SetParent(entry.trackable.transform);
            }
        }

        void OnTrackablesParentTransformChanged(ARTrackablesParentTransformChangedEventArgs eventArgs)
        {
            SetOrigin(eventArgs.Origin);

            foreach (var entry in m_EntriesByTrackableId.Values)
            {
                if (entry.trackable.parentId == TrackableId.invalidId)
                    entry.trackable.transform.SetParent(eventArgs.TrackablesParent);

                var pose = GetWorldspacePose(entry.trackable.pose);
                entry.trackable.transform.SetPositionAndRotation(pose.position, pose.rotation);
            }
        }

        internal void SetOrigin(XROrigin origin)
        {
            Assert.IsNotNull(origin);
            if (m_Origin == origin)
                return;

            if (m_Origin != null)
                m_Origin.TrackablesParentTransformChanged -= OnTrackablesParentTransformChanged;

            m_Origin = origin;
            m_Origin.TrackablesParentTransformChanged += OnTrackablesParentTransformChanged;
        }

        internal ARTrackable GetTrackableById(TrackableId trackableId)
        {
            return m_EntriesByTrackableId[trackableId]?.trackable;
        }

        Pose GetWorldspacePose(Pose sessionRelativePose)
        {
            return m_Origin.TrackablesParent.TransformPose(sessionRelativePose);
        }

        static string GenerateTrackableName(string prefix, TrackableId trackableId)
        {
            return $"{prefix} {trackableId.ToString()}";
        }
    }
}
