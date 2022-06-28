---
uid: arfoundation-tracked-object-manager
---
# AR tracked object manager

The [ARTrackedObjectManager](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectManager) component is a type of [trackable manager](trackable-managers.md).

![AR Tracked Object Manager](images/ar-tracked-object-manager.png "AR Tracked Object Manager")<br/>*AR Tracked Object Manager*

The tracked object manager creates a GameObject for each object detected in the environment. Before a real-world object can be detected, you must scan it to create a reference object. You can then add the reference object to the tracked object manager's reference object library.

> [!NOTE]
> * Currently, the [Apple ARKit XR Plug-in](xref:arkit-object-tracking) is the only Unity-supported provider plug-in that implements object tracking. 
> * The [Scanning and Detecting 3D Objects](https://developer.apple.com/documentation/arkit/scanning_and_detecting_3d_objects) page on Apple's developer website allows you to download an app that you can use on an iOS device to produce such a scan. Note that this is a third-party application, and Unity is not involved in its development.

## Reference object library

A reference object library is an asset in your project that contains a collection of reference objects. For more information about creating and using a reference object library, see the [object tracking subsystem](arsubsystems/object-tracking.md). 

## Creating a manager at runtime

When you add a component to an active GameObject at runtime, Unity immediately invokes the component's `OnEnable` method. However, the `ARTrackedObjectManager` requires a non-null reference object library. If the reference object library is null when the `ARTrackedObjectManager` is enabled, it automatically disables itself.

To add an `ARTrackedObjectManager` at runtime, set its reference object library and then re-enable it:

```csharp
var manager = gameObject.AddComponent<ARTrackedObjectManager>();
manager.referenceLibrary = myLibrary;
manager.enabled = true;
```

## Tracked object Prefab

This Prefab is instantiated whenever an object from the reference object library is detected. The manager ensures the instantiated GameObject includes an `ARTrackedObject` component. You can get the reference object that was used to detect the `ARTrackedObject` with the `ARTrackedObject.referenceObject` property.
