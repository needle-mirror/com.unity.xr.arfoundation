---
uid: arfoundation-whats-new
---
# What's new in version 6.0

## New features

### Bounding boxes

- Added an API for provider plug-ins to implement the detection and tracking of 3D bounding boxes. Refer to [Bounding box detection](xref:arfoundation-bounding-box-detection) for more information.

### Persistent anchors

- Added AR Foundation API definitions for persistent anchors. Provider plug-ins can implement these methods, which allow you to save anchors during an AR session and re-load them during subsequent sessions:
  - [ARAnchorManager.TrySaveAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TrySaveAnchorAsync(UnityEngine.XR.ARSubsystems.TrackableId,System.Threading.CancellationToken))
  - [ARAnchorManager.TryLoadAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryLoadAnchorAsync(UnityEngine.XR.ARSubsystems.SerializableGuid,System.Threading.CancellationToken))
  - [ARAnchorManager.TryEraseAnchorAsync](UnityEngine.XR.ARFoundation.ARAnchorManager.TryEraseAnchorAsync(UnityEngine.XR.ARSubsystems.SerializableGuid,System.Threading.CancellationToken))
  - [ARAnchorManager.TryGetSavedAnchorIdsAsync](UnityEngine.XR.ARFoundation.ARAnchorManager.TryGetSavedAnchorIdsAsync(Unity.Collections.Allocator,System.Threading.CancellationToken))

### Image Stabilization

