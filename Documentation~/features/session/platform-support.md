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
| Unity OpenXR: Meta | Yes | [Session](xref:meta-openxr-session) (Meta OpenXR) |
| Unity OpenXR: Android XR | Yes | [Session](xref:androidxr-openxr-session) (Android XR) |
| XR Simulation | Yes | [Session](xref:arfoundation-simulation-session) (XR Simulation) |

## Optional features

The following table lists the optional features of the session subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRSessionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor), which you can check at runtime to determine whether your target platform supports a feature. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported.

| Feature              | Descriptor Property | Description |
| :------------------- | :------------------ | :----------------- |
| **Install** (Deprecated) | [supportsInstall](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsInstall) | Whether the session supports the update or installation of session software. |
| **Match frame rate** | [supportsMatchFrameRate](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsMatchFrameRate) | Whether the session supports matching the AR frame rate to the Unity frame rate. |

<a id="optional-feature-support"></a>

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature              | ARCore | ARKit | visionOS | Meta OpenXR | Android XR | XR Simulation|
| :------------------- | :----: | :---: | :------: | :--------:  | :--------: | :----------: |
| **Install** (Deprecated) |  Yes   |       |          |             |            |              |
| **Match frame rate** |  Yes   |  Yes  |          |             |            |              |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether a session provider supports any optional features on the user's device. The [XRSessionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor) contains Boolean properties for each optional feature that tell you whether they're supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/Runtime/CodeSamples/ARSessionSamples.cs#DescriptorChecks)]

## AR support on mobile devices

Some mobile devices require additional software to run AR. You can request the device to install or update necessary XR software. You can learn more about AR support on mobile devices, by referring to the [ARCore](xref:arcore-manual) and [ARKit](xref:arkit-manual) documentation.

Refer to the following section to install XR software on your target device if required.

<a id="install"></a>

### Install XR software on mobile devices

To attempt to install XR software, pass [InstallSoftwareIfNeeded](xref:UnityEngine.XR.ARSubsystems.XRSubsystemStartOptions.InstallSoftwareIfNeeded) to `TryStartAsync`.

When AR isn't supported on the device, `TryStartAsync` returns an [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus) with a `StatusCode.Unsupported` status code, which you can check to provide an alternative experience.

The following example installs required software if needed, starts the session, and handles an unsupported device:

[!code-cs[InstallAndStartSession](../../../Tests/Runtime/CodeSamples/ARSessionSamples.cs#InstallAndStartSession)]

[!include[](../../snippets/apple-arkit-trademark.md)]
