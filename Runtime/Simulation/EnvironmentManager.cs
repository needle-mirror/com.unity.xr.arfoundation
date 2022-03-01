using System;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Manages the simulation environment.
    /// </summary>
    class EnvironmentManager
    {
        const string k_EnvironmentSceneName = "Simulated Environment Scene";

        static readonly CreateSceneParameters k_EnvironmentSceneParameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);

        Scene m_EnvironmentScene;
        GameObject m_EnvironmentRoot;

        /// <summary>
        /// Setup a simulation environment based on the current Simulation Settings.
        /// </summary>
        public void SetupEnvironment()
        {
            m_EnvironmentScene = SceneManager.CreateScene(k_EnvironmentSceneName, k_EnvironmentSceneParameters);
            if (!m_EnvironmentScene.IsValid())
                throw new InvalidOperationException("Environment scene could not be created.");

            GameObject prefab = null;
            var settings = SimulationSettings.currentSettings;
            if (settings != null)
                prefab = settings.environmentPrefab != null ? settings.environmentPrefab : settings.fallbackEnvironmentPrefab;

            if (prefab == null)
                throw new InvalidOperationException("No environment prefab set.  Set an environment prefab in Project Settings > XR-Plugin Management > XR Simulation");

            m_EnvironmentRoot = GameObjectUtils.Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(m_EnvironmentRoot, m_EnvironmentScene);
        }

        /// <summary>
        /// Destroy the current simulation environment.
        /// </summary>
        public void TearDownEnvironment()
        {
            UnityObjectUtils.Destroy(m_EnvironmentRoot);
            SceneManager.UnloadSceneAsync(m_EnvironmentScene);
        }
    }
}
