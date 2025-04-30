using NUnit.Framework;
using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARSubsystems.Tests
{
    [TestFixture]
    public class SerializableGuidTests
    {
        [Test]
        public void SerializableGuidAsByteNativeArray()
        {
            var guid = new Guid("A38E0104-A130-4F96-8EDF-D4BECD32513B");
            var serializableGuid = new SerializableGuid(guid);
            var actualBytes = serializableGuid.AsByteNativeArray();

            var expectedBytes = guid.ToByteArray();
            Assert.AreEqual(actualBytes, expectedBytes);
        }

        [Test]
        public void EmptySerializableGuidAsByteNativeArray()
        {
            var serializableGuid = SerializableGuid.empty;
            var actualBytes = serializableGuid.AsByteNativeArray();

            var expectedBytes = serializableGuid.guid.ToByteArray();
            Assert.AreEqual(actualBytes, expectedBytes);
        }
    }
}
