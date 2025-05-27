---
uid: arfoundation-simulation-session
---
# Session

This page is a supplement to the AR Foundation [Session](xref:arfoundation-session) manual. The following sections only contain information about APIs where XR Simulation exhibits unique platform-specific behavior.

[!include[](../../snippets/arf-docs-tip.md)]

## Optional feature support

ARCore implements the following optional features of AR Foundation's [XRSessionSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem):

| Feature | Descriptor Property | Supported |
| :------ | :--------------- | :--------: |
| **Install** | [supportsInstall](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsInstall) | |
| **Match frame rate** | [supportsMatchFrameRate](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsMatchFrameRate) | |

> [!NOTE]
> Refer to AR Foundation [Session platform support](xref:arfoundation-session-platform-support) for more information on the optional features of the session subsystem.
