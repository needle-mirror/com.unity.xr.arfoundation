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

### Depth images requirements

The following table describes which platforms support each type of depth image:

| **Depth image**                      | **Supported devices** |
| :----------------------------------- | :---------------- |
| **Environment depth**                | Some Android devices. Apple devices with LiDAR sensor. |
| **Human segmentation stencil image** | Apple devices with an A12 bionic chip (or later) running iOS 13 or newer. |
| **Human segmentation depth image**   | Apple devices with an A12 bionic chip (or later) running iOS 13 or newer. |

Refer to ARKit Occlusion [Requirements](xref:arkit-occlusion#requirements) and ARCore Occlusion [Requirements](xref:arcore-occlusion#requirements) for more information about hardware and software requirements.

## HMD occlusion scene

The `HMDOcclusion` scene demonstrates how to use AR Foundation occlusion with shaders, as described in [AR Shader Occlusion component](xref:arfoundation-shader-occlusion). You can use this sample on a head-mounted display with an [OpenXR runtime](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.14/manual/index.html#runtimes).

[!include[](../../snippets/apple-arkit-trademark.md)]
