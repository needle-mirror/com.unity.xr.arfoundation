---
uid: arfoundation-samples-simple-ar
---
# Simple AR sample

The `Simple AR` sample is a good starting sample for beginners to AR Foundation development. This scene enables AR Foundation [Point cloud visualization](xref:arfoundation-point-clouds) and [Plane detection](xref:arfoundation-plane-detection). You can open this sample in Unity from the `Assets/Scenes/SimpleAR` folder.

[!include[](../../snippets/samples-tip.md)]

## Simple AR scene

This scene provides on-screen buttons to you pause, resume, reset, and reload the [AR Session](xref:arfoundation-session).

When a plane is detected, you can tap on the detected plane to place a cube on it. This uses the [ARRaycastManager](xref:arfoundation-raycast-manager) to perform a ray cast against the plane. If the plane is in `TrackingState.Limited`, it will highlight red. On [ARCore](xref:arcore-manual), this means that ray casting will not be available until the plane is in `TrackingState.Tracking` again.

The following table describes the available actions in this scene:

| Action     |  Definition |
|:---------- | :---------- |
| **Pause**   | Pauses the `ARSession`, meaning device tracking and trackable detection (for example plane detection) is temporarily paused. While paused, the `ARSession` doesn't consume CPU resources. |
| **Resume** | Resumes a paused `ARSession`. The device will attempt to relocalize and previously detected objects might shift around as tracking is reestablished. |
| **Reset**  | Clears all detected trackables and effectively begins a new `ARSession`. |
| **Reload** | Completely destroys the **ARSession** GameObject and re-instantiates it. This simulates the behavior you might experience during scene switching. |
