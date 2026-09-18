---
uid: arfoundation-whats-new
---
# What's new in version 6.7

This release includes the following significant changes:

## New features

### Install software via TryStartAsync

- Added [XRSubsystemStartOptions.InstallSoftwareIfNeeded](xref:UnityEngine.XR.ARSubsystems.XRSubsystemStartOptions.InstallSoftwareIfNeeded), a new start option you can pass to [TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*). On platforms that require additional runtime software, the subsystem will check whether the required software is installed and attempt to install it before starting. This is now the recommended way to handle AR software installation.

### OpenXR image tracking support

- Added support for AR Foundation image tracking via [Unity OpenXR: Spatial Entities](xref:spatialentities-openxr-manual) which allows you to identify and track images in the real world based on a list of reference images. Refer to [Image tracking](xref:spatialentities-openxr-image-tracking) (Spatial Entities) for more information.

## Other API additions

- Added `TrackableTypeExtensions`, a new static class with an `IsPlane()` extension method that tests whether a `TrackableType` value includes any plane sub-type, and an `allPlaneTypes` property that returns the combined mask of all plane sub-types.
- Added `XRResultStatus.StatusCode.LimitReached` as a new error code that AR Foundation methods can return.
- Added `XRPlaneSubsystemDescriptor.supportsPlaneSubsumption`, to allow a platform to indicate whether it will perform plane subsumption. If a platform supports plane subsumption, it sets this value to `true`, and populates `ARPlane.subsumedBy` for the subsumed plane.

## Deprecations

- The following APIs have been deprecated in AR Foundation 6.7 as part of the transition to the new install flow:
  - `ARSession.CheckAvailability` and `ARSession.Install`: To install required AR software, pass [XRSubsystemStartOptions.InstallSoftwareIfNeeded](xref:UnityEngine.XR.ARSubsystems.XRSubsystemStartOptions.InstallSoftwareIfNeeded) to [TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*) instead.
  - `ARSessionState.CheckingAvailability`, `ARSessionState.NeedsInstall`, and `ARSessionState.Installing`: These states are only set by the deprecated `ARSession.CheckAvailability` and `ARSession.Install` APIs.
  - `XRSessionSubsystem.GetAvailabilityAsync`, `XRSessionSubsystem.InstallAsync`, and corresponding `XRSessionSubsystem.Provider` methods: Pass [XRSubsystemStartOptions.InstallSoftwareIfNeeded](xref:UnityEngine.XR.ARSubsystems.XRSubsystemStartOptions.InstallSoftwareIfNeeded) to [TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*) instead.
  - `SessionAvailability` enum and `SessionAvailabilityExtensions` class: Use the [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus) returned by [TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*) instead.
  - `SessionInstallationStatus` enum: Use the [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus) returned by [TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*) instead.
  - `XRSessionSubsystemDescriptor.supportsInstall` and `Cinfo.supportsInstall`: Pass [XRSubsystemStartOptions.InstallSoftwareIfNeeded](xref:UnityEngine.XR.ARSubsystems.XRSubsystemStartOptions.InstallSoftwareIfNeeded) to [TryStartAsync](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3.TryStartAsync*) to install required software instead.
- Deprecated `TrackableType.Planes`. Use `TrackableTypeExtensions.IsPlane()` to test whether a value contains any plane sub-type, or `TrackableTypeExtensions.allPlaneTypes` to build a mask that includes all plane sub-types.
- The following APIs have been deprecated in AR Foundation 6.7 as part of the transition to asynchronous reference image library APIs:
  - The `ARTrackedImageManager.referenceLibrary` setter: Use `ARTrackedImageManager.TrySetReferenceLibraryAsync` or `ARTrackedImageManager.TrySetReferenceLibrary` instead.
  - The synchronous `CreateRuntimeLibrary` methods on `XRImageTrackingSubsystem` and `ARTrackedImageManager`: Use `TryCreateRuntimeLibraryAsync` instead.

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
