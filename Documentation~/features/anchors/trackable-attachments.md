---
uid: arfoundation-trackable-attachments
---
# Trackable attachments

Understand how to attach an anchor to a trackable so that virtual content stays aligned with a feature that AR Foundation tracks and refines.

A trackable attachment binds an anchor to a detected trackable, such as a plane. If a provider [supports trackable attachments](xref:arfoundation-anchors-platform-support#optional-features-support-table), the provider maintains the spatial relationships between attached anchors and their parent trackables as it refines tracking data.

## How trackable attachments work

When you attach an anchor to a trackable, AR Foundation routes the request to the active provider. The set of trackables you can attach an anchor to depends on the provider.

The attachment can fail, for example because the active provider doesn't support trackable attachments or you pass a trackable that the provider can't attach to. In these cases, `AttachAnchor` creates no anchor and returns `null`.

AR Foundation reports a successfully attached anchor through the AR Anchor Manager's change events, like any other anchor. The anchor can begin in a limited or non-tracking state and reach the tracking state once the provider resolves it.

## When to use trackable attachments

Use a trackable attachment when both of the following are true:

- You want to anchor content to a position on a trackable, such as a plane.
- You want that content to remain associated with that trackable while detection is still active, rather than anchored to a fixed world position.

If you only need a fixed point in space and no relationship to a trackable, use a standard anchor instead.

## Attach an anchor to a plane

AR Foundation currently supports attaching anchors to planes only, and not all providers support trackable attachments. Before you attach an anchor, confirm that the active provider supports trackable attachments. To find out which platforms and providers support the feature, and how to check for support at runtime, refer to [Anchors platform support](xref:arfoundation-anchors-platform-support).

To attach an anchor to a plane:

1. Ensure your scene contains an XR Origin component. If it doesn't, first follow the [Scene setup](xref:arfoundation-scene-setup) instructions.
1. Add an AR Anchor Manager component to your XR Origin GameObject if not already present.
1. In a custom C# script, call [ARAnchorManager.AttachAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.AttachAnchor*).

If `AttachAnchor` returns a non-null `ARAnchor` value, it successfully attached the anchor, and you can't unattach it.

## Additional resources

- [Introduction to anchors](xref:arfoundation-anchors-introduction)
- [AR Anchor Manager component](xref:arfoundation-anchors-aranchormanager)
- [Anchors platform support](xref:arfoundation-anchors-platform-support)
