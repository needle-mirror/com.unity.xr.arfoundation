---
uid: arfoundation-plane-platform-support
---
# Plane detection platform support

Plane detection is supported on the ARCore, ARKit, Meta OpenXR and XR Simulation platforms, as shown in the table below:

| Provider plug-in | Plane detection supported | Provider documentation |
| :--------------- | :-----------------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Plane detection](xref:arcore-plane-detection) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Plane detection](xref:arkit-plane-detection) (ARKit) |
| Apple visionOS XR Plug-in | Yes | N/A |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | Yes | [Plane detection](xref:meta-openxr-planes) (Meta OpenXR) |
| XR Simulation | Yes | N/A |

## Check for plane detection support

Your app can check at runtime whether a provider plug-in supports plane detection on the user's device. This is primarily useful in situations where you are unsure if the device supports plane detection, which may be the case if you are using AR Foundation with a third-party provider plug-in.

Use the example code below to check whether the device supports plane detection:

[!code-cs[CheckIfPlanesLoaded](../../../Tests/CodeSamples/LoaderUtilitySamples.cs#CheckIfPlanesLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

Plane detection providers may choose whether to implement any of the optional features of AR Foundation's [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem), as indicated in the table below:

| Feature | Description | ARCore | ARKit | Meta OpenXR | XR Simulation |
| :------ | :---------- | :----: | :---: | :---------: | :-----------: |
| **Horizontal plane detection** | Indicates whether the provider implementation supports the detection of horizontal planes, such as the floor. | Yes | Yes | Yes | Yes |
| **Vertical plane detection** | Indicates whether the provider implementation supports the detection of vertical planes, such as walls. | Yes | iOS 11.3+ | Yes | Yes |
| **Arbitrary plane detection** | Indicates whether the provider implementation supports the detection of planes that are aligned with neither the horizontal nor vertical axes. | | | Yes | |
| **Boundary vertices** | Indicates whether the provider implementation supports boundary vertices for its planes. | Yes | Yes | Yes | Yes |
| **Classification** | Indicates whether the provider implementation can provide a value for [ARPlane.classifications](xref:UnityEngine.XR.ARFoundation.ARPlane.classifications). | | iOS 12+ | Yes | |

### Check for optional feature support

Your app can check at runtime whether a plane detection provider supports any optional features on the user's device. The [XRPlaneSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor) contains boolean properties for each optional feature that tell you whether they are supported.

Refer to example code below to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/CodeSamples/ARPlaneManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
