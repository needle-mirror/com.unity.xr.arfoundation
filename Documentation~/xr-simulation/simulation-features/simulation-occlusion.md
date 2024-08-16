---
uid: arfoundation-simulation-occlusion
---
# Occlusion

This page is a supplement to the AR Foundation [Occlusion](xref:arfoundation-occlusion) manual. The following sections only contain information about APIs where XR Simulation exhibits unique platform-specific behavior.

[!include[](../../snippets/arf-docs-tip.md)]

## Optional feature support

XR Simulation implements the following optional features of AR Foundation's [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem):

| Feature | Descriptor Property | Supported |
| :------ | :--------------- | :----------: |
| **Environment Depth Image** | [environonmentDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthImageSupported) | Yes |
| **Environment Depth Confidence Image** | [environmentDepthConfidenceImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthConfidenceImageSupported) |  |
| **Environment Depth Temporal Smoothing** | [environmentDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthImageSupported) |  |
| **Human Segmentation Stencil Image** | [humanSegmentationStencilImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationStencilImageSupported) |  |
| **Human Segmentation Depth Image** | [humanSegmentationDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationDepthImageSupported) |  |

> [!NOTE]
> Refer to AR Foundation [Occlusion platform support](xref:arfoundation-occlusion-platform-support) for more information
> on the optional features of the occlusion subsystem.
