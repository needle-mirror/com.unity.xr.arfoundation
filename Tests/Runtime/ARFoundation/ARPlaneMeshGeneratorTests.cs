using NUnit.Framework;
using Unity.Collections;
using UnityEngine.TestTools;

namespace UnityEngine.XR.ARFoundation.Tests
{
    [TestFixture]
    class ARPlaneMeshGeneratorTestFixture
    {
        [Test]
        public void SimplePlaneTest()
        {
            var mesh = new Mesh();
            var boundary = new NativeArray<Vector2>(3, Allocator.Persistent);
            boundary[0] = new Vector2(0, 0);
            boundary[1] = new Vector2(0, 1);
            boundary[2] = new Vector2(1, 1);

            var success = ARPlaneMeshGenerator.TryGenerateMesh(mesh, boundary);

            Assert.That(success, Is.True);
        }

        [Test]
        public void DegeneratePlaneTest()
        {
            var mesh = new Mesh();
            var boundary = new NativeArray<Vector2>(3, Allocator.Persistent);
            boundary[0] = new Vector2(0, 0);
            boundary[1] = new Vector2(0, 0);
            boundary[2] = new Vector2(0, 0);

            LogAssert.Expect(LogType.Error, "Cannot triangulate mesh for a plane because its vertices do not form a simple polygon");

            var success = ARPlaneMeshGenerator.TryGenerateMesh(mesh, boundary);

            Assert.That(success, Is.False);
        }
    }
}