- Added support for Image Stabilization, which helps stabilize shaky video from the camera. Refer to [AR Camera Manager component](xref:arfoundation-camera-components#ar-camera-manager-component) for more information.

### XR Simulation occlusion

- Added support for occlusion to XR Simulation, allowing you to test occlusion without deploying to device.

### XR Simulation light estimation

- Added support for light estimation in [SimulationCameraSubsystem](xref:UnityEngine.XR.Simulation.SimulationCameraSubsystem). Add the new [SimulatedLight](xref:UnityEngine.XR.ARFoundation.SimulatedLight) component to any lights in your simulation environment.

### Build AssetBundles window

- Added an editor window in **Assets** > **AR Foundation** > **Build AssetBundles** that you can use to build AssetBundles containing [XRReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.XRReferenceImageLibrary) objects.

### Asynchronous anchor creation

- Added [XRAnchorSubsystem.Provider.TryAddAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchorAsync*), backed by the new [Awaitable\<T\>](xref:UnityEngine.Awaitable`1) class introduced in Unity 2023.1. The default implementation will forward calls to the synchronous [TryAddAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.Provider.TryAddAnchor*) method and return its results.
- Added [ARAnchorManager.TryAddAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryAddAnchorAsync*) to asynchronously create `ARAnchor`s.
- Added [XRAnchorSubsystemDescriptor.supportsSynchronousAdd](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSynchronousAdd). If you are a provider of the `XRAnchorSubsystem` and you support synchronous anchor creation, you should set your descriptor's `supportsSynchronousAdd` value to `true`.

### Multiple classifications for planes

- Added support for planes to have multiple semantic labels via the [PlaneClassifications](xref:UnityEngine.XR.ARSubsystems.PlaneClassifications) flags enum, [ARPlane.classifications](xref:UnityEngine.XR.ARFoundation.ARPlane.classifications), and [BoundedPlane.classifications](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.classifications).

## Added

### Generic `trackablesChanged` event

- Added a generic [ARTrackableManager.trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged) event to replace all previous trackables-changed events (`ARPlaneManager.planesChanged`, etc). This simplifies the code needed to work across all managers.
  - This new event can be used in the Inspector.
  - This new event also fixes an issue where destroying an `ARAnchor` or `AREnvironmentProbe` component would not result in a respective `anchorsChanged` or `environmentProbesChanged` event invocation.
- Added a generic [ARTrackablesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1) to replace all previous trackables-changed event arguments structs.

### Platform-specific error codes and success codes

- Added [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus), a new way for AR Foundation to provide status information for completed operations. Provider plug-ins can add extension methods to this type to give users access to platform-specific error codes and success codes.

### More consistent image tracking in XR Simulation

- Added `SimulatedTrackedImage.imageAssetGuid` API to make it possible for XR Simulation to identify images even if **Keep Texture at Runtime** is disabled in the reference image library.

### Support for concave plane boundary meshes

- Added new API [ARPlaneMeshGenerator.TryGenerateMesh](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshGenerator.TryGenerateMesh) to support generating meshes of simple polygons, i.e. concave and convex polygons.

### Render graph

- Added support for the Universal Render Pipeline Render Graph introduced in URP 17.

### Other API design improvements

- Added the following subsystem descriptor registration methods for consistency with other descriptors:
  - [XRCameraSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.Register*)
  - [XREnvironmentProbeSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XREnvironmentProbeSubsystemDescriptor.Register*)
  - [XRHumanBodySubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRHumanBodySubsystemDescriptor.Register*)
  - [XROcclusionSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.Register*)
  - [XRObjectTrackingSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRObjectTrackingSubsystemDescriptor.Register*)
  - [XRParticipantSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRParticipantSubsystemDescriptor.Register*)
- Added [XRCameraSubsystem.GetShaderKeywords](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem.GetShaderKeywords) and [XROcclusionSubsystem.GetShaderKeywords](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.GetShaderKeywords). Both return a new read-only [ShaderKeywords](xref:UnityEngine.XR.ARSubsystems.ShaderKeywords) struct.
- Added `NotAxisAligned` Plane Detection Mode.
- Added a constructor to [SerializableGuid](xref:UnityEngine.XR.ARSubsystems.SerializableGuid) allowing the creation of `SerializableGuid`s with a `System.Guid`.

## Changed

### Minimum Unity Editor version

- Upgraded minimum Unity Editor version from 2021.2 to 6000.0. Refer to the official [Unity 6 New Naming Convention](https://forum.unity.com/threads/unity-6-new-naming-convention.1558592/) announcement for more information.

### XR Simulation image tracking workflow

- Changed the `SimulatedTrackedImage` component to render a textured mesh of its image, allowing you to see the image in the Scene view and Game view without requiring additional GameObjects.
- Removed now-unnecessary Quad GameObjects from the DefaultSimulationEnvironment.

### Navigation Input Actions in XR Simulation

- Changed XR Simulation navigation controls to be bound to configurable Input Actions instead of hard-coded to WASD keys. You can configure these Input Actions in the XR Simulation Preferences window.

### ARAnchor life cycle

- Changed the life cycle behavior of [ARAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchor) in the event that adding the anchor to the `XRAnchorSubsystem` failed. Instead of retrying the add operation every frame, the `ARAnchor` component now disables itself after the first failed attempt.

### Other API design improvements

- Changed `ARTrackable` to now implement the `ITrackable` interface, enabling generic API designs when dealing with trackables.
- Changed `Promise<T>.OnKeepWaiting()` to `virtual` instead of `abstract`.
- Changed `XRPlaneSubsystem.Provider.requestedPlaneDetectionMode` from `virtual` to `abstract`, as `ARPlaneManager` requires a concrete implementation.
- Changed `SubsystemLifecycleManager.GetActiveSubsystemInstance()` from `protected` to `protected static`, as it does not use any instance members of `SubsystemLifecycleManager`.
- Changed the behavior of `SimulationSessionSubsystem.sessionId` to now return a non-empty, unique value when the subsystem is running. You can access the session id using `XRSessionSubsystem.sessionId`.
- Changed `SimulationPlaneSubsystem` to respect the currently set `PlaneDetectionMode`, detecting updates only from planes that match the current mode.
- Changed the behavior of `ARMeshManager` to recalculate normals on a mesh if normals were requested and the provider did not calculate them.

## Deprecated

All deprecated APIs are replaced by new API additions.

- Deprecated the following APIs, all of which are replaced by [ARTrackableManager.trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged):
  - [ARAnchorManager.anchorsChanged](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.anchorsChanged)
  - [ARAnchorManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.OnTrackablesChanged)
  - [AREnvironmentProbeManager.environmentProbesChanged](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbeManager.environmentProbesChanged)
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
- Deprecated the following structs, all of which are replaced by [ARTrackablesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs):
  - [ARAnchorsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARAnchorsChangedEventArgs)
  - [AREnvironmentProbesChangedEvent](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbesChangedEvent)
  - [ARFacesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARFacesChangedEventArgs)
  - [ARHumanBodiesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARHumanBodiesChangedEventArgs)
  - [ARParticipantsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARParticipantsChangedEventArgs)
  - [ARPlanesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARPlanesChangedEventArgs)
  - [ARPointCloudChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARPointCloudChangedEventArgs)
  - [ARTrackedImagesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackedImagesChangedEventArgs)
  - [ARTrackedObjectsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectsChangedEventArgs)
- Deprecated the following Visual Scripting nodes, and replaced them with new updated versions. To upgrade a graph containing a deprecated node, delete the node from your graph and replace it with the new version of the node.
  - [On Anchors Changed](xref:arfoundation-vs-node-on-anchors-changed)
  - [On Environment Probes Changed](xref:arfoundation-vs-node-on-environment-probes-changed)
  - [On Faces Changed](xref:arfoundation-vs-node-on-faces-changed)
  - [On Human Bodies Changed](xref:arfoundation-vs-node-on-human-bodies-changed)
  - [On Participants Changed](xref:arfoundation-vs-node-on-participants-changed)
  - [On Planes Changed](xref:arfoundation-vs-node-on-planes-changed)
  - [On Point Clouds Changed](xref:arfoundation-vs-node-on-point-clouds-changed)
  - [On Tracked Images Changed](xref:arfoundation-vs-node-on-tracked-images-changed)
  - [On Tracked Objects Changed](xref:arfoundation-vs-node-on-tracked-objects-changed)
- Deprecated and replaced the following APIs:
  - [PlaneClassification](xref:UnityEngine.XR.ARSubsystems.PlaneClassification) to [PlaneClassifications](xref:UnityEngine.XR.ARSubsystems.PlaneClassifications)
  - [ARPlane.classification](xref:UnityEngine.XR.ARFoundation.ARPlane.classification) to [ARPlane.classifications](xref:UnityEngine.XR.ARFoundation.ARPlane.classifications)
  - [BoundedPlane.classification](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.classification) to [BoundedPlane.classifications](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.classifications)
  - [BoundedPlane constructor](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.%23ctor(UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.Pose,UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.XR.ARSubsystems.PlaneAlignment,UnityEngine.XR.ARSubsystems.TrackingState,System.IntPtr,UnityEngine.XR.ARSubsystems.PlaneClassification)) to new constructor overload.
- Deprecated and replaced the following APIs for consistency with other subsystems:
  - `XRCameraSubsystemCinfo`  to `XRCameraSubsystemDescriptor.Cinfo`
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
- Deprecated the structs `XRObjectTrackingSubsystemDescriptor.Capabilities` and `XRParticipantSubsystemDescriptor.Capabilities` as they had no functionality.
- Deprecated `XRObjectTrackingSubsystemDescriptor.Register` and `XRParticipantSubsystemDescriptor.Register` overloads that use the now-deprecated `Capabilities` structs, and replaced them with new overloaded versions.
- Deprecated and replaced the following APIs:
  - `XRCameraSubsystem.GetMaterialKeywords` to `XRCameraSubsystem.GetShaderKeywords`
  - `XROcclusionSubsystem.GetMaterialKeywords` to `XROcclusionSubsystem.GetShaderKeywords`
  - `ARCameraFrameEventArgs.enabledMaterialKeywords` to `ARCameraFrameEventArgs.enabledShaderKeywords`
  - `ARCameraFrameEventArgs.disabledMaterialKeywords` to `ARCameraFrameEventArgs.disabledShaderKeywords`
  - `AROcclusionFrameEventArgs.enabledMaterialKeywords` to `AROcclusionFrameEventArgs.enabledShaderKeywords`
  - `AROcclusionFrameEventArgs.disabledMaterialKeywords` to `AROcclusionFrameEventArgs.disabledShaderKeywords`
- Deprecated and replaced the following APIs for consistency:
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
- Deprecated and replaced [ARPlaneMeshGenerators](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshGenerators) with `ARPlaneMeshGenerator`.
  - [ARPlaneMeshGenerators.GenerateMesh](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshGenerators.GenerateMesh)
  - [ARPlaneMeshGenerators.GenerateUvs](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshGenerators.GenerateUvs)
  - [ARPlaneMeshGenerators.GenerateIndices](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshGenerators.GenerateIndices)

## Removed

- Removed the following APIs which were previously deprecated. Refer to the recommended upgrade guidance if your code used any of these APIs.

| Removed API | Upgrade recommendation |
| :---------- | :--------------------- |
| `ARAnchorManager.AddAnchor` | Add an anchor using `AddComponent<ARAnchor>`  or `ARAnchorManager.TryAddAnchorAsync` |
| `ARAnchorManager.RemoveAnchor` | Call `Destroy()` on the `ARAnchor` component to remove it. |
| `ARCameraManager.focusMode` | Use `autoFocusEnabled` or `autoFocusRequested` instead. |
| `ARCamerManager.lightEstimationMode` | Use `currentLightEstimation` or `requestedLightEstimation` instead. |
| `ARCamerManager.TryGetLatestImage` | Use `TryAcquireLatestCpuImage` instead. |
| `AREnvironmentProbeManager.automaticPlacement` | Use `automaticPlacementRequested` or `automaticPlacementEnabled`instead. |
| `AREnvironmentProbeManager.environmentTextureHDR` | Use `environmentTextureHDRRequested` or `environmentTextureHDREnabled` instead. |
| `AREnvironmentProbeManager.AddEnvironmentProbe` | Add an environment probe using `AddComponent<AREnvironmentProbe>`. |
| `AREnvironmentProbeManager.RemoveEnvironmentProbe` | Call `AREnvironmentProbe.Destroy()` to remove it. |
| `ARFaceManager.maximumFaceCount` | Use `requestedMaximumFaceCount` or `currentMaximumFaceCount` instead. |
| `ARHumanBodyManager.humanBodyPose2DEstimationEnabled` | Use `pose2DEnabled` or `pose2DRequested` instead. |
| `ARHumanBodyManager.humanBodyPose3DEstimationEnabled` | Use `pose3DEnabled` or `pose3DRequested` instead. |
| `ARHumanBodyManager.humanBodyPose3DScaleEstimationEnabled` | Use `pose3DScaleEstimationRequested` or `pose3DScaleEstimationRequested` instead. |
| `AROcclusionManager.humanSegmentationStencilMode` | Use `requestedSegmentationStencilMode` or `currentSegmentationStencilMode` instead. |
| `AROcclusionManager.humanSegmentationDepthMode` | Use `requestedSegmentationDepthMode` or `currentSegmentationDepthMode` instead. |
| `ARPlaneManager.detectionMode` | Use `requestedDetectionMode` or `currentDetectionMode` instead. |
| `ARSession.matchFrameRate` | Use `matchFrameRateRequested` or `matchFrameRateEnabled` instead. |
| `ARTrackableManager.sessionOrigin` | Use `origin` instead. |
| `ARTrackedImageManager.maxNumberOfMovingImages` | Use `requestedMaxNumberOfMovingImages` or `currentMaxNumberOfMovingImages` instead. |
| `MutableRuntimeReferenceImageLibrary.ScheduleAddImageJob` | Use `ScheduleAddImageWithValidationJob` instead. |
| `XRAnchorSubsystemDescriptor.Cinfo.subsystemImplementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XRCameraSubsystem.TryGetLatestImage` | Use `TryAcquireLatestCpuImage` instead. |
| `XRCameraSubsystemCinfo.implementationType`  | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XREnvironmentProbeSubsystemCinfo.implmentationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `FaceSubsystemParams.subsystemImplementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XRHumanBodySubsystemCinfo.implementationType` | Use `providerType` and, optionally, `subsystemTypeOverride instead`. |
| `XRImageTrackingSubsystem.Cinfo.subsystemImplementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XROcclusionSubsystemCinfo.implementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XROcclusionSubsystemCinfo.supportsHumanSegmentationStencilImage` | Use `humanSegmentationStencilImageSupportedDelegate` instead. |
| `XROcclusionSubsystemCinfo.supportsHumanSegmentationDepthImage` | Use `humanSegmentationDepthImageSupportedDelegate` instead. |
| `XROcclusionSubsystemCinfo.queryForSupportsEnvironmentDepthImage` | Use `environmentDepthImageSupportedDelegate` instead. |
| `XROcclusionSubsystemCinfo.queryForSupportsEnvironmentDepthConfidenceImage` | Use `environmentDepthConfidenceImageSupportedDelegate` instead. |
| `XRPlaneSubsystemDescriptor.Cinfo.subsystemImplementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XRPointCloudSubsystemDescriptor.Cinfo.implementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XRRaycastSubsystemDescriptor.Cinfo.subsystemImplementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XRReferenceObjectLibrary.indexOf` | Use `IndexOf` instead. |
| `XRSessionSubsystem.subsystemImplementationType` | Use `providerType` and, optionally, `subsystemTypeOverride` instead. |
| `XRSubsystem` | Use `UnityEngine.SubsystemsImplementation.SubsystemWithProvider` instead. |

- Removed the image file `/Editor/Icons/ARVR@4x.png` as it was unused.
- Removed the `Description` attribute from values of several enums, as the attribute was unused:
  - `XRCameraFrameProperties`
  - `AREnvironmentProbePlacementType`
  - `CameraFocusMode`
  - `LightEstimationMode`
  - `EnvironmentDepthMode`
  - `HumanSegmentationDepthMode`
  - `HumanSegmentationStencilMode`
  - `OcclusionPreferenceMode`

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
