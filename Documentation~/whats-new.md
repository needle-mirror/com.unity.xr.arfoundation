---
uid: arfoundation-whats-new
---
# What's new in version 7.0

This release includes the following significant changes:

## Breaking changes

AR Foundation 7.0 contains breaking changes:

- `XRDepthSubsystem` and `XRDepthSubsystemDescriptor` are removed. Use `XRPointCloudSubsystem` and `XRPointCloudSubsystemDescriptor` instead.
- APIs deprecated in earlier versions are now obsolete and produce compile errors, including the per-type trackables changed events, the session availability and install APIs, `ARSessionOrigin`, the AR Debug Menu, and the Visual Scripting event nodes deprecated in AR Foundation 6.0.
- `BoundedPlane` and its related APIs are renamed to use the `XR` prefix, consistent with other AR Foundation trackable types.
- The `ARSession` component is renamed to `ARSessionManager`, consistent with other AR Foundation managers.

To upgrade your project, refer to [Upgrade to AR Foundation 7.0](xref:arfoundation-upgrade-guide-7-0).

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
