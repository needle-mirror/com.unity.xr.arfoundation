# AR Anchor Manager

The anchor manager is a type of [trackable manager](trackable-managers.md).

![alt text](images/ar-anchor-manager.png "AR Anchor Manager")

The anchor manager will create `GameObject`s for each anchor. A anchor is a particular point in space that you are asking the device to track. Some SDKs refer to these as "anchors". The device typically performs additional work to update the position and orientation of the anchor throughout its lifetime. Anchors are generally resource intensive objects that should be used sparingly.

## Adding and Removing Anchors

Anchors are typically added and removed via script, by calling `AddAnchor` and `RemoveAnchor` on the `ARAnchorManager`. In some cases anchors may be created through other means, such as loading an AR World Map on ARKit which includes saved anchors.

When you add a anchor, it may take a frame or two before it is reported as added by the anchor manager's `anchorsChanged` event. Between the time you add it and it is reported as added, the anchor will be in a "pending" state. You can query for this with the `ARAnchor.pending` property.

Likewise, when you remove a anchor, it may be a frame before it is reported as having been removed. If you remove a anchor before it is reported as added, you will not receive any events for it.

You should always remove anchors through the anchor manager. Do not `Destroy` an `ARAnchor` unless its manager has also been destroyed.

## Attaching Anchors

You can also create anchors that are "attached" to a plane. The following `ARAnchorManager` method does this
```csharp
public ARAnchor AttachAnchor(ARPlane plane, Pose pose)
```

"Attaching" to a plane affects the semantics for a anchor update. Anchors will only change their position along the normal of the plane they are attached to, effectively maintaining a constant distance from the plane.
