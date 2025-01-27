---
uid: arfoundation-simulation-image-tracking
---
# Image tracking

This page is a supplement to the AR Foundation [Image tracking](xref:arfoundation-image-tracking) manual. The following sections only contain information about APIs where XR Simulation exhibits unique platform-specific behavior.

[!include[](../../snippets/arf-docs-tip.md)]

## Optional feature support

XR Simulation implements the following optional features of AR Foundation's [XRImageTrackingSubsystem](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystem):

| Feature | Descriptor Property | Supported |
| :------ | :--------------- | :----------: |
| **Moving images** | [supportsMovingImages](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.supportsMovingImages) | Yes |
| **Requires physical image dimensions** | [requiresPhysicalImageDimensions](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.requiresPhysicalImageDimensions) | |
| **Mutable library** | [supportsMutableLibrary](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.supportsMutableLibrary) | Yes |
| **Image validation** | [supportsImageValidation](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.supportsImageValidation) | Yes |
