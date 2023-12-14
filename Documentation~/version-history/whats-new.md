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
- Added support for planes to have multiple semantic labels via the [PlaneClassifications](xref:UnityEngine.XR.ARSubsystems.PlaneClassifications) flags enum, [ARPlane.classifications](xref:UnityEngine.XR.ARFoundation.ARPlane.classifications), and [BoundedPlane.classifications](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.classifications).
- Added `XRObjectTrackingSubsystemDescriptor.Register(XRObjectTrackingSubsystemDescriptor.Cinfo cinfo)` to replace the deprecated register methods present in `XRObjectTrackingSubsystem`
- Added `XRParticipantSubsystemDescriptor.Register(XRParticipantSubsystemDescriptor.Cinfo cinfo)` to replace the deprecated register methods present in `XRParticipantSubsystem`
- Added [XRCameraSubsystem.GetShaderKeywords](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem.GetMaterialKeywords) and [XROcclusionSubsystem.GetShaderKeywords](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.GetMaterialKeywords). Both return a new read-only [ShaderKeywords](xref:UnityEngine.XR.ARSubsystems.ShaderKeywords) struct.
- Added new API to make it possible for XR Simulation to identify images even if **Keep Texture at Runtime** is disabled in the reference image library:
  - Added [SimulatedTrackedImage.imageAssetGuid](xref:UnityEngine.XR.Simulation.SimulatedTrackedImage.imageAssetGuid)
- Added documentation:
  - Added [Display matrix format and derivation](xref:arfoundation-display-matrix-format-and-derivation) manual page.
  - Added [Custom background shaders](xref:arfoundation-custom-background-shaders) manual page.
- Added settings in the XR Simulation Preferences window for configuring navigation `InputAction`s and navigation speed.

### Asynchronous TryAddAnchor API

