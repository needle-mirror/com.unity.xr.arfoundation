---
uid: arfoundation-anchors-platform-support
---
# Anchors platform support

The AR Foundation [XRAnchorSubsystem](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem) is supported on the following platforms:

| Provider plug-in | Anchors supported | Provider documentation |
| :--------------- | :---------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Anchors](xref:arcore-anchors) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Anchors](xref:arkit-anchors) (ARKit) |
| Apple visionOS XR Plug-in | Yes | N/A |
| HoloLens | Yes | N/A |
| Unity OpenXR: Meta | Yes | [Anchors](xref:meta-openxr-anchors) (OpenXR Meta) |
| Unity OpenXR: Android XR | Yes | [Anchors](xref:androidxr-openxr-anchors) (Android XR) |
| XR Simulation | Yes | [Anchors](xref:arfoundation-simulation-anchors) (XR Simulation) |

## Check for support at runtime

Your app can check at runtime whether a provider plug-in supports anchors on the user's device. This is useful in situations where you are unsure if the device supports anchors, which may be the case if you are using AR Foundation with a third-party provider plug-in.

Use the following example code to check if the device supports anchors:

[!code-cs[CheckIfAnchorsLoaded](../../../Tests/Runtime/CodeSamples/LoaderUtilitySamples.cs#CheckIfAnchorsLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table lists the optional features of the anchor subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRAnchorSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported.

> [!TIP]
> Check the API documentation to understand how each API behaves when the provider doesn't support a feature. For example, some properties throw an exception if you try to set the value when the feature isn't supported.

| Feature | Descriptor Property | Description |
| :------ | :------------------ | :---------- |
| **Trackable attachments** | [supportsTrackableAttachments](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsTrackableAttachments) | Indicates whether the provider implementation supports the ability to attach an anchor to a trackable via [TryAttachAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryAttachAnchor(UnityEngine.XR.ARSubsystems.TrackableId,UnityEngine.Pose,UnityEngine.XR.ARSubsystems.XRAnchor@)). |
| **Synchronous add** | [supportsSynchronousAdd](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSynchronousAdd) | Indicates whether the provider implementation supports the ability to synchronously add anchors via [TryAddAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryAddAnchor(UnityEngine.Pose,UnityEngine.XR.ARSubsystems.XRAnchor@)). |
| **Save anchor** | [supportsSaveAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsSaveAnchor) | Indicates whether the provider implementation supports the ability to persistently save anchors via [TrySaveAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TrySaveAnchorAsync(UnityEngine.XR.ARSubsystems.TrackableId,CancellationToken)). |
| **Load anchor** | [supportsLoadAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsLoadAnchor) | Indicates whether the provider implementation supports the ability to load persistently saved anchors via [TryLoadAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryLoadAnchorAsync(UnityEngine.XR.ARSubsystems.SerializableGuid,CancellationToken)). |
| **Erase anchor** | [supportsEraseAnchor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsEraseAnchor) | Indicates whether the provider implementation supports the ability to erase the persistent saved data associated with an anchor via [TryEraseAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryEraseAnchorAsync(UnityEngine.XR.ARSubsystems.SerializableGuid,CancellationToken)). |
| **Get saved anchor IDs** | [supportsGetSavedAnchorIds](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsGetSavedAnchorIds) | Indicates whether the provider implementation supports the ability to get all saved persistent anchor GUIDs via [TryGetSavedAnchorIdsAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryGetSavedAnchorIdsAsync(Unity.Collections.Allocator,CancellationToken)). |
| **Async cancellation** | [supportsAsyncCancellation](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor.supportsAsyncCancellation) | Indicates whether the provider implementation supports cancelling async operations in progress using a [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-8.0). |

<a id="optional-features-support-table"></a>

### Optional features support table

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature                   | ARCore | ARKit  | visionOS |HoloLens | OpenXR Meta | Android XR | XR Simulation |
| :------------------------ | :----: | :---:  | :------: |:------: | :---------: | :--------: |:------------: |
| **Trackable attachments** | Yes    | Yes    |          |         |             | Yes        | Yes           |
| **Synchronous add**       | Yes    | Yes    |          |         |             | Yes        |               |
| **Save anchor**           | Yes    |        |          |         | Yes         | Yes        |               |
| **Load anchor**           | Yes    |        |          |         | Yes         | Yes        |               |
| **Erase anchor**          |        |        |          |         | Yes         | Yes        |               |
| **Get saved anchor IDs**  |        |        |          |         |             | Yes        |               |
| **Async cancellation**    | Yes    |        |          |         |             |            |               |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether an anchor provider supports any optional features on the user's device. The [XRAnchorSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystemDescriptor) contains boolean properties for each optional feature that tell you if they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/ARAnchorManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
