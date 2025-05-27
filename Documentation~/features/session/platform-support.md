---
uid: arfoundation-session-platform-support
---
# Session platform support

Understand the features of the session subsystem, and which platforms support these features.

AR Foundation requires that all provider implementations on all platforms support the session feature, as shown in the following table:

| Provider plug-in | Session supported | Provider documentation |
| :--------------- | :---------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Session](xref:arcore-session) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Session](xref:arkit-session) (ARKit) |
| Apple visionOS XR Plug-in | Yes |  N/A |
| Microsoft HoloLens | Yes | N/A |
| Unity OpenXR: Meta | Yes | [Session](xref:meta-openxr-session) (Meta OpenXR) |
| Unity OpenXR: Android XR | Yes | [Session](xref:androidxr-openxr-session) (Android XR) |
| XR Simulation | Yes | [Session](xref:arfoundation-simulation-session) (XR Simulation) |

## Optional features

The following table lists the optional features of the session subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRSessionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor), which you can check at runtime to determine whether your target platform supports a feature. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported.

| Feature              | Descriptor Property | Description |
| :------------------- | :------------------ | :----------------- |
| **Install**          | [supportsInstall](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsInstall) | Whether the session supports the update or installation of session software. |
| **Match frame rate** | [supportsMatchFrameRate](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsMatchFrameRate) | Whether the session supports matching the AR frame rate to the Unity frame rate. |

<a id="optional-feature-support"></a>

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature              | ARCore | ARKit | VisionOS | Meta OpenXR | Android XR | XR Simulation|
| :------------------- | :----: | :---: | :------: | :--------:  | :--------: | :----------: |
| **Install**          |  Yes   |       |          |             |            |              |
| **Match frame rate** |  Yes   |  Yes  |          |             |            |              |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether a session provider supports any optional features on the user's device. The [XRSessionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor) contains Boolean properties for each optional feature that tell you whether they're supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/ARSessionSamples.cs#DescriptorChecks)]

## AR support on mobile devices

Not all mobile devices support AR by default. If you're targeting mobile platforms (ARCore and ARKit), you can check whether the user's device supports AR. On supported devices, you can request install of additional software if needed. You can learn more about AR support on mobile devices, by referring to the [ARCore](xref:arcore-manual) and [ARKit](xref:arkit-manual) documentation.

Refer to the following sections to check for AR support on your target device, and install XR software if required.

### Check for AR support on mobile devices

If you're targeting mobile devices, your application needs to be able to detect support for AR Foundation to provide an alternative experience when AR isn't supported. This is relevant to projects that support ARCore, or ARKit for devices older than the iPhone 6s. iPhone models including and newer than the 6s support AR by default.

To check whether AR is available on the target device might require checking a remote server for software availability. To check for AR support, call [CheckAvailability](xref:UnityEngine.XR.ARFoundation.ARSession.CheckAvailability), as shown in the following code example:

[!code-cs[CheckForARSupport](../../../Tests/Runtime/CodeSamples/ARSessionSamples.cs#CheckForARSupport)]

> [!TIP]
> To learn more about the available session states, refer to [Session state](xref:arfoundation-session-arsession#session-state).

### Install XR software on mobile devices

If your target platform [supports install](#optional-feature-support), you can request the device to install or update necessary XR software.

To attempt to install XR software, call [XRSessionSubsystem.InstallAsync](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.InstallAsync). When you call `XRSessionSubsystem.InstallAsync`, you can use the promised [SessionInstallationSession](xref:UnityEngine.XR.ARSubsystems.SessionInstallationStatus) to understand the result of the installation request.

[!include[](../../snippets/apple-arkit-trademark.md)]
