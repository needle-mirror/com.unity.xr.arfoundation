---
uid: arfoundation-samples-camera
---
# Camera samples

Camera samples demonstrate AR Foundation [Camera](xref:arfoundation-camera) features. You can open these samples in Unity from the `Assets/Scenes/Camera` folder.

> [!TIP]
> You can check which platforms support AR Foundation [Camera](xref:arfoundation-camera) features by checking the [Camera platform support](xref:arfoundation-camera-platform-support) page.

To understand each of the camera sample scenes, refer to the following sections:

| Sample scene                                     | Description                                         |
| :----------------------------------------------- | :-------------------------------------------------- |
| [CPU images](#cpu-images) | Demonstrates how to acquire and manipulate CPU images. |
| [Basic light estimation](#basic-light) | Performs AR Foundation basic light estimation. |
| [HDR light estimation](#hdr-light) | Reads HDR light information on supported platforms. |
| [Background rendering order](#background-rendering-order) | Provides an example of changing the camera's background rendering order. |
| [Camera grain](#camera-grain) (ARKit) | Implements the camera grain effect on ARKit. |
| [EXIF data](#exif-data) | Demonstrates how to access camera frame's EXIF metadata. |
| [Image stabilization](#image-stabilization) (ARCore) | Implements image stabilization on supported ARCore devices. |
| [Camera torch mode](#torch-mode) | Demonstrates how to activate the device's torch. |

<a id="cpu-images"></a>

## CPU images scene

The `CPU Images` scene shows how to acquire and manipulate textures obtained from AR Foundation on the CPU.

Most textures in AR Foundation are GPU textures. For example the passthrough video supplied by the [ARCameraManager](xref:UnityEngine.XR.ARFoundation.ARCameraManager), and the human depth and human stencil buffers provided by the [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager). Computer vision or other CPU-based applications often require the pixel buffers on the CPU, which involves an expensive GPU read back. AR Foundation provides an API for obtaining these textures on the CPU for further processing, without incurring the costly GPU read back.

### Camera configuration

The resolution of the camera image is affected by the camera's configuration. You can use the dropdown box at the bottom left of the screen to understand the current configuration, and select one of the supported camera configurations.

`CameraConfigController.cs` demonstrates enumerating and selecting a camera configuration. This script is attached to the **CameraConfigs** GameObject (**Canvas** > **CameraConfigs**).

### Human depth and stencil textures

Where available, the human depth and human stencil textures are also available on the CPU. These appear inside two additional boxes underneath the camera's image.

### Human depth and stencil texture requirements

Human depth and human stencil textures are currently available on ARKit using devices with iOS 13 or newer.

<a id="basic-light"></a>

## Basic light estimation scene

The `Basic Light Estimation` scene demonstrates basic light estimation information from the camera frame. You will find values for **Ambient Intensity** and **Ambient Color** on screen.

<a id="hdr-light"></a>

## HDR light estimation scene

The `HDR Light Estimation` scene attempts to read HDR lighting information.

You will find values for **Ambient Intensity**, **Ambient Color**, **Main Light Direction**, **Main Light Intensity Lumens**, **Main Light Color**, and **Spherical Harmonics** on screen. Most devices only support a subset of these values, so some will be **Unavailable**. To check which of these values your chosen platform supports, refer to [Camera platform support](xref:arfoundation-camera-platform-support).

### Requirements

On iOS, HDR light estimation is only available when face tracking is enabled and requires a device that supports face tracking. Supported devices include iPhone X, XS, or 11. When available, a virtual arrow appears in front of the camera which indicates the estimated main light direction. The virtual light direction is also updated, so that virtual content appears to be lit from the direction of the real light source.

When using HDR light estimation, the sample will automatically pick the supported camera facing direction for you. On Android, this is the world-facing camera, and on iOS, this is the user-facing (selfie) camera. The facing direction you select in the [ARCameraManager](xref:UnityEngine.XR.ARFoundation.ARCameraManager) component will be overridden.

<a id="background-rendering-order"></a>

## Background rendering order scene

The `Background Rendering Order` scene produces a visual example of how changing the [Camera background rendering mode](xref:UnityEngine.XR.ARFoundation.CameraBackgroundRenderingMode) between `BeforeOpaqueGeometry` and `AfterOpaqueGeometry` can affect a rudimentary AR application. This sample leverages occlusion where available to display `AfterOpaqueGeometry` support for occlusion.

<a id="camera-grain"></a>

## Camera grain scene (ARKit)

The `Camera Grain` scene demonstrates the camera grain effect.

After a plane is detected, you can place a cube on it with a material that simulates the camera grain noise in the camera feed. Use `CameraGrain.cs` to apply camera grain noise from the camera feed to GameObjects.

`CameraGrain.shader` animates and applies the camera grain texture (through linear interpolation) in screen space.

### Camera grain requirements

This sample is available on ARKit only, and requires a device running iOS 13 or newer, and Unity 2020.2 or later.

<a id="exif-data"></a>

## EXIF data scene

The `EXIF Data` scene demonstrates how to access camera frame's EXIF metadata.

You can find values for all the supported EXIF tags on screen. Refer to `ExifDataLogger.cs` for more details.

### EXIF data requirements

On ARKit, the EXIF data sample requires iOS 16 or newer.

<a id="image-stabilization"></a>

## Image stabilization scene (ARCore)

The `Image Stabilization` scene shows how to toggle the Image Stabilization feature on and off.

### Image stabilization requirements

The image stabilization sample is available on ARCore only. The target ARCore device must have Google Play Services for AR version 1.37 or newer.

<a id="torch-mode"></a>

## Camera torch mode scene

The `Camera Torch Mode` scene demonstrates the AR Foundation [Camera torch mode](xref:arfoundation-camera-torch-mode) feature.

<!-- To do: expected behaviour  -->

## Additional resources

* [Camera components](xref:arfoundation-camera-components)

[!include[](../../snippets/apple-arkit-trademark.md)]
