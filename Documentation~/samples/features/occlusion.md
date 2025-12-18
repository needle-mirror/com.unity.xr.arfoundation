---
uid: arfoundation-samples-occlusion
---
# Occlusion samples

Occlusion samples demonstrate AR Foundation [Occlusion](xref:arfoundation-occlusion) functionality. You can open these samples in Unity from the `Assets/Scenes/Occlusion` folder.

> [!TIP]
> You can check which platforms support AR Foundation [Occlusion](xref:arfoundation-occlusion) features by checking the [Occlusion platform support](xref:arfoundation-occlusion-platform-support) page.

## Simple occlusion scene

The `Simple Occlusion` scene demonstrates occlusion of virtual content by real world content through the use of environment depth images on supported devices.

## Depth images scene

The `Depth Images` scene demonstrates raw texture depth images from different methods.

## HMD occlusion scene

The `HMDOcclusion` scene demonstrates how to use AR Foundation occlusion with shaders, as described in [AR Shader Occlusion component](xref:arfoundation-shader-occlusion). You can use this sample on a head-mounted display with an [OpenXR runtime](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.14/manual/index.html#runtimes).

## Hands occlusion scene

The `HandsOcclusion` scene demonstrates how to use occlusion with hand meshes as the occlusion source. Refer to [AR Shader Occlusion component](xref:arfoundation-shader-occlusion) for more information.

## Meta occlusion scene

The `MetaOcclusion` scene implements [soft occlusion](xref:arfoundation-occlusion-introduction#types) on Meta Quest devices. Access this sample from `Assets/Scenes/Meta`. Refer to the OpenXR Meta [Occlusion](xref:meta-openxr-occlusion) documentation to learn more about occlusion on Meta Quest.

<a id="shaders"></a>

## Shader samples

Unity provides sample shaders that you can use with the [AR Shader Occlusion component](xref:arfoundation-shader-occlusion) for soft occlusion in your app.

You can access the shader samples for soft occlusion from `Assets/Shaders/Occlusion`.

[!include[](../../snippets/apple-arkit-trademark.md)]
