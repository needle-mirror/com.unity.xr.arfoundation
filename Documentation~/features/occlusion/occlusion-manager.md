---
uid: arfoundation-occlusion-manager
---
# AR Occlusion Manager component

Understand the AR Occlusion manager component.

The [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager) enables occlusion in your project.

The AR Occlusion Manager acquires per-frame images from the AR platform containing depth or stencil data that shaders can use to implement the occlusion effect. Incorporating depth images into the rendering process allows shaders to sample both the depth of your Unity scene and the depth of the physical environment, and render the pixels that are closer to the camera.

## Enable occlusion

To enable occlusion, add the `AROcclusionManager` to your [Camera](xref:arfoundation-camera) as outlined in [Occlusion workflow](xref:arfoundation-occlusion-workflow).

<a id="reference"></a>

## Component reference

![AR Occlusion Manager component](../../images/occlusion/ar-occlusion-manager.png)<br/>*AR Occlusion Manager component.*

The following table describes the properties of the AR Occlusion Manager component:

| Property | Description |
| :------- | :---------- |
| **Environment Depth Mode** | The mode for generating the environment depth texture. There are four options: <ul><li><strong>Disabled:</strong> No environment depth texture produced.</li><li><strong>Fastest:</strong> Minimal rendering quality. Minimal frame computation.</li><li><strong>Medium:</strong> Medium rendering quality. Medium frame computation.</li><li><strong>Best:</strong> Best rendering quality. Increased frame computation.</li></ul> |
| **Temporal Smoothing** | Whether temporal smoothing should be applied to the environment depth image, if supported on the chosen platform. Temporal smoothing reduces noise to provide smoother images and more consistent occlusion. |
| **Human Segmentation Stencil Mode** | The mode for generating human segmentation stencil texture: <ul><li><strong>Disabled:</strong> No human stencil texture produced.</li><li><strong>Fastest:</strong> Minimal rendering quality. Minimal frame computation.</li><li><strong>Medium:</strong> Medium rendering quality. Medium frame computation.</li><li><strong>Best:</strong> Best rendering quality. Increased frame computation.</li></ul> |
| **Human Segmentation Depth Mode** | The mode for generating human segmentation depth texture: <ul><li><strong>Disabled:</strong> No human stencil texture produced.</li><li><strong>Fastest:</strong> Minimal rendering quality. Minimal frame computation.</li><li><strong>Best:</strong> Best rendering quality. Increased frame computation.</li></ul> |
| **Occlusion Preference Mode** | If both environment texture and human stencil & depth textures are available, this mode specifies which should be used for occlusion. There are three options: <ul><li><strong>Prefer Environment Occlusion </li><li><strong>Prefer Human Occlusion</li><li><strong>No Occlusion</strong></li></ul> |

> [!NOTE]
> Use **Occlusion Preference Mode** to choose the type of depth image to use on ARKit. Human stencil depth images are only supported on ARKit. Environment and depth images are never simultaneously enabled.<br/>
> ARCore only supports **Disabled** and **Fastest** modes for **Environment Depth Mode**. Using the other modes would not further improve depth texture quality or increase frame computation.

## Types of depth images

The types of depth images supported are:

| Type of depth image | Description |
| :------- | :---------- |
| **Environment Depth** | Distance from the device to any part of the environment in the camera field of view. |
| **Human Depth** | Distance from the device to any part of a human recognized within the camera field of view. |
| **Human Stencil** | Value designating, for each pixel, whether that pixel contains a recognized human. |

## Additional resources

* [Configure occlusion in your project](xref:arfoundation-occlusion-workflow)
* [AR Shader Occlusion component](xref:arfoundation-shader-occlusion)

[!include[](../../snippets/apple-arkit-trademark.md)]
