---
uid: arfoundation-image-tracking-platform-support
---
# Image tracking platform support

Learn about the features of image tracking in AR Foundation and which platforms support them.

The AR Foundation [XRImageTrackingSubsystem](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystem) is supported on the following platforms:

| Provider plug-in | Image tracking supported | Provider documentation |
| :--------------- | :-----------------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Image tracking](xref:arcore-image-tracking) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Image tracking](xref:arkit-image-tracking) (ARKit) |
| Apple visionOS XR Plug-in | Yes |  N/A |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | | |
| Unity OpenXR: Android XR | | |
| XR Simulation | Yes | [Image tracking](xref:arfoundation-simulation-image-tracking) (XR Simulation) |

## Check for image tracking support

Your app can check at runtime whether a provider plug-in supports image tracking on the user's device. Use the following example code to check whether the device supports image tracking:

[!code-cs[CheckIfImageTrackingLoaded](../../../Tests/Runtime/CodeSamples/LoaderUtilitySamples.cs#CheckIfImageTrackingLoaded)]

[!include[](../../snippets/initialization.md)]

<a id="optional-features"/>

## Optional features

The following table lists the optional features of the image tracking subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRImageTrackingSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported.

| Feature | Descriptor Property | Description |
| :------ | :--------------- | :----------------- |
| **Moving images** | [supportsMovingImages](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.supportsMovingImages) | Whether the subsystem supports tracking the poses of moving images in real time. |
| **Requires physical image dimensions** | [requiresPhysicalImageDimensions](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.requiresPhysicalImageDimensions) | Whether the subsystem requires physical image dimensions to be provided for all reference images. If `False`, specifying the physical dimensions is optional. |
| **Mutable library** | [supportsMutableLibrary](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.supportsMutableLibrary) | Whether the subsystem supports [MutableRuntimeReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary), a reference image library which can modified at runtime, as opposed to the [XRReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.XRReferenceImageLibrary), which is generated at edit time and cannot be modified at runtime. |
| **Image validation** | [supportsImageValidation](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor.supportsImageValidation) | Whether the subsystem supports image validation (validating images before they are added to a [MutableRuntimeReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary)).|

<a id="optional-feature-platform-support"/>

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature | ARCore | ARKit | VisionOS | XR Simulation |
| :------ | :----: | :---: | :------: | :-----------: |
| **Moving images** | Yes | iOS 12+ | Yes | Yes |
| **Requires physical image dimensions** | | Yes |  | |
| **Mutable library** | Yes | Yes | Yes | Yes |
| **Image validation** | Yes | iOS 13+ | Yes | Yes |

<a id="check-feature-support"/>

### Check for optional feature support

Your app can check at runtime whether an image tracking provider supports any optional features on the user's device. The [XRImageTrackingSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystemDescriptor) contains Boolean properties for each optional feature that tell you whether they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/ARTrackedImageManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
