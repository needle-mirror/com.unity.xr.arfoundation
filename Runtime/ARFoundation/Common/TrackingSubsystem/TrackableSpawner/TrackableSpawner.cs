using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Assertions;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    static class ListTrackableEntryExtensions
    {
        internal static bool TryFindEntryOfType(this List<TrackableEntry> list, Type type, out TrackableEntry entry)
        {
            foreach (var item in list)
            {
                if (item.trackableType == type)
                {
                    entry = item;
                    return true;
                }
            }

            entry = null;
            return false;
        }
    }

    class TrackableSpawner
    {
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
        internal readonly Pool.ObjectPool<List<TrackableEntry>> m_EntryListPool;
        internal readonly Pool.ObjectPool<TrackableEntry> m_EntryPool;
        internal readonly Dictionary<TrackableId, List<TrackableEntry>> m_EntriesByTrackableId = new();
        internal readonly Pool.ObjectPool<List<TrackableEntry>> m_OrphanPool;
        internal readonly Dictionary<TrackableId, List<TrackableEntry>> m_OrphanedEntriesByMissingParentIds = new();

        TrackableSpawner()
        {
            m_EntryListPool = new Pool.ObjectPool<List<TrackableEntry>>(
                createFunc: () => new(1),
                actionOnGet: null,
                actionOnRelease: list => list.Clear(),
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: 8,
                maxSize: 1024);

            m_OrphanPool = new Pool.ObjectPool<List<TrackableEntry>>(
                createFunc: () => new(),
                actionOnGet: null,
                actionOnRelease: list => list.Clear(),
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: 8,
                maxSize: 1024);

            m_EntryPool = new Pool.ObjectPool<TrackableEntry>(
                createFunc: () => new(null),
                actionOnGet: null,
                actionOnRelease: entry => entry.Reset(),
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: 8,
                maxSize: 1024);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void ResetInstance()
        {
            if (s_Instance != null)
            {
                s_Instance.m_EntryPool.Dispose();
                s_Instance.m_EntryListPool.Dispose();
                s_Instance.m_OrphanPool.Dispose();
            }
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
            Assert.AreNotEqual(sessionRelativeData.parentId, sessionRelativeData.trackableId,
                "Trackable can't be parented to itself");

            // If trackable is already created, update it and return.
            if (m_EntriesByTrackableId.TryGetValue(sessionRelativeData.trackableId, out var entryList)
                && entryList.TryFindEntryOfType(typeof(TTrackable), out var existingEntry))
            {
                var typedTrackable = existingEntry.trackable as TTrackable;
                SetSessionRelativeDataAndPose(typedTrackable, sessionRelativeData);
                return typedTrackable;
            }

            TTrackable trackable;
            var name = GenerateTrackableName(namePrefix, sessionRelativeData.trackableId);
            GameObject toSetActive;
            bool activeValue;

            if (prefab == null)
            {
                var go = new GameObject(name);
                go.SetActive(false);
                go.transform.parent = m_Origin.TrackablesParent;
                trackable = GetOrAddTrackableComponent<TTrackable, TSessionRelativeData>(go, sessionRelativeData);
                toSetActive = trackable.gameObject;
                activeValue = true;
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
                toSetActive = prefabInstance;
                activeValue = active;
            }

            var entry = RegisterCreatedTrackable(trackable);
            PopulateParent(entry);
            ResolveOrphans(entry);

            toSetActive.SetActive(activeValue);
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
        /// Creates an entry for a trackable. Does not populate information about parents or children.
        /// </summary>
        TrackableEntry RegisterCreatedTrackable(ARTrackable trackable)
        {
            Assert.IsNotNull(trackable);
            if (!m_EntriesByTrackableId.TryGetValue(trackable.trackableId, out var entryList))
            {
                entryList = m_EntryListPool.Get();
                m_EntriesByTrackableId.Add(trackable.trackableId, entryList);
            }

            Assert.IsFalse(entryList.TryFindEntryOfType(trackable.GetType(), out _));

            var entry = m_EntryPool.Get();
            entry.trackable = trackable;
            entryList.Add(entry);
            return entry;
        }

        void PopulateParent(TrackableEntry entry)
        {
            var parentId = entry.trackable.parentId;
            if (parentId == TrackableId.invalidId)
                return;

            var parentEntry = FindClosestEntryOrNull(new TrackableKey(parentId, entry.trackableType));
            if (parentEntry == null)
            {
                MarkAsOrphan(entry, entry.trackable.parentId);
                return;
            }

            var trackable = entry.trackable;
            var parentTrackable = parentEntry.trackable;
            trackable.transform.SetParent(parentTrackable.transform);
            entry.parentKey = new TrackableKey(parentTrackable.trackableId, parentEntry.trackableType);
            parentEntry.childKeys.Add(new TrackableKey(trackable.trackableId, entry.trackableType));
        }

        /// <summary>
        /// Given a key, return the closest match from the following:
        /// * An entry that exactly matches the key
        /// * An entry with the same trackableId but a different type
        /// * null
        /// </summary>
        TrackableEntry FindClosestEntryOrNull(TrackableKey key)
        {
            if (!m_EntriesByTrackableId.TryGetValue(key.trackableId, out var entryList))
                return null;

            var hasExactMatch = entryList.TryFindEntryOfType(key.trackableType, out var exactMatch);
            return hasExactMatch ? exactMatch : entryList[0];
        }

        void MarkAsOrphan(TrackableEntry entry, TrackableId missingParentId)
        {
            if (!m_OrphanedEntriesByMissingParentIds.ContainsKey(missingParentId))
                m_OrphanedEntriesByMissingParentIds.Add(missingParentId, m_OrphanPool.Get());

            m_OrphanedEntriesByMissingParentIds[missingParentId].Add(entry);
        }

        /// <summary>
        /// Given a trackable entry, if there are any other trackable entries waiting for this trackable to be
        /// created as their parent, apply all the relevant state to establish the parent-child relationship.
        ///
        /// This process prevents us from creating placeholder GameObjects and then immediately destroying them if
        /// `changes.added` lists children before parents.
        /// </summary>
        void ResolveOrphans(TrackableEntry parentEntry)
        {
            var parentId = parentEntry.trackable.trackableId;
            if (!m_OrphanedEntriesByMissingParentIds.TryGetValue(parentId, out var orphans))
                return;

            foreach (var orphanEntry in orphans)
            {
                orphanEntry.trackable.transform.SetParent(parentEntry.trackable.transform);
                parentEntry.childKeys.Add(orphanEntry.key);
                orphanEntry.parentKey = parentEntry.key;
            }

            m_OrphanPool.Release(orphans);
            m_OrphanedEntriesByMissingParentIds.Remove(parentId);
        }

        void CreatePlaceholderParentsForRemainingOrphans()
        {
            foreach (var (parentId, orphans) in m_OrphanedEntriesByMissingParentIds)
            {
                var parentGameObject = new GameObject($"Parent {parentId}");
                var parentTrackable = parentGameObject.AddComponent<ARNullTrackable>();
                parentTrackable.trackableIdInternal = parentId;
                parentGameObject.transform.SetParent(m_Origin.TrackablesParent);

                var parentEntryList = m_EntryListPool.Get();
                var parentEntry = m_EntryPool.Get();
                parentEntry.trackable = parentTrackable;
                parentEntryList.Add(parentEntry);
                m_EntriesByTrackableId.Add(parentId, parentEntryList);

                foreach (var orphanEntry in orphans)
                {
                    orphanEntry.trackable.transform.SetParent(parentGameObject.transform);
                    parentEntry.childKeys.Add(orphanEntry.key);
                    orphanEntry.parentKey = new TrackableKey(parentId, typeof(ARNullTrackable));
                }

                m_OrphanPool.Release(orphans);
            }

            m_OrphanedEntriesByMissingParentIds.Clear();
        }

        internal void OnTrackableDestroyed(TrackableKey key)
        {
            if (!m_EntriesByTrackableId.TryGetValue(key.trackableId, out var entryList)
                || !entryList.TryFindEntryOfType(key.trackableType, out var destroyedEntry))
                return;

            if (destroyedEntry.parentKey.HasValue)
            {
                var (parentId, parentType) = destroyedEntry.parentKey.Value;

                // Parent entry might not be present if parent was also destroyed this frame
                if (m_EntriesByTrackableId.TryGetValue(parentId, out var parentEntryList))
                {
                    var hasParentEntry = parentEntryList.TryFindEntryOfType(parentType, out var parentEntry);
                    Assert.IsTrue(hasParentEntry);
                    parentEntry.childKeys.Remove(key);
                }
            }

            // We assume that if a parent is destroyed, the children are destroyed as well.
            // So we don't worry about clearing any state in the children here, as they are also destroyed.

            entryList.Remove(destroyedEntry);
            m_EntryPool.Release(destroyedEntry);
            if (entryList.Count == 0)
            {
                m_EntriesByTrackableId.Remove(key.trackableId);
                m_EntryListPool.Release(entryList);
            }
        }

        internal void SetSessionRelativeDataAndPose<TTrackable, TSessionRelativeData>(
            TTrackable trackable, TSessionRelativeData data)
            where TSessionRelativeData : struct, ITrackable
            where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        {
            trackable.SetSessionRelativeData(data);

            var anyTrackableExists = m_EntriesByTrackableId.TryGetValue(trackable.trackableId, out var entryList);
            if (!anyTrackableExists || !entryList.TryFindEntryOfType(typeof(TTrackable), out var entry))
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

            foreach (var childKey in entry.childKeys)
            {
                var entryList = m_EntriesByTrackableId[childKey.trackableId];
                var hasChildEntry = entryList.TryFindEntryOfType(childKey.trackableType, out var childEntry);
                Assert.IsTrue(hasChildEntry);
                childEntry.trackable.transform.SetParent(m_Origin.TrackablesParent);
            }

            var worldspacePose = GetWorldspacePose(sessionPose);
            entry.trackable.transform.SetPositionAndRotation(worldspacePose.position, worldspacePose.rotation);

            foreach (var childKey in entry.childKeys)
            {
                var entryList = m_EntriesByTrackableId[childKey.trackableId];
                entryList.TryFindEntryOfType(childKey.trackableType, out var childEntry);
                childEntry.trackable.transform.SetParent(entry.trackable.transform);
            }
        }

        void OnTrackablesParentTransformChanged(ARTrackablesParentTransformChangedEventArgs eventArgs)
        {
            SetOrigin(eventArgs.Origin);

            foreach (var entryList in m_EntriesByTrackableId.Values)
            {
                foreach (var entry in entryList)
                {
                    if (entry.trackable.parentId == TrackableId.invalidId)
                        entry.trackable.transform.SetParent(eventArgs.TrackablesParent);

                    var pose = GetWorldspacePose(entry.trackable.pose);
                    entry.trackable.transform.SetPositionAndRotation(pose.position, pose.rotation);
                }
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

        internal bool TryGetTrackableByKey(TrackableKey key, out ARTrackable trackable)
        {
            trackable = null;
            if (!m_EntriesByTrackableId.TryGetValue(key.trackableId, out var entryList)
                || !entryList.TryFindEntryOfType(key.trackableType, out var entry))
                return false;

            trackable = entry.trackable;
            return true;
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
