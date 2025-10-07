---
uid: arfoundation-samples-meshing
---
# Meshing sample scenes

Meshing samples demonstrate AR Foundation [Meshing](xref:arfoundation-meshing) functionality. You can open these samples in Unity from the `Assets/Scenes/Meshing` folder.

[!include[](../../snippets/samples-tip.md)]

To understand each of the meshing sample scenes, refer to the following sections:

| Sample                               | Description  |
| :----------------------------------- | :----------- |
| [Normal meshes](#normal) | Renders an overlay on top of the real world scanned geometry illustrating the normal of the surface. |
| [Classification meshes](#classification) (ARKit) | Demonstrates mesh classification functionality. |
| [Occlusion meshes](#occlusion) | Demonstrates how to use meshes of real world geometry to occlude virtual content. |

## Requirements

The meshing sample scenes use features of some devices to construct meshes from scanned data of real world surfaces. These meshing scenes will not work on all devices. Refer to the [meshing](xref:arfoundation-meshing-platform-support) documentation for your target platform to understand any platform-specific requirements.

<a id="normal"></a>

## Normal meshes scene

The `Normal Meshes` scene renders an overlay on top of the real world scanned geometry illustrating the normal of the surface.

![NormalMeshes](../../images/arfoundation-arkit-normal-meshing.gif)

<a id="classification"></a>

## Classification meshes scene

The `Classification Meshes` scene demonstrates mesh classification functionality.

With mesh classification enabled, each triangle in the mesh surface is identified as one of several surface types. This sample scene creates submeshes for each classification type and renders each mesh type with a different color.

![ClassificationMeshes](../../images/arfoundation-arkit-classified-meshing.gif)

<a id="occlusion"></a>

## Occlusion meshes scene

The `Occlusion Meshes` scene demonstrates how to use meshes of real world geometry to occlude virtual content.

At first, this scene might appear to be doing nothing. However, it's rendering a depth texture on top of the scene based on the real world geometry. This allows for the real world to occlude virtual content. The scene has a script on it that fires a red ball into the scene when you tap. To observe occlusion working, fire the red balls into a space and move the iPad camera behind a real world object. You will see that the virtual red balls are occluded by the real world object.

![OcclusionMeshes](../../images/arfoundation-arkit-occlusion-meshing.gif)

[!include[](../../snippets/apple-arkit-trademark.md)]
