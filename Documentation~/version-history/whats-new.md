---
uid: arfoundation-whats-new
---
# What's new in version 6.0

The most significant updates in this release include:

## Added

- Added support for Image Stabilization, which helps stabilize shaky video from the camera.
- Added support for Occlusion to XR Simulation, allowing you to test occlusion without having to deploy to device. The pixel values provided are given in Unity units from the camera. (By default, 1 Unity unit = 1 meter.)
- Added the following subsystem descriptor registration methods for consistency with other subsystem descriptors:
 - [XRCameraSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.Register)
 - [XREnvironmentProbeSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XREnvironmentProbeSubsystemDescriptor.Register)
 - [XRHumanBodySubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRHumanBodySubsystemDescriptor.Register)
 - [XROcclusionSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.Register)
- Added `XRObjectTrackingSubsystemDescriptor.Register(XRObjectTrackingSubsystemDescriptor.Cinfo cinfo)` to replace the deprecated register methods present in `XRObjectTrackingSubsystem`
- Added `XRParticipantSubsystemDescriptor.Register(XRParticipantSubsystemDescriptor.Cinfo cinfo)` to replace the deprecated register methods present in `XRParticipantSubsystem`

### Asynchronous TryAddAnchor API

In previous versions of AR Foundation, it was only possible to create anchors synchronously, either via `XRAnchorSubsystem.TryAddAnchor` or `XRAnchorSubsystem.TryAttachAnchor`. However, not all AR platforms natively support synchronous anchor creation. To improve quality of life on these platforms, AR Foundation 6 adds a new [XRAnchorSubsystem.Provider.TryAddAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchorAsync) method, backed by the [Awaitable\<T\>](xref:UnityEngine.Awaitable`1) class introduced in Unity 2023.1.

[ARAnchorManager](xref:UnityEngine.XR.ARFoundation.ARAnchorManager) also has a new method, [TryAddAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryAddAnchorAsync), that allows you to asynchronously create `ARAnchor`s.

#### A note for providers

[XRAnchorSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor) has a new property: [supportsSynchronousAdd](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSynchronousAdd). All providers are required to implement the new [XRAnchorSubsystem.Provider.TryAddAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchorAsync) method, and if you also support the synchronous [TryAddAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchor) method, you should set your descriptor's `supportsSynchronousAdd` value to `true`.

To implement `TryAddAnchorAsync`, it is possible that no action is required on your part. The default implementation will attempt to forward calls to your provider's synchronous implementation and return its results.

## Changed

- Changed `Promise<T>.OnKeepWaiting()` to `virtual` instead of `abstract`.
- Changed `XRPlaneSubsystem.Provider.requestedPlaneDetectionMode` to `abstract` from `virtual`. Inheriting classes must now implement this property.
- Changed `SubsystemLifecycleManager.GetActiveSubsystemInstance()` to `protected static` from `protected`, as it does not use any instance members of the `SubsystemLifecycleManager` class.
- Changed the life cycle behavior of [ARAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchor) in the event that adding the anchor to the `XRAnchorSubsystem` failed. Instead of retrying the add operation every frame, the `ARAnchor` component now disables itself after the first failed attempt.
- Changed `ARTrackable` to now implement the `ITrackable` interface, enabling generic API designs when dealing with trackables.

### Deprecated

- Deprecated and replaced the following APIs for consistency with other subsystems:
  - `XRCameraSubsystemCinfo` to `XRCameraSubsystemDescriptor.Cinfo`
  - `FaceSubsystemParams` to `XRFaceSubsystemDescriptor.Cinfo`
  - `XRHumanBodySubsystemCinfo` to `XRHumanBodySubsystemDescriptor.Cinfo`
  - `XREnvironmentProbeSubsystemCinfo` to `XREnvironmentProbeSubsystemDescriptor.Cinfo`
  - `XROcclusionSubsystemCinfo` to `XROcclusionSubsystemDescriptor.Cinfo`
  - `XRAnchorSubsystemDescriptor.Create` to `XRAnchorSubsystemDescriptor.Register`
  - `XRFaceSubsystemDescriptor.Create` to `XRFaceSubsystemDescriptor.Register`
  - `XRImageTrackingSubsystemDescriptor.Create` to `XRImageTrackingSubsystemDescriptor.Register`
  - `XRPlaneSubsystemDescriptor.Create` to `XRPlaneSubsystemDescriptor.Register`
  - `XRPointCloudSubsystemDescriptor.RegisterDescriptor` to `XRPointCloudSubsystemDescriptor.Register`
  - `XRRaycastSubsystemDescriptor.RegisterDescriptor` to `XRRaycastSubsystemDescriptor.Register`
  - `XRSessionSubsystemDescriptor.RegisterDescriptor` to `XRSessionSubsystemDescriptor.Register`
  - `XRCameraSubsystem.Register` to `XRCameraSubsystemDescriptor.Register`
  - `XREnvironmentProbeSubsystem.Register` to `XREnvironmentProbeSubsystemDescriptor.Register`
  - `XRHumanBodySubsystem.Register` to `XRHumanBodySubsystemDescriptor.Register`
  - `XROcclusionSubsystem.Register` to `XROcclusionSubsystemDescriptor.Register`
- Deprecated the structs `XRObjectTrackingSubsystemDescriptor.Capabilities` and `XRParticipantSubsystemDescriptor.Capabilities`, since they do not have any functionality.
- Deprecated the `XRObjectTrackingSubsystemDescriptor.Register` and `XRParticipantSubsystemDescriptor.Register` methods that use the now-deprecated `Capabilities` struct.

## Removed

- Removed the image file `/Editor/Icons/ARVR@4x.png` as it was unused.
- The following table contains a list of APIs that have been removed from AR Foundation 6.0.
If your code uses any of these APIs, you must upgrade to use the recommended replacement instead.

| Obsolete API                                                                | Recommendation                                                                                                                                                                        |
|:----------------------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ARAnchorManager.AddAnchor`                                                 | Add an anchor using AddComponent<ARAnchor>                                                                                                                                            |
| `ARAnchorManager.RemoveAnchor`                                              | Call Destroy() on the ARAnchor component to remove it.                                                                                                                                |
| `ARCameraManager.focusMode`                                                 | Use autoFocusEnabled or autoFocusRequested instead.                                                                                                                                   |
| `ARCamerManager.lightEstimationMode`                                        | Use currentLightEstimation or requestedLightEstimation instead.                                                                                                                       |
| `ARCamerManager.TryGetLatestImage`                                          | Use TryAcquireLatestCpuImage instead.                                                                                                                                                 |
| `AREnvironmentProbeManager.automaticPlacement`                              | Use automaticPlacementRequested or automaticPlacementEnabled instead.                                                                                                                 |
| `AREnvironmentProbeManager.environmentTextureHDR`                           | Use environmentTextureHDRRequested or environmentTextureHDREnabled instead.                                                                                                           |
| `AREnvironmentProbeManager.AddEnvironmentProbe`                             | Add an environment probe using AddComponent<AREnvironmentProbe>().                                                                                                                    |
| `AREnvironmentProbeManager.RemoveEnvironmentProbe`                          | Call Destroy() on the AREnvironmentProbe component to remove it.                                                                                                                      |
| `ARFaceManager.maximumFaceCount`                                            | Use requestedMaximumFaceCount or currentMaximumFaceCount instead.                                                                                                                     |
| `ARHumanBodyManager.humanBodyPose2DEstimationEnabled`                       | Use pose2DEnabled or pose2DRequested instead.                                                                                                                                         |
| `ARHumanBodyManager.humanBodyPose3DEstimationEnabled`                       | Use pose3DEnabled or pose3DRequested instead.                                                                                                                                         |
| `ARHumanBodyManager.humanBodyPose3DScaleEstimationEnabled`                  | Use pose3DScaleEstimationRequested or pose3DScaleEstimationRequested instead.                                                                                                         |
| `AROcclusionManager.humanSegmentationStencilMode`                           | Use requestedSegmentationStencilMode or currentSegmentationStencilMode instead.                                                                                                       |
| `AROcclusionManager.humanSegmentationDepthMode`                             | Use requestedSegmentationDepthMode or currentSegmentationDepthMode instead.                                                                                                           |
| `ARPlaneManager.detectionMode`                                              | Use requestedDetectionMode or currentDetectionMode instead                                                                                                                            |
| `ARSession.matchFrameRate`                                                  | Use matchFrameRateRequested or matchFrameRateEnabled instead.                                                                                                                         |
| `ARTrackableManager.sessionOrigin`                                          | 'sessionOrigin' has been obsoleted; use 'origin' instead.                                                                                                                             |
| `ARTrackedImageManager.maxNumberOfMovingImages`                             | Use requestedMaxNumberOfMovingImages or currentMaxNumberOfMovingImages instead.                                                                                                       |
| `MutableRuntimeReferenceImageLibrary.ScheduleAddImageJob`                   | Use ScheduleAddImageWithValidationJob instead.                                                                                                                                        |
| `XRAnchorSubsystemDescriptor.Cinfo.subsystemImplementationType`             | XRAnchorSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.           |
| `XRCameraSubsystem.TryGetLatestImage`                                       | Use TryAcquireLatestCpuImage instead.                                                                                                                                                 |
| `XRCameraSubsystemCinfo.implementationType`                                 | XRCameraSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.           |
| `XREnvironmentProbeSubsystemCinfo.implmentationType`                        | XREnvironmentProbeSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead. |
| `FaceSubsystemParams.subsystemImplementationType`                           | XRFaceSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.             |
| `XRHumanBodySubsystemCinfo.implementationType`                              | XRHumanBodySubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.        |
| `XRImageTrackingSubsystem.Cinfo.subsystemImplementationType`                | XRImageTrackingSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.    |
| `XROcclusionSubsystemCinfo.implementationType`                              | XROcclusionSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.        |
| `XROcclusionSubsystemCinfo.supportsHumanSegmentationStencilImage`           | Use humanSegmentationStencilImageSupportedDelegate instead.                                                                                                                           |
| `XROcclusionSubsystemCinfo.supportsHumanSegmentationDepthImage`             | Use humanSegmentationDepthImageSupportedDelegate instead.                                                                                                                             |
| `XROcclusionSubsystemCinfo.queryForSupportsEnvironmentDepthImage`           | Use environmentDepthImageSupportedDelegate instead.                                                                                                                                   |
| `XROcclusionSubsystemCinfo.queryForSupportsEnvironmentDepthConfidenceImage` | Use environmentDepthConfidenceImageSupportedDelegate instead.                                                                                                                         |
| `XRPlaneSubsystemDescriptor.Cinfo.subsystemImplementationType`              | XRPlaneSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.            |
| `XRPointCloudSubsystemDescriptor.Cinfo.implementationType`                  | XRPointCloudSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.       |
| `XRRaycastSubsystemDescriptor.Cinfo.subsystemImplementationType`            | XRRaycastSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.          |
| `XRReferenceObjectLibrary.indexOf`                                          | Use IndexOf instead.                                                                                                                                                                  |
| `XRSessionSubsystem.subsystemImplementationType`                            | XRSessionSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.          |
| `XRSubsystem`                                                               | XRSubsystem has been deprecated. Use UnityEngine.SubsystemsImplementation.SubsystemWithProvider instead.                                                                              |

For a full list of changes and updates in this version, see the [AR Foundation package changelog](xref:arfoundation-changelog).

- Removed the `Description` attribute from values of several enums, as the attribute was unused:
  - `XRCameraFrameProperties`
  - `AREnvironmentProbePlacementType`
  - `CameraFocusMode`
  - `LightEstimationMode`
  - `EnvironmentDepthMode`
  - `HumanSegmentationDepthMode`
  - `HumanSegmentationStencilMode`
  - `OcclusionPreferenceMode`
  