In previous versions of AR Foundation, it was only possible to create anchors synchronously, either via `XRAnchorSubsystem.TryAddAnchor` or `XRAnchorSubsystem.TryAttachAnchor`. However, not all AR platforms natively support synchronous anchor creation. To improve quality of life on these platforms, AR Foundation 6 adds a new [XRAnchorSubsystem.Provider.TryAddAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchorAsync) method, backed by the [Awaitable\<T\>](xref:UnityEngine.Awaitable`1) class introduced in Unity 2023.1.

[ARAnchorManager](xref:UnityEngine.XR.ARFoundation.ARAnchorManager) also has a new method, [TryAddAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryAddAnchorAsync), that allows you to asynchronously create `ARAnchor`s.

#### A note for providers

[XRAnchorSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor) has a new property: [supportsSynchronousAdd](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSynchronousAdd). All providers are required to implement the new [XRAnchorSubsystem.Provider.TryAddAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchorAsync) method, and if you also support the synchronous [TryAddAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchor) method, you should set your descriptor's `supportsSynchronousAdd` value to `true`.

To implement `TryAddAnchorAsync`, it is possible that no action is required on your part. The default implementation will attempt to forward calls to your provider's synchronous implementation and return its results.

### ARTrackableManager API

In AR Foundation 6, each class that inherits `ARTrackableManager` had its trackables changed event and `OnTrackablesChanged` method deprecated to be replaced by a single event [ARTrackableManager.trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager.trackablesChanged). This allows each trackable manager to now generically handle its corresponding trackables changed event and simplify the code needed to work across all `ARTrackableManager`s.

Additionally, each event arguments struct used in the trackables changed events of all the `ARTrackableManager`s has been deprecated in favor of a generic [ARTrackablesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs).

Lastly, the new `ARTrackableManager.trackablesChanged` event is a `UnityEvent` which means it is now accessible from the Inspector.

#### Visual Scripting

The following Visual Scripting nodes were deprecated, and new versions were built using the new `ARTrackableManager.trackablesChanged` event:

  - `OnAnchorsChanged`
  - `OnEnvironmentProbesChanged`
  - `OnFacesChanged`
  - `OnHumanBodiesChanged`
  - `OnParticipantsChanged`
  - `OnPlanesChanged`
  - `OnPointCloudsChanged`
  - `OnTrackedImagesChanged`
  - `OnTrackedObjectsChanged`

## Changed

- Changed `Promise<T>.OnKeepWaiting()` to `virtual` instead of `abstract`.
- Changed `XRPlaneSubsystem.Provider.requestedPlaneDetectionMode` to `abstract` from `virtual`. Inheriting classes must now implement this property.
- Changed `SubsystemLifecycleManager.GetActiveSubsystemInstance()` to `protected static` from `protected`, as it does not use any instance members of the `SubsystemLifecycleManager` class.
- Changed the life cycle behavior of [ARAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchor) in the event that adding the anchor to the `XRAnchorSubsystem` failed. Instead of retrying the add operation every frame, the `ARAnchor` component now disables itself after the first failed attempt.
- Changed `ARTrackable` to now implement the `ITrackable` interface, enabling generic API designs when dealing with trackables.
- Changed the [SimulatedTrackedImage](xref:UnityEngine.XR.Simulation.SimulatedTrackedImage) component to render a textured mesh of its image, allowing you to see the image in the Scene view and Game view without requiring additional GameObjects.
  - Removed now-unnecessary Quad GameObjects from the DefaultSimulationEnvironment.
- Changed the behavior of `SimulationSessionSubsystem.sessionId` to now return a non-empty unique value when the subsystem is running. You can access the session id using `XRSessionSubsystem.sessionId`.
- Changed XR Simulation navigation controls to be bound to configurable `InputAction`s instead of hard-coded to WASD keys.

## Deprecated

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
- Deprecated the following APIs:
  - [PlaneClassification](xref:UnityEngine.XR.ARSubsystems.PlaneClassification)
  - [ARPlane.classification](xref:UnityEngine.XR.ARFoundation.ARPlane.classification)
  - [BoundedPlane.classification](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.classification)
  - [BoundedPlane constructor](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.%23ctor(UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.Pose,UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.XR.ARSubsystems.PlaneAlignment,UnityEngine.XR.ARSubsystems.TrackingState,System.IntPtr,UnityEngine.XR.ARSubsystems.PlaneClassification))
  - [ARAnchorManager.anchorsChanged](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.anchorsChanged)
  - [ARAnchorManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.OnTrackablesChanged)
  - [AREnvironmentProbeManager.environmenProbesChanged](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbeManager.environmentProbesChanged)
  - [AREnvironmentProbeManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbeManager.OnTrackablesChanged)
  - [ARFaceManager.facesChanged](xref:UnityEngine.XR.ARFoundation.ARFaceManager.facesChanged)
  - [ARFaceManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARFaceManager.OnTrackablesChanged)
  - [ARHumanBodyManager.humanBodiesChanged](xref:UnityEngine.XR.ARFoundation.ARHumanBodyManager.humanBodiesChanged)
  - [ARHumanBodyManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARHumanBodyManager.OnTrackablesChanged)
  - [ARParticipantManager.participantsChanged](xref:UnityEngine.XR.ARFoundation.ARParticipantManager.participantsChanged)
  - [ARParticipantManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARParticipantManager.OnTrackablesChanged)
  - [ARPlaneManager.planesChanged](xref:UnityEngine.XR.ARFoundation.ARPlaneManager.planesChanged)
  - [ARPlaneManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARPlaneManager.OnTrackablesChanged)
  - [ARPointCloudManager.pointCloudsChanged](xref:UnityEngine.XR.ARFoundation.ARPointCloudManager.pointCloudsChanged)
  - [ARPointCloudManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARPointCloudManager.OnTrackablesChanged)
  - [ARTrackedImageManager.trackedImagesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.trackedImagesChanged)
  - [ARTrackedImageManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.OnTrackablesChanged)
  - [ARTrackedObjectManager.trackedObjectsChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectManager.trackedObjectsChanged)
  - [ARTrackedObjectManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectManager.OnTrackablesChanged)
  - [ARTrackableManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager.OnTrackablesChanged)
- Deprecated the following structs to be replaced by [ARTrackablesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs):
  - [ARAnchorsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARAnchorsChangedEventArgs)
  - [AREnvironmentProbesChangedEvent](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbesChangedEvent)
  - [ARFacesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARFacesChangedEventArgs)
  - [ARHumanBodiesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARHumanBodiesChangedEventArgs)
  - [ARParticipantsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARParticipantsChangedEventArgs)
  - [ARPlanesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARPlanesChangedEventArgs)
  - [ARPointCloudChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARPointCloudChangedEventArgs)
  - [ARTrackedImagesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackedImagesChangedEventArgs)
  - [ARTrackedObjectsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectsChangedEventArgs)
- Deprecated the structs `XRObjectTrackingSubsystemDescriptor.Capabilities` and `XRParticipantSubsystemDescriptor.Capabilities`, since they do not have any functionality.
- Deprecated the `XRObjectTrackingSubsystemDescriptor.Register` and `XRParticipantSubsystemDescriptor.Register` methods that use the now-deprecated `Capabilities` struct.
- Deprecated and replaced the following APIs:
  - `XRCameraSubsystem.GetMaterialKeywords` to `XRCameraSubsystem.GetShaderKeywords`
  - `XROcclusionSubsystem.GetMaterialKeywords` to `XROcclusionSubsystem.GetShaderKeywords`
  - `ARCameraFrameEventArgs.enabledMaterialKeywords` to `ARCameraFrameEventArgs.enabledShaderKeywords`
  - `ARCameraFrameEventArgs.disabledMaterialKeywords` to `ARCameraFrameEventArgs.disabledShaderKeywords`
  - `AROcclusionFrameEventArgs.enabledMaterialKeywords` to `AROcclusionFrameEventArgs.enabledShaderKeywords`
  - `AROcclusionFrameEventArgs.disabledMaterialKeywords` to `AROcclusionFrameEventArgs.disabledShaderKeywords`
- Deprecated the following Visual Scripting nodes:
  - `OnAnchorsChanged`
  - `OnEnvironmentProbesChanged`
  - `OnFacesChanged`
  - `OnHumanBodiesChanged`
  - `OnParticipantsChanged`
  - `OnPlanesChanged`
  - `OnPointCloudsChanged`
  - `OnTrackedImagesChanged`
  - `OnTrackedObjectsChanged`
- Deprecated and replaced the following APIs:
  - `XRCameraFrame.timestampNs` to `XRCameraFrame.TryGetTimestamp`
  - `XRCameraFrame.hasTimestamp` to `XRCameraFrame.TryGetTimestamp`
  - `XRCameraFrame.averageBrightness` to `XRCameraFrame.TryGetAverageBrightness`
  - `XRCameraFrame.hasAverageBrightness` to `XRCameraFrame.TryGetAverageBrightness`
  - `XRCameraFrame.averageColorTemperature` to `XRCameraFrame.TryGetAverageColorTemperature`
  - `XRCameraFrame.hasAverageColorTemperature` to `XRCameraFrame.TryGetAverageColorTemperature`
  - `XRCameraFrame.colorCorrection` to `XRCameraFrame.TryGetColorCorrection`
  - `XRCameraFrame.hasColorCorrection` to `XRCameraFrame.TryGetColorCorrection`
  - `XRCameraFrame.projectionMatrix` to `XRCameraFrame.TryGetProjectionMatrix`
  - `XRCameraFrame.hasProjectionMatrix` to `XRCameraFrame.TryGetProjectionMatrix`
  - `XRCameraFrame.displayMatrix` to `XRCameraFrame.TryGetDisplayMatrix`
  - `XRCameraFrame.hasDisplayMatrix` to `XRCameraFrame.TryGetDisplayMatrix`
  - `XRCameraFrame.averageIntensityInLumens` to `XRCameraFrame.TryGetAverageIntensityInLumens`
  - `XRCameraFrame.hasAverageIntensityInLumens` to `XRCameraFrame.TryGetAverageIntensityInLumens`
  - `XRCameraFrame.exposureDuration` to `XRCameraFrame.TryGetExposureDuration`
  - `XRCameraFrame.hasExposureDuration` to `XRCameraFrame.TryGetExposureDuration`
  - `XRCameraFrame.exposureOffset` to `XRCameraFrame.TryGetExposureOffset`
  - `XRCameraFrame.hasExposureOffset` to `XRCameraFrame.TryGetExposureOffset`
  - `XRCameraFrame.mainLightIntensityLumens` to `XRCameraFrame.TryGetMainLightIntensityLumens`
  - `XRCameraFrame.hasMainLightIntensityLumens` to `XRCameraFrame.TryGetMainLightIntensityLumens`
  - `XRCameraFrame.mainLightColor` to `XRCameraFrame.TryGetMainLightColor`
  - `XRCameraFrame.hasMainLightColor` to `XRCameraFrame.TryGetMainLightColor`
  - `XRCameraFrame.mainLightDirection` to `XRCameraFrame.TryGetMainLightDirection`
  - `XRCameraFrame.hasMainLightDirection` to `XRCameraFrame.TryGetMainLightDirection`
  - `XRCameraFrame.ambientSphericalHarmonics` to `XRCameraFrame.TryGetAmbientSphericalHarmonics`
  - `XRCameraFrame.hasAmbientSphericalHarmonics` to `XRCameraFrame.TryGetAmbientSphericalHarmonics`
  - `XRCameraFrame.cameraGrain` to `XRCameraFrame.TryGetCameraGrain`
  - `XRCameraFrame.hasCameraGrain` to `XRCameraFrame.TryGetCameraGrain`
  - `XRCameraFrame.noiseIntensity` to `XRCameraFrame.TryGetNoiseIntensity`
  - `XRCameraFrame.hasNoiseIntensity` to `XRCameraFrame.TryGetNoiseIntensity`

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

- Removed **Enable Navigation** setting from XR Simulation Preferences window. Navigation controls can now be disabled by clearing the **Navigation Input Action References** settings in the same window.

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
  