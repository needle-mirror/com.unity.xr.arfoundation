using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEditor.XR.ARFoundation.Tests
{
    class SceneUtilsTests
    {
        /// <summary>
        /// This is necessary for correctness because Edit Mode tests contain all GameObjects in the active scene.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DestroyAllGameObjects();
        }

        [Test]
        public void CreateARSessionWithParent_CreatesSuccessfully()
        {
            var parent = new GameObject().transform;
            var arSession = SceneUtils.CreateARSessionWithParent(parent);
            Assert.IsNotNull(arSession);
            Assert.AreEqual(parent, arSession.transform.parent);
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateARSessionWithoutParent_CreatesSuccessfully()
        {
            var arSession = SceneUtils.CreateARSessionWithParent(null);
            Assert.IsNotNull(arSession);
            Assert.IsNull(arSession.transform.parent);
            Object.DestroyImmediate(arSession);
        }

        [Test]
        public void UndoRedoCreateARSession_WorksWithNoErrors()
        {
            Undo.IncrementCurrentGroup();
            var parent = new GameObject().transform;
            var arSession = SceneUtils.CreateARSessionWithParent(parent).GetComponent<ARSession>();
            Assert.IsNotNull(arSession);

            Undo.PerformUndo();
            arSession = Object.FindAnyObjectByType<ARSession>();
            Assert.IsTrue(arSession == null);

            Undo.PerformRedo();
            arSession = Object.FindAnyObjectByType<ARSession>();
            Assert.IsNotNull(arSession);
            Assert.AreEqual(parent, arSession.transform.parent);
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateARPlaneVisualizer_CreatesSuccessfully()
        {
            var parent = new GameObject().transform;
            var planeViz = SceneUtils.CreateARPlaneVisualizerWithParent(parent);
            Assert.IsNotNull(planeViz);
            Assert.AreEqual(parent, planeViz.transform.parent);

            var meshRenderer = planeViz.GetComponent<MeshRenderer>();
            Assert.IsNotNull(meshRenderer);
            Assert.IsNotNull(meshRenderer.sharedMaterials);
            Assert.IsNotEmpty(meshRenderer.sharedMaterials);
            Assert.IsNotNull(meshRenderer.sharedMaterials[0]);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateARFaceVisualizer_CreatesSuccessfully()
        {
            var parent = new GameObject().transform;
            var faceViz = SceneUtils.CreateARFaceVisualizerWithParent(parent);
            Assert.IsNotNull(faceViz);
            Assert.AreEqual(parent, faceViz.transform.parent);

            var meshRenderer = faceViz.GetComponent<MeshRenderer>();
            Assert.IsNotNull(meshRenderer);
            Assert.IsNotNull(meshRenderer.sharedMaterials);
            Assert.IsNotEmpty(meshRenderer.sharedMaterials);
            Assert.IsNotNull(meshRenderer.sharedMaterials[0]);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateARDebugMenuWithParent_CreatesSuccessfully()
        {
            var parent = new GameObject().transform;
            var debugMenu = SceneUtils.CreateARDebugMenuWithParent(parent);
            Assert.IsNotNull(debugMenu);
            Assert.AreEqual(parent, debugMenu.transform.parent);
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateARDebugMenuWithoutParent_CreatesSuccessfully()
        {
            var debugMenu = SceneUtils.CreateARDebugMenuWithParent(null);
            Assert.IsNotNull(debugMenu);
            Assert.IsNull(debugMenu.transform.parent);
            Object.DestroyImmediate(debugMenu);
        }

        [Test]
        public void UndoRedoCreateARDebugMenu_WorksWithNoErrors()
        {
            Undo.IncrementCurrentGroup();
            var debugMenu = SceneUtils.CreateARDebugMenuWithParent(null).GetComponent<ARDebugMenu>();
            Assert.IsNotNull(debugMenu);
            Undo.PerformUndo();
            debugMenu = Object.FindAnyObjectByType<ARDebugMenu>();
            Assert.IsTrue(debugMenu == null); // using Unity's overload for the == operator
            Undo.PerformRedo();
            debugMenu = Object.FindAnyObjectByType<ARDebugMenu>();
            Assert.IsNotNull(debugMenu);
            Object.DestroyImmediate(debugMenu);
        }

        static void DestroyAllGameObjects()
        {
            foreach (var g in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                // Don't destroy GameObjects that are children within a Prefab instance
                if (g == null || (PrefabUtility.IsPartOfAnyPrefab(g) && !PrefabUtility.IsAnyPrefabInstanceRoot(g)))
                    continue;

                Object.DestroyImmediate(g);
            }
        }
    }
}
