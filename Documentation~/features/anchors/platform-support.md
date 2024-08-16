---
uid: arfoundation-anchors-platform-support
---
# Anchors platform support

Anchors are supported on the ARCore, ARKit, HoloLens, Meta OpenXR, and XR Simulation platforms, as shown in the table below:

| Provider plug-in | Anchors supported | Provider documentation |
| :--------------- | :---------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Anchors](arcore-anchors) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Anchors](arkit-anchors) (ARKit) |
| Apple visionOS XR Plug-in | Yes | N/A |
| HoloLens | Yes | N/A |
| Unity OpenXR: Meta | Yes | [Anchors](xref:meta-openxr-anchors) (Meta OpenXR) |
| XR Simulation | Yes | [Anchors](xref:arfoundation-simulation-anchors) (XR Simulation) |

## Check for support at runtime

Your app can check at runtime whether a provider plug-in supports anchors on the user's device. This is useful in situations where you are unsure if the device supports anchors, which may be the case if you are using AR Foundation with a third-party provider plug-in.

Use the following example code to check if the device supports anchors:

[!code-cs[CheckIfAnchorsLoaded](../../../Tests/CodeSamples/LoaderUtilitySamples.cs#CheckIfAnchorsLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table lists the optional features of the anchor subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRAnchorSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor), which you can check at runtime to determine if a feature is supported. The **Description** column lists the optional API that may or may not be implemented on each platform.

> [!TIP]
> Check the API documentation to understand how each API behaves when the provider doesn't support a feature. For example, some methods may throw an exception if you call them when the feature isn't supported.

| Feature | Descriptor Property | Description |
| :------ | :------------------ | :---------- |
| **Trackable attachments** | [supportsTrackableAttachments](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsTrackableAttachments) | Indicates whether the provider implementation supports the ability to attach an anchor to a trackable via [TryAttachAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryAttachAnchor(UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.Pose,UnityEngine.XR.ARSubsystems.XRAnchor@)). |
| **Synchronous add** | [supportsSynchronousAdd](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSynchronousAdd) | Indicates whether the provider implementation supports the ability to synchronously add anchors via [TryAddAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryAddAnchor(UnityEngine.Pose,UnityEngine.XR.ARSubsystems.XRAnchor@)). |
| **Save anchor** | [supportsSaveAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSaveAnchor) | Indicates whether the provider implementation supports the ability to persistently save anchors via [TrySaveAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TrySaveAnchorAsync(UnityEngine.XR.ARSubsystems.TrackableId,CancellationToken)). |
| **Load anchor** | [supportsLoadAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsLoadAnchor) | Indicates whether the provider implementation supports the ability to load persistently saved anchors via [TryLoadAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryLoadAnchorAsync(UnityEngine.XR.ARSubsystems.SerializableGuid,CancellationToken)). |
| **Erase anchor** | [supportsEraseAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsEraseAnchor) | Indicates whether the provider implementation supports the ability to erase the persistent saved data associated with an anchor via [TryEraseAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryEraseAnchorAsync(UnityEngine.XR.ARSubsystems.SerializableGuid,CancellationToken)). |
| **Get saved anchor IDs** | [supportsGetSavedAnchorIds](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsGetSavedAnchorIds) | Indicates whether the provider implementation supports the ability to get all saved persistent anchor GUIDs via [TryGetSavedAnchorIdsAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryGetSavedAnchorIdsAsync(Unity.Collections.Allocator,CancellationToken)). |
| **Async cancellation** | [supportsAsyncCancellation](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsAsyncCancellation) | Indicates whether the provider implementation supports cancelling async operations in progress using a [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-8.0). |

<a id="optional-features-support-table"/>

### Optional features support table

To understand the optional features that are implemented in each supported XR plug-in provider, refer to the following table:

| Feature | ARCore | ARKit | HoloLens | Meta OpenXR | XR Simulation |
| :------ | :----: | :---: | :------: | :---------: | :-----------: |
| **Trackable attachments** | Yes | Yes | | | Yes |
| **Synchronous add** | Yes | Yes | | | |
| **Save anchor** | | | | Yes | |
| **Load anchor** | | | | Yes | |
| **Erase anchor** | | | | Yes | |
| **Get saved anchor IDs** | | | | | |
| **Async cancellation** | | | | | |

### Check for optional feature support

Your app can check at runtime whether an anchor provider supports any optional features on the user's device. The [XRAnchorSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor) contains boolean properties for each optional feature that tell you if they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/CodeSamples/ARAnchorManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
