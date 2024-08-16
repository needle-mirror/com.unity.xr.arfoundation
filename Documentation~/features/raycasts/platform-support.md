---
uid: arfoundation-raycasts-platform-support
---
# Ray cast platform support

The AR Foundation [XRRaycastSubsystem](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystem) is supported on the ARCore, ARKit, Microsoft HoloLens, Meta OpenXR, and XR Simulation platforms, as shown in the following table:

| Provider plug-in          | Ray casting supported | Provider documentation                         |
| :------------------------ | :-------------------: | :--------------------------------------------- |
| Google ARCore XR Plug-in  |           Yes         | [Ray casts](xref:arcore-raycasts) (ARCore)     |
| Apple ARKit XR Plug-in    |           Yes         | [Ray casts](xref:arkit-raycasts) (ARKit)       |
| Apple visionOS XR Plug-in |                       |                                                |
| Microsoft HoloLens        |           Yes         | N/A                                            |
| Unity OpenXR: Meta        |           Yes         | [Ray casts](xref:meta-openxr-raycasts) (Meta OpenXR) |
| XR Simulation             |           Yes         | [Ray casts](xref:arfoundation-simulation-raycasts) (XR Simulation) |

## Check for ray casting support

Your app can check at runtime whether a provider plug-in supports ray casting on the user's device. Use the following example code to check whether the device supports ray casting:

[!code-cs[CheckIfRaycastLoaded](../../../Tests/CodeSamples/LoaderUtilitySamples.cs#CheckIfRaycastLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table lists the optional features of the [XRRaycastSubsystem](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystem). Each optional feature is defined by a **Descriptor Property** of the [XRRaycastSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported.

| Feature                    | Descriptor Property | Description |
| :------------------------- | :------------------ | :---------- |
| **Viewport based raycast** | [supportsViewportBasedRaycast](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystemDescriptor.supportsViewportBasedRaycast)| Whether the provider supports casting a ray from a screen point. |
| **World based raycast**    |  [supportsWorldBasedRaycast](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystemDescriptor.supportsWorldBasedRaycast)   | Whether the provider supports casting an arbitrary ray. |
| **Trackable types**        | [supportedTrackableTypes](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystemDescriptor.supportedTrackableTypes) | The types of trackables against which ray casting is supported. |
| **Tracked raycasts**       | [supportsTrackedRaycasts](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystemDescriptor.supportsTrackedRaycasts) | Whether tracked raycasts are supported. A tracked raycast is repeated over time and the results are updated automatically. |

<a id="optional-features-support-table"/>

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature                    | ARCore | ARKit   | Meta OpenXR | XR Simulation |
| :------------------------- | :----: | :-----: | :---------: | :-----------: |
| **Viewport based raycast** |   Yes  |   Yes   |             |               |
| **World based raycast**    |   Yes  |         |     Yes     |      Yes      |
| **Tracked raycasts**       |   Yes  | iOS 13+ |             |               |

> [!NOTE]
> The Meta OpenXR ray cast implementation performs calculations in Unity world space and does not rely on native platform implementation. Refer to the Meta OpenXR [Ray cast](xref:meta-openxr-raycasts) documentation to understand ray casting on Meta OpenXR devices.

<a id="supported-trackables"/>

### Supported trackables

[ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager) supports ray casting against most [TrackableTypes](xref:UnityEngine.XR.ARSubsystems.TrackableType). The following table shows which trackables each platform supports ray casting against:

| TrackableType           | ARCore | ARKit | Meta OpenXR | XR Simulation |
| :---------------------- | :----: | :---: | :---------: | :-----------: |
| **BoundingBox**         |        |       |     Yes     |               |
| **Depth**               |  Yes   |       |             |               |
| **Face**                |        |       |             |               |
| **FeaturePoint**        |  Yes   |  Yes  |             |      Yes      |
| **Image**               |        |       |             |               |
| **Planes**              |  Yes   |  Yes  |             |      Yes      |
| **PlaneEstimated**      |  Yes   |  Yes  |             |      Yes      |
| **PlaneWithinBounds**   |  Yes   |  Yes  |     Yes     |      Yes      |
| **PlaneWithinInfinity** |        |  Yes  |             |      Yes      |
| **PlaneWithinPolygon**  |  Yes   |  Yes  |             |      Yes      |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether a ray casting provider supports any optional features on the user's device. The [XRRaycastSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystemDescriptor) contains Boolean properties for each optional feature that tell you whether they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/CodeSamples/ARRaycastManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
