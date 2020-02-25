# Migration Guide

This will guide you through the changes coming from AR Foundation 3.x to 4.x.

## Camera and Tracking Mode Selection

In previous versions of ARFoundation, it was not possible to explicitly select which camera provided the camera texture used for pass-through video, nor was it possible to indicate the desired type of tracking (e.g., 3 or 6 degrees of freedom). With ARFoundation 4, you can now
* Explicitly request a particular camera. ARFoundation distinguishes between "world facing" and "user facing" cameras. On a phone, this corresponds to the rear facing and selfie cameras, respectively.
* Explicitly request the type of tracking.

In previous versions of ARFoundation, this was an implicit choice. For example, if you enabled face detection, you would likely get a 3dof (rotation only) session which used the user-facing camera.

ARKit 3 added modes of operation that were not possible to express with previous versions of ARFoundation. In particular, ARKit 3 supports both 3dof and 6dof tracking when using the user-facing camera to perform face tracking, as well as a mode that uses the world-facing camera for the video feed but also uses the user-facing camera to perform face tracking.

This table represents the face tracking support ARFoundation 3 could provide:

|              | 3 DOF  | 6 DOF |
| ------       | ------ | ----- |
| User  Camera |   ✓    |       |
| World Camera |        |   ✓*  |

\* This was only available by enabling another feature which required the world-facing camera, such as plane detection.

This table represents what ARKit3 supports:

|               | 3 DOF  | 6 DOF |
| ------        | ------ | ----- |
| User Camera   |   ✓    |   ✓   |
| World Camera  |        |   ✓   |

As you can see, there was no way to say "I want the user-facing camera with 6 dof tracking", and it was awkward to say "I want face tracking with the world-facing camera".

Not all features are available simultaneously. For instance, on ARCore, face and plane detection are mutually exclusive features; you cannot have both at the same time. On ARKit, it depends on the hardware. However, there was no way to _know_ what would happen if you enabled a particular combination of features. "Face tracking with the world-facing camera" requires particular hardware only available on newer iOS devices, and there was no way to query for support in a generic way.

### Configuration Chooser

To solve this problem, we've added methods to enumerate each discrete mode of operation (called a "configuration descriptor") and list its capabilities. All capabilities in a configuration descriptor are simultaneously available on the current device.

ARFoundation 4 also introduces the concept of a "configuration chooser", which uses the current set of requested features (e.g., "auto focus", "plane detection", and "face detection") and selects the best available configuration descriptor. This also allows you to query what would happen if you were to enable a particular set of features before enabling them.

The default configuration chooser simply chooses the configuration that supports the most number of requested features, but you can also implement a custom configuration chooser to perform your own logic. See [`XRSessionSubsystem.configurationChooser`](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.0/api/UnityEngine.XR.ARSubsystems.XRSessionSubsystem.html#UnityEngine_XR_ARSubsystems_XRSessionSubsystem_configurationChooser) in the ARSubsystems package.

### Adapting an existing project

The breaking change here is mostly behavioral. In previous versions, face tracking tended to trump other features, so enabling face tracking would give you a 3dof user-facing camera experience. Now you _must_ be explicit, and the default mode is 6 dof, world-facing. This means previous apps which used face tracking may not work the same as they did before. In addition to enabling face tracking, you may also need to specify the camera and tracking mode to achieve the same result. Note there is a "don't care" mode, which means tracking will not be a consideration when choosing the configuration.

In ARFoundation 4, the `ARCameraManager` (a component on a camera in your scene) controls which hardware camera is requested, and the `ARSession` component controls which tracking mode is requested. There is also a new scripting API for these modes.

## Requested vs Current

In previous versions of ARFoundation, several features had simple getters and setters. For instance, the `ARCameraManager` had an `autoFocusMode` property that you could set. This, however, did not distinguish between what had been requested and what the actual mode was. Note that it is possible that auto focus mode is not supported in the current state (ARKit's `ARFaceTrackingConfiguration`, for instance, does not support auto focus).

ARFoundation 3 was inconsistent in its handling of such properties. Some properties only had setters, and, of those that had getters, some getters returned what had been requested, while others returned the actual value by querying the underlying platform SDK.

ARFoundation 4 makes this explicit. Parameters now have "requested" and "current" properties. "Requested" properties are both gettable and settable while the "current" property is readonly. The "current" properties tell you what is actually enabled in the underlying SDK, not just what you have requested.

| Component                 | Previous                                  | Requested                             | Actual                            |
| ---------                 | --------                                  | ---------                             | ------                            |
| ARCameraManager           | `autoFocusMode`                           | `autoFocusRequested`                  | `autoFocusEnabled`                |
| ARCameraManager           | `lightEstimationMode`                     | `requestedLightEstimation`            | `currentLightEstimation`          |
| ARCameraManager           | --                                        | `requestedFacingDirection`            | `currentFacingDirection`          |
| ARSession                 | --                                        | `requestedTrackingMode`               | `currentTrackingMode`             |
| ARSession                 | `matchFrameRate`                          | `matchFrameRateRequested`             | `matchFrameRateEnabled`           |
| ARHumanBodyManager        | `humanBodyPose2DEstimationEnabled`        | `pose2DRequested`                     | `pose2DEnabled`                   |
| ARHumanBodyManager        | `humanBodyPose3DEstimationEnabled`        | `pose3DRequested`                     | `pose3DEnabled`                   |
| ARHumanBodyManager        | `humanBodyPose3DScaleEstimationEnabled`   | `pose3DScaleEstimationRequested`      | `pose3DScaleEstimationEnabled`    |
| ARFaceManager             | `maximumFaceCount`                        | `requestedMaximumFaceCount`           | `currentMaximumFaceCount`         |
| AROcclusionManager        | `humanSegmentationDepthMode`              | `requestedHumanDepthMode`             | `currentHumanDepthMode`           |
| AROcclusionManager        | `humanSegmentationDepthMode`              | `requestedHumanStencilMode`           | `currentHumanStencilMode`         |
| ARPlaneManager            | `planeDetectionMode`                      | `requestedPlaneDetectionMode`         | `currentPlaneDetectionMode`       |
| ARTrackedImageManager     | `maxNumberOfMovingImages`                 | `requestedMaxNumberOfMovingImages`    | `currentMaxNumberOfMovingImages`  |
| AREnvironmentProbeManager | `automaticPlacement`                      | `automaticPlacementRequested`         | `automaticPlacementEnabled`       |
| AREnvironmentProbeManager | `environmentTextureHDR`                   | `environmentTextureHDRRequested`      | `environmentTextureHDREnabled`    |
