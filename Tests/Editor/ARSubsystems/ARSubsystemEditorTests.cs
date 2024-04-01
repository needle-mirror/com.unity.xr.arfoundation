using NUnit.Framework;
using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARSubsystems.Tests
{
    [TestFixture]
    class ARSubsystemEditorTests
    {
        [MenuItem("AR Foundation/Tests/Clear image library data stores")]
        static void ClearReferenceImageLibraryDataStores()
        {
            foreach (var library in XRReferenceImageLibrary.All())
            {
                library.ClearDataStore();
            }
        }

        [Test]
        public void CanRoundtripGuid()
        {
            var guid = Guid.NewGuid();
            guid.Decompose(out var low, out var high);
            var recomposedGuid = GuidUtil.Compose(low, high);
            Assert.AreEqual(guid, recomposedGuid);
        }

        [Test]
        public void SerializableGuidToTrackableIdConversion()
        {
            var guid = Guid.NewGuid();
            guid.Decompose(out var low, out var high);
            SerializableGuid serializableGuid = new(low, high);
            TrackableId serializableGuidToTrackableId = serializableGuid;
            SerializableGuid trackableIdToSerializableGuid = serializableGuidToTrackableId;
            Assert.AreEqual(serializableGuid, trackableIdToSerializableGuid);
        }

        [Test]
        public void TrackableIdToSerializableGuidConversion()
        {
            var guid = Guid.NewGuid();
            guid.Decompose(out var low, out var high);
            TrackableId trackableId = new(low, high);
            SerializableGuid trackableIdToSerializableGuid = trackableId;
            TrackableId serializableGuidToTrackableId = trackableIdToSerializableGuid;
            Assert.AreEqual(trackableId, serializableGuidToTrackableId);
        }

        [Test]
        public void SerializableGuidCreatedWithGuid()
        {
            var guid = Guid.NewGuid();
            var serializableGuid = new SerializableGuid(guid);
            Assert.AreEqual(guid, serializableGuid.guid);
        }
    }
}
