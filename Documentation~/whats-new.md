---
uid: arfoundation-whats-new
---
# What's new in version 6.6

This release includes the following significant changes:

## XRSubsystem

- Added a new [XRSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSubsystem`3) base class that enables providers to incorporate permissions and asynchronous resource creation into their `Start` logic. If you are the owner of a subsystem or provider type, Unity recommends that you inherit the new base class instead of `SubsystemWithProvider`.
- Changed `SubsystemLifecycleManager.Start` logic to be compatible with the new `XRSubsystem` base class. This is a backwards-compatible change, so Manager components still support `SubsystemWithProvider` as the subsystem base class as well.
- Reverted a previous change to `SubsystemLifecycleManager.OnEnable`, which allowed the manager component to poll for subsystems that weren't synchronously available. With the addition of `XRSubsystem`, all providers are now expected to create subsystems synchronously during `XRLoader.Initialize`.

## Other API additions

- Added a `ToString` override for `ARMarker`.
- Added `XRRaycastSubsystemDescriptor.Cinfo.supportedTrackableTypesDelegate`, which allows a raycast provider to determine at runtime which trackable types it supports.

## UI changes

- Added support for Apple visionOS to the Build AssetBundles window (**Assets** > **AR Foundation** > **Build AssetBundles**).

## Deprecations

- Deprecated `XRPlaneSubsystem.CreateOrResizeNativeArrayIfNecessary<T>` and `XRPlaneSubsystem.Provider.CreateOrResizeNativeArrayIfNecessary<T>`. Use [NativeArrayUtils](xref:Unity.XR.CoreUtils.NativeArrayUtils) instead.
- Deprecated `XRRaycastSubsystemDescriptor.Cinfo.supportedTrackableTypes`. Use `XRRaycastSubsystemDescriptor.Cinfo.supportedTrackableTypesDelegate` instead.

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
