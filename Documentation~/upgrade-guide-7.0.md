---
uid: arfoundation-upgrade-guide-7-0
---
# Upgrade to AR Foundation 7.0

Upgrade your project to AR Foundation 7.0.

AR Foundation 7.0 contains breaking changes which require action to successfully upgrade your project when you [upgrade the package](xref:um-upm-ui-update) from previous versions.

The following sections outline the changes you need to make to your project to use AR Foundation 7.0.

## BoundedPlane renamed to XRPlane

AR Foundation 7.0 renames `BoundedPlane` to [XRPlane](xref:UnityEngine.XR.ARSubsystems.XRPlane), so that every AR Foundation trackable type uses the `XR` prefix. This release also renames the following related APIs:

| Previous API | Replacement |
| :----------- | :---------- |
| `BoundedPlane` | [XRPlane](xref:UnityEngine.XR.ARSubsystems.XRPlane) |
| `BoundedPlaneBuilder` | [XRPlaneBuilder](xref:UnityEngine.XR.ARSubsystems.XRPlaneBuilder) |
| `BoundedPlaneBuilder.FromBoundedPlane` | [XRPlaneBuilder.FromPlane](xref:UnityEngine.XR.ARSubsystems.XRPlaneBuilder.FromPlane(UnityEngine.XR.ARSubsystems.XRPlane)) |

The previous APIs are obsolete as errors. If your project's scripts use them and you don't upgrade, your project doesn't compile and the Unity Console displays error `CS0619`.

To upgrade your scripts, let the Unity API Updater replace the previous APIs with their replacements. To do this:

1. Back up your project, or make sure your changes are committed to version control.
2. Upgrade the AR Foundation package to version 7.0.
3. When Unity displays the **API Update Required** dialog, select **I Made a Backup. Go Ahead!**. The API Updater rewrites your scripts to use the replacement APIs.

If you dismissed the dialog, go to **Assets** &gt; **Run API Updater** to run the API Updater again.

### Provider plug-ins

If you implement a provider plug-in, your [XRPlaneSubsystem.Provider](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem.Provider) implementation must override [GetChanges(XRPlane, Allocator)](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem.Provider.GetChanges(UnityEngine.XR.ARSubsystems.XRPlane,Unity.Collections.Allocator)) instead of `GetChanges(BoundedPlane, Allocator)`.

The API Updater can't modify scripts in packages that Unity installs from a package registry, because those packages are read-only. If your provider is distributed as a package, update its source code and release a new version of your package that depends on AR Foundation 7.0. To do this, change your override as follows:

```csharp
// Before
public override TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator)

// After
public override TrackableChanges<XRPlane> GetChanges(XRPlane defaultPlane, Allocator allocator)
```

## ARSession renamed to ARSessionManager

AR Foundation 7.0 renames the `ARSession` component to [ARSessionManager](xref:UnityEngine.XR.ARFoundation.ARSessionManager), so that the component follows the same naming convention as other AR Foundation managers. The component's behavior doesn't change, and it still appears as **AR Session** in the **Add Component** and **GameObject** menus.

`ARSession` is obsolete as an error. If your project's scripts use `ARSession`, including its static members such as `ARSession.state` and `ARSession.stateChanged`, and you don't upgrade, your project doesn't compile and the Unity Console displays error `CS0619`.

To upgrade your scripts, use the Unity API Updater to replace `ARSession` with `ARSessionManager`. To do this:

1. Back up your project, or make sure your changes are committed to version control.
2. Upgrade the AR Foundation package to version 7.0.
3. When Unity displays the **API Update Required** dialog, select **I Made a Backup. Go Ahead!**. The API Updater rewrites your scripts to use `ARSessionManager`.

Your scenes and prefabs don't need to change. AR Session components in your scenes and prefabs keep working, because the component keeps the same script reference.

## Depth subsystem is replaced by the point cloud subsystem

AR Foundation 7.0 removes `XRDepthSubsystem` and `XRDepthSubsystemDescriptor`. These APIs were deprecated in earlier versions. If your project references them, it won't compile.

