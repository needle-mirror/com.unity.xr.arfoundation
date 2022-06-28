---
uid: arsubsystems-manual
---
# Subsystems

A [subsystem](xref:UnityEngine.Subsystem) is a platform-agnostic interface for surfacing different types of functionality and data within Unity. The subsystems in this package use the namespace `UnityEngine.XR.ARSubsystems`, but this namespace only contains the interfaces for these subsystems. Subsystem implementations are called *providers*, and are typically made available in separate packages called *provider plug-ins*.

This package contains interfaces for the following subsystems:

- [Session](session-subsystem.md)
- [Anchors](anchor-subsystem.md)
- [Raycasting](raycasting-subsystem.md)
- [Camera](camera-subsystem.md)
- [Plane Detection](plane-subsystem.md)
- [Point Cloud](point-cloud-subsystem.md)
- [Image Tracking](image-tracking.md)
- [Face Tracking](face-tracking.md)
- [Environment Probes](environment-probe-subsystem.md)
- [Object Tracking](object-tracking.md)
- [Body Tracking](xref:UnityEngine.XR.ARSubsystems.XRHumanBodySubsystem)
- [Occlusion](occlusion-subsystem.md)
- [Meshes](mesh-subsystem.md)

## Subsystem life cycle

All subsystems have the same life cycle: they can be created, started, stopped, and destroyed. You don't typically need to create or destroy a subsystem instance yourself, as this is the responsibility of Unity's active `XRLoader`. Each provider plug-in contains an `XRLoader` implementation (or simply, a loader).  Most commonly, a loader creates an instance of all applicable subsystems when your application initializes and destroys them when your application quits, although this behavior is configurable. When a trackable manager goes to `Start` a subsystem, it gets the subsystem instance from the project's active loader based on the settings found in **Project Settings** > **XR Plug-in Management**. For more information about loaders and their configuration, see the [XR Plug-in Management end-user documentation](https://docs.unity3d.com/Packages/com.unity.xr.management@latest?subfolder=/manual/EndUser.html).

In a typical AR Foundation Scene, any [trackable managers](../trackable-managers.md) present in the scene will `Start` and `Stop` their subsystems when the manager is enabled or disabled, respectively. The exact behavior of `Start` and `Stop` varies by subsystem, but generally corresponds to "start doing work" and "stop doing work", respectively. You can start or stop a subystem at any time based on the needs of your application.

## Subsystem descriptors

Each subsystem has a corresponding `SubsystemDescriptor` whose properties describe the range of the subystem's capabilities. Providers might assign different values to these properties at runtime based on different platform or device limitations. Whenever you use a capability described in a `SubsystemDescriptor`, you should check the corresponding property value on device to confirm whether that capability is present, as shown in the example below.

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

## Implementing a provider

To implement a provider for one or more of the subsystems in this package (for example, say you are a hardware manufacturer for a new AR device), Unity recommends that your implementation inherit from that subsystem's base class. These base class types follow a naming convention of `XR<Feature>Subsystem`, and they are found in the `UnityEngine.XR.ARSubsystems` namespace. Each subsystem base class has a nested abstract class called `Provider`, which is the primary interface you must implement for each subsystem you plan to support.

### Tracking subsystems

A **tracking subsystem** is a subsystem that detects and tracks one or more objects in the physical environment. Each tracking subsystem defines a method called `GetChanges`, which reports all added, updated, and removed trackables since the previous call to `GetChanges`. You are required to implement the `GetChanges` method, and you should expect it to be called once per frame.

A **trackable** represents anything that can be detected and tracked in the physical environment. Each trackable can be uniquely identified by a `TrackableId`, a 128-bit GUID (Globally Unique Identifier) which can be used to identify trackables across multiple frames as they are added, updated, or removed. Your provider must not update or remove a trackable without adding it first, nor update a trackable after it has been removed.

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

Some XR subsystems, notably including the mesh subsystem, are not defined in the `ARSubsystems` namespace. These subsystems conform to Unity's **native plug-in interface**, and their descriptors cannot be registered via C#. For more information about native plug-ins see the [Unity XR SDK documentation](https://docs.unity3d.com/Manual/xr-sdk.html).

### Implementing an XR Loader

An `XRLoader` is responsible for creating and destroying subsystem instances based on the settings found in **Project Settings** > **XR Plug-in Management**. All provider plug-ins must implement an `XRLoader`. For more information on authoring an `XRLoader`, see the [XR Plug-in Management provider documentation](https://docs.unity3d.com/Packages/com.unity.xr.management@latest?subfolder=/manual/Provider.html). Example `XRLoader` implementations can be found in existing provider plug-ins.
