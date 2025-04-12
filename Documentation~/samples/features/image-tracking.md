---
uid: arfoundation-samples-image-tracking
---
# Image tracking samples

Image tracking samples demonstrate AR Foundation [Image tracking](xref:arfoundation-image-tracking) functionality. You can open these samples in Unity from the `Assets/Scenes/ImageTracking` folder.

> [!TIP]
> You can check which platforms support AR Foundation [Image tracking](xref:arfoundation-image-tracking) features by checking the [Image tracking platform support](xref:arfoundation-image-tracking-platform-support) page.

## Enable image tracking

To enable image tracking, you must first create an `XRReferenceImageLibrary`. This is the set of images to look for in the environment. To understand how to create an `XRReferenceImageLibrary`, refer to [Create a reference image library](xref:arfoundation-image-tracking-reference-images).

You can also add images to the reference image library at runtime. This sample includes a button that adds the images `one.png` and `two.png` to the reference image library. Refer to `DynamicLibrary.cs` for example code.

## Run image tracking samples

Run the sample on an ARCore or ARKit device and point your device at one of the images in `Assets/Scenes/ImageTracking/Images`. You can display these images on a computer monitor, and you don't need to print them out.

## Basic image tracking scene

The `Basic Image Tracking` scene generates an [ARTrackedImage](xref:UnityEngine.XR.ARFoundation.ARTrackedImage) for each detected reference image at runtime.

This sample uses the `TrackedImageInfoManager.cs` script to overlay the original image on top of the detected image, along with some metadata.

## Image tracking with multiple prefabs scene

The `Image Tracking With Multiple Prefabs` scene demonstrates how to track multiple images and associate different prefabs with each image.

`PrefabImagePairManager.cs` assigns different prefabs for each image in the reference image library.

You can also change prefabs at runtime. This sample includes a button that switches between the original and alternative prefab for the first image in the reference image library. Refer to `DynamicPrefab.cs` for example code.

[!include[](../../snippets/apple-arkit-trademark.md)]
