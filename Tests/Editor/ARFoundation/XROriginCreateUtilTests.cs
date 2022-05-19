using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace UnityEditor.XR.ARFoundation.Tests
{
    class XROriginCreateUtilTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DestroyAllGameObjects();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyAllGameObjects();
        }

        [Test]
        public void CreateXROriginWithParent_CreatesSuccessfully()
        {
            var parent = new GameObject().transform;
            var origin = XROriginCreateUtil.CreateXROriginWithParent(parent);
            Assert.IsNotNull(origin);
            Assert.AreEqual(parent, origin.transform.parent);
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateXROriginWithoutParent_CreatesSuccessfully()
        {
            var origin = XROriginCreateUtil.CreateXROriginWithParent(null);
            Assert.IsNotNull(origin);
            Assert.IsNull(origin.transform.parent);
        }

        [Test]
        public void UndoRedo_WorksWithNoErrors()
        {
            Undo.IncrementCurrentGroup();
            var origin = XROriginCreateUtil.CreateXROriginWithParent(null);
            Assert.IsNotNull(origin);
            Undo.PerformUndo();
            origin = Object.FindObjectOfType<XROrigin>();
            Assert.IsTrue(origin == null);
            Undo.PerformRedo();
            origin = Object.FindObjectOfType<XROrigin>();
            Assert.IsNotNull(origin);
        }

        static void DestroyAllGameObjects()
        {
            foreach (var g in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(g);
            }
        }
    }
}
