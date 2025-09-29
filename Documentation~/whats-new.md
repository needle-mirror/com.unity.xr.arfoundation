---
uid: arfoundation-whats-new
---
# What's new in version 6.3

This release includes the following significant changes:

## New features

### Face tracking blend shapes API

- Added [ARFaceManager.TryGetBlendShapes](xref:UnityEngine.XR.ARFoundation.ARFaceManager.TryGetBlendShapes), which providers can implement to provide information about the facial expression of a tracked face.

## Other API additions

- Added new values to the [BoundingBoxClassifications](xref:UnityEngine.XR.ARSubsystems.BoundingBoxClassifications) enum: `Keyboard`, `Mouse`, and `Laptop`.
- Added constructors and `defaultValue` properties to `XRSaveAnchorResult`, `XRLoadAnchorResult`, and `XREraseAnchorResult` structs.
- Added a constructor to `Result<T>` that enables more convenient construction of successful results.
- Added a `parentId` property to `ARTrackable` with a public getter.
- Added conversion operators between `XRResultStatus` and the new `OpenXRResultStatus` type introduced in OpenXR Plug-in 1.16.0-pre.1.
- Added a new enum member `XRResultStatus.StatusCode.Unsupported`.
- Added a constructor to `XRAnchor` that allows you to initialize all the struct fields.
- Added [XRAnchorBuilder](xref:UnityEngine.XR.ARSubsystems.XRAnchorBuilder), which provides a fluent API for constructing `XRAnchor` instances.
- Added a constructor to `TrackableChanges<T>` that allows you construct an instance given `IEnumerable<T>` input collections.
- Added a static property `XRResultStatus.unqualifiedSuccess`.

## Changes

### Ray casts are easier to support

- Changed [ARRaycastManager](xref:arfoundation-raycasts-raycastmanager) to always support physics-based ray casts as a fallback implementation, even if the provider doesn't implement the `XRRaycastSubsystem`. For more information, refer to [Fallback ray casts](xref:arfoundation-raycasts-raycastmanager#fallback-ray-casts).

### Trackable GameObject hierarchies

- Changed how trackable GameObjects are spawned to support parent-child hierarchies of any depth, allowing trackable GameObjects to have other trackable GameObjects as their parent.

## Deprecations

### URP compatibility mode

- Deprecated everything associated with URP Compatibility Mode, as URP Compatibility Mode is now hidden by default in Unity 6.3. Refer to [Render Graph Updates in Unity 6.3](https://discussions.unity.com/t/render-graph-updates-in-unity-6-3/1668122) (Unity Discussions) for more information. The following methods are deprecated:
* `ARBackgroundRendererFeature.Execute`
* `ARBackgroundRendererFeature.Configure`
* `ARCommandBufferSupportRendererFeature.Execute`

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
