---
uid: arfoundation-whats-new
---
# What's new in version 6.4

This release includes the following significant changes:

## New features

### AR markers

- We added new types that can be used by provider plug-ins to implement support for tracking and decoding markers such as QR codes in mixed reality apps. Currently Unity doesn't support any provider plug-ins that implement this feature, but we expect to release more support for these APIs in the future. Refer to [Markers](xref:arfoundation-markers) for more information about how to build an app that uses these APIs.

### Mesh classification API

- Added [ARMeshManager.TryGetSubmeshClassifications](xref:UnityEngine.XR.ARFoundation.ARMeshManager.TryGetSubmeshClassifications), which allows you to retrieve semantic classifications for mesh components on supported platforms, using the new [XRMeshSubsystem](xref:UnityEngine.XR.XRMeshSubsystem) with submesh classification pipeline.

## Other API additions

- Added a new virtual [Raycast](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystem.Provider.Raycast(UnityEngine.XR.ARSubsystems.XRRaycastHit,UnityEngine.Ray,UnityEngine.XR.ARSubsystems.TrackableType,Unity.Collections.Allocator,System.Single)) function to XRRaycastSubsystem that takes a float maxDistance argument, that will only return ray cast hits within that distance.
- Added [XRBoundingBoxBuilder](xref:UnityEngine.XR.ARSubsystems.XRBoundingBoxBuilder), [BoundedPlaneBuilder](xref:UnityEngine.XR.ARSubsystems.BoundedPlaneBuilder), and [XRMarkerBuilder](xreef:UnityEngine.XR.ARSubsystems.XRMarkerBuilder) which provide fluent APIs for constructing `XRBoundingBox`, `BoundedPlane`, and `XRMarker` instances, respectively.
- Added `InnerWallFace` semantic label to [PlaneClassifications](xref:UnityEngine.XR.ARSubsystems.PlaneClassifications).
- Added support for the `IEquatable<XRResultStatus>` and `IEquatable<Result<T>>` interfaces to [Result\<T\>](xref:UnityEngine.XR.ARSubsystems.Result`1).
- Added a `ToString` override to [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus).
- Added a `ToString` override to [Result\<T\>](xref:UnityEngine.XR.ARSubsystems.Result`1).
- Added an implicit conversion operator from [SerializableGuid](xref:UnityEngine.XR.ARSubsystems.SerializableGuid) to [XrUuid](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.16/api/UnityEngine.XR.OpenXR.NativeTypes.XrUuid.html) if your project contains OpenXR Plug-in 1.16.0 or newer.

## Deprecations

### URP compatibility mode is removed in Unity 6.4

- In Unity 6000.4 and newer Editor versions, all methods that depend on URP Compatibility Mode have been changed from `Obsolete(false)` to `Obsolete(true)`. URP Compatibility Mode is removed in Unity 6000.4, so these APIs are no longer supported in Unity 6000.4 or newer. The following methods are affected:
  - `ARBackgroundRendererFeature.Execute`
  - `ARBackgroundRendererFeature.Configure`
  - `ARCommandBufferSupportRendererFeature.Execute`

### Other deprecations

- Deprecated and replaced the following properties of [XRAnchorSubsystemDescriptor.Cinfo](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.Cinfo) to allow providers to determine at runtime whether AR Foundation persistent anchor APIs are supported:
  - `supportsSaveAnchor` to `supportsSaveAnchorDelegate`
  - `supportsLoadAnchor` to `supportsLoadAnchorDelegate`
  - `supportsEraseAnchor` to `supportsEraseAnchorDelegate`
  - `supportsGetSavedAnchorIds` to `supportsGetSavedAnchorIdsDelegate`

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
