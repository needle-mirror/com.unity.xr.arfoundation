---
uid: arfoundation-whats-new
---
# What's new in version 6.2

This release includes the following significant changes:

## Other API additions

- Added constructors and `defaultValue` properties to `XRSaveAnchorResult`, `XRLoadAnchorResult`, and `XREraseAnchorResult` structs.
- Added `SerializableGuid.AsByteNativeArray` to convert a `SerializableGuid` to a `NativeArray<byte>`.

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).

### Added

- Added `parentId` field to all trackable struct types: `XRAnchor`, `XRBoundingBox`, `XREnvironmentProbe`, `XRFace`, `XRHumanBody`, `XRTrackedImage`, `XRTrackedObject`, `XRParticipant`, `BoundedPlane`, `XRPointCloud`, and `XRRaycast`. Providers can use this field to indicate scene hierarchy information.
- Changed the Transform hierarchy of trackable GameObjects spawned by AR Foundation manager components to reflect the scene hierarchy reported by the provider.
