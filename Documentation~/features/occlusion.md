---
uid: arfoundation-occlusion
---
# Occlusion

Occlusion allows mixed reality content in your app to appear hidden or partially obscured behind objects in the physical environment. Without occlusion, geometry in your scene will always render on top of physical objects in the AR background, regardless of their difference in depth.

AR Foundation provides two components to control occlusion functionality in your AR scene. The [AR Occlusion Manager](xref:arfoundation-occlusion-manager) provides simple occlusion alongside the [ARCameraBackground](xref:arfoundation-camera-components) component, primarily on mobile devices.
The [AR Shader Occlusion](xref:arfoundation-shader-occlusion) component enables customizable occlusion using shaders on [OpenXR runtimes](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.13/manual/index.html#runtimes).

Refer to the following topics to understand how to use AR Foundation occlusion:

| Topic | Description |
| :---- | :---------- |
| [Occlusion platform support](xref:arfoundation-occlusion-platform-support) | Discover which AR platforms support occlusion features. |
| [AR Occlusion Manager component](xref:arfoundation-occlusion-manager) | Understand the AR Occlusion Manager component. |
| [AR Shader Occlusion component](xref:arfoundation-shader-occlusion) | Understand the AR Shader Occlusion component. |

