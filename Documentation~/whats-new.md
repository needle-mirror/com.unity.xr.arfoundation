---
uid: arfoundation-whats-new
---
# What's new in version 6.1

## Added

- Added `StatusCode.ProviderUninitialized` as an error code to represent an uninitialized state. Methods that return [XRResultStatus](xref:UnityEngine.XR.ARSubsystems.XRResultStatus) can use this more specific error code instead of `StatusCode.UnknownError` when the provider is uninitialized.
- Added [SupportedUtils](xref:UnityEngine.XR.ARSubsystems.SupportedUtils) for easier conversion between the types `Supported` and `bool`.
- Added three new APIs to the `XRSessionSubsystem` and provider class which can be extended by AR session providers to handle Universal Render Pipeline rendering events signaled by the `ARCommandBufferSupportRendererFeature` when it is included in the renderer features list for the `Universal Renderer` asset.
  - [XRSessionSubsystem.requiresCommandBuffer](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.requiresCommandBuffer*)
  - [XRSessionSubsystem.OnCommandBufferSupportEnabled](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.OnCommandBufferSupportEnabled*)
  - [XRSessionSubsystem.OnCommandBufferExecute](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.OnCommandBufferExecute*)
- Added a new `ARCommandBufferSupportRendererFeature` which calls the newly exposed `XRSessionSubsystem` APIs for integration into **Universal Render Pipeline** command buffer execution.  Refer to [Universal Render Pipeline](xref:arfoundation-universal-render-pipeline) for more information.
- Added support for simulated bounding box detection to XR Environment via the [SimulatedBoundingBox](xref:UnityEngine.XR.Simulation.SimulatedBoundingBox) component.
- Added an API that allows you to turn on the device's camera torch (flash). Refer to [Camera torch mode (flash)](xref:arfoundation-camera-torch-mode) for more information
- Added support for stereo occlusion, enabling HMD providers to implement the XR occlusion subsystem:
  - Added [XRTextureType](xref:UnityEngine.XR.ARSubsystems.XRTextureType) enum with extension methods to convert from [TextureDimension](xref:UnityEngine.Rendering.TextureDimension).
  - Added [XRTextureDescriptor.textureType](xref:UnityEngine.XR.ARSubsystems.XRTextureDescriptor.textureType) property to get a texture descriptor's type.
  - Added the following structs to represent data used for occlusion: [XRFov](xref:UnityEngine.XR.ARSubsystems.XRFov), [XRNearFarPlanes](xref:UnityEngine.XR.ARSubsystems.XRNearFarPlanes), [XROcclusionFrame](xref:UnityEngine.XR.ARSubsystems.XROcclusionFrame), and [ARGpuTexture](xref:UnityEngine.XR.ARFoundation.ARGpuTexture).
  - Added the following members to [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem): `depthViewProjectionMatricesPropertyId` and `TryGetFrame(Allocator, out XROcclusionFrame)`.
  - Added more data to [AROcclusionFrameEventArgs](xref:UnityEngine.XR.ARFoundation.AROcclusionFrameEventArgs).
  - Added the [ARShaderOcclusion](xref:arfoundation-shader-occlusion) component to write depth textures to global shader memory.

## Changed

- Changed [BoundingBoxClassifications](xref:UnityEngine.XR.ARSubsystems.BoundingBoxClassifications) to add additional labels provided by Apple RoomPlan.
- Changed the Simulation Environment to be visibile in the scene hierarchy.

## Deprecated

- Deprecated and replaced the following APIs:
  - `XRTextureDescriptor.dimension` to `XRTextureDescriptor.textureType`
  - `XRTextureDescriptor` constructor with `dimension` parameter to `XRTextureType` parameter.
  - `AROcclusionFrameEventArgs.propertyNameIds` to `AROcclusionFrameEventArgs.gpuTextures`.
  - `AROcclusionFrameEventArgs.textures` to `AROcclusionFrameEventArgs.gpuTextures`.
  - `AROcclusionManager.environmentDepthConfidenceTexture` to `AROcclusionManager.TryGetEnvironmentDepthConfidenceTexture`.
  - `AROcclusionManager.environmentDepthTexture` to `AROcclusionManager.TryGetEnvironmentDepthTexture`.

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).
