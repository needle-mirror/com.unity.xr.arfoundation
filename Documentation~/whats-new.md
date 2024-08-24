---
uid: arfoundation-whats-new
---
# What's new in version 6.1

## Added

- Added three new APIs to the `XRSessionSubsystem` and provider class which can be extended by AR session providers to handle Universal Render Pipeline rendering events signaled by the `ARCommandBufferSupportRendererFeature` when it is included in the renderer features list for the `Universal Renderer` asset.
  - [XRSessionSubsystem.requiresCommandBuffer](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.requiresCommandBuffer*)
  - [XRSessionSubsystem.OnCommandBufferSupportEnabled](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.OnCommandBufferSupportEnabled*)
  - [XRSessionSubsystem.OnCommandBufferExecute](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.OnCommandBufferExecute*)
- Added a new `ARCommandBufferSupportRendererFeature` which calls the newly exposed `XRSessionSubsystem` APIs for integration into **Universal Render Pipeline** command buffer execution.  Refer to [Universal Render Pipeline](xref:arfoundation-universal-render-pipeline) for more information.
- Added support for simulated bounding box detection to XR Environment via the [SimulatedBoundingBox](xref:UnityEngine.XR.Simulation.SimulatedBoundingBox) component.

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arfoundation-changelog).

# Changed

- Changed [BoundingBoxClassifications](xref:UnityEngine.XR.ARSubsystems.BoundingBoxClassifications) to add additional labels provided by Apple RoomPlan.
