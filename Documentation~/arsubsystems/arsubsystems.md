---
uid: arsubsystems-manual
---
# Subsystems

A *subsystem* (shorthand for [SubsystemWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemWithProvider)) defines the life cycle and scripting interface of a Unity engine feature. All subsystems share a common subsystem life cycle, but their feature implementations can vary on different platforms, providing a layer of abstraction between your application code and platform-specific SDK's such as Google ARCore or Apple ARKit.

AR Foundation defines its AR features using subsystems. For example, the [plane subsystem](xref:arsubsystems-plane-subsystem) defines an interface for plane detection. You use the same application code to interact with a detected plane on iOS and Android — or any other platform with an implementation of the plane subsystem — but AR Foundation itself does not contain subsystem implementations for these platforms.

Subsystem implementations are called *providers*, and are typically made available in separate packages called *provider plug-ins*. For example, the [Google ARCore XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arcore@5.0/manual/index.html) provides subsystem implementations for the Android platform, and the [Apple ARKit XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit@5.0/manual/index.html) provides implementations for iOS.

This package contains interfaces for the following subsystems:
- [Session](session-subsystem.md)
- [Camera](camera-subsystem.md)
- [Plane Detection](plane-subsystem.md)
- [Image Tracking](image-tracking.md)
- [Object Tracking](object-tracking.md)
- [Face Tracking](face-tracking.md)
- [Anchors](anchor-subsystem.md)
- [Raycasts](raycasting-subsystem.md)
- [Point Clouds](point-cloud-subsystem.md)
- [Environment Probes](environment-probe-subsystem.md)
- [Body Tracking](xref:UnityEngine.XR.ARSubsystems.XRHumanBodySubsystem)
- [Occlusion](occlusion-subsystem.md)
- [Meshing](mesh-subsystem.md)

## Subsystem life cycle

All subsystems have the same life cycle: they can be created, started, stopped, and destroyed. You don't typically need to create or destroy a subsystem instance yourself, as this is the responsibility of Unity's active `XRLoader`. Each provider plug-in contains an `XRLoader` implementation (or simply, a loader).  Most commonly, a loader creates an instance of all applicable subsystems when your application initializes and destroys them when your application quits, although this behavior is configurable. When a trackable manager goes to `Start` a subsystem, it gets the subsystem instance from the project's active loader based on the settings found in **Project Settings** > **XR Plug-in Management**. For more information about loaders and their configuration, see the [XR Plug-in Management end-user documentation](https://docs.unity3d.com/Packages/com.unity.xr.management@latest?subfolder=/manual/EndUser.html).

In a typical AR Foundation Scene, any [trackable managers](xref:arfoundation-trackable-managers) present in the scene will `Start` and `Stop` their subsystems when the manager is enabled or disabled, respectively. The exact behavior of `Start` and `Stop` varies by subsystem, but generally corresponds to "start doing work" and "stop doing work", respectively. You can start or stop a subystem at any time based on the needs of your application.

## Subsystem descriptors

Each subsystem has a corresponding [SubsystemDescriptor](xref:UnityEngine.SubsystemsImplementation.SubsystemDescriptorWithProvider) whose properties describe the range of the subystem's capabilities. Providers might assign different values to these properties at runtime based on different platform or device limitations. 

Wherever you use a capability described in a subsystem descriptor, you should check its property value at runtime first to confirm whether that capability is supported on the target device, as shown in the example below:

```csharp
var trackedImageManager = FindObjectOfType(typeof(ARTrackedImageManager));
var imageTrackingSubystem = trackedImageManager.subsystem;

// Query whether the image tracking provider supports runtime modification
// of reference image libraries
if (imageTrackingSubsystem.subsystemDescriptor.supportsMutableLibrary)
{
    // take some action
}

// Equivalently:
if (trackedImageManager.descriptor.supportsMutableLibrary)
{
    // take some action
}
```

## Tracking subsystems

A *tracking subsystem* is a subsystem that detects and tracks one or more objects, called trackables, in the physical environment.

