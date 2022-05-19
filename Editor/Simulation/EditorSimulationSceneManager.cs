using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [Serializable]
    class EditorSimulationSceneManager : BaseSimulationSceneManager
    {
        const string k_EditorEnvironmentSceneName = "Preview " + k_EnvironmentSceneName;

        protected override Scene CreateEnvironmentScene()
        {
            var scene = EditorSceneManager.NewPreviewScene();
            scene.name = k_EditorEnvironmentSceneName;
            return scene;
        }

        protected override void DestroyEnvironmentScene()
        {
            if (environmentScene != default)
                EditorSceneManager.ClosePreviewScene(environmentScene);
        }
    }
}
