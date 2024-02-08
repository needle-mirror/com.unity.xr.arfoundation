---
uid: arfoundation-vs-node-on-planes-changed
---
# On Planes Changed

Triggers when AR Planes have changed. AR Planes can be added, updated, and/or removed every frame if there is an enabled AR Plane Manager in the scene.

![On Planes Changed](../../images/visual-scripting/vs-on-planes-changed.png)<br/>*On Planes Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [ARPlaneManager](xref:arfoundation-plane-detection#ar-plane-manager-component) component. If you do not connect this port, this node searches for an enabled AR Plane Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [ReadOnlyList](xref:Unity.XR.CoreUtils.Collections.ReadOnlyList`1) of [ARPlane](xref:UnityEngine.XR.ARFoundation.ARPlane) | AR Planes that have been added. |
| **Updated** | [ReadOnlyList](xref:Unity.XR.CoreUtils.Collections.ReadOnlyList`1) of [ARPlane](xref:UnityEngine.XR.ARFoundation.ARPlane) | AR Planes that have been updated. |
| **Removed** | [ReadOnlyList](xref:Unity.XR.CoreUtils.Collections.ReadOnlyList`1) of `KeyValuePair<TrackableId, ARPlane>` | AR Planes that have been removed. |