A *trackable* represents something that a tracking subsystem can detect and track. For example, the [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem) detects and tracks [BoundedPlane](xref:UnityEngine.XR.ARSubsystems.BoundedPlane) trackables. Each trackable has a `TrackableId`, which is a 128-bit GUID (Globally Unique Identifier) that can be used to uniquely identify trackables across multiple frames as they are added, updated, or removed.

In code, a trackable is defined as any class which implements [ITrackable](xref:UnityEngine.XR.ARSubsystems.ITrackable). In the `UnityEngine.XR.ARSubsystems` namespace, all trackable implementations (like [BoundedPlane](xref:UnityEngine.XR.ARSubsystems.BoundedPlane)) are structs. In the `UnityEngine.XR.ARFoundation` namespace, all trackable implementations (like [ARPlane](xref:UnityEngine.XR.ARFoundation.ARPlane)) are components which wrap these structs.

## Implementing a provider

To implement a provider for one or more of the subsystems in this package (for example, say you are a hardware manufacturer for a new AR device), Unity recommends that your implementation inherit from that subsystem's base class. These base class types follow a naming convention of **XR<Feature>Subsystem**, and they are found in the `UnityEngine.XR.ARSubsystems` namespace. Each subsystem base class has a nested abstract class called `Provider`, which is the primary interface you must implement for each subsystem you plan to support.

Subsystem implementations should be independent from each other. For example, your implementation of the [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem) should have the same behavior whether or not your [XRPointCloudSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem) implementation is also active in a user's scene.

### Registering a subsystem descriptor

Each subsystem type has a corresponding subsystem descriptor type. Your provider should create and register a subsystem descriptor instance with Unity's [SubsystemManager](https://docs.unity3d.com/ScriptReference/SubsystemManager.html) to enable runtime discovery and activation of subsystems. To register your subsystem descriptor, include a static void method with the `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]` attribute as shown in the example below, and in it call the type-appropriate registration method for the type of subsystem descriptor you are registering.

```csharp
// This class defines a Raycast subsystem provider
class MyRaycastSubsystem : XRRaycastSubsystem
{
    class MyProvider : Provider
    {
        // ...
        // XRRaycastSubsystem.Provider is a nested abstract class for you 
        // to implement
        // ... 
    }

    // This method registers the subsystem descriptor with the SubsystemManager
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        // In this case XRRaycastSubsystemDescriptor provides a registration 
        // helper method. See each subsystem descriptor's API documentation 
        // for more information.
        XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRImageTrackingSubsystemDescriptor.Cinfo
        {
            providerType = typeof(MyProvider),
            subsystemTypeOverride = typeof(MyRaycastSubsystem),
            // ...
            // You populate all required fields based on the details of 
            // your provider implementation
            // ...
        });
    }
}
```

#### Native plug-ins

Some XR subsystems, notably including the mesh subsystem, are not defined in the `ARSubsystems` namespace. These subsystems conform to Unity's **native plug-in interface**, and their descriptors cannot be registered via C#. For more information about native plug-ins, see the [Unity XR SDK documentation](https://docs.unity3d.com/Manual/xr-sdk.html).

### Implementing a tracking subsystem

Each tracking subsystem defines a method called [GetChanges](xref:UnityEngine.XR.ARSubsystems.TrackingSubsystem`4.GetChanges(Unity.Collections.Allocator)), which reports all added, updated, and removed trackables since the previous call to [GetChanges](xref:UnityEngine.XR.ARSubsystems.TrackingSubsystem`4.GetChanges(Unity.Collections.Allocator). You are required to implement the [GetChanges](xref:UnityEngine.XR.ARSubsystems.TrackingSubsystem`4.GetChanges(Unity.Collections.Allocator) method and should expect it to be called once per frame. Your provider must not update or remove a trackable without adding it first, nor update a trackable after it has been removed.

### Implementing an XR Loader

An `XRLoader` is responsible for creating and destroying subsystem instances based on the settings found in **Project Settings** > **XR Plug-in Management**. All provider plug-ins must implement an `XRLoader`. For more information on authoring an `XRLoader`, see the [XR Plug-in Management provider documentation](https://docs.unity3d.com/Packages/com.unity.xr.management@latest?subfolder=/manual/Provider.html). Example `XRLoader` implementations can be found in existing provider plug-ins.
