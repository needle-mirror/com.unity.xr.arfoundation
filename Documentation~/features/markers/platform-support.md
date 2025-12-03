---
uid: arfoundation-markers-platform-support
---
# Markers platform support

Discover which AR platforms support AR Foundation's marker subsystem.

The AR Foundation XRMarkerSubsystem is supported on the following platforms:

| Provider plug-in | Markers supported | Provider documentation |
| :--------------- | :---------------: | :--------------------- |
| Unity OpenXR: Spatial Entities | Yes | N/A |
| Google ARCore XR Plug-in | | |
| Apple ARKit XR Plug-in | | |
| Apple visionOS XR Plug-in | | |
| HoloLens | | |
| Unity OpenXR: Meta | Yes (via Spatial Entities) | N/A |
| Unity OpenXR: Android XR | | |
| XR Simulation | | |

## Check for support at runtime

Not all devices and platforms support every type of marker. Before building features that rely on a specific marker type, you should check whether the current device supports it. To check whether the current device supports a specific marker type, query the [XRMarkerSubsystemDescriptor.supportedMarkerTypes](xref:UnityEngine.XR.ARSubsystems.XRMarkerSubsystemDescriptor.supportedMarkerTypes) property.

`XRMarkerSubsystemDescriptor.supportedMarkerTypes` returns a [Result](xref:UnityEngine.XR.ARSubsystems.Result`1) which indicates whether the platform supports marker tracking. If the platform supports marker tracking, `XRMarkerSubsystemDescriptor.supportedMarkerTypes` returns a [ReadOnlyListSpan](xref:Unity.XR.CoreUtils.Collections.ReadOnlyListSpan`1) of the supported [XRMarkerTypes](xref:UnityEngine.XR.ARSubsystems.XRMarkerType).

The following code example shows how to check which marker types a platform supports:

[!code-cs[GetSupportedMarkerTypes](../../../Tests/Runtime/CodeSamples/ARMarkerSamples.cs#GetSupportedMarkerTypes)]