To upgrade, replace `XRDepthSubsystem` with [`XRPointCloudSubsystem`](xref:UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem), and `XRDepthSubsystemDescriptor` with [`XRPointCloudSubsystemDescriptor`](xref:UnityEngine.XR.ARSubsystems.XRPointCloudSubsystemDescriptor).

## Per-type trackables changed events are obsolete

Each trackable manager's own changed event, such as `ARPlaneManager.planesChanged`, and its event arguments type, such as `ARPlanesChangedEventArgs`, were deprecated in AR Foundation 6.0. They're obsolete in 7.0 and produce compile errors.

To upgrade, subscribe to the manager's [`trackablesChanged`](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged) event instead. Its argument is [`ARTrackablesChangedEventArgs<TTrackable>`](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1), which has `added`, `updated`, and `removed` collections.

| Previous API | Replacement |
| :-- | :-- |
| `ARAnchorManager.anchorsChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARAnchor>` |
| `AREnvironmentProbeManager.environmentProbesChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<AREnvironmentProbe>` |
| `ARFaceManager.facesChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARFace>` |
| `ARHumanBodyManager.humanBodiesChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARHumanBody>` |
| `ARParticipantManager.participantsChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARParticipant>` |
| `ARPlaneManager.planesChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARPlane>` |
| `ARPointCloudManager.pointCloudsChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARPointCloud>` |
| `ARTrackedImageManager.trackedImagesChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARTrackedImage>` |
| `ARTrackedObjectManager.trackedObjectsChanged` | `trackablesChanged` with `ARTrackablesChangedEventArgs<ARTrackedObject>` |

`trackablesChanged` is a `UnityEvent`, so use `AddListener` and `RemoveListener` instead of `+=` and `-=`. The `removed` collection contains `KeyValuePair<TrackableId, TTrackable>` entries instead of trackables.

For example, to upgrade a plane handler:

```csharp
// Before
planeManager.planesChanged += OnPlanesChanged;
void OnPlanesChanged(ARPlanesChangedEventArgs args) { /* ... */ }

// After
planeManager.trackablesChanged.AddListener(OnPlanesChanged);
void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args) { /* ... */ }
```

`ARTrackableManager.OnTrackablesChanged` and its override on `ARPointCloudManager` were also deprecated in AR Foundation 6.0, and are obsolete in 7.0.

## Session availability and install APIs are obsolete

The coroutine-based availability check and install workflow was deprecated in AR Foundation 6.7. Most of its APIs are obsolete in 7.0, including the following:

- `ARSessionState.CheckingAvailability`, `NeedsInstall`, and `Installing`
- `SessionAvailability`, `SessionAvailabilityExtensions`, and `SessionInstallationStatus`
- `XRSessionSubsystem.GetAvailabilityAsync` and `XRSessionSubsystem.InstallAsync`
- `XRSessionSubsystemDescriptor.supportsInstall`

If your project uses any of these, it won't compile.

`ARSessionManager.CheckAvailability` and `ARSessionManager.Install` remain deprecated, not obsolete. If your project uses them, it still compiles, but the Unity Console displays deprecation warnings.

