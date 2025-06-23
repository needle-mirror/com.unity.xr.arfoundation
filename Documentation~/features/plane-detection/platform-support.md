---
uid: arfoundation-plane-platform-support
---
# Plane detection platform support

The AR Foundation [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem) is supported on the following platforms:

| Provider plug-in | Plane detection supported | Provider documentation |
| :--------------- | :-----------------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Plane detection](xref:arcore-plane-detection) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Plane detection](xref:arkit-plane-detection) (ARKit) |
| Apple visionOS XR Plug-in | Yes | N/A |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | Yes | [Plane detection](xref:meta-openxr-planes) (OpenXR Meta) |
| Unity OpenXR: Android XR | Yes | [Plane detection](xref:androidxr-openxr-plane-detection) (Android XR) |
| XR Simulation | Yes | N/A |

## Check for plane detection support

Your app can check at runtime whether a provider plug-in supports plane detection on the user's device. This is primarily useful in situations where you are unsure if the device supports plane detection, which may be the case if you are using AR Foundation with a third-party provider plug-in.

Use the example code below to check whether the device supports plane detection:

[!code-cs[CheckIfPlanesLoaded](../../../Tests/Runtime/CodeSamples/LoaderUtilitySamples.cs#CheckIfPlanesLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table lists the optional features of the plane subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRPlaneSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported.

> [!TIP]
> Check the API documentation to understand how each API behaves when the provider doesn't support a feature. For example, some properties throw an exception if you try to set the value when the feature isn't supported.

| Feature | Descriptor Property | Description |
| :------ | :------------------ | :---------- |
| **Horizontal plane detection** | [supportsHorizontalPlaneDetection](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor.supportsHorizontalPlaneDetection) |Indicates whether the provider implementation supports the detection of horizontal planes, such as the floor. |
| **Vertical plane detection** | [supportsVerticalPlaneDetection](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor.supportsVerticalPlaneDetection) | Indicates whether the provider implementation supports the detection of vertical planes, such as walls. |
| **Arbitrary plane detection** | [supportsArbitraryPlaneDetection](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor.supportsArbitraryPlaneDetection) | Indicates whether the provider implementation supports the detection of planes that are aligned with neither the horizontal nor vertical axes. |
| **Boundary vertices** | [supportsBoundaryVertices](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor.supportsBoundaryVertices) | Indicates whether the provider implementation supports boundary vertices for its planes. |
| **Classification** | [supportsClassification](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor.supportsClassification) | Indicates whether the provider implementation can provide a value for [ARPlane.classifications](xref:UnityEngine.XR.ARFoundation.ARPlane.classifications). |

<a id="optional-features-support-table"></a>

### Optional features support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature | ARCore | ARKit | visionOS | OpenXR Meta | Android XR | XR Simulation |
| :------ | :----: | :---: | :------: | :---------: | :--------: |:-----------: |
| **Horizontal plane detection** | Yes | Yes | |Yes | Yes | Yes |
| **Vertical plane detection** | Yes | iOS 11.3+ | Yes | Yes | Yes | Yes |
| **Arbitrary plane detection** | | | | Yes | Yes | |
| **Boundary vertices** | Yes | Yes | Yes | Yes | Yes | Yes |
| **Classification** | | iOS 12+ | Yes |Yes | Yes | |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether a plane detection provider supports any optional features on the user's device. The [XRPlaneSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystemDescriptor) contains boolean properties for each optional feature that tell you whether they are supported.

Refer to example code below to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/ARPlaneManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
