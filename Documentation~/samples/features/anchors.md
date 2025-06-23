---
uid: arfoundation-samples-anchors
---
# Anchors samples

The `Anchors` sample demonstrates AR Foundation [Anchors](xref:arfoundation-anchors) functionality. You can open this sample in Unity from the `Assets/Scenes/Anchors` folder.

> [!TIP]
> You can check which platforms support AR Foundation [Anchors](xref:arfoundation-anchors) features by checking the [Anchors platform support](xref:arfoundation-anchors-platform-support) page.

## Anchors scene

The anchors sample scene shows how to create anchors as the result of a ray cast hit. The **Remove all anchors** button removes all created anchors.

The `ARAnchorManager` can create two kinds of anchors based on a ray cast hit:

* If the ray hits a feature point, it creates a normal anchor at the hit pose using the [TryAddAnchorAsync](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem.TryAddAnchorAsync(UnityEngine.Pose)) method.
* If the ray hits a plane, it creates an anchor attached to the plane using the [AttachAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.AttachAnchor(UnityEngine.XR.ARFoundation.ARPlane,UnityEngine.Pose)) method.

## Anchor visualizer

The AR Foundation Samples GitHub repository contains a prefab to visualize anchors in your scene. You can use this prefab to get that you can use to get started with visualizing anchors:

| Prefab | Description |
| :----- | :---------- |
| [AR Anchor Debug Visualizer](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Prefabs/AR%20Anchor%20Debug%20Visualizer.prefab) | Visualize anchors with a Transform gizmo, and optionally visualize additional information such as the anchor's [trackableId](xref:UnityEngine.XR.ARFoundation.ARTrackable`2.trackableId), [sessionId](xref:UnityEngine.XR.ARFoundation.ARAnchor.sessionId), [trackingState](xref:UnityEngine.XR.ARFoundation.ARTrackable`2.trackingState), and whether the anchor is attached to a plane. |
