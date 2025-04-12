---
uid: arfoundation-samples-plane-detection
---
# Plane detection samples

Plane detection samples demonstrate AR Foundation [Plane detection](xref:arfoundation-plane-detection) functionality. You can open these samples in Unity from the `Assets/Scenes/PlaneDetection` folder.

> [!TIP]
> You can check which platforms support AR Foundation [Plane detection](xref:arfoundation-plane-detection) features by checking the [Plane detection platform support](xref:arfoundation-plane-platform-support) page.

## Toggle plane detection scene

The `Toggle Plane Detection` scene shows how to toggle plane detection on and off. When off, it will also hide all previously detected planes by disabling their GameObjects.

## Plane detection mode scene

The `Plane Detection Mode` scene shows how to change the plane detection mode flags. Each type of plane (Horizontal, Vertical, and NotAxisAligned) can be toggled on and off.

## Plane masking scene

The `Plane Masking` scene demonstrates basic plane detection.

This sample uses an occlusion shader for the plane's material. This makes the plane appear invisible, but virtual objects behind the plane are culled. This provides an additional level of realism when, for example, placing objects on a table.

Move the device around until a plane is detected (its edges are still drawn) and then tap on the plane to place or move content.
