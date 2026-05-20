---
uid: arfoundation-object-tracking-platform-support
---
# Object tracking platform support

Find out which platforms support AR Foundation object tracking.

The AR Foundation [XRObjectTrackingSubsystem](xref:UnityEngine.XR.ARSubsystems.XRObjectTrackingSubsystem) is supported on the following platforms:

| **Provider plug-in** | **Object tracking supported** | **Provider documentation** |
| :------------------- | :---------------------------: | :------------------------- |
| Google ARCore XR Plug-in | | |
| Apple ARKit XR Plug-in | Yes | [Object tracking](xref:arkit-object-tracking) (ARKit) |
| Apple visionOS XR Plug-in | Yes | [Object tracking](xref:psl-vos-unbounded-samples#object-tracking) (Apple visionOS XR Plug-in) |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | | |
| Unity OpenXR: Android XR | | |
| XR Simulation | | |

## Check for object tracking support

Your app can check at runtime whether a provider plug-in supports object tracking on the user's device. Use the following example code to check whether the device supports object tracking:

[!code-cs[CheckIfObjectTrackingLoaded](../../../Tests/Runtime/CodeSamples/LoaderUtilitySamples.cs#CheckIfObjectTrackingLoaded)]

[!include[](../../snippets/initialization.md)]

## Additional resources

* [Configure a reference object library](xref:arfoundation-object-tracking-reference-objects)
* [AR Tracked Object Manager component](xref:arfoundation-object-tracking-manager)

[!include[](../../snippets/apple-arkit-trademark.md)]
