#if UNITY_EDITOR
using UnityEditor;

namespace UnityEngine.XR.Simulation.Tests
{
    /// <summary>
    /// This scene manager is used for testing purposes and allows for the
    /// overriding of the SimulationSceneManager's method for retrieving
    /// the simulation environment prefab to use.
    /// </summary>
    class SimulationTestSceneManager : SimulationSceneManager
    {
        private string environmentPrefabPath { set; get; }

        internal SimulationTestSceneManager(string environmentPrefabPath)
        {
            this.environmentPrefabPath = environmentPrefabPath;
        }

        protected override GameObject GetOrCreateEnvironmentPrefab()
        {
            return PrefabUtility.LoadPrefabContents(environmentPrefabPath);
        }
    }
}
#endif //UNITY_EDITOR
