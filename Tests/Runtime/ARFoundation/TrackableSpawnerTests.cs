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
        BoundedPlaneBuilder m_PlaneBuilder = new();

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject("XR Origin");
            m_Origin = go.AddComponent<XROrigin>();
            TrackableSpawner.instance.SetOrigin(m_Origin);
        }

        [TearDown]
        public void TearDown()
        {
            UnityObjectUtils.Destroy(m_Origin.gameObject);
            TrackableSpawner.ResetInstance();
            m_PlaneBuilder.Reset();
        }

        [Test]
        public void CreateOrUpdateTrackables_ParentsToTrackablesRootIfNoParentId()
        {
            var id0 = new TrackableId(123, 0);
            var plane0 = m_PlaneBuilder.WithTrackableId(id0).Build();
            var array = new NativeArray<BoundedPlane>(1, Allocator.Temp);
            array[0] = plane0;

            TrackableSpawner.instance.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");

            var entries = TrackableSpawner.instance.m_EntriesByTrackableId;
            Assert.AreEqual(1, entries.Count);
            Assert.IsTrue(entries.ContainsKey(id0));

            var entry = entries[id0];
            Assert.AreEqual(0, entry.childIds.Count);
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

            TrackableSpawner.instance.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");

            var entries = TrackableSpawner.instance.m_EntriesByTrackableId;
            Assert.AreEqual(2, entries.Count);
            Assert.IsTrue(entries.ContainsKey(id0));
            Assert.IsTrue(entries.ContainsKey(missingParentId));

            var parentEntry = entries[missingParentId];
            Assert.AreEqual(1, parentEntry.childIds.Count);
            Assert.IsNotNull(parentEntry.trackable);

            var parentTrackable = parentEntry.trackable;
            Assert.AreEqual(typeof(ARNullTrackable), parentTrackable.GetType());
            Assert.AreEqual(missingParentId, parentTrackable.trackableId);
            Assert.AreEqual(m_Origin.TrackablesParent, parentTrackable.transform.parent);

            Assert.AreEqual(parentTrackable.transform, entries[id0].trackable.transform.parent);

            // Validate that object pools are correctly being released
            Assert.AreEqual(0, TrackableSpawner.instance.m_OrphanedEntriesByMissingParentIds.Count);
            Assert.AreEqual(0, TrackableSpawner.instance.m_OrphanPool.CountActive);
        }

        (TrackableId id0, TrackableId id1, TrackableId id2) SetupChain()
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

            TrackableSpawner.instance.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");
            return (id0, id1, id2);
        }

        [Test]
        public void CreateOrUpdateTrackables_CorrectlyCreatesDeepHierarchy()
        {
            var (id0, id1, id2) = SetupChain();

            var spawner = TrackableSpawner.instance;
            var entries = spawner.m_EntriesByTrackableId;
            Assert.AreEqual(3, entries.Count);
            Assert.IsTrue(entries.ContainsKey(id0));
            Assert.IsTrue(entries.ContainsKey(id1));
            Assert.IsTrue(entries.ContainsKey(id2));

            var rootEntry = entries[id0];
            Assert.AreEqual(1, rootEntry.childIds.Count);
            Assert.IsNotNull(rootEntry.trackable);
            Assert.IsTrue(rootEntry.childIds.Contains(id1));

            var rootTrackable = rootEntry.trackable;
            Assert.AreEqual(m_Origin.TrackablesParent, rootTrackable.transform.parent);
            Assert.AreEqual(id0, rootTrackable.trackableId);
            Assert.AreEqual(TrackableId.invalidId, rootTrackable.parentId);
            Assert.AreEqual(typeof(ARPlane), rootTrackable.GetType());

            var stemEntry = entries[id1];
            Assert.AreEqual(1, stemEntry.childIds.Count);
            Assert.IsNotNull(stemEntry.trackable);
            Assert.IsTrue(stemEntry.childIds.Contains(id2));

            var stemTrackable = stemEntry.trackable;
            Assert.AreEqual(rootTrackable.transform, stemTrackable.transform.parent);
            Assert.AreEqual(id1, stemTrackable.trackableId);
            Assert.AreEqual(id0, stemTrackable.parentId);
            Assert.AreEqual(typeof(ARPlane), stemTrackable.GetType());

            var leafEntry = entries[id2];
            Assert.AreEqual(0, leafEntry.childIds.Count);
            Assert.IsNotNull(leafEntry.trackable);

            var leafTrackable = leafEntry.trackable;
            Assert.AreEqual(stemTrackable.transform, leafTrackable.transform.parent);
            Assert.AreEqual(id2, leafTrackable.trackableId);
            Assert.AreEqual(id1, leafTrackable.parentId);
            Assert.AreEqual(typeof(ARPlane), leafTrackable.GetType());

            Assert.AreEqual(0, spawner.m_OrphanedEntriesByMissingParentIds.Count);
            Assert.AreEqual(0, spawner.m_OrphanPool.CountActive);
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

            TrackableSpawner.instance.CreateOrUpdateTrackables<ARPlane, BoundedPlane>(array, null, "Plane");
        }

        [Test]
        public void CreateOrUpdateTrackables_UpdatesTrackables()
        {
            var (id0, id1, id2) = SetupChain();
            UpdateChain();

            var spawner = TrackableSpawner.instance;
            var entries = spawner.m_EntriesByTrackableId;
            Assert.AreEqual(3, entries.Count);

            var rootPlane = entries[id0].trackable as ARPlane;
            Assert.IsNotNull(rootPlane);
            Assert.AreEqual(PlaneClassifications.Floor, rootPlane.classifications);

            var stemPlane = entries[id1].trackable as ARPlane;
            Assert.IsNotNull(stemPlane);
            Assert.AreEqual(PlaneClassifications.Couch, stemPlane.classifications);

            var leafPlane = entries[id2].trackable as ARPlane;
            Assert.IsNotNull(leafPlane);
            Assert.AreEqual(PlaneClassifications.Seat, leafPlane.classifications);

            Assert.AreEqual(3, spawner.m_EntryPool.CountActive);
            Assert.AreEqual(0, spawner.m_OrphanedEntriesByMissingParentIds.Count);
            Assert.AreEqual(0, spawner.m_OrphanPool.CountActive);
        }

        [UnityTest]
        public IEnumerator TrackableDestroy_HandledCorrectly()
        {
            var (_, id1, id2) = SetupChain();

            var spawner = TrackableSpawner.instance;
            var leafTrackable = spawner.GetTrackableById(id2);
            Assert.IsNotNull(leafTrackable);
            Object.Destroy(leafTrackable.gameObject);
            yield return null; // OnDestroy is not called synchronously

            var entries = spawner.m_EntriesByTrackableId;
            Assert.AreEqual(2, entries.Count);

            var stemEntry = entries[id1];
            Assert.AreEqual(0, stemEntry.childIds.Count);

            Assert.IsFalse(entries.ContainsKey(id2));

            Assert.AreEqual(2, spawner.m_EntryPool.CountActive);
        }
    }
}
