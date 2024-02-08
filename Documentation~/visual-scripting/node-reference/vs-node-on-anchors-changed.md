---
uid: arfoundation-vs-node-on-anchors-changed
---
# On Anchors Changed

Triggers when AR Anchors have changed. AR Anchors can be added, updated, and/or removed every frame if there is an enabled AR Anchor Manager in the scene.

![On Anchors Changed](../../images/visual-scripting/vs-on-anchors-changed.png)<br/>*On Anchors Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [ARAnchorManager](xref:arfoundation-anchors#ar-anchor-manager-component) component. If you do not connect this port, this node searches for an enabled AR Anchor Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [ReadOnlyList](xref:Unity.XR.CoreUtils.Collections.ReadOnlyList`1) of [ARAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchor) | AR Anchors that have been added. |
| **Updated** | [ReadOnlyList](xref:Unity.XR.CoreUtils.Collections.ReadOnlyList`1) of [ARAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchor) | AR Anchors that have been updated. |
| **Removed** | [ReadOnlyList](xref:Unity.XR.CoreUtils.Collections.ReadOnlyList`1) of `KeyValuePair<TrackableId, ARAnchor>` | AR Anchors that have been removed. |
