---
uid: arfoundation-bounding-box-platform-support
---
# Bounding box detection platform support

The AR Foundation [XRBoundingBoxSubsystem](xref:UnityEngine.XR.ARSubsystems.XRBoundingBoxSubsystem) is supported on the following platforms:

| Provider plug-in | Bounding box detection supported | Provider documentation |
| :--------------- | :------------------------------: | :--------------------- |
| Google ARCore XR Plug-in | | |
| Apple ARKit XR Plug-in | Yes | [Bounding box detection](xref:arkit-bounding-boxes) (ARKit) |
| Apple visionOS XR Plug-in | | |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | Yes | [Bounding boxes](xref:meta-openxr-bounding-boxes) (OpenXR Meta) |
| Unity OpenXR: Android XR | | |
| XR Simulation | Yes | N/A |

## Check for bounding box detection support

Your app can check at runtime whether a provider plug-in supports bounding box detection on the user's device. This allows you to implement branching logic such as displaying information to the user if bounding boxes are not supported.

Use the example code below to check whether the device supports bounding box detection:

[!code-cs[CheckIfBoundingBoxesLoaded](../../../Tests/Runtime/CodeSamples/LoaderUtilitySamples.cs#CheckIfBoundingBoxesLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table lists the optional features of the bounding box subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRBoundingBoxSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRBoundingBoxSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported.

| Feature                    | Descriptor Property | Description |
| :------------------------- | :------------------ | :---------- |
| **Classification** | [supportsClassifications](xref:UnityEngine.XR.ARSubsystems.XRBoundingBoxSubsystemDescriptor.supportsClassifications) | Indicates whether the provider implementation can provide a value for [ARBoundingBox.classifications](xref:UnityEngine.XR.ARFoundation.ARBoundingBox.classifications). |

<a id="optional-features-support-table"></a>

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature | ARKit | OpenXR Meta | XR Simulation |
| :------ | :---: | :---------: | :-----------: |
| **Classification** | Yes | Yes | Yes |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether a bounding box detection provider supports any optional features on the user's device. The [XRBoundingBoxSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRBoundingBoxSubsystemDescriptor) contains boolean properties for each optional feature that tell you whether they are supported.

Refer to example code below to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/ARBoundingBoxManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
