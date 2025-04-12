---
uid: arfoundation-samples-body-tracking
---
# Body tracking samples

Body tracking samples demonstrate AR Foundation [Body tracking](xref:arfoundation-body-tracking) functionality. You can open these samples in Unity from the `Assets/Scenes/BodyTracking` folder.

> [!NOTE]
> Body tracking is supported on ARKit for iOS only. Body tracking samples require an iOS device with an A12 bionic chip or later, and iOS 13 or newer. To check whether a device has an A12 bionic chip or later, refer to Apple's [Tech Specs](https://support.apple.com/en_US/specs).

## Body tracking 2D scene

The `Human Body Tracking 2D` scene demonstrates 2D screen space body tracking.

A 2D skeleton is generated when a person is detected. Refer to the `ScreenSpaceJointVisualizer.cs` to understand the visualizer used in this scene.

## Body tracking 3D scene

The `Human Body tracking 3D` scene demonstrates 3D world space body tracking.

A 3D skeleton is generated when a person is detected. Refer to `HumanBodyTracker.cs` for example code.

[!include[](../../snippets/apple-arkit-trademark.md)]
