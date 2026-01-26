---
uid: arfoundation-whats-new
---
# What's new in version 6.5

This release includes the following significant changes:

## API additions

- Added `BoundedPlaneBuilder.WithPose` overload that takes an OpenXR `XrPosef` if your app uses OpenXR Plug-in 1.16.0 or newer.
- Added `XRAnchorBuilder.FromAnchor`, which enables you to initialize all fields of an `XRAnchorBuilder` from a given anchor.
- Added `XRResultStatus.StatusCode.NotFound` and `XRResultStatus.StatusCode.NotTracking` as a new possible error codes for AR Foundation operations.
- Added an implicit operator to convert from `XrUuid` to `SerializableGuid` if your project uses OpenXR Plug-in 1.16.0 or newer.
- Added an `IEquatable<XrUuid>` implementation to `SerializableGuid` if your project uses OpenXR Plug-in 1.16.0 or newer.
- Added `XRResultStatus.HasNativeStatusCode` as a convenience method to determine whether a native status code is present.

## Behavior changes

### Allowing asynchronous subsytem creation

- Changed `SubsystemLifecycleManager.OnEnable` to poll for a number of seconds until a subsytem is created before it disables itself, allowing subsystems to be created asynchronously and still connect with their managers.

## Deprecations

### Other deprecations

- Deprecated and replaced the following properties of [XRPlaneSubsystemDescriptor.Cinfo](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor.Cinfo) to allow providers to determine at runtime whether AR Foundation plane APIs are supported:
  - `supportsBoundaryVertices` to `supportsBoundaryVerticesDelegate`
  - `supportsClassification` to `supportsClassificationDelegate`

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
