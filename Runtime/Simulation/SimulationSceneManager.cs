using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Manages the runtime simulation scene and environment instance.
    /// </summary>
    class SimulationSceneManager : BaseSimulationSceneManager
    {
        static readonly CreateSceneParameters k_EnvironmentSceneParameters = new (LocalPhysicsMode.Physics3D);

        readonly HashSet<SimulatedLight> lightInstances = new();
        internal IReadOnlyCollection<SimulatedLight> simulationEnvironmentLights => lightInstances;

        readonly HashSet<SimulatedAnchor> anchorInstances = new();
        internal IReadOnlyCollection<SimulatedAnchor> simulationEnvironmentAnchors => anchorInstances;

        internal void TrackLight(SimulatedLight light)
        {
            lightInstances.Add(light);
        }

        internal void UntrackLight(SimulatedLight light)
        {
            lightInstances.Remove(light);
        }

        internal void TrackAnchor(SimulatedAnchor anchor)
        {
            anchorInstances.Add(anchor);
        }

        internal void UntrackAnchor(SimulatedAnchor anchor)
        {
            anchorInstances.Remove(anchor);
        }

        protected override Scene CreateEnvironmentScene()
        {
            ClearTrackedObjects();

            var scene = SceneManager.CreateScene(GenerateUniqueSceneName(), k_EnvironmentSceneParameters);
            if (!scene.IsValid())
                throw new InvalidOperationException("Environment scene could not be created.");

            return scene;
        }

        protected override void DestroyEnvironmentScene()
        {
            ClearTrackedObjects();

            if (environmentScene.IsValid() && environmentScene != default)
                SceneManager.UnloadSceneAsync(environmentScene);
        }

        protected override GameObject InstantiateEnvironment(GameObject environmentPrefab)
        {
            return GameObjectUtils.Instantiate(environmentPrefab);
        }

        void ClearTrackedObjects()
        {
            lightInstances.Clear();
            anchorInstances.Clear();
        }
    }
}
