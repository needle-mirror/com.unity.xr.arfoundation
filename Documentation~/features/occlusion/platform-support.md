---
uid: arfoundation-occlusion-platform-support
---
# Occlusion platform support

The AR Foundation [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem) is supported on the following platforms:

| Provider plug-in | Occlusion supported | Provider documentation |
| :--------------- | :---------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Occlusion](xref:arcore-occlusion) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Occlusion](xref:arkit-occlusion) (ARKit) |
| Apple visionOS XR Plug-in | | |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | Yes | [Occlusion](xref:meta-openxr-occlusion) (OpenXR Meta) |
| Unity OpenXR: Android XR | Yes | [Occlusion](xref:androidxr-openxr-occlusion) (Android XR) |
| XR Simulation | Yes | [Occlusion](xref:arfoundation-simulation-occlusion) (XR Simulation) |

## Check for occlusion support

Your app can check at runtime whether a provider plug-in supports occlusion on the user's device.

Use the example code below to check whether the device supports occlusion:

[!code-cs[CheckIfOcclusionLoaded](../../../Tests/Runtime/CodeSamples/LoaderUtilitySamples.cs#CheckIfOcclusionLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table lists the optional features of the occlusion subsystem. Each optional feature is defined by a **Descriptor Property** of the [XROcclusionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported. Refer to [AR Occlusion Manager component](xref:arfoundation-occlusion-manager) for more information about the types of depth image described.

| Feature | Descriptor Property | Description  |
| :------ | :---------- | :------------ |
| **Environment Depth Confidence Image** | [environmentDepthConfidenceImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthConfidenceImageSupported) | Whether the subsystem supports environment depth confidence image. |
| **Environment Depth Image** | [environmentDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthImageSupported) | Whether the subsystem supports environment depth image. |
| **Environment Depth Temporal Smoothing** | [environmentDepthTemporalSmoothingSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthTemporalSmoothingSupported) | Whether temporal smoothing of the environment image is supported. |
| **Human Segmentation Depth Image** | [humanSegmentationDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationDepthImageSupported) | Whether a subsystem supports human segmentation depth image. |
| **Human Segmentation Stencil Image** | [humanSegmentationStencilImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationStencilImageSupported) | Whether a subsystem supports human segmentation stencil image. |

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature | ARCore | ARKit | OpenXR Meta | Android XR | XR Simulation |
| :------ | :----: | :---: | :---------: | :------- : | :-----------: |
| **Environment Depth Confidence Image** | Yes | Yes | | Yes | |
| **Environment Depth Image** | Yes | Yes | Yes | Yes | Yes |
| **Environment Depth Temporal Smoothing** | Yes | Yes | | Yes | |
| **Human Segmentation Depth Image** | | Yes | | | |
| **Human Segmentation Stencil Image** | | Yes | | | |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether an occlusion provider supports any optional features on the user's device. The [XROcclusionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor) contains properties for each optional feature that tell you whether they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/AROcclusionManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
