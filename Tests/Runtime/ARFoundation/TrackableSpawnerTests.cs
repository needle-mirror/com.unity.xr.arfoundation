using System.Collections;
using NUnit.Framework;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class TrackableSpawnerTests
    {
        XROrigin m_Origin;
        TrackableSpawner m_Spawner;
        BoundedPlaneBuilder m_PlaneBuilder = new();
        XRBoundingBoxBuilder m_BoxBuilder = new();

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject("XR Origin");
            m_Origin = go.AddComponent<XROrigin>();
            TrackableSpawner.instance.SetOrigin(m_Origin);
            m_Spawner = TrackableSpawner.instance;
        }

        [TearDown]
        public void TearDown()
        {
            UnityObjectUtils.Destroy(m_Origin.gameObject);
            TrackableSpawner.ResetInstance();
            m_PlaneBuilder.Reset();
            m_BoxBuilder.Reset();
        }

        [Test]
        public void CreateOrUpdateTrackables_ParentsToTrackablesRootIfNoParentId()
        {
            var id0 = new TrackableId(123, 0);
            var plane0 = m_PlaneBuilder.WithTrackableId(id0).Build();
            var array = new NativeArray<BoundedPlane>(1, Allocator.Temp);
            array[0] = plane0;

            m_Spawner.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");

            var allEntries = m_Spawner.m_EntriesByTrackableId;
            Assert.AreEqual(1, allEntries.Count);
            Assert.IsTrue(allEntries.ContainsKey(id0));

            var entry = allEntries[id0][0];
            Assert.AreEqual(0, entry.childKeys.Count);
            Assert.IsNotNull(entry.trackable);

            var trackable = entry.trackable;
            Assert.AreEqual(typeof(ARPlane), trackable.GetType());
            Assert.AreEqual(id0, trackable.trackableId);
            Assert.AreEqual(m_Origin.TrackablesParent, trackable.transform.parent);
        }

        [Test]
        public void CreateOrUpdateTrackables_CreatesParentIfOrphaned()
        {
            var id0 = new TrackableId(123, 0);
            var missingParentId = new TrackableId(0, 456);
            var plane0 = m_PlaneBuilder.WithTrackableId(id0).WithParentId(missingParentId).Build();
            var array = new NativeArray<BoundedPlane>(1, Allocator.Temp);
            array[0] = plane0;

            m_Spawner.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");

            var allEntries = m_Spawner.m_EntriesByTrackableId;
            Assert.AreEqual(2, allEntries.Count);
            Assert.IsTrue(allEntries.ContainsKey(id0));
            Assert.IsTrue(allEntries.ContainsKey(missingParentId));

            var parentEntry = allEntries[missingParentId][0];
            Assert.AreEqual(1, parentEntry.childKeys.Count);
            Assert.IsNotNull(parentEntry.trackable);

            var parentTrackable = parentEntry.trackable;
            Assert.AreEqual(typeof(ARNullTrackable), parentTrackable.GetType());
            Assert.AreEqual(missingParentId, parentTrackable.trackableId);
            Assert.AreEqual(m_Origin.TrackablesParent, parentTrackable.transform.parent);

            Assert.AreEqual(parentTrackable.transform, allEntries[id0][0].trackable.transform.parent);

            // Validate that object pools are correctly being released
            Assert.AreEqual(0, m_Spawner.m_OrphanedEntriesByMissingParentIds.Count);
            Assert.AreEqual(0, m_Spawner.m_OrphanPool.CountActive);
        }

        (TrackableId key0, TrackableId key1, TrackableId key2) SetupChain()
        {
            var id0 = new TrackableId(111, 0);
            var id1 = new TrackableId(222, 0);
            var id2 = new TrackableId(333, 0);
            var plane0 = m_PlaneBuilder.WithTrackableId(id0).WithParentId(TrackableId.invalidId).Build();
            var plane1 = m_PlaneBuilder.WithTrackableId(id1).WithParentId(id0).Build();
            var plane2 = m_PlaneBuilder.WithTrackableId(id2).WithParentId(id1).Build();
            var array = new NativeArray<BoundedPlane>(3, Allocator.Temp);
            array[0] = plane2;
            array[1] = plane1;
            array[2] = plane0;

            m_Spawner.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");
            return (id0, id1, id2);
        }

        [Test]
        public void CreateOrUpdateTrackables_CreatesDeepHierarchy()
        {
            var (id0, id1, id2) = SetupChain();

            var allEntries = m_Spawner.m_EntriesByTrackableId;
            Assert.AreEqual(3, allEntries.Count);
            Assert.IsTrue(allEntries.ContainsKey(id0));
            Assert.IsTrue(allEntries.ContainsKey(id1));
            Assert.IsTrue(allEntries.ContainsKey(id2));

            var rootEntry = allEntries[id0][0];
            Assert.AreEqual(1, rootEntry.childKeys.Count);
            Assert.IsNotNull(rootEntry.trackable);
            Assert.IsTrue(rootEntry.childKeys.Contains(new TrackableKey(id1, typeof(ARPlane))));

            var rootTrackable = rootEntry.trackable;
            Assert.AreEqual(m_Origin.TrackablesParent, rootTrackable.transform.parent);
            Assert.AreEqual(id0, rootTrackable.trackableId);
            Assert.AreEqual(TrackableId.invalidId, rootTrackable.parentId);
            Assert.AreEqual(typeof(ARPlane), rootTrackable.GetType());

            var stemEntry = allEntries[id1][0];
            Assert.AreEqual(1, stemEntry.childKeys.Count);
            Assert.IsNotNull(stemEntry.trackable);
            Assert.IsTrue(stemEntry.childKeys.Contains(new TrackableKey(id2, typeof(ARPlane))));

            var stemTrackable = stemEntry.trackable;
            Assert.AreEqual(rootTrackable.transform, stemTrackable.transform.parent);
            Assert.AreEqual(id1, stemTrackable.trackableId);
            Assert.AreEqual(id0, stemTrackable.parentId);
            Assert.AreEqual(typeof(ARPlane), stemTrackable.GetType());

            var leafEntry = allEntries[id2][0];
            Assert.AreEqual(0, leafEntry.childKeys.Count);
            Assert.IsNotNull(leafEntry.trackable);

            var leafTrackable = leafEntry.trackable;
            Assert.AreEqual(stemTrackable.transform, leafTrackable.transform.parent);
            Assert.AreEqual(id2, leafTrackable.trackableId);
            Assert.AreEqual(id1, leafTrackable.parentId);
            Assert.AreEqual(typeof(ARPlane), leafTrackable.GetType());

            Assert.AreEqual(0, m_Spawner.m_OrphanedEntriesByMissingParentIds.Count);
            Assert.AreEqual(0, m_Spawner.m_OrphanPool.CountActive);
        }

        void UpdateChain()
        {
            var id0 = new TrackableId(111, 0);
            var id1 = new TrackableId(222, 0);
            var id2 = new TrackableId(333, 0);

            var plane0 = m_PlaneBuilder
                .WithTrackableId(id0)
                .WithParentId(TrackableId.invalidId)
                .WithClassifications(PlaneClassifications.Floor)
                .Build();

            var plane1 = m_PlaneBuilder
                .WithTrackableId(id1)
                .WithParentId(id0)
                .WithClassifications(PlaneClassifications.Couch)
                .Build();

            var plane2 = m_PlaneBuilder
                .WithTrackableId(id2)
                .WithParentId(id1)
                .WithClassifications(PlaneClassifications.Seat)
                .Build();

            var array = new NativeArray<BoundedPlane>(3, Allocator.Temp);
            array[0] = plane2;
            array[1] = plane1;
            array[2] = plane0;

            m_Spawner.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");
        }

        [Test]
        public void CreateOrUpdateTrackables_UpdatesTrackables()
        {
            var (id0, id1, id2) = SetupChain();
            UpdateChain();

            var entries = m_Spawner.m_EntriesByTrackableId;
            Assert.AreEqual(3, entries.Count);

            var rootPlane = entries[id0][0].trackable as ARPlane;
            Assert.IsNotNull(rootPlane);
            Assert.AreEqual(PlaneClassifications.Floor, rootPlane.classifications);

            var stemPlane = entries[id1][0].trackable as ARPlane;
            Assert.IsNotNull(stemPlane);
            Assert.AreEqual(PlaneClassifications.Couch, stemPlane.classifications);

            var leafPlane = entries[id2][0].trackable as ARPlane;
            Assert.IsNotNull(leafPlane);
            Assert.AreEqual(PlaneClassifications.Seat, leafPlane.classifications);

            Assert.AreEqual(3, m_Spawner.m_EntryListPool.CountActive);
            Assert.AreEqual(3, m_Spawner.m_EntryPool.CountActive);
            Assert.AreEqual(0, m_Spawner.m_OrphanedEntriesByMissingParentIds.Count);
            Assert.AreEqual(0, m_Spawner.m_OrphanPool.CountActive);
        }

        [UnityTest]
        public IEnumerator TrackableDestroy_ClearsEntries()
        {
            var (_, id1, id2) = SetupChain();

            var success = m_Spawner.TryGetTrackableByKey(new TrackableKey(id2, typeof(ARPlane)), out var leafTrackable);
            Assert.IsTrue(success);
            Assert.IsNotNull(leafTrackable);
            Object.Destroy(leafTrackable.gameObject);
            yield return null; // OnDestroy is not called synchronously

            var allEntries = m_Spawner.m_EntriesByTrackableId;
            Assert.AreEqual(2, allEntries.Count);

            var stemEntry = allEntries[id1][0];
            Assert.AreEqual(0, stemEntry.childKeys.Count);

            Assert.IsFalse(allEntries.ContainsKey(id2));

            Assert.AreEqual(2, m_Spawner.m_EntryListPool.CountActive);
        }

        [Test]
        public void CreateOrUpdateTrackables_SameTrackableIdInDifferentTrackableTypes()
        {
            var id0 = new TrackableId(111, 0);
            var plane0 = m_PlaneBuilder.WithTrackableId(id0).Build();
            var box0 = m_BoxBuilder.WithTrackableId(id0).Build();
            var planeArray = new NativeArray<BoundedPlane>(1, Allocator.Temp);
            planeArray[0] = plane0;
            var boxArray = new NativeArray<XRBoundingBox>(1, Allocator.Temp);
            boxArray[0] = box0;
            m_Spawner.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(planeArray, null, "Plane");
            m_Spawner.CreateOrUpdateTrackables<ARBoundingBox, XRBoundingBox>(boxArray, null, "BoundingBox");

            Assert.AreEqual(2, m_Spawner.m_EntryPool.CountActive);
            Assert.AreEqual(1, m_Spawner.m_EntryListPool.CountActive);
            Assert.AreEqual(0, m_Spawner.m_OrphanPool.CountActive);
            Assert.AreEqual(0, m_Spawner.m_OrphanedEntriesByMissingParentIds.Count);
            Assert.AreEqual(1, m_Spawner.m_EntriesByTrackableId.Count);
            Assert.IsTrue(m_Spawner.m_EntriesByTrackableId.ContainsKey(id0));
            var entryList = m_Spawner.m_EntriesByTrackableId[id0];
            Assert.AreEqual(2, entryList.Count);
            var planeEntry = entryList[0];
            var boxEntry = entryList[1];
            Assert.AreEqual(typeof(ARPlane), planeEntry.trackableType);
            Assert.AreEqual(typeof(ARBoundingBox), boxEntry.trackableType);
            Assert.AreNotEqual(planeEntry.trackable.gameObject, boxEntry.trackable.gameObject);
            Assert.AreEqual(m_Origin.TrackablesParent, planeEntry.trackable.transform.parent);
            Assert.AreEqual(m_Origin.TrackablesParent, boxEntry.trackable.transform.parent);
            Assert.AreEqual(null, planeEntry.parentKey);
            Assert.AreEqual(null, boxEntry.parentKey);
            Assert.AreEqual(typeof(ARPlane), planeEntry.trackable.GetType());
            Assert.AreEqual(typeof(ARBoundingBox), boxEntry.trackable.GetType());
            Assert.AreEqual(new TrackableKey(id0, typeof(ARPlane)), planeEntry.key);
            Assert.AreEqual(new TrackableKey(id0, typeof(ARBoundingBox)), boxEntry.key);
        }

        [Test]
        public void CreateOrUpdateTrackables_PlaneParentedToBoundingBox()
        {
            var id0 = new TrackableId(111, 0);
            var box0 = m_BoxBuilder.WithTrackableId(id0).Build();
            var id1 = new TrackableId(222, 0);
            var plane1 = m_PlaneBuilder.WithTrackableId(id1).WithParentId(id0).Build();
            var boxArray = new NativeArray<XRBoundingBox>(1, Allocator.Temp);
            boxArray[0] = box0;
            var planeArray = new NativeArray<BoundedPlane>(1, Allocator.Temp);
            planeArray[0] = plane1;
            m_Spawner.CreateOrUpdateTrackables<ARBoundingBox, XRBoundingBox>(boxArray, null, "BoundingBox");
            m_Spawner.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(planeArray, null, "Plane");

            Assert.AreEqual(2, m_Spawner.m_EntriesByTrackableId.Count);
            Assert.IsTrue(m_Spawner.m_EntriesByTrackableId.ContainsKey(id0));
            Assert.IsTrue(m_Spawner.m_EntriesByTrackableId.ContainsKey(id1));
            Assert.AreEqual(1, m_Spawner.m_EntriesByTrackableId[id0].Count);
            Assert.AreEqual(1, m_Spawner.m_EntriesByTrackableId[id1].Count);

            var boxEntry = m_Spawner.m_EntriesByTrackableId[id0][0];
            var planeEntry = m_Spawner.m_EntriesByTrackableId[id1][0];
            Assert.AreEqual(planeEntry.trackable.transform.parent, boxEntry.trackable.transform);
            Assert.AreEqual(new TrackableKey(id0, typeof(ARBoundingBox)), planeEntry.parentKey);
            Assert.AreEqual(1, boxEntry.childKeys.Count);
            Assert.AreEqual(new TrackableKey(id1, typeof(ARPlane)), boxEntry.childKeys[0]);
        }
    }
}
