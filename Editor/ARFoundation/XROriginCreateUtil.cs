using System;
using Unity.XR.CoreUtils;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;

namespace UnityEditor.XR.ARFoundation
{
    static class XROriginCreateUtil
    {
#if !XRI_PRESENT
        [MenuItem("GameObject/XR/XR Origin (Mobile AR)", false, 10)]
        static void CreateXROrigin(MenuCommand menuCommand)
        {
            var context = (menuCommand.context as GameObject);
            var parent = context != null ? context.transform : null;
            var xrOrigin = CreateXROriginWithParent(parent);
            Selection.activeGameObject = xrOrigin.gameObject;
        }
#endif

        public static XROrigin CreateXROriginWithParent(Transform parent)
        {
            var originGo = ObjectFactory.CreateGameObject("XR Origin", typeof(XROrigin));
            Place(originGo, parent);

            var offsetGo = ObjectFactory.CreateGameObject("Camera Offset");
            Place(offsetGo, originGo.transform);
            
            var arCamera = CreateARMainCamera();
            Place(arCamera.gameObject, offsetGo.transform);

            var origin = originGo.GetComponent<XROrigin>();
            origin.CameraFloorOffsetObject = offsetGo;
            origin.Camera = arCamera;
            
            Undo.RegisterCreatedObjectUndo(originGo, "Create XR Origin");
            return origin;
        }

        static void Place(GameObject go, Transform parent)
        {
            var transform = go.transform;

            if (parent != null)
            {
                ResetTransform(transform);
                Undo.SetTransformParent(transform, parent, "Reparenting");
                ResetTransform(transform);
                go.layer = parent.gameObject.layer;
            }
            else
            {
                // Puts it at the scene pivot, and otherwise world origin if there is no Scene view.
                var view = SceneView.lastActiveSceneView;
                if (view != null)
                    view.MoveToView(transform);
                else
                    transform.position = Vector3.zero;

                StageUtility.PlaceGameObjectInCurrentStage(go);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(go);
        }
        
        static void ResetTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            if (transform.parent is RectTransform)
            {
                var rectTransform = transform as RectTransform;
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.sizeDelta = Vector2.zero;
                }
            }
        }
        
        static Camera CreateARMainCamera()
        {
            var mainCam = Camera.main;
            if (mainCam != null)
            {
                Debug.LogWarningFormat(
                    mainCam.gameObject,
                    "XR Origin Main Camera requires the \"MainCamera\" Tag, but the current scene contains another enabled Camera tagged \"MainCamera\". For AR to function properly, remove the \"MainCamera\" Tag from \'{0}\' or disable it.",
                    mainCam.name);
            }

            var cameraGo = ObjectFactory.CreateGameObject(
                "Main Camera",
                typeof(Camera),
                typeof(AudioListener),
                typeof(ARCameraManager),
                typeof(ARCameraBackground),
                typeof(TrackedPoseDriver));

            var camera = cameraGo.GetComponent<Camera>();
            Undo.RecordObject(camera, "Configure Camera");
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.black;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 20f;

            var trackedPoseDriver = cameraGo.GetComponent<TrackedPoseDriver>();

            Undo.RecordObject(trackedPoseDriver, "Configure Tracked Pose Driver");
            var positionAction = new InputAction("Position", binding: "<XRHMD>/centerEyePosition", expectedControlType: "Vector3");
            positionAction.AddBinding("<HandheldARInputDevice>/devicePosition");
            var rotationAction = new InputAction("Rotation", binding: "<XRHMD>/centerEyeRotation", expectedControlType: "Quaternion");
            rotationAction.AddBinding("<HandheldARInputDevice>/deviceRotation");
            trackedPoseDriver.positionInput = new InputActionProperty(positionAction);
            trackedPoseDriver.rotationInput = new InputActionProperty(rotationAction);
            return camera;
        }
    }
}
