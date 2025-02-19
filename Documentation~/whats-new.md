---
uid: arfoundation-whats-new
---
# What's new in version 6.1

## New features

### Stereo occlusion

- Added support for stereo occlusion, enabling providers for head-mounted displays (HMDs) to implement the XR occlusion subsystem. Refer to [Occlusion](xref:arfoundation-occlusion) for more information.

### Persistent anchor batch operations

- Added APIs for batch save, load, and erase of persistent anchors. Refer to [Persistent anchors](xref:arfoundation-anchors-persistent) for more information.

### Camera torch mode

- Added an API that allows you to turn on the device's camera torch (flash). Refer to [Camera torch mode (flash)](xref:arfoundation-camera-torch-mode) for more information

### Deeper URP integration to support ARCore Vulkan

- Added three new APIs to the `XRSessionSubsystem` and provider class which can be extended by AR session providers to handle Universal Render Pipeline rendering events signaled by the `ARCommandBufferSupportRendererFeature` when it is included in the renderer features list for the `Universal Renderer` asset:
  - [XRSessionSubsystem.requiresCommandBuffer](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.requiresCommandBuffer*)
  - [XRSessionSubsystem.OnCommandBufferSupportEnabled](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.OnCommandBufferSupportEnabled*)
  - [XRSessionSubsystem.OnCommandBufferExecute](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.OnCommandBufferExecute*)
- Added a new `ARCommandBufferSupportRendererFeature` which calls the newly exposed `XRSessionSubsystem` APIs for integration into **Universal Render Pipeline** command buffer execution.  Refer to [Universal Render Pipeline](xref:arfoundation-universal-render-pipeline) for more information.

### XR Simulation improvements

- Added support for simulated bounding box detection in XR Simulation via the [SimulatedBoundingBox](xref:UnityEngine.XR.Simulation.SimulatedBoundingBox) component.
- Added support for EXIF data in [SimulationCameraSubsystem](xref:UnityEngine.XR.Simulation.SimulationCameraSubsystem) via the [SimulatedExifData](xref:UnityEngine.XR.Simulation.SimulatedExifData) component.
- Added camera torch mode support to XR Simulation.

### Other API additions

- Added the following values to [StatusCode](xref:UnityEngine.XR.ARSubsystems.XRResultStatus.StatusCode): `ProviderUninitialized`, `ProviderNotStarted`, and `ValidationFailure`. These error codes may be returned instead of `StatusCode.UnknownError` for more specific error information.
- Added [SupportedUtils](xref:UnityEngine.XR.ARSubsystems.SupportedUtils) for easier conversion between the types `Supported` and `bool`.
- Added additional values provided by Apple RoomPlan to the [BoundingBoxClassifications](xref:UnityEngine.XR.ARSubsystems.BoundingBoxClassifications) flags enum.

## Changes

### XR Simulation environments now visible in the Hierarchy window

- Changed XR Simulation so that simulation environments are now visibile in the **Hierarchy** window, allowing you to inspect the environment while in Play mode.

> [!WARNING]
> There are many possible runtime modifications to XR Simulation environments that are not supported, such as instantiating or destroying GameObjects. As a best practice, use the Hierarchy window to debug XR Simulation environments, and don't modify environments while in Play mode.

### AR Occlusion Manager GameObject hierarchy

- Changed the [AR Occlusion Manager component](xref:arfoundation-occlusion-manager) to add `[RequireComponent(typeof(Camera))]`. Previously, it was logically required that this component was on the same GameObject as your XR Origin's Camera, but this wasn't as clearly enforced.

### AR Occlusion Manager frame timing

- Changed the timing of `AROcclusionManager.frameReceived` so that this event is now invoked during `Application.onBeforeRender` instead of `MonoBehaviour.Update`. This change is required for compatibility with head-mounted-display (HMD) providers, and may result in improved precision of occlusion frames on all platforms.

### Size of `XRTextureDescriptor`

- As part of the implementation for stereo occlusion support, we added a new `textureType` field to the [XRTextureDescriptor](xref:UnityEngine.XR.ARSubsystems.XRTextureDescriptor) struct. If you implement a provider for AR Foundation's camera or occlusion subystems, you should update your native plug-in(s) to match the new struct size.

## Deprecated

- Deprecated and replaced the following APIs:
  - `XRTextureDescriptor.dimension` to `XRTextureDescriptor.textureType`
  - `XRTextureDescriptor` constructor with `dimension` parameter to `XRTextureType` parameter.
  - `AROcclusionFrameEventArgs.propertyNameIds` to `AROcclusionFrameEventArgs.gpuTextures`.
  - `AROcclusionFrameEventArgs.textures` to `AROcclusionFrameEventArgs.gpuTextures`.
  - `AROcclusionManager.environmentDepthConfidenceTexture` to `AROcclusionManager.TryGetEnvironmentDepthConfidenceTexture`.
  - `AROcclusionManager.environmentDepthTexture` to `AROcclusionManager.TryGetEnvironmentDepthTexture`.
  - `ShaderKeywords` to `XRShaderKeywords`
  - `XRCameraSubsystem.GetShaderKeywords` to `XRCameraSubsystem.GetShaderKeywords2`
  - `XRCameraSubsystem.Provider.GetShaderKeywords` to `XRCameraSubsystem.Provider.GetShaderKeywords2`
  - `XROcclusionSubsystem.GetShaderKeywords` to `XROcclusionSubsystem.GetShaderKeywords2`
  - `XROcclusionSubsystem.Provider.GetShaderKeywords` to `XROcclusinSubsystem.Provider.GetShaderKeywords2`
  - `ARCameraFrameEventArgs.disabledShaderKeywords` to `ARCameraFrameEventArgs.shaderKeywords`
  - `ARCameraFrameEventArgs.enabledShaderKeywords` to `ARCameraFrameEventArgs.shaderKeywords`
  - `AROcclusionFrameEventArgs.disabledShaderKeywords` to `AROcclusionFrameEventArgs.shaderKeywords`
  - `AROcclusionFrameEventArgs.enabledShaderKeywords` to `AROcclusionFrameEventArgs.shaderKeywords`

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
