using NUnit.Framework;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Simulation.Tests
{
    [TestFixture]
    class SimulationEnvironmentTestFixture : SimulationSessionTestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupSession();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TearDownSession();
        }

        Scene FindSimulationScene(string sceneName)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                    return scene;
            }

            return default;
        }

        [Test]
        [Order(1)]
        public void EnvironmentLoaded()
        {
            const string sceneName = SimulationSceneManager.k_EnvironmentSceneName;
            var environmentScene = FindSimulationScene(sceneName);

            // Check simulation scene is initialized
            Assert.AreEqual(sceneName, environmentScene.name);
            Assert.AreEqual(1, environmentScene.rootCount, $"{sceneName} should only have one root GameObject.");

            // Check the environment root is valid
            var rootGO = environmentScene.GetRootGameObjects()[0];
            var simulationEnvironment = rootGO.GetComponent<SimulationEnvironment>();
            Assert.IsNotNull(simulationEnvironment, $"{sceneName} doesn't have a valid environment root GameObject.");
        }

        [Test]
        [Order(2)]
        public void CorrectEnvironmentPrefab()
        {
            const string sceneName = SimulationSceneManager.k_EnvironmentSceneName;
            var environmentScene = FindSimulationScene(sceneName);

            Assert.AreEqual(sceneName, environmentScene.name);
            Assert.AreEqual(1, environmentScene.rootCount, $"{sceneName} should only have one root GameObject.");

            // Check if the environment was created from the right prefab
            // When instantiating a prefab the resulting GameObject will
            // have a name "<Prefab.name>(Clone)
            var environmentPrefab = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            var rootGO = environmentScene.GetRootGameObjects()[0];
            Assert.AreEqual($"{environmentPrefab.name}(Clone)", rootGO.name, $"\"{rootGO.name}\" root game object is not created from \"{environmentPrefab.name}\" environment prefab.");
        }
    }
}
