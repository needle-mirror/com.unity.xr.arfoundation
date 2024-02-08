using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityEditor.XR.ARFoundation
{
    [InitializeOnLoad]
    class SceneHook
    {
        static SceneHook()
        {
            EditorSceneManager.sceneSaved += OnSceneSaved;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        static void OnSceneOpened(Scene scene, OpenSceneMode openSceneMode)
        {
            SendARUsageAnalyticsEvent(ARUsageAnalyticsEvent.Context.SceneOpen, scene);
        }

        static void OnSceneSaved(Scene scene)
        {
            SendARUsageAnalyticsEvent(ARUsageAnalyticsEvent.Context.SceneSave, scene);
        }

        static void SendARUsageAnalyticsEvent(ARUsageAnalyticsEvent.Context eventName, Scene scene)
        {
            AREditorAnalytics.arUsageAnalyticsEvent.Send(new ARUsageAnalyticsEvent.EventPayload(
                eventName: eventName,
                sceneGuid: AssetDatabase.GUIDFromAssetPath(scene.path),
                arManagersInfo: ARSceneAnalysis.GetARManagersInfo(scene)));
        }
    }
}
