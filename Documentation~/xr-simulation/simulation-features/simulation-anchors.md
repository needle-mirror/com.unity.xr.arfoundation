---
uid: arfoundation-simulation-anchors
---
# Anchors

This page is a supplement to the AR Foundation [Anchors](xref:arfoundation-anchors) manual. The following sections only contain information about APIs where XR Simulation exhibits unique platform-specific behavior.

[!include[](../../snippets/arf-docs-tip.md)]

## Optional feature support

XR Simulation implements the following optional features of AR Foundation's [XRAnchorSubsystem](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem):

| Feature | Descriptor Property | Supported |
| :------ | :------------------ | :-------: |
| **Trackable attachments** | [supportsTrackableAttachments](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsTrackableAttachments) | Yes |
| **Synchronous add** | [supportsSynchronousAdd](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSynchronousAdd) |  |
| **Save anchor** | [supportsSaveAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSaveAnchor) |  |
| **Load anchor** | [supportsLoadAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsLoadAnchor) |  |
| **Erase anchor** | [supportsEraseAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsEraseAnchor) |  |
| **Get saved anchor IDs** | [supportsGetSavedAnchorIds](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsGetSavedAnchorIds) |  |
| **Async cancellation** | [supportsAsyncCancellation](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsAsyncCancellation) |  |

> [!NOTE]
> Refer to AR Foundation [Anchors platform support](xref:arfoundation-anchors-platform-support) for more information on the optional features of the anchor subsystem.

## Simulated anchors

XR Simulation contains a Simulated Anchor component that you can add to your XR Simulation environments. Add the Simulated Anchor component to a GameObject in your environment to designate the GameObject as a simulated anchor. XR Simulation adds simulated anchors at runtime using the anchor subsystem's [load existing anchors](xref:arfoundation-anchors-persistent#load-anchor) functionality.

Refer to [Simulated anchor component](xref:arfoundation-simulation-environments#simulated-anchor-component) for more information on simulated anchors, and [XR simulation environments](xref:arfoundation-simulation-environments) to understand how to add a simulated anchor to your XR Simulation environment.
