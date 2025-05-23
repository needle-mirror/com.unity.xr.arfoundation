---
uid: arfoundation-bounding-box-platform-support
---
# Bounding box detection platform support

Bounding box detection is supported on ARKit and OpenXR Meta, as shown in the following table:

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

Bounding box detection providers may choose whether to implement the optional features of AR Foundation's [XRBoundingBoxSubsystem](xref:UnityEngine.XR.ARSubsystems.XRBoundingBoxSubsystem), as indicated in the table below:

| Feature | Description | ARCore | ARKit | OpenXR Meta | XR Simulation |
| :------ | :---------- | :----: | :---: | :---------: | :-----------: |
| **Classification** | Indicates whether the provider implementation can provide a value for [ARBoundingBox.classifications](xref:UnityEngine.XR.ARFoundation.ARBoundingBox.classifications). | | | | |

### Check for optional feature support

Your app can check at runtime whether a bounding box detection provider supports any optional features on the user's device. The [XRBoundingBoxSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRBoundingBoxSubsystemDescriptor) contains boolean properties for each optional feature that tell you whether they are supported.

Refer to example code below to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/ARBoundingBoxManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
