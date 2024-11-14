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

        readonly HashSet<SimulatedBoundingBox> boundingBoxInstances = new();
        internal IReadOnlyCollection<SimulatedBoundingBox> simulationEnvironmentBoundingBoxes => boundingBoxInstances;

        private readonly HashSet<SimulatedTrackedImage> imageInstances = new();
        internal IReadOnlyCollection<SimulatedTrackedImage> simulationEnvironmentImages => imageInstances;

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

        internal void TrackBoundingBox(SimulatedBoundingBox box)
        {
            boundingBoxInstances.Add(box);
        }

        internal void UntrackBoundingBox(SimulatedBoundingBox box)
        {
            boundingBoxInstances.Remove(box);
        }

        internal void TrackImage(SimulatedTrackedImage image)
        {
            imageInstances.Add(image);
        }

        internal void UntrackImage(SimulatedTrackedImage image)
        {
            imageInstances.Remove(image);
        }

        protected override Scene CreateEnvironmentScene()
        {
            ClearTrackedObjects();

            var scene = SceneManager.CreateScene(GenerateUniqueSceneName(), k_EnvironmentSceneParameters);
            if (!scene.IsValid())
                throw new InvalidOperationException("Environment scene could not be created.");

            return scene;
        }

        protected override GameObject GetOrCreateEnvironmentPrefab()
        {
            return GetPreferencesEnvironmentPrefab();
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
            boundingBoxInstances.Clear();
        }
    }
}
