---
uid: arfoundation-changelog
---
# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [6.0.3] - 2024-08-16

### Changed

- Updated documentation to reflect that the Apple visionOS XR Plug-in now supports AR Foundation 6.

### Fixed

- Fixed the `SimulationRaycastSubsystem` to correctly state via `XRRaycastSubsystemDescriptor.supportedTrackableTypes` that casting rays against planes is supported in XR Simulation.
- Fixed the `SimulationSessionSubsystem` so it doesn't log an error if you call `ARSession.Reset` before the session is initialized.

## [6.0.2] - 2024-05-22

### Fixed

- Fixed an issue where the Build AssetBundles window (**Assets** > **AR Foundation** > **Build AssetBundles**) threw a `NullReferenceException` if no AssetBundles were built.
- Fixed an issue where XR Simulation's `CameraTextureProvider` component could throw a `MissingReferenceException` when an AR scene is unloaded.
- Fixed XR Simulation components so they now behave correctly if you disable the **Reload Domain** option in [Configurable Enter Play Mode](https://docs.unity3d.com/Manual/ConfigurableEnterPlayMode.html) settings.
- Fixed the native XR Simulation Input provider so that it now explicitly sets **Device** mode as its only supported [Tracking Origin Mode](xref:Unity.XR.CoreUtils.XROrigin.TrackingOriginMode). The [XR Origin component](xref:xr-core-utils-xr-origin-reference) will always use **Device** mode as its Tracking Origin Mode and apply the [XROrigin.CameraYOffset](xref:Unity.XR.CoreUtils.XROrigin.CameraYOffset) value.

## [6.0.1] - 2024-04-01

### Added

- Added AR Foundation API definitions for persistent anchors. Provider plug-ins can implement these methods, which allow you to save anchors during an AR session and re-load them during subsequent sessions:
  - [ARAnchorManager.TrySaveAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TrySaveAnchorAsync*)
  - [ARAnchorManager.TryLoadAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryLoadAnchorAsync*)
  - [ARAnchorManager.TryEraseAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryEraseAnchorAsync*)
  - [ARAnchorManager.TryGetSavedAnchorIdsAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryGetSavedAnchorIdsAsync*)
- Added an implicit conversion operator between [TrackableId](xref:UnityEngine.XR.ARSubsystems.TrackableId) and [SerializableGuid](xref:UnityEngine.XR.ARSubsystems.SerializableGuid).
- Added a constructor to [SerializableGuid](xref:UnityEngine.XR.ARSubsystems.SerializableGuid) allowing the creation of `SerializableGuid`s with a `System.Guid`.
- Added [ARAnchorManager.TryRemoveAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryRemoveAnchor*) API for removing anchors. You can either use this new method or disable the `ARAnchor` component to remove an anchor.
- Added XR Simulation's `SimulationCameraPoseProvider` as a public component.

### Changed

- Changed documentation to indicate that Meshing is now a supported feature in the Unity OpenXR: Meta provider plug-in.
- Changed the behavior of `ARMeshManager` to recalculate normals on a mesh if normals were requested and the provider did not calculate them.
- Changed the minimum Unity version from 2023.3 to 6000.0, reflecting the new version number of Unity 6. Refer to the official [Unity 6 New Naming Convention](https://forum.unity.com/threads/unity-6-new-naming-convention.1558592/) announcement for more information.

### Fixed

- Fixed [issue ARFB-421](https://issuetracker.unity3d.com/product/unity/issues/guid/ARFB-421) which caused `ARMeshManager` to crash the Editor in XR Simulation when its **Normals** property was not enabled.
- Fixed [issue ARFB-380](https://issuetracker.unity3d.com/issues/android-elements-of-ui-toolkit-arent-displayed-when-ar-foundation-camera-facing-direction-is-set-to-user) where UI toolkit elements disappeared after one frame when running an XR scene with a user-facing camera on Android devices.
- Fixed an issue where disabling the `AROcclusionManager` component in XR Simulation would not stop the simulation background shader from sampling the depth texture.
- Fixed an issue where XR Simulation camera pose data could leak between multiple AR sessions.
- Corrected a previous fix for an issue where the tvOS build target would have compile errors in `Simulation.InputLayoutLoader`.

## [6.0.0-pre.8] - 2024-03-19

### Changed

- Rebuilt simulation plugin for linux on Ubuntu 20.04 (glibc-2.31).

### Fixed

- Fixed an issue where the AR Debug Menu script would not spawn a Point Cloud prefab.

## [6.0.0-pre.7] - 2024-03-11

### Added

- Added an API for provider plug-ins to implement the detection and tracking of 3D bounding boxes. Refer to [Bounding box detection](xref:arfoundation-bounding-box-detection) for more information.
- Added support for the Universal Render Pipeline Render Graph introduced in URP 17.

### Changed

- Changed [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus) to more easily allow for a wide range of platforms to use the type:
  - Renamed the `value` property to `nativeStatusCode`.
  - Added a cross-platform `statusCode` property and corresponding constructors.
  - Removed the implicit conversion operators between `int` and `XRResultStatus`. Use a constructor instead.

### Fixed

- Fixed an issue where destroying and recreating the `SimulationSessionSubsystem` could result in the creation of a `SimulationMeshSubsystem` instance, even if the `XRLoader` did not initially create it.
- Fixed an issue where the tvOS build target would have compile errors in `Simulation.InputLayoutLoader`.

## [6.0.0-pre.6] - 2024-02-08

### Added

- Added [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus), a new way for AR Foundation to provide status information for completed operations. Provider plug-ins can add extension methods to this type to give users access to platform-specific error codes and success codes.

### Changed

- Changed `SimulationPlaneSubsystem` to respect the currently set `PlaneDetectionMode`, detecting updates only from planes that match the current mode.
- Upgraded `com.unity.xr.core-utils` dependency version from `2.2.3` to `2.3.0`.
- Changed [ARTrackablesChangedEventArgs\<TTrackable\>](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1):
  - `added`, `updated`, and `removed` now return [ReadOnlyList](xref:Unity.XR.CoreUtils.Collections.ReadOnlyList`1) instead of `IReadOnlyCollection`, removing an unnecessary heap allocation when each collection is enumerated.
  - `removed` now uses `KeyValuePair<TrackableId, TTrackable>` instead of `TTrackable`, allowing `ARTrackableManager` to send notifications of removed trackables even if you already destroyed them (for example, by destroying an `ARAnchor`).
- Changed [Result\<T\>](xref:UnityEngine.XR.ARSubsystems.Result`1):
  - Removed `bool TryGetResult(out T)` and the constructor that took `(bool, T)` as parameters.
  - Added `status`, `value`, and a constructor that takes `(XRResultStatus, T)` as parameters.
- Updated Universal Render Pipeline documentation to reflect recent changes to URP concepts and setup.
- Changed minimum Unity Editor version to `2023.3`.

### Fixed

- Fixed an issue where calling `ARSession.Reset` could cause XR Simulation plane detection to fail to discover planes for a period of time before resuming.
- Fixed an issue where the `SimulationEnvironmentProbeSubsystem` could destroy environment probe GameObjects when they were removed, even if their **Destroy on Removal** property was set to false.
- Fixed an issue where `EditorAnalytics` would throw obsolete warnings during project import in Unity 2023.2 or newer.
- Fixed an issue in XR Simulation where a command buffer for the Built-in Render Pipeline was added to a camera even when URP was active.

## [6.0.0-pre.5] - 2023-12-14

### Added

- Added new API to make it possible for XR Simulation to identify images even if **Keep Texture at Runtime** is disabled in the reference image library:
  - Added [SimulatedTrackedImage.imageAssetGuid](xref:UnityEngine.XR.Simulation.SimulatedTrackedImage.imageAssetGuid)
- Added an editor window in **Assets** > **AR Foundation** > **Build AssetBundles** that you can use to build AssetBundles containing [XRReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.XRReferenceImageLibrary) objects.
- Added new versions of these Visual Scripting nodes, based on the new `ARTrackableManager.trackablesChanged` event:
  - `OnAnchorsChanged`
  - `OnEnvironmentProbesChanged`
  - `OnFacesChanged`
  - `OnHumanBodiesChanged`
  - `OnParticipantsChanged`
  - `OnPlanesChanged`
  - `OnPointCloudsChanged`
  - `OnTrackedImagesChanged`
  - `OnTrackedObjectsChanged`
- Added new API [ARPlaneMeshGenerator.TryGenerateMesh](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshGenerator.TryGenerateMesh*) to support generating meshes of simple polygons, i.e. concave and convex polygons.
- Added documentation:
  - Added [Display matrix format and derivation](xref:arfoundation-display-matrix-format-and-derivation) manual page.
  - Added [Custom background shaders](xref:arfoundation-custom-background-shaders) manual page.
- Added [SimulatedTrackedImage](xref:arfoundation-simulation-environments#simulated-tracked-image-component) as a public class. This class has been present since AR Foundation 5.0, but previously was not public.
- Added settings in the XR Simulation Preferences window for configuring navigation `InputAction`s and navigation speed.
- Added `NotAxisAligned` [PlaneDetectionMode](xref:UnityEngine.XR.ARSubsystems.PlaneDetectionMode).

### Changed

- Changed the [Use reference image libraries with AssetBundles](xref:arfoundation-image-tracking#use-reference-image-libraries-with-assetbundles) section of the Image tracking documentation to mention that `ARBuildProcessor.PreprocessBuild` must be called before building AssetBundles.
- Changed the location of the **Refresh XR Environment List** menu item from **Assets** to **Assets** > **AR Foundation**.
- Changed the materials "Debug Face" and "Debug Plane" to be compatible with URP by changing them to the 'Simulation/StandardLit' shader instead of the unity standard shader.
- Changed the XR Simulation Environments version imported by the XR Environment Overlay from 1.0.0 to 2.0.1.
- Changed the [Plane detection](xref:arfoundation-plane-detection) documentation to contain more information and better organization.
- Changed the `SimulatedLight` component to disallow multiple copies of the component on the same GameObject, which would result in incorrect light estimation.
- Changed XR Simulation navigation controls to be bound to configurable `InputAction`s instead of hard-coded to WASD keys.

### Deprecated

- Deprecated [ARPlaneMeshGenerators](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshGenerators).

### Removed

- Removed semantic labels from [PlaneClassifications](xref:UnityEngine.XR.ARSubsystems.PlaneClassifications) that describe 3D volumes.
- Removed the "Version History" documentation section, which contained upgrade guides for previous versions of AR Foundation. To access upgrade guides for previous AR Foundation versions, refer to the respective documentation versions.
- Removed **Enable Navigation** setting from XR Simulation Preferences window. Navigation controls can now be disabled by clearing the **Navigation Input Action References** settings in the same window.

### Fixed

- Fixed an issue in XR Simulation where a warning was logged stating that saving had no effect because of a missing FilePathAttribute.
- Fixed an issue in XR Simulation where if the AR Occlusion Manager component was disabled at the start of a scene, occlusion would not work correctly if you later enabled the component.
- Fixed an issue where the [ARTrackableManager.trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged) event was initialized to `null`.
- Fixed an issue where `SimulatedAnchor` components could incorrectly affect XR Simulation when used in scenes other than the XR simulation environment.
- Fixed an issue where disabling and re-enabling the [ARDebugMenu](xref:arfoundation-debug-menu) component would cause the origin toggle to become unresponsive.

## [6.0.0-pre.4] - 2023-10-18

### Added

- Added `UnityEvent` [trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged) to `ARTrackableManager`.
- Added struct [ARTrackablesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1) for the `trackablesChanged` event argument.
- Added support for lighting estimation in [SimulationCameraSubsystem](xref:UnityEngine.XR.Simulation.SimulationCameraSubsystem).
- Added 'InvisibleWallFace' semantic label to [PlaneClassifications](xref:UnityEngine.XR.ARSubsystems.PlaneClassifications).

### Changed

- Changed the [SimulatedTrackedImage](xref:UnityEngine.XR.Simulation.SimulatedTrackedImage) component to render a textured mesh of its image, allowing you to see the image in the Scene view and Game view without requiring additional GameObjects.
  - Removed now-unnecessary Quad GameObjects from the DefaultSimulationEnvironment.
- Changed the behavior of `SimulationSessionSubsystem.sessionId` to now return a non-empty unique value when the subsystem is running.

### Deprecated

- Deprecated and replaced the following APIs:
  - `XRCameraSubsystem.GetMaterialKeywords` to `XRCameraSubsystem.GetShaderKeywords`
  - `XROcclusionSubsystem.GetMaterialKeywords` to `XROcclusionSubsystem.GetShaderKeywords`
  - `ARCameraFrameEventArgs.enabledMaterialKeywords` to `ARCameraFrameEventArgs.enabledShaderKeywords`
  - `ARCameraFrameEventArgs.disabledMaterialKeywords` to `ARCameraFrameEventArgs.disabledShaderKeywords`
  - `AROcclusionFrameEventArgs.enabledMaterialKeywords` to `AROcclusionFrameEventArgs.enabledShaderKeywords`
  - `AROcclusionFrameEventArgs.disabledMaterialKeywords` to `AROcclusionFrameEventArgs.disabledShaderKeywords`
- Deprecated the following APIs:
  - [ARAnchorManager.anchorsChanged](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.anchorsChanged)
  - [ARAnchorManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.OnTrackablesChanged*)
  - [AREnvironmentProbeManager.environmenProbesChanged](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbeManager.environmentProbesChanged)
  - [AREnvironmentProbeManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbeManager.OnTrackablesChanged*)
  - [ARFaceManager.facesChanged](xref:UnityEngine.XR.ARFoundation.ARFaceManager.facesChanged)
  - [ARFaceManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARFaceManager.OnTrackablesChanged*)
  - [ARHumanBodyManager.humanBodiesChanged](xref:UnityEngine.XR.ARFoundation.ARHumanBodyManager.humanBodiesChanged)
  - [ARHumanBodyManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARHumanBodyManager.OnTrackablesChanged*)
  - [ARParticipantManager.participantsChanged](xref:UnityEngine.XR.ARFoundation.ARParticipantManager.participantsChanged)
  - [ARParticipantManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARParticipantManager.OnTrackablesChanged*)
  - [ARPlaneManager.planesChanged](xref:UnityEngine.XR.ARFoundation.ARPlaneManager.planesChanged)
  - [ARPlaneManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARPlaneManager.OnTrackablesChanged*)
  - [ARPointCloudManager.pointCloudsChanged](xref:UnityEngine.XR.ARFoundation.ARPointCloudManager.pointCloudsChanged)
  - [ARPointCloudManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARPointCloudManager.OnTrackablesChanged*)
  - [ARTrackedImageManager.trackedImagesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.trackedImagesChanged)
  - [ARTrackedImageManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.OnTrackablesChanged*)
  - [ARTrackedObjectManager.trackedObjectsChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectManager.trackedObjectsChanged)
  - [ARTrackedObjectManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectManager.OnTrackablesChanged*)
  - [ARTrackableManager.OnTrackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.OnTrackablesChanged*)
- Deprecated the following structs:
  - [ARAnchorsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARAnchorsChangedEventArgs)
  - [AREnvironmentProbesChangedEvent](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbesChangedEvent)
  - [ARFacesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARFacesChangedEventArgs)
  - [ARHumanBodiesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARHumanBodiesChangedEventArgs)
  - [ARParticipantsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARParticipantsChangedEventArgs)
  - [ARPlanesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARPlanesChangedEventArgs)
  - [ARPointCloudChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARPointCloudChangedEventArgs)
  - [ARTrackedImagesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackedImagesChangedEventArgs)
  - [ARTrackedObjectsChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectsChangedEventArgs)
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

### Removed

- Removed an assembly reference from `Unity.XR.ARSubsystems.Editor.asmdef` to an internal assembly that does not exist anymore.

### Fixed

- Fixed an issue where disabling and re-enabling the [ARDebugMenu](xref:arfoundation-debug-menu) component would create duplicate trackable visualizers and cause UI buttons to become unresponsive.

## [6.0.0-pre.3] - 2023-09-11

### Added

- Added [XRObjectTrackingSubsystemDescriptor.Register(XRObjectTrackingSubsystemDescriptor.Cinfo)](xref:UnityEngine.XR.ARSubsystems.XRObjectTrackingSubsystemDescriptor.Register(UnityEngine.XR.ARSubsystems.XRObjectTrackingSubsystemDescriptor.Cinfo)) to replace the deprecated register methods present in `XRObjectTrackingSubsystem`
- Added [XRParticipantSubsystemDescriptor.Register(XRParticipantSubsystemDescriptor.Cinfo)](UnityEngine.XR.ARSubsystems.XRParticipantSubsystemDescriptor.Register(UnityEngine.XR.ARSubsystems.XRParticipantSubsystemDescriptor.Cinfo)) to replace the deprecated register methods present in `XRParticipantSubsystem`
- Added support for asynchronous anchor creation. Refer to [What's New](xref:arfoundation-whats-new#asynchronous-tryaddanchor-api) for more information.
- Added support for Image Stabilization, which helps stabilize shaky video from the camera.
- Added support for Occlusion to XR Simulation.
- Added support for planes to have multiple semantic labels via the [PlaneClassifications](xref:UnityEngine.XR.ARSubsystems.PlaneClassifications) flags enum, [ARPlane.classifications](xref:UnityEngine.XR.ARFoundation.ARPlane.classifications), and [BoundedPlane.classifications](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.classifications).
- Added the following subsystem descriptor registration methods for consistency with other subsystem descriptors:
  - [XRCameraSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.Register*)
  - [XREnvironmentProbeSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XREnvironmentProbeSubsystemDescriptor.Register*)
  - [XRHumanBodySubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XRHumanBodySubsystemDescriptor.Register*)
  - [XROcclusionSubsystemDescriptor.Register](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.Register*)
- Added [XRCameraSubsystem.GetShaderKeywords](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem.GetShaderKeywords) and [XROcclusionSubsystem.GetShaderKeywords](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.GetShaderKeywords). Both return a new read-only [ShaderKeywords](xref:UnityEngine.XR.ARSubsystems.ShaderKeywords) struct.

### Changed

- Changed [ScheduleAddImageWithValidationJob](xref:UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageWithValidationJob*) to check if the `ARSession.state` has reached the ready state before scheduling the add image operation.
- Changed `ARTrackable` to now implement the `ITrackable` interface, enabling generic API designs when dealing with trackables.
- Changed `XRPlaneSubsystem.Provider.requestedPlaneDetectionMode` to `abstract` from `virtual`.
- Changed `Promise<T>.OnKeepWaiting()` to `virtual` instead of `abstract`.
- Changed `SubsystemLifecycleManager.GetActiveSubsystemInstance()` to `protected static` from `protected`, as it does not use any instance members of the `SubsystemLifecycleManager` class.
- Changed the life cycle behavior of [ARAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchor) in the event that adding the anchor to the `XRAnchorSubsystem` failed. Instead of retrying the add operation every frame, the `ARAnchor` component now disables itself after the first failed attempt.

### Deprecated

- Deprecated the following APIs:
  - [PlaneClassification](xref:UnityEngine.XR.ARSubsystems.PlaneClassification)
  - [ARPlane.classification](xref:UnityEngine.XR.ARFoundation.ARPlane.classification)
  - [BoundedPlane constructor](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.#ctor(UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.Pose,UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.XR.ARSubsystems.PlaneAlignment,UnityEngine.XR.ARSubsystems.TrackingState,IntPtr,UnityEngine.XR.ARSubsystems.PlaneClassification))
  - [BoundedPlane.classification](xref:UnityEngine.XR.ARSubsystems.BoundedPlane.classification)
- Deprecated the structs `XRObjectTrackingSubsystemDescriptor.Capabilities` and `XRParticipantSubsystemDescriptor.Capabilities`.
- Deprecated the `XRObjectTrackingSubsystemDescriptor.Register` and `XRParticipantSubsystemDescriptor.Register` methods that use the now-deprecated `Capabilities` struct.
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

### Removed

- Removed the `Description` attribute from values of several enums, as the attribute was unused:
  - `XRCameraFrameProperties`
  - `AREnvironmentProbePlacementType`
  - `CameraFocusMode`
  - `LightEstimationMode`
  - `EnvironmentDepthMode`
  - `HumanSegmentationDepthMode`
  - `HumanSegmentationStencilMode`
  - `OcclusionPreferenceMode`
- Removed the following deprecated APIs:
  - `ARAnchorManager.AddAnchor` method
  - `ARAnchorManager.RemoveAnchor` method
  - `ARCamerManager.focusMode` property
  - `ARCamerManager.lightEstimationMode` property
  - `ARCamerManager.TryGetLatestImage` method
  - `AREnvironmentProbeManager.automaticPlacement` property
  - `AREnvironmentProbeManager.environmentTextureHDR` property
  - `AREnvironmentProbeManager.AddEnvironmentProbe` method
  - `AREnvironmentProbeManager.RemoveEnvironmentProbe` method
  - `ARFaceManager.maximumFaceCount` property
  - `ARHumanBodyManager.humanBodyPose2DEstimationEnabled` property
  - `ARHumanBodyManager.humanBodyPose3DEstimationEnabled` property
  - `ARHumanBodyManager.humanBodyPose3DScaleEstimationEnabled` property
  - `AROcclusionManager.humanSegmentationStencilMode` property
  - `AROcclusionManager.humanSegmentationDepthMode` property
  - `ARPlaneManager.detectionMode` property
  - `ARSession.matchFrameRate` property
  - `ARTrackableManager.sessionOrigin` property
  - `ARTrackedImageManager.maxNumberOfMovingImages` property
  - `MutableRuntimeReferenceImageLibrary.ScheduleAddImageJob` method
  - `XRAnchorSubsystemDescriptor.Cinfo.subsystemImplementationType` property
  - `XRCameraSubsystem.TryGetLatestImage` method
  - `XRCameraSubsystemCinfo.implementationType` property
  - `XREnvironmentProbeSubsystemCinfo.implmentationType` property
  - `FaceSubsystemParams.subsystemImplementationType` property
  - `XRHumanBodySubsystemCinfo.implementationType` property
  - `XRImageTrackingSubsystem.Cinfo.subsystemImplementationType` property
  - `XROcclusionSubsystemCinfo.implementationType` property
  - `XROcclusionSubsystemCinfo.supportsHumanSegmentationStencilImage` property
  - `XROcclusionSubsystemCinfo.supportsHumanSegmentationDepthImage` property
  - `XROcclusionSubsystemCinfo.queryForSupportsEnvironmentDepthImage` property
  - `XROcclusionSubsystemCinfo.queryForSupportsEnvironmentDepthConfidenceImage` property
  - `XROcclusionSubsystemDescriptor.supportsHumanSegmentationStencilImage` property
  - `XROcclusionSubsystemDescriptor.supportsHumanSegmentationDepthImage` property
  - `XROcclusionSubsystemDescriptor.supportsEnvironmentDepthImage` property
  - `XROcclusionSubsystemDescriptor.supportsEnvironmentDepthConfidenceImage` property
  - `XRPlaneSubsystemDescriptor.Cinfo.subsystemImplementationType` property
  - `XRPointCloudSubsystemDescriptor.Cinfo.implementationType` property
  - `XRRaycastSubsystemDescriptor.Cinfo.subsystemImplementationType` property
  - `XRReferenceObjectLibrary.indexOf` method
  - `XRSessionSubsystem.subsystemImplementationType` property
  - `XRSubsystem` class
- Removed the image file `/Editor/Icons/ARVR@4x.png` as it was unused.

### Fixed

- Fixed [ScheduleAddImageJob](xref:UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageJob*) and [ScheduleAddImageWithValidationJob](xref:UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageWithValidationJob*) documentation to describe the correct exceptions thrown if `library` is null.
- Fixed an issue in XR Simulation where disabling and re-enabling the AR Plane Manager component would cause duplicate planes to be detected where planes had already been detected previously.

## [5.1.0-pre.10] - 2023-07-21

### Changed

- Entering XR Simulation without an `XR Origin` in the scene now logs an error to the console rather than throwing an exception.

### Fixed

- Fixed an issue where `SimulationAnchorImpl` would incorrectly fail to start and log a warning when using the `ARAnchorManager` in XR Simulation.
- Fixed issue [ARFB-307](https://issuetracker.unity3d.com/issues/ios-arcamerabackground-retains-depth-texture-when-switching-to-a-scene-with-no-occlusion) where occlusion was persisting after the `AROcclusionManager` was disabled.
- Fixed an issue in `SimulationEditorUtilities` where uninstalling an XR package while its loader is active causes `NullReferenceException` in the Unity Editor.
- Fixed an issue where `SimulationEnvironmentAssetsManager` could cause errors on fresh project import by trying to create a new settings assets when the project already has one.
- Fixed an issue where `SimulationEditorUtilities` could result in an `NullReferenceException` when checking whether simulation loader is enabled or not.

## [5.1.0-pre.9] - 2023-07-06

### Fixed

- Fixed an issue where shader compilation for the `SimulationURPRoomXRay` and `SimulationURPSynthetic` shaders could fail if Universal RP is not installed.

## [5.1.0-pre.8] - 2023-06-09

### Fixed

- Fixed issue where `SimulationPlaneSubsystem` could throw a `NullReferenceException` when `XRPlaneSubsystem.GetChanges` is called.
- Fixed issue [ARFB-271](https://issuetracker.unity3d.com/issues/ar-simulation-error-on-arsession-dot-reset-simulationprovider-dot-createvoxelgrids) where `SimulationPlaneSubsystem` could throw a `NullReferenceException` when `ArSession.Reset` is called.

- Fixed an issue in `SimulationMeshingProvider` where mesh data updates were not properly marked as no longer dirty. This caused the subsystem to unnecessarily update meshes every frame.

## [5.1.0-pre.6] - 2023-05-23

### Added

- Added a set of custom visual scripting nodes designed to help you use AR Foundation in visual scripting projects. Refer to [Visual Scripting](xref:arfoundation-visual-scripting) to learn more about how to set up an AR Foundation visual scripting project.
- Added support for the [Anchors](xref:arfoundation-anchors) feature in XR Simulation. You can now add and remove anchors in Play mode, add a [Simulated Anchor component](xref:arfoundation-simulation-environments#simulated-anchor-component) to XR Simulation environments, and adjust anchor discovery settings in [XR Simulation project settings](xref:arfoundation-simulation-project-settings).
- Added enum value `Other` to [PlaneClassification](xref:UnityEngine.XR.ARSubsystems.PlaneClassification).
- Added the ability for camera subsystem providers to include [EXIF](https://en.wikipedia.org/wiki/Exif) data with the `XRCameraFrame`.

### Changed

- If XR Simulation is enabled in your project, it will now automatically attempt to set its Environment Layer name to "XR Simulation". XR Simulation uses Layer 30 as its default Environment Layer, but you can change this setting by going to **Edit** > **Project Settings** > **XR Plug-in Management** > **XR Simulation**.

## [5.1.0-pre.4] - 2023-04-11

### Deprecated

- Deprecated the `UnityEngine.XR.ARSubsystems.LightEstimationMode` enum and related extension methods. This functionality was replaced by the [Feature](xref:UnityEngine.XR.ARSubsystems.Feature) flags enum in AR Foundation 3.1, and `LightEstimationMode` has since been unused.

### Fixed

- Fixed an issue where `UnityEditor.XR.Simulation.SimulationEditorUtilities` could incorrectly throw a `NullReferenceException` when you opened the Editor if no XR Plug-in was enabled for the standalone build target.
- Fixed an issue for the XR Simulation camera where looking down or up would incorrectly affect the direction of movement in some cases.
- Fixed an [issue](https://issuetracker.unity3d.com/issues/opengl-xrsimulation-does-not-show-scene-gameobjects-when-using-opengl-graphics-api) for the XR Simulation background where the depth value would be incorrect for graphics APIs using non reversed depth buffer.

## [5.1.0-pre.3] - 2023-02-06

### Added

- Added a public constructor for [XRCpuImage](xref:UnityEngine.XR.ARSubsystems.XRCpuImage).
- Added support for XRCpuImage API to XR Simulation.
- Added additional enumeration values to [XRCpuImage.Format](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Format) for `RGB` image formats.
- Added [XRCpuImageFormatExtensions.ToXRCpuImageFormat](xref:UnityEngine.XR.ARSubsystems.XRCpuImageFormatExtensions.ToXRCpuImageFormat(UnityEngine.TextureFormat)) extension method for converting [UnityEngine.TextureFormat](xref:UnityEngine.TextureFormat) to [XRCpuImage.Format](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Format).
- Added support for the [Environment Probes](xref:arfoundation-environment-probes) feature in XR Simulation. You can now add and remove environment probes in Play mode, add a [Simulated Environment Probe component](xref:arfoundation-simulation-environments#simulated-environment-probe-component) to XR Simulation environments, and adjust environment probe discovery settings in [XR Simulation project settings](xref:arfoundation-simulation-project-settings).
- Added [ARSession.Reset](xref:UnityEngine.XR.ARFoundation.ARSession.Reset) support to relevant subsystems in XR Simulation.

### Changed

- Downloads version 1.0.0 of simulation environments package rather than 1.0.0-pre.2.
- Added the `StructLayout(LayoutKind.Sequential)` attribute to [XRCpuImage.Plane.Cinfo](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Plane.Cinfo) to ensure layout compatibility when marshalling the struct.
- Changed [XRReferenceImage.ToString](xref:UnityEngine.XR.ARSubsystems.XRReferenceImage.ToString) to include the reference image name in the generated string, in addition to previously included fields.
- Changed the Editor behavior of XR Simulation Runtime Settings to add input validation and standardize naming conventions for similar settings across simulation subsystem providers.
- Changed default values of XR Simulation Runtime Settings to improve plane detection performance. If your project already includes AR Foundation, you can accept the new default values by deleting the Asset at `Assets/XR/Resources/XRSimulationRuntimeSettings.asset` and restarting the Editor.
- Added support for mutable runtime reference image libraries to XR Simulation.

### Removed

- Removed a warning that was previously logged when your AR Foundation project build contained a scene with an `XROrigin` but not an `ARCameraBackground` or `ARCameraManager`. There are valid reasons to set up a scene this way, such as building for OpenXR, so the warning was not helpful.

### Fixed

- Fixed an issue where AR content would not render in the Game view in XR Simulation when using [XRCameraBackgroundRenderingMode.AfterOpaques](xref:UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode).
- Fixed an issue where some XR Simulation subsystem providers could break if you destroyed and immediately re-instantiated the Simulation Session Subsystem.

## [5.1.0-pre.2] - 2022-11-01

### Changed

- Downloads version 1.0.0 of simulation environments package rather than 1.0.0-pre.2.

### Fixed

- Fixed various issues with the **GameObject** > **XR** menu:
    - Fixed an issue where attempting to create an AR Debug Menu would fail.
    - Fixed issues where the AR Default Plane and AR Default Face were created with a missing Material.
    - Fixed issues where AR Session, AR Debug Menu, and AR default trackables were always created at the root of the scene hieararchy and did not become selected after creation. These menu items now use the currently selected GameObject in the scene as the parent.
    - Fixed issues where some create operations could not be undone.
- Fixed an issue in simulation where pausing and resuming the session would freeze the camera background.
- Fixed [issue ARFB-125](https://issuetracker.unity3d.com/issues/plane-discovery-fails-in-ar-sim-upon-re-entry-into-simplear-via-menu) in simulation where plane and pointcloud discovery would stop working after loading a new scene.

## [5.1.0-pre.1] - 2022-10-06

### Added

- Added button to the AR Environment Overlay that when toggled on, the Scene View changes to an AR Environment View and displays the current simulation environment. When toggled off, the Scene View displays the active editing context.
- Added Camera Configuration Menu to the [ARDebugMenu](xref:arfoundation-debug-menu).
- Added a custom Editor for the `SimulationEnvironment` component that displays serializable `Pose.rotation` properties as `Vector3` Euler angles rather than `Quaternion`.

### Changed

- Simulation environment is now instantiated as a prefab instance in edit mode, so that changes made to the prefab are reflected in the XR Environment view.

### Fixed

- Fixed an issue where the shaders "Simulation/Standard Lit" and "Simulation/URP/Lit" needed to be manually re-imported after converting a project to URP.
- Fixed truncation of property labels for XR Simulation settings in Project Settings window.

## [5.0.2] - 2022-09-11

### Changed

- Renamed AR Environment toolbar and view to XR Environment.
- Removed "experimental" from name of XR Simulation plugin in XR Management.
- Fixed handling of scale and offset of XR Origin in simulation.

### Fixed

- Fixed [an issue](https://issuetracker.unity3d.com/issues/ar-camera-feed-jitters-and-shakes-with-mutithreaded-rendering-when-running-on-ios-16) where rendered frames comes out of order on iOS 16 when the app is built with multi-threaded rendering enabled in Unity.
- Fixed an issue where both `ARRaycastManager` and `ARPointCloudManager` could attempt to copy invalid `NativeArray<T>`s when using the raycast fallback API.
- Fixed an issue where `MaterialInspector` would throw obsolete warnings in URP projects using Unity 2022 or later.
- Fixed an issue where the shaders "Simulation/Standard Lit" and "Simulation/URP/Lit" would throw errors if included in a build using the built-in render pipeline.
- Fixed an issue in `SimulationPointCloudSubsystem` where the identifiers for points in a point cloud would not increment with new environment scans.
- Fixed an issue where the obsolete `ARSessionOrigin.camera` property shows a warning during the build.
- Fixed an [issue](https://issuetracker.unity3d.com/issues/simulation-hangs-on-arm-macs-with-silicon-editor-build) where XR Simulation would hang on Apple Silicon Macs.

## [5.0.0-pre.13] - 2022-06-28

### Changed

- Removed error message for when XR Simulation Environments package sample is not found upon installing sample environments, and replaced it with a warning instructing how to import the sample.
- Use version 1.0.0-pre.2 of *XR Simulation Environments* package installed through the AR Environment toolbar. This version adds documentation and an explicit dependency on AR Foundation.
- Moved `XRCameraBackgroundRenderingMode` and `XRSupportedCameraBackgroundRenderingMode` enums to the UnityEngine.XR.ARSubsystems namespace.
- Temporary scenes generated for XR Environment Simulation now have a uniquely generated hash appended to their names to prevent loading errors when restarting a simulated AR Session.

### Fixed

- Fixed an issue where `ARBackgroundRendererFeature` would overwrite rendered geometry when rendering in `AfterOpaques` mode while Occlusion was enabled.
- Fixed an issue where the DefaultSimulationEnvironment would not render correctly when using the Universal Render Pipeline.
- Fixed error in the AR Environment Overlay if the XR General Settings asset was not created before opening the overlay.
- Fixed binding error in the AR Environment Overlay when enabling/disabling multiple Plug-in Providers in XR Plug-in Management.
- Fixed issue in [Simulation Raycast Subsystem](xref:UnityEngine.XR.Simulation.SimulationRaycastSubsystem) where empty arrays were causing out of bounds exceptions.
- Fixed simulation material inspectors so values correctly update from the inspector to the material.
- Fixed hard crash from simulation mesh subsystem upon quickly entering and exiting play mode.
- Fixed simulated image tracking subsystem checking for occlusion of images from the camera.
- Fixed error loading AR Environment icon when AR Foundation is first added to a project.
- Fixed crash in `RenderSettings` that was preventing the `SimulationRenderSettings` from being used in the AR Environment View in Unity 2022.1+.

## [5.0.0-pre.12] - 2022-05-19

### Added

- Added automatic refresh of AR environment list upon importing or deleting a prefab with the `SimulationEnvironment` component.
- Added analytics to anonymously collect usage data of some AR Foundation features. These analytics are captured only in the Unity Editor and not added to the player builds. See [Unity manual on Editor Analytics](https://docs.unity3d.com/Manual/EditorAnalytics.html) for more details.
- Added support for changing the Camera Background rendering order so that the background can be rendered either `BeforeOpaques` or `AfterOpaques` by setting the `ARCameraManager.requestedRenderingMode` in the editor or at runtime and then checking `ARCameraManager.currentRenderingMode` at runtime for the real rendering mode. See migration guide for more information.
- Added support for changing the Camera Background rendering order in simulation so that the background can be rendered either `BeforeOpaques` or `AfterOpaques` by setting the `SimulationCameraSubsystem.requestedRenderingMode`.
- Added support for the following subsystems in simulation.
  - Point cloud subsystem
  - Plane subsystem
  - Image tracking subsystem
  - Raycast subystem
  - Mesh subsystem
  - Camera subsystem
- Added AR Environment Toolbar Overlay for Scene view. The toolbar supports selecting the active Simulation environment, installing sample Simulation environments, refreshing the list of environments, creating a new environment, duplicating the active environment, and opening the active environment for editing.
- AR Environment Toolbar Overlay will change the Scene View to an AR Environment View, where you can preview the environment that will be used for Simulation.
- Added manager to enable X-Ray visualization of simulation environments and options to the Simulation Runtime Settings for enabling or disabling the X-Ray Visuals.
- Added a **GameObject** &gt; **XR** &gt; **XR Origin (Mobile AR)** menu item, giving users a one-click option to create an [XR Origin](xref:Unity.XR.CoreUtils.XROrigin) which is fully configured for mobile AR.
- Added `SimulatedMeshClassification` and `SimulatedBoundedPlane` components to default environment prefab to support mesh and plane subsystems in simulation.
- Added support for `ARBackgroundRenderer` in simulation.
- Added public constructors for `XRTextureDescriptor` and `XRCameraFrame`.
- Added point cloud visualization to the [ARDebugMenu](xref:arfoundation-debug-menu).
- Added `SimulationPreferences` to user preferences in **Edit > Preferences > XR Simulation**.
- Project wide simulation parameters can be modified in **Edit > Project Settings > XR Plug-in Management > XR Simulation**.

### Changed

- Moved menu item for refreshing the AR environment list from the AR Environment toolbar to the Assets menu.
- `ARBackgroundRendererFeature` contains an abstract base class for custom render passes to use when defining an ARBackgroundRendererFeature custom pass and defines 2 custom render passes. A `BeforeOpaques` pass and an `AfterOpaques` pass.
- Moved `UnitySubsystemsManifest.json` from Editor to Runtime since the integrated subsystems for simulation are in the Runtime assembly.
- Removed `#if UNITY_EDITOR` guards from `ARBackgroundRendererFeature` so that the render feature can now be used in the editor.
- Changed render settings in default simulation environment so that it uses trilight ambient mode and brighter ambient colors.
- Updated simulation input subsystem to use `HandheldARInputDevice` instead of `XR HMD`.
- The unused Post Processing settings field is now hidden in the inspector for `SimulationEnvironment`.

### Deprecated

- Deprecated the depth subsystem abstraction, `XRDepthSubsystem` and `XRDepthSubsystemDescriptor`, in favor of `XRPointCloudSubsystem` and `XRPointCloudSubsystemDescriptor` respectively. This is just a rename of the subsystem without any significant changes to the APIs. Unity's API Updater should automatically convert any deprecated depth subsystem API references to the point cloud subsystem APIs when the project is loaded into the Editor again. See migration guide for more details.
  - `XRDepthSubsystem` renamed to `XRPointCloudSubsystem`
  - `XRDepthSubsystemDescriptor` renamed to `XRPointCloudSubsystemDescriptor`

### Fixed

- Fixed error loading AR Environment view icon on first domain load.
- Fixed issue with simulated image tracking using the wrong vector for checking image direction.
- Fixed a bug when the vertex count in `SimulationMeshSubsystem` could overflow beyond the limit of 16-bit indexing for large simulation environments. The index format now always uses 32-bit indexing to avoid the issue.
- Fixed `MissingReferenceException` accessing camera when stopping simulation session subsystem.
- Fixed error importing sample environments after downloading environments package through AR Environment toolbar.
- Fixed navigation keys working in simulation even when right mouse button wasn't pressed.
- Fixed an issue in simulation where the camera starts at the `XROrigin` instead of the simulation environment's specified starting pose.
- Fixed [issue 960](https://github.com/Unity-Technologies/arfoundation-samples/issues/960) where `ARPointCloudManager.Append` could incorrectly throw an `ArgumentException`. For developers with custom implementations of `XRRaycastSubsystem.Provider` which do not support raycasting with an arbitrary ray, fallback raycasts including hits from point clouds will now correctly return all values.
- Fixed [issue 963](https://github.com/Unity-Technologies/arfoundation-samples/issues/963) where `ARRaycastManager.RaycastFallback` would sometimes return incomplete results. Developers with custom implementations of `XRRaycastSubsystem.Provider` which do not support raycasting with an arbitrary ray will now see correct results from `ARRaycastManager.Raycast(Ray, TrackableType, Allocator)`.
- Fixed graphics library error caused when binding a Unity texture object to a native texture object when their mipmap settings do not match.
- Fixed an issue where recompiling scripts during Play Mode could cause memory leaks if `ARPlaneManager` or `ARPointCloudManager` were present in the scene. XR Simulation does not support script recompilation while playing, but now if a recompilation occurs in Play Mode, AR Foundation will not leak any memory.

## [5.0.0-pre.9] - 2022-03-01

### Added

- Simulation loads an environment prefab from Simulation Settings.
- Added support for a new [OcclusionPreferenceMode.NoOcclusion](xref:UnityEngine.XR.ARSubsystems.OcclusionPreferenceMode) mode that, when set, disables occlusion rendering on the camera background when using [ARCameraBackground](xref:UnityEngine.XR.ARFoundation.ARCameraBackground) and [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager).
- Added simulation setting to turn the navigation off.
- Added anchor visualization to the [ARDebugMenu](xref:arfoundation-debug-menu).

### Changed

- Changed the string "People Occlusion" to "Human Occlusion" in [Feature](xref:UnityEngine.XR.ARSubsystems.Feature) so that it matches the documentation.

### Fixed

- Fixed [issue 1392753](https://issuetracker.unity3d.com/issues/flickering-slash-frozen-textures-when-ar-occlusion-manager-is-disabled) where occlusion texture would remain frozen instead of being cleared when the occlusion manager is disabled.
- Fixed issue where sliders would slide even when not `interactable` in the [ARDebugMenu](xref:arfoundation-debug-menu).

## [5.0.0-pre.8] - 2022-02-09

### Added

- Added a configuration menu to the [ARDebugMenu](xref:arfoundation-debug-menu) that will help in visualizing the currently active [Configuration](xref:UnityEngine.XR.ARSubsystems.Configuration) for the session and other available configurations for the current device.
- Work in progress: AR Simulation in XR Plug-in Management; initial support for camera movement and simulation environment.

## [5.0.0-pre.7] - 2021-12-10

### Fixed

- Fixed [issue 1353859](https://issuetracker.unity3d.com/issues/ar-each-time-an-arface-is-added-three-new-gameobjects-are-created-in-the-scene-and-never-destroyed): During face tracking, each `ARFace` may create three additional GameObjects to represent the left eye, right eye, and fixation point. Previously, this would also incorrectly create three additional, superfluous GameObjects at the origin. These additional GameObjects are no longer created.

## [5.0.0-pre.6] - 2021-11-17

### Deprecated

- `ARSessionOrigin` is now deprecated. Use [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) from the XR Core Utilities package instead.
- `ARPoseDriver` is now deprecated. Use [TrackedPoseDriver](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.1/api/UnityEngine.InputSystem.XR.TrackedPoseDriver.html) from the Input System package instead.

## [5.0.0-pre.5] - 2021-10-28

### Added

- Added [ARDebugMenu](xref:UnityEngine.XR.ARFoundation.ARDebugMenu) that will help in visualizing the location of the [XROrigin](xref:Unity.XR.CoreUtils.XROrigin), the current FPS, tracking state, and some trackables. See the [manual entry for AR Debug Menu](xref:arfoundation-debug-menu) for more information.
- Added support for new [XRMeshSubsystem](xref:UnityEngine.XR.XRMeshSubsystem) interface available in 2021.2, which allows providers to specify a separate transform for each mesh.

### Changed

- The AR Subsystems package (`com.unity.xr.arsubsystems`) has been combined with this package (`com.unity.xr.arfoundation`). The AR Subsystems package has been deprecated, and you should not use or depend on it.
- The minimum Unity version for this package is now 2021.2.

### Deprecated

- Deprecated the `XRSubsystem`. If you implement a custom subsystem by extending `XRSubsystem` then update it to use [SubsystemWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemWithProvider) instead. This is the new Subsystem base class in Unity core. See the migration guide for details.

### Removed

- Removed deprecated APIs:
  - `UnityEditor.XR.ARSubsystems.InternalBridge` - this was used to get a texture's original source dimensions. There is now a built-in function [TextureImporter.GetSourceTextureWidthAndHeight](xref:UnityEditor.TextureImporter.GetSourceTextureWidthAndHeight(System.Int32&,System.Int32&)) that provides the same functionality.
  - "AR Reference Point" APIs were deprecated and renamed to "AR Anchor" several releases ago. In this release, the following classes have been removed:
    - `ARReferencePoint`
    - `ARReferencePointManager`
    - `ARReferencePointsChangedEventArgs`
    - `XRReferencePoint`
    - `XRReferencePointSubsystem`
    - `XRReferencePointSubsystemDescriptor`
- Removed conditional dependency on the deprecated Lightweight Render Pipeline (LWRP) package. Any dependency on LWRP package should be replaced with URP package.

### Fixed

- [ARMeshManager](xref:UnityEngine.XR.ARFoundation.ARMeshManager) no longer throws `ArgumentNullException`s ("Value cannot be null") when exiting play mode in the editor.
- Fixed [issue 1346735](https://issuetracker.unity3d.com/issues/arfoundation-error-cs0246-is-thrown-when-particle-system-built-in-package-is-removed): Removing the built-in particle system module results in a compilation error due to a hard dependency. Now the dependency is explicitly defined in the package which ensures that the built-in particle system module is enabled in the project.
- Fixed an issue where the built-in UI module and the UGUI package can be removed which results in a compilation error due to hard dependencies. Now the dependencies are explicitly defined in the package which ensures that the UI built-in module and the UGUI package are enabled in the project.
- Fixed [issue 878](https://github.com/Unity-Technologies/arfoundation-samples/issues/878) where `XROcclusionSubsystem` would use `Provider.environmentDepthCpuImageApi` instead of `Provider.environmentDepthConfidenceCpuImageApi` when acquiring the Environment Depth Confidence CPU Image. This did not affect the provider implementations of Google ARCore and Apple ARKit packages because these providers use singleton `XRCpuImage.Api` for all CPU image types. However, it could be an issue for a custom AR provider that uses different instances of `XRCpuImage.Api` for different CPU image types.

## [4.2.0] - 2021-08-11

No changes

## [4.2.0-pre.12] - 2021-07-08

### Added

- Added support for new [XRMeshSubsystem](xref:UnityEngine.XR.XRMeshSubsystem) interface available in 2021.2, which allows providers to specify a separate transform for each mesh.
- Added methods to get the [raw](xref:UnityEngine.XR.ARFoundation.AROcclusionManager.TryAcquireRawEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) and [smoothed](xref:UnityEngine.XR.ARFoundation.AROcclusionManager.TryAcquireSmoothedEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) depth images independently.
- Added a [depthSensorSupported](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration.depthSensorSupported) flag to the `XRCameraConfiguration` to indicate whether or not a depth sensor is supported by the camera configuration.

### Fixed

- Fixed a `NullReferenceException` when accessing mesh information from an [ARFace](xref:UnityEngine.XR.ARFoundation.ARFace) in the Editor. The exception was thrown from ``Unity.Collections.NativeArray`1[T].CheckElementReadAccess``.

## [4.2.0-pre.10] - 2021-06-28

No changes

## [4.2.0-pre.9] - 2021-05-27

No changes

## [4.2.0-pre.8] - 2021-05-20

### Changed

- [Environment depth temporal smoothing](xref:UnityEngine.XR.ARFoundation.AROcclusionManager.environmentDepthTemporalSmoothingRequested) now defaults to true, which is more consistent with the behavior prior to the introduction of this option.

## [4.2.0-pre.7] - 2021-05-13

### Added

- Add support for temporal smoothing of the environment depth image. See [AROcclusionManager.environmentDepthTemporalSmoothingRequested](xref:UnityEngine.XR.ARFoundation.AROcclusionManager.environmentDepthTemporalSmoothingRequested).

### Fixed

- Fixed incorrect validation in [ARMeshManager](xref:UnityEngine.XR.ARFoundation.ARMeshManager) which prevented its use in a prefab.

## [4.2.0-pre.5] - 2021-04-07

### Fixed

- Fixed [warning messages regarding cameraDepthTarget when using URP rendering](https://issuetracker.unity3d.com/issues/urp-ar-foundation-warning-message-regarding-cameradepthtarget-with-urp-slash-ar-foundation-projects) issue.
- Fixed [camera background doesn't work when stacking cameras with URP rendering](https://issuetracker.unity3d.com/issues/arfoundation-arkit-ios-urp-arbackgroundrenderreature-doesnt-work-correctly-when-stacking-overlay-cameras) issue.
- Fixed [enabling depth texture causes the camera to render black](https://issuetracker.unity3d.com/issues/ios-ar-foundation-urp-enabling-depth-texture-causes-the-camera-to-render-black) issue.
- Fixed an issue which caused [black reflections on iOS when using AREnvironmentProbeManager](https://github.com/Unity-Technologies/arfoundation-samples/issues/749).

## [4.2.0-pre.4] - 2021-03-19

### Fixed

- Clarified documentation for [ARCameraManager.TryGetInstrincis](xref:UnityEngine.XR.ARFoundation.ARCameraManager.TryGetIntrinsics(UnityEngine.XR.ARSubsystems.XRCameraIntrinsics@)).

## [4.2.0-pre.3] - 2021-03-15

No changes

## [4.2.0-pre.2] - 2021-03-03

### Added

- Added options to the [ARPlaneMeshVisualizer component](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshVisualizer) to control when MeshRenderer and LineRenderer components should be enabled. See
  - [ARPlaneMeshVisualizer.trackingStateVisibilityThreshold](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshVisualizer.trackingStateVisibilityThreshold)
  - [ARPlaneMeshVisualizer.hideSubsumed](xref:UnityEngine.XR.ARFoundation.ARPlaneMeshVisualizer.hideSubsumed)

### Changed

- Added description for occlusion on the main AR Foundation manual page, and edit the human segmentations description.
- The [ARSession](xref:UnityEngine.XR.ARFoundation.ARSession) optionally sets the application's `vSyncCount` and `targetFrameRate` (see [matchFrameRateRequested](xref:UnityEngine.XR.ARFoundation.ARSession.matchFrameRateRequested)). In earlier versions, the original values were not restored when the ARSession was disabled. The behavior has changed so that the original values are restored when the ARSession disabled if the frame rate was set while the ARSession was enabled.
- Updated [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@4.0) dependency to 4.0.
- The minimum Unity version for this package is now 2020.3.

### Fixed

- Added missing icon to the [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager).
- Replaced uses of [PostProcessScene](xref:UnityEditor.Callbacks.PostProcessSceneAttribute) attribute with [IProcessSceneWithReport](xref:UnityEditor.Build.IProcessSceneWithReport) callback mechanism. This prevents scene hashes from changing when adding the AR Foundation package.
- Added `MODULE_URP_10_OR_NEWER` and updated use of obsolete `ScriptableRenderer.cameraDepth` API to fix issues related to API upgrades that occur in builds with URP 10.0 and above

## [4.1.3] - 2021-01-05

### Changed

- The [ARCameraBackground](xref:UnityEngine.XR.ARFoundation.ARCameraBackground) component now sets the camera's [field of view](xref:UnityEngine.Camera.fieldOfView). Because the `ARCameraBackground` already overrides the camera's [projection matrix](xref:UnityEngine.Camera.projectionMatrix), this has no effect on ARFoundation. However, any code that reads the camera's [fieldOfView](xref:UnityEngine.Camera.fieldOfView) property will now read the correct value.
- Minor documentation formatting fixes.
- Update the inspector for the ARSession component to include a help box describing the function of the [Match Frame Rate](xref:UnityEngine.XR.ARFoundation.ARSession.matchFrameRateRequested) option.

## [4.1.1] - 2020-11-11

### Changed

- Released package for Unity 2021.1

## [4.1.0-preview.13] - 2020-11-09

### Fixed

- Remove fix made in 4.1.0-preview.10 for URP 10+ rendering. URP 10 now has a proper fix for this issue.

## [4.1.0-preview.12] - 2020-11-02

### Fixed

- Trackable managers no longer attempt to query subsystems for trackable changes when the subsystems are stopped. This fixes the console error that will occur with MagicLeap:
<pre>
InvalidOperationException: Can't call "GetChanges" without "Start"ing the anchor subsystem!
</pre>

## [4.1.0-preview.11] - 2020-10-22

### Added

- Added a new extension method [ScheduleAddImageWithValidationJob](xref:UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageWithValidationJob(UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary,UnityEngine.Texture2D,System.String,System.Nullable{System.Single},Unity.Jobs.JobHandle)) for the [MutableRuntimeReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary) which adds new reference images to a reference library only after validating that the new reference image is suitable for image tracking. The new method returns a new type [AddReferenceImageJobState](xref:UnityEngine.XR.ARSubsystems.AddReferenceImageJobState) which you can use to determine whether the image was successfully added.

### Changed

- **Anchors:** Previously, [ARAnchors](xref:UnityEngine.XR.ARFoundation.ARAnchor) were added and removed by calling `AddAnchor` and `RemoveAnchor`, respectively. Now, you can add an anchor by simply adding an `ARAnchor` component to any GameObject, e.g., by calling [AddComponent](xref:UnityEngine.GameObject.AddComponent(System.Type)). The GameObject may exist anywhere in your scene hierarchy; it need not be under the [ARSessionOrigin](xref:UnityEngine.XR.ARFoundation.ARSessionOrigin). Similarly, remove an anchor by [destroying](xref:UnityEngine.Object.Destroy(UnityEngine.Object)) the `ARAnchor` component (or its GameObject). See the [manual entry for anchors](xref:arfoundation-anchors) for more details.
- **Environment Probes:** Previously, [environment probes](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbe) were added and removed by calling `AddEnvironmentProbe` and `RemoveEnvironmentProbe`, respectively. Now, you can add an anchor by simply adding an `AREnvironmentProbe` component to any GameObject, e.g., by calling [AddComponent](xref:UnityEngine.GameObject.AddComponent(System.Type)). The GameObject may exist anywhere in your scene hierarchy; it need not be under the [ARSessionOrigin](xref:UnityEngine.XR.ARFoundation.ARSessionOrigin). Similarly, remove an environment probe by [destroying](xref:UnityEngine.Object.Destroy(UnityEngine.Object)) the `AREnvironmentProbe` component (or its GameObject). See the [manual entry for environment probes](xref:arfoundation-environment-probes) for more details.

### Fixed

- Fixed in an incorrect `Debug.Assert` regarding a texture descriptor changing from `None` to something other than `None`. A typical error message (only seen in Development Builds) was
<pre>
Texture descriptor dimension should not change from None to Tex2D.
</pre>
- Fix a null reference exception in the occlusion manager when setting the environment depth mode or occlusion preference mode when the device does not support the occlusion subsystem.

## [4.1.0-preview.10] - 2020-10-12

### Changed

- Changed internal uses of [Debug.Assert](xref:UnityEngine.Debug.Assert(System.Boolean,System.Object)) to a custom version that does not allocate the message string unless the assert fails. This reduces per-frame GC allocations when `UNITY_ASSERTIONS` are enabled (typically in development builds).
- Update [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2) to 3.2.16.

### Fixed

- Minor documentation fixes.
- Fix an issue with background rendering with URP 10+ rendering with a Final Blit pass.

## [4.1.0-preview.9] - 2020-09-25

### Added

- Added new sections in the manual documentation for image tracking and anchors.
- Added a [trackable](xref:UnityEngine.XR.ARFoundation.ARRaycastHit.trackable) property to the [ARRaycastHit](xref:UnityEngine.XR.ARFoundation.ARRaycastHit) which will refer to the [ARTrackable](xref:UnityEngine.XR.ARFoundation.ARTrackable) hit by the raycast, or `null` if no trackable was hit.

### Fixed

- Fix several documentation links.
- Fix [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager)'s handling of depth when non-unit scale is applied.

## [4.1.0-preview.7] - 2020-08-26

### Changed

- Added a warning when an `ARSession` is used in a scene (or scenes), but no providers are selected in [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2) for the current build platform.

### Fixed

- Updated "Copying the Camera Texture to a Render Texture when accessing the camera image on the GPU" documentation to use a '[Command Buffer](xref:UnityEngine.Rendering.CommandBuffer)' instead of a `Graphics.Blit()` to fix an issue where blit would not work on certain devices.
- Fixed a `NullReferenceException` which would happen if you invoked [`ARSession.CheckAvailability()`](xref:UnityEngine.XR.ARFoundation.ARSession.CheckAvailability) when the ARSession's GameObject was disabled and had never been enabled.

## [4.1.0-preview.6] - 2020-07-27

No changes

## [4.1.0-preview.5] - 2020-07-16

### Changed

- Updated "Configuring the AR Camera background using a Scriptable Render Pipeline" documentation for further clarity on setup steps.
- Added documentation for the `ARMeshManager`.

### Fixed

- The choice of whether to use the legacy or Universal render pipeline was based on an incorrect GraphicsSettings parameter (`renderPipelineAsset` instead of `currentRenderPipeline`). This meant that certain quality settings may not have been respected properly.
- Fixed a bug where it was possible for the ARSession component to set the [Application.targetFrameRate](xref:UnityEngine.Application.targetFrameRate) even when [`matchFrameRateRequested`](xref:UnityEngine.XR.ARFoundation.ARSession.matchFrameRateRequested) was `false`.

## [4.1.0-preview.3] - 2020-07-09

### Changed

- Update [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2) to 3.2.13.

### Fixed

- Minor documentation fixes.

## [4.1.0-preview.2] - 2020-06-24

### Changed

- Updating the documentation for new package versions.

## [4.1.0-preview.1] - 2020-06-23

### Added

- Add support for environment depth through the AROcclusionManager/XROcclusionSubsystem.

### Changed

- Minimum Unity version for this package is now 2019.4.

## [4.0.2] - 2020-05-29

### Added

- The `ARCameraManager` now invokes `XRCameraSubsystem.OnBeforeBackgroundRender` immediately before rendering the camera background.
- Added a helper utility, `LoaderUtility`, for interacting with `XR Management` and added a section to the migration guide explaining how to use it.

### Changed

- Changed `XRCameraImage` to `XRCpuImage` along with APIs that used this type (e.g., `ARCameraManager.TryGetLatestImage`). See the migration guide for more details.
- The `ARMeshManager` no longer creates and destroys an [`XRMeshSubsystem`](https://docs.unity3d.com/ScriptReference/XR.XRMeshSubsystem.html). Instead, it relies on [XR Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html) to create and destroy the subsystem. The `ARMeshManager` still starts and stops it.

### Fixed

- Fixed a bug which could throw an `ArgumentNullException`, typically on application shutdown:
    ```
    ArgumentNullException: Value cannot be null.
    Parameter name: _unity_self
    ```

## [4.0.0-preview.3] - 2020-05-04

### Added

- Add support for tracked raycasts. A tracked raycast is repeated and updated automatically. See [ARRaycastManager.AddRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycastManager.AddRaycast*).
- `ARCameraFrameReceivedEventArgs` now provides a camera grain texture along with an associated intensity value which can be used to add image noise characteristics to virtual content. The usage of these properties may vary by platform, so please consult the documentation of the specific provider plug-in when using this feature.

### Changed

- Updating dependency on com.unity.xr.management to 3.2.10.

### Fixed

- Fixed all broken or missing scripting API documentation.

## [4.0.0-preview.1] - 2020-02-26

### Added

- Improved performance of face mesh updates by using the [new mesh APIs in 2019.3 which accept NativeArrays](xref:UnityEngine.Mesh.SetVertices(Unity.Collections.NativeArray`1<T>,System.Int32,System.Int32)). This avoids a copy of the `NativeArray` to `List`.

### Changed

- See the Migration Guide.
- AR Foundation now relies on [XR Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html) to initialize subsystems. If your project's configuration does not enable an XR Loader appropriate for your target platforms then the underlying subsystems AR Foundation depends on will not be available. Previously AR Foundation would attempt to initialize subsystems itself in the absence of a valid XR Management configuration.

## [3.1.3] - 2020-04-13

### Fixed

- The documentation for the `ARFace` scripting API for accessing left eye, right eye, and fixation point incorrectly referred to a nullable value type, when in fact the returned type is a [Transform](xref:UnityEngine.Transform). This has been fixed.
- Eliminating shader compiler errors that started with Unity 2020.1 and that originate from the Apple ARKit package.

## [3.1.0-preview.8] - 2020-03-12

No changes

## [3.1.0-preview.7] - 2020-03-03

### Fixed

- Patched a memory leak by destroying the camera textures when the camera manager is disabled.

## [3.1.0-preview.6] - 2020-01-21

### Fixed

- Fixed memory leak when accessing the `humanStencilTexture` and `humanDepthTexture` properties of the `AROcclusionManager`.

## [3.1.0-preview.4] - 2020-01-16

### Added

- Updated documentation.

## [3.1.0-preview.3] - 2019-12-20

### Added

- Added support for HDR light estimation to ARCameraManager.
- Added script updater support for facilitating the rename of Reference Points to Anchors

### Fixed

- Fix `HelpURL`s on `MonoBehaviour`s to point to the 3.1 version of the documentation.
- Added `FormerlySerializedAs` to `ARAnchorManager.m_AnchorPrefab` field to preserve any prefab specified when it was the `ARReferencePointManager`.
- Fixed issue that broke the camera background rendering when URP post-processing was enabled on the AR camera.

## [3.1.0-preview.1] - 2019-11-21

### Added

- Added the new [AROcclusionManager](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.AROcclusionManager.html) with human segmentation functionality (on some iOS hardware).

## [3.0.1] - 2019-11-18

### Changed

- Renaming the concept of `Reference Points` to `Anchors`. The Unity script updater should convert any references to `ARReferencePointManager`, `ARReferencePoint`, and `ARReferencePointsChangedEventArgs` the next time the project is loaded into the Editor.

### Fixed

- Removed references to LWRP from the documentation.

## [3.0.0-preview.6] - 2019-11-14

### Changed

- The following virtual methods on the `ARCameraBackground` have been removed:
  - `ConfigureLegacyRenderPipelineBackgroundRendering`
  - `TeardownLegacyRenderPipelineBackgroundRendering`

  Previously, `ConfigureLegacyRenderPipelineBackgroundRendering` configured a `CommandBuffer` and added the command buffer to the camera. If you had overriden this method previously, you can override the `legacyCameraEvents` property to specify the camera events which use the CommandBuffer, and `ConfigureLegacyCommandBuffer(CommandBuffer)` to configure the command buffer. There is no need to override `TeardownLegacyRenderPipelineBackgroundRendering` anymore because the `ARCameraBackground` remembers which `CameraEvent`s the buffer was added to so it can remove them later.

### Fixed

- Fixed an issue which could cause the background camera texture to stop functioning correctly on certain devices running OpenGLES3.
- Previously, when using LWRP or UniversalRP, the requested culling mode was ignored. This resulted in an incorrect culling mode when using the front facing camera with ARCore (e.g., during face detection). This has been fixed.

## [3.0.0-preview.4] - 2019-10-22

### Added

- Querying the [`ARCameraManager`'s `focusMode`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARCameraManager.html#UnityEngine_XR_ARFoundation_ARCameraManager_focusMode) will return the actual camera focus value rather than a cached value.
- Added `classification` property on `ARPlane` which may return contextual information about planes. See [PlaneClassification](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@3.1/api/UnityEngine.XR.ARSubsystems.PlaneClassification.html) for the list of all possible classifications.
- Added `supportsClassification` property to [XRPlaneSubsystemDescriptor](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@3.1/api/UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor.html).
- Add getter for the current AR frame rate to the `ARSession`. See [`ARSession.frameRate`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARSession.html#UnityEngine_XR_ARFoundation_ARSession_frameRate).
- [`ARFace.supportedFaceCount`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARFaceManager.html#UnityEngine_XR_ARFoundation_ARFaceManager_supportedFaceCount) would throw if face tracking was not supported. Now it returns 0.

### Fixed

- Fixed an issue with the `ARSessionOrigin` which could produce incorrect behavior with non-identity transforms.
- [`ARPlane.center`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARPlane.html#UnityEngine_XR_ARFoundation_ARPlane_center) was computed incorrectly (though the result was close to the correct answer). This has been fixed.

## [3.0.0-preview.3] - 2019-09-26

### Added

- If ["match frame rate"](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.ARSession.html#UnityEngine_XR_ARFoundation_ARSession_matchFrameRate) is enabled, the `ARSession` now checks for changes to the recommended frame rate each frame.

### Changed

- Some properties on `ARPointCloud` changed from `NativeArray`s to nullable `NativeSlice`s. See the migration guide for more details.
- The `ARFaceManager.supported` property has been removed. If face tracking is not supported, the manager's subsystem will be null. This was done for consistency as no other manager has this property. If a manager's subsystem is null after enabling the manager, that generally means the subsystem is not supported.

### Fixed

- Fixed a typo in Face Tracking documentation which incorrectly referred to planes instead of faces. Also updated the screenshot of the ARFaceManager to reflect the "Maximum Face Count" field.

## [3.0.0-preview.2] - 2019-09-05

### Added

- Converted dependency on Legacy Input Helpers package to optional dependency.  ARFoundation will continue to support the use of [TrackedPoseDriver](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/SpatialTracking.TrackedPoseDriver.html) and the `UnityEngine.SpatialTracking` namespace but there is no longer a dependency on that package.  ARFoundation can instead use the `ARTrackedPoseDriver` self-contained component to surface the device's pose.

### Fixed

- Fix issue where having the camera clearFlags property set to Skybox would break background rendering.
- Fix issue where screen would flash green before the first camera frame was received.

## [3.0.0-preview.1] - 2019-08-21

### Added

- Added support for eye tracking.
- Added support for [XRParticipantSubsystem](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@3.1/manual/participant-subsystem.html), which can track other users in a multi-user collaborative session.
- Added support for exposure duration
- Added support for exposure offset
- Add support for Lightweight Render Pipeline and Universal Render Pipeline.
- Add support for height scale estimatation for the 3D human body subsystem.

### Added

- Update the Legacy Input Helpers package to 1.3.7. This has no impact on existing functionality in ARFoundation, but should help with compatibility when using other packages.
- Add `IEquatable` to `TrackableCollection`.
- Previously, the `ARTrackedImageManager` and `ARTrackedObjectManager` would throw an exception if enabled without a valid reference library. These managers no longer throw; now, they disable themselves. This makes it possible to add one of these managers at runtime, since Unity will automatically invoke `OnEnable` before you have a chance to set a valid reference library. If you create one of these managers at runtime, you will need to set a valid reference library and then re-enable it.

### Fixed

- Fixed the issue where native providers were not being reinstantiated after being destroyed when loading/reloading a new scene.

## [2.2.0-preview.3] - 2019-07-16

### Added

- Added support for [XR Management](https://docs.unity3d.com/Packages/com.unity.xr.management@2.0/manual/index.html).
- Add support for `ARSession.notTrackingReason`.
- Add an option to synchronize the Unity frame with the AR session's update. See `ARSession.matchFrameRate`.
- Add an `ARMeshManager` to interface with the [`XRMeshSubsystem`](https://docs.unity3d.com/ScriptReference/Experimental.XR.XRMeshSubsystem.html). This is useful for XR Plug-ins that can generate meshes at runtime.

### Changed

- Previously, a trackable manager's changed event (e.g., `ARPlaneSubsystem.planesChanged`) was invoked every frame even if nothing had changed. Now, it is only invoked on frames when a change has occurred.

## [2.2.0-preview.2] - 2019-06-05

### Added

- Adding support for ARKit 3 functionality: Human pose estimation, human segmentation images, session collaboration, multiple face tracking, and tracking a face (with front camera) while in world tracking (with rear camera).

## [2.1.0-preview.4] - 2019-06-03

### Added

- Added a convenience method `ARTrackableManager.SetTrackablesActive(bool)` which will call `SetActive` on each trackable's `GameObject`. This is useful, for example, to disable all `ARPlane`s after disabling the `ARPlaneManager`.
- Fix NativeArray handling when `ENABLE_UNITY_COLLECTIONS_CHECKS` is set. This fixes an issue when running ARFoundation in the Editor. This is not set in the Player, so it is not an issue when running on device.

## [2.1.0-preview.3] - 2019-05-22

### Fixed

- Fix documentation links.

## [2.1.0-preview.2] - 2019-05-07

### Added

- Add support for image tracking (`XRImageTrackingSubsystem`).
- Add support for environment probes (`XREnvironmentProbeSubsystem`).
- Add support for face tracking (`XRFaceSubsystem`).
- Add support for object tracking (`XRObjectTrackingSubsystem`).

## [2.0.1] - 2019-03-05

### Changed

- See the Migration Guide.

### Fixed

- Add dependency on Legacy Input Helpers, which were moved from Unity to a package in 2019.1.

## [1.0.0-preview.22] - 2018-12-13

### Fixed

- Fix package dependency

## [1.0.0-preview.21] - 2018-12-13

### Added

- Added Face Tracking support:  `ARFaceManager`, `ARFace`, `ARFaceMeshVisualizer` and related scripts. See documentation for usage.

### Added

- Plane detection modes: Add ability to selectively enable detection for horizontal, vertical, or both types of planes. The `ARPlaneManager` includes a new setting, which defaults to both.
- Add support for setting the camera focus mode. Added a new component, `ARCameraOptions`, which lets you set the focus mode.

### Changed

- The light estimation option, which was previously on the `ARSession` component, has been moved to the new `ARCameraOptions` component.

## [1.0.0-preview.20] - 2018-11-06

### Added

- Support reference points reported without a corresponding `TryAddReferencePoint` call. This handles reference points that are added by some other means, e.g., loaded from a serialized session.

## [1.0.0-preview.19] - 2018-10-10

### Added

- Added support for `XRCameraExtensions` API to get the raw camera image data on the CPU. See the [manual documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/cpu-camera-image.html) for more information.

## [1.0.0-preview.18] - 2018-09-13

### Added

- Added `ARPlane.normal` to get the `ARPlane`'s normal in world space.

### Added

- Added LWRP support by allowing `ARCameraBackground` to use a background renderer that overrides the default functionality.  This works in conjunction with some *LWRPSupport* files (see [arfoundation-samples](https://github.com/Unity-Technologies/arfoundation-samples)) that will live in your LWRP project.

### Changed

- The `ARPlaneManager`, `ARPointCloudManager`, and `ARReferencePointManager` all instantiate either prefabs that you specify or `GameObject`s with at least an `ARPlane`, `ARPointCloud`, or `ARReferencePoint`, respectively, on them. The instantiated `GameObject`'s layer was set to match the `ARSessionOrigin`, overwriting the layer specified in the prefab. This has been changed so that if a prefab is specified, no changes to the layer are made. If no prefab is specified, then a new `GameObject` is created, and its layer will match that of the `ARSessionOrigin`'s `GameObject`.

### Fixed

- The `ARPlaneMeshVisualizer` did not disable its visible components (`MeshRenderer` and `LineRenderer`) when disabled. This has been fixed.

## [1.0.0-preview.17] - 2018-08-02

### Fixed

- Add `FormerlySerializedAs` attribute to serialized field changes to `ARCameraBackground`.

## [1.0.0-preview.16] - 2018-07-26

### Changed

- Removed static constructor from `ARSubsystemManager`. This allows access to the manager without forcing the creation of the subsystems, making the initialization more flexible.
- The API for overriding the material has been refactored. Previously, a custom material could be set with the `ARCameraBackground.material` setter, but this might be overwritten later if the option to override was disabled.
- Rename: `overrideMaterial` is now `useCustomMaterial`
- New member: `customMaterial`
- The following properties are now private:
  - `material` setter (getter is still public)
  - `backgroundRenderer`
- Use the `ARCameraBackground.material` getter to get the currently active material.

### Fixed

- `ARPlane.vertexChangedThreshold` is no longer allowed to be negative.
- The `ARCameraBackground` component did not reset the projection matrix on disable, leading to stretching or other distortion. This has been fixed.
- The `ARCameraBackground` component did not properly handle an overriden material. This has been fixed (see API Changes below).
- The `ARPlaneMeshGenerators` was meant to generate a triangle fan about the center of the plane. However, indices were instead generated as a fan about one of the bounary points. The visual result is similar, but can lead to long thin triangles. This has been fixed to use the plane's center point as intended.
- Update for compatibility with 2018.3
- If the `ARPlaneMeshVisualizer` has a `LineRenderer` on it, then it will be scaled to match the `ARPlane`'s scale, making the visual width invariant under differing `ARSessionOrigin` scales.
- When the `ARPointCloudManager` instantiated a point cloud prefab, it did not change its transform. If it was not identity, then feature points would appear in unexpected places. It now instantiates the point cloud prefab with identity transform.
- The menu item "GameObject > XR > AR Default Point Cloud" created a `GameObject` which used a particle system whose "Scaling Mode" was set to "Local". If used as the `ARPointCloudManager`'s point cloud prefab, this would produce odd results when the `ARSessionOrigin` was scaled. The correct setting is "Hierarchy", and the utility now creates a particle system with the correct setting.

## [1.0.0-preview.14] - 2018-07-17

### Added

- Added `ARSession.Reset()` to destroy the current AR Session and establish a new one. This destroys all trackables, such as planes.
- Added an `ARSubsystemManager.sessionDestroyed` event. The `ARPlaneManager`, `ARPointCloudManager`, and `ARReferencePointManager` subscribe to this event, and they remove their trackables when the `ARSession` is destroyed.

### Fixed

- Fixed a bug in the `ARCameraBackground` which would not render the camera texture properly if the `ARSession` had been destroyed and then recreated or if the `ARCameraBackground` had been disabled and then re-enabled.
- `ARSubsystemManager.systemState`'s setter was not private, allowing user code to change the system state. The setter is now private.
- `ARPlane.trackingState` returned a cached value, which was incorrect if the `ARSession` was no longer active.

## [1.0.0-preview.13] - 2018-07-03

### Added

- Added a getter on the `ARPointCloudManager` for the instantiated `ARPointCloud`.
- Added UVs to the `Mesh` generated by the `ARPlaneMeshVisualizer`.
- Added a `meshUpdated` event to the `ARPlaneMeshVisualizer`, which lets you customize the generated `Mesh`.
- Added AR Icons.

### Changed

- Refactored out plane mesh generation functionality into a new static class `ARPlaneMeshGenerators`.

### Fixed

- Fixed a bug where point clouds did not stop rendering when disabled.

## [1.0.0-preview.12] - 2018-06-14

### Added

- Add color correction value to `LightEstimationData`.
- Add availability and AR install support

### Changed

- Improve lifecycle reporting: remove public members `ARSubsystemManager.availability` and `ARSubsystemManager.trackingState`. Combine with `ARSubsystemManager.systemState` and the public event `ARSubsystemManager.systemStateChanged`.
- Docs improvements
- Move `ParticleSystem` to the top of the `ARDebugPointCloudVisualizer`
- Update documentation: ARSession image and written description.
- Rename `ARBackgroundRenderer` to `ARCameraBackground`
- Unify `ARSessionState` & `ARSystemState` enums
- Change dependency to `ARExtension` 1.0.0-preview.2
- Significant rework to startup lifecycle & asynchronous availability check
- Rename ARUtilities to ARFoundation
- This package is now called `ARFoundation`.
- `ARSessionOrigin` no longer requires its `Camera` to be a child of itself.

### Removed

- Remove "timeout" from AR Session
- Removed `ARPlaceOnPlane`, `ARMakeAppearOnPlane`, `ARPlaneMeshVisualizer`, and `ARPointCloudParticleVisualizer` as these were deemed sample content.
- Removed setup wizard.
- Renamed `ARRig` to `ARSessionOrigin`.


<hr>
\* *Apple and ARKit are trademarks of Apple Inc., registered in the U.S. and other countries and regions.*

