---
uid: arfoundation-samples-face-tracking
---
# Face tracking samples

Face tracking samples demonstrate AR Foundation [Face tracking](xref:arfoundation-face-tracking) features. You can open these samples in Unity from the `Assets/Scenes/FaceTracking` folder.

> [!TIP]
> You can check which platforms support AR Foundation [Face tracking](xref:arfoundation-face-tracking) features by checking the [Face tracking platform support](xref:arfoundation-face-tracking-platform-support) page.

To understand each of the face tracking sample scenes, refer to the following sections:

| Sample                                | Description  |
| :------------------------------------ | :----------- |
| [Face pose](#face-pose)               | Draws an axis at the detected face's pose. |
| [Face mesh](#face-mesh)               | Instantiates and updates a mesh representing the detected face. |
| [Face regions](#face-region) (ARCore) | Demonstrates ARCore face regions. |
| [Blend shapes](#blend-shape) (ARKit)  | Implements ARKit blend shapes. |
| [Eye lasers, eye poses, and fixation point](#eye-lasers) (ARKit) | Demonstrate eye and fixation point tracking on ARKit. |
| [Rear camera](#rear-camera) (ARKit)   | Use face tracking while the world-facing (rear) camera is active. |

## ARKit requirements

Face tracking supports devices with Apple Neural Engine in iOS 14 and newer, and iPadOS 14 and newer. Devices with iOS 13 and earlier, and iPadOS 13 and earlier, require a TrueDepth camera for face tracking. Refer to Apple's [Tracking and Visualizing Faces](https://developer.apple.com/documentation/arkit/content_anchors/tracking_and_visualizing_faces?language=objc) documentation for more information.

> [!NOTE]
> To check whether a device has an Apple Neural Engine or a TrueDepth camera, refer to Apple's [Tech Specs](https://support.apple.com/en_US/specs).

<a id="face-pose"></a>

## Face pose scene

The `Face Pose` scene is the simplest face tracking sample. This sample draws an axis at the detected face's pose.

This sample uses the front-facing (selfie) camera.

<a id="face-mesh"></a>

## Face mesh scene

The `Face Mesh` scene instantiates and updates a mesh representing the detected face.

Information about the device support (for example the number of faces that can be simultaneously tracked) is displayed on the screen.

This sample uses the front-facing (selfie) camera.

<a id="face-region"></a>

## Face regions scene (ARCore)

The `AR Core Face Regions` scene demonstrates the ARCore face regions feature.

Face regions are an ARCore-specific feature which provides pose information for specific regions on the detected face. For example, the left eyebrow. To understand more about face regions, refer to the ARCore [Face tracking](xref:arcore-face-tracking) documentation.

The face regions sample draws axes at each face region. Refer to `ARCoreFaceRegionManager.cs` to learn more about the sample code.

### Face regions requirements

This sample is available on ARCore only and uses the front-facing (selfie) camera.

<a id="blend-shape"></a>

## Blend shapes scene (ARKit)

The `ARKit Face Blend Shapes` scene demonstrates Apple's [Blend shapes](https://developer.apple.com/documentation/arkit/arfaceanchor/2928251-blendshapes) (Apple developer documentation) feature.

Blend shapes are an ARKit-specific feature which provides information about various facial features on a scale of `0..1`. For instance wink and frown are two blend shapes.

In the blend shapes sample, blend shapes are used to puppet a cartoon face which is displayed over the detected face. Refer to `ARKitBlendShapeVisualizer.cs` to understand the sample code.

### Blend shape requirements

This sample is available on ARKit only and uses the front-facing (selfie) camera.

<a id="eye-lasers"></a>

## Eye lasers, eye poses, and fixation point scenes (ARKit)

The `Eye Lasers`, `Eye Poses`, and `Fixation Point` scenes demonstrate eye and fixation point tracking.

Eye tracking produces a pose (position and rotation) for each eye in the detected face. The fixation point is the point the face is looking at (fixated upon). The eye lasers sample uses the eye pose to draw laser beams emitted from the detected face.

### Requirements

These samples use the front-facing (selfie) camera and require an iOS device with a TrueDepth camera.

<a id="rear-camera"></a>

## Rear camera face tracking scene (ARKit)

The `World Camera With User Facing Face Tracking` scene implements ARKit support to use face tracking while the world-facing camera is active. You can open this sample in Unity from the `Assets/Scenes/FaceTracking/WorldCameraWithUserFacingFaceTracking` folder.

iOS 13 adds support for face tracking while the world-facing (rear) camera is active. This means the user-facing (selfie) camera is used for face tracking, but the passthrough video uses the world-facing camera.

To enable this mode in AR Foundation, you must:

* Enable an [ARFaceManager](xref:arfoundation-face-tracking-face-manager).
* Set the [ARSession](xref:arfoundation-session) **Tracking mode** to **Position and Rotation** or **Don't Care**.
* Set the [ARCameraManager](xref:arfoundation-camera-components#ar-camera-manager-component)'s **Facing direction** to **World**. Tap the screen to toggle between the user-facing and world-facing cameras.

The sample code in `DisplayFaceInfo.OnEnable.cs` shows how to detect support for these face tracking features.

When using the world-facing camera, a cube is displayed in front of the camera. The orientation of the cube is driven by the face in front of the user-facing camera.

[!include[](../../snippets/apple-arkit-trademark.md)]