To upgrade, call `TryStartAsync` on the session subsystem, and pass `XRSubsystemStartOptions.InstallSoftwareIfNeeded` to check for and install required AR software. Call `XRResultStatus.IsSuccess` on the result of `TryStartAsync`. For more information and a code example, refer to [Install XR software on mobile devices](xref:arfoundation-session-platform-support#install).

If your project uses `SessionInstallationStatus` to check the result of an install, use the `XRResultStatus` from a platform-specific `TryInstallAsync` method instead. This method returns an `Awaitable<XRResultStatus>`, so await it to get the result.

## AR Session Origin and AR Pose Driver are obsolete

`ARSessionOrigin` and `ARPoseDriver` are obsolete. If your project uses them, it won't compile.

To upgrade:

1. Replace the **AR Session Origin** component with the **XR Origin** component from XR Core Utils. Replace `camera` with `Camera`, and `trackablesParent` with `TrackablesParent`.
2. Replace the **AR Pose Driver** component with the Input System **Tracked Pose Driver** component.

## Camera, occlusion, and shader keyword APIs are obsolete

The APIs in the following table were deprecated in earlier versions and are obsolete in 7.0. If your project uses them, it won't compile. To upgrade, replace each API with its replacement in the table.

| Previous API | Replacement |
| :-- | :-- |
| `XRCameraFrame` value properties, such as `timestampNs` and `averageBrightness`, and their `has` properties, such as `hasTimestamp` | The matching `TryGet` method, such as `TryGetTimestamp` |
| `XRCameraSubsystem.GetMaterialKeywords` and `GetShaderKeywords`, and the same methods on `XROcclusionSubsystem` | `GetShaderKeywords2` |
| `ShaderKeywords` | `XRShaderKeywords` |
| `ARCameraFrameEventArgs` and `AROcclusionFrameEventArgs` keyword properties, such as `enabledShaderKeywords` | `shaderKeywords` |
| `AROcclusionFrameEventArgs.textures` and `propertyNameIds` | `externalTextures` |
| `AROcclusionManager.environmentDepthTexture` and `environmentDepthConfidenceTexture` | `TryGetEnvironmentDepthTexture` and `TryGetEnvironmentDepthConfidenceTexture` |
| `XRTextureDescriptor.dimension` and the constructor that takes a `TextureDimension` | `textureType` and the constructor that takes an `XRTextureType` |
| `LightEstimationMode` | The `Feature` flags enum |

`LightEstimationExtensions.ToLightEstimation(LightEstimationMode)` and `LightEstimationExtensions.ToLightEstimationMode(LightEstimation)` are also obsolete.

## Plane, raycast, mesh, and image tracking APIs are obsolete

The APIs in the following table were deprecated in earlier versions and are obsolete in 7.0. If your project uses them, it won't compile. To upgrade, replace each API with its replacement in the table.

| Previous API | Replacement |
| :-- | :-- |
| `PlaneClassification`, `PlaneClassificationExtensions`, `ARPlane.classification`, and `XRPlane.classification` | `PlaneClassifications` and `classifications` |
| `XRPlane` constructor that takes a `PlaneClassification` | `XRPlane` constructor that takes `PlaneClassifications` |
| `XRPlaneSubsystem.CreateOrResizeNativeArrayIfNecessary` | `NativeArrayUtils` from XR Core Utils |
| `ARPlaneMeshGenerators` | `ARPlaneMeshGenerator`, for example `TryGenerateMesh` |
| `TrackableType.Planes` | `TrackableTypeExtensions.allPlaneTypes` or `TrackableTypeExtensions.IsPlane` |
| `TrackableType.All` | `TrackableType.AllTypes` |
| `ARRaycastHit(XRRaycastHit, float, Transform)` | `ARRaycastHit(XRRaycastHit, float, Transform, ARTrackable)` |
| `ARMeshManager.meshesChanged` | `meshInfosChanged` |
| `ARTrackedImageManager.referenceLibrary` setter | `TrySetReferenceLibraryAsync` or `TrySetReferenceLibrary` |
| `ARTrackedImageManager.CreateRuntimeLibrary` and `XRImageTrackingSubsystem.CreateRuntimeLibrary` | `TryCreateRuntimeLibraryAsync` |
| `MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageJob` | `ScheduleAddImageWithValidationJob` |

## Provider registration APIs are obsolete

This section applies if you implement a provider plug-in. The older ways to register subsystem descriptors were deprecated in earlier versions and are obsolete in 7.0. If your provider uses them, it won't compile. This covers the following APIs:

- The `*Cinfo` structs and `FaceSubsystemParams`
- The `Create` and `RegisterDescriptor` methods
- The `Capabilities` types on the object tracking and participant descriptors
- Some capability properties on the anchor, plane, and raycast `Cinfo` structs

To upgrade a provider:

1. Replace each top-level `*Cinfo` struct, such as `XRCameraSubsystemCinfo`, with the descriptor's nested `Cinfo` struct, such as `XRCameraSubsystemDescriptor.Cinfo`.
2. Replace `Create`, `RegisterDescriptor`, and the subsystem `Register` methods with the descriptor's `Register(Cinfo)` method.
3. Replace each obsolete capability property with its delegate equivalent.

The capability properties in the following table are obsolete. Other capability properties on these `Cinfo` structs aren't affected.

| Previous API | Replacement |
| :-- | :-- |
| `XRAnchorSubsystemDescriptor.Cinfo.supportsTrackableAttachments` | `supportsTrackableAttachmentsDelegate` |
| `XRAnchorSubsystemDescriptor.Cinfo.supportsSaveAnchor` | `supportsSaveAnchorDelegate` |
| `XRAnchorSubsystemDescriptor.Cinfo.supportsLoadAnchor` | `supportsLoadAnchorDelegate` |
| `XRAnchorSubsystemDescriptor.Cinfo.supportsEraseAnchor` | `supportsEraseAnchorDelegate` |
| `XRAnchorSubsystemDescriptor.Cinfo.supportsGetSavedAnchorIds` | `supportsGetSavedAnchorIdsDelegate` |
| `XRPlaneSubsystemDescriptor.Cinfo.supportsBoundaryVertices` | `supportsBoundaryVerticesDelegate` |
| `XRPlaneSubsystemDescriptor.Cinfo.supportsClassification` | `supportsClassificationDelegate` |
| `XRRaycastSubsystemDescriptor.Cinfo.supportedTrackableTypes` | `supportedTrackableTypesDelegate`, which returns a `TrackableType` |

## AR Debug Menu is obsolete

`ARDebugMenu`, `DebugSlider`, and `ToolButton` are obsolete. If your project uses them, it won't compile.

For a session configuration debug visualizer, import the **Debug Visualizers** sample from **Package Manager** > **AR Foundation** > **Samples**. `DebugSlider` and `ToolButton` have no replacement.

## Visual Scripting event nodes are obsolete

The Visual Scripting event units, event unit descriptors, and message listeners that were deprecated in AR Foundation 6.0 are obsolete in 7.0. Examples are `PlanesChangedEventUnit` and `ARPlaneManagerListener`. If your project uses them, it won't compile.

## XR Simulation settings are obsolete

`XRSimulationSettings` and `SimulationBuildProcessor` are obsolete. If your project references them, it won't compile.

To upgrade, remove references to these APIs from your project. To do this:

1. Remove references to `SimulationBuildProcessor`, which was never used.
2. Remove references to `XRSimulationSettings`, including `XRSimulationSettings.currentSettings` and `XRSimulationSettings.k_SettingsKey`. The replacement settings class is internal, so your code can't access it.

## Built-in Render Pipeline support is removed

The Built-in Render Pipeline is not supported in Unity 7000.0. AR Foundation 7.0 removes its Built-in Render Pipeline code paths for camera background rendering and XR Simulation camera texture readback.

To upgrade:

1. Set up your project to use the Universal Render Pipeline, and add the **AR Background Renderer Feature** to your renderer. Refer to [Universal Render Pipeline](xref:arfoundation-universal-render-pipeline) for instructions.
2. Remove references to and overrides of the obsolete APIs in the following table. Code that references them won't compile. Overrides still compile with a warning, but AR Foundation no longer calls them.

| Obsolete API | Upgrade action |
| :-- | :-- |
| `ARCameraBackground.legacyCameraEvents` | Remove any overrides. AR Foundation no longer uses this property. |
| `ARCameraBackground.ConfigureLegacyCommandBuffer` | Remove any overrides. AR Foundation no longer calls this method. To customize background rendering, use a custom material with `ARCameraBackground.customMaterial`. |
| `ARCameraBackground.OnCameraRenderingModeChanged` | Remove any subscriptions. AR Foundation no longer invokes this delegate. To react to rendering mode changes, read `ARCameraBackground.currentRenderingMode` or `ARCameraManager.currentRenderingMode`. |
| `ARRenderingUtils.useLegacyRenderPipeline` | Use `GraphicsSettings.currentRenderPipeline` to check whether a Scriptable Render Pipeline is active. |

## Additional resources

- [What's new in version 7.0](xref:arfoundation-whats-new)
- [API Updater](https://docs.unity3d.com/Manual/APIUpdater.html) (Unity Manual)
- [Implement a provider](xref:arfoundation-implement-a-provider)
- [Changelog](xref:arfoundation-changelog)
