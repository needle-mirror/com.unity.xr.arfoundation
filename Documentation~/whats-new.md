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

## Deprecations

## URP compatibility mode is removed in Unity 6.4

- In Unity 6000.4 and newer Editor versions, all methods that depend on URP Compatibility Mode have been changed from `Obsolete(false)` to `Obsolete(true)`. URP Compatibility Mode is removed in Unity 6000.4, so these APIs are no longer supported in Unity 6000.4 or newer. The following methods are affected:
  - `ARBackgroundRendererFeature.Execute`
  - `ARBackgroundRendererFeature.Configure`
  - `ARCommandBufferSupportRendererFeature.Execute`

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
