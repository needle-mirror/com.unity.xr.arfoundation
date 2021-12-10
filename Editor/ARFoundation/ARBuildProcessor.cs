using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.XR.CoreUtils;

namespace UnityEditor.XR.ARFoundation
{
    /// <summary>
    /// A utility class to run AR build processors.
    /// </summary>
    class ARBuildProcessor: IProcessSceneWithReport
    {
        public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
        {
            var origin = (XROrigin) BuildReport.FindObjectOfType(typeof(XROrigin));
            if(origin)
            {
                var cameraBackground = origin.GetComponentInChildren<ARCameraBackground>();
                if(!cameraBackground)
                {
                    Debug.LogWarning($"AR Foundation is installed, but there is an XROrigin and no ARCameraBackground component in {scene.name} attached to the camera. Video pass-through will be disabled without this component.");

                    var cameraManager = origin.GetComponentInChildren<ARCameraManager>();
                    if(!cameraManager)
                    {
                        Debug.LogWarning($"AR Foundation is installed, but there is an XROrigin and no ARCameraManager component in {scene.name} attached to the camera. Video pass-through will be disabled without this component.");
                    }
                }
            }
        }

        public int callbackOrder => 0;
    }
}
