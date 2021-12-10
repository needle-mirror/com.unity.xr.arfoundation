---
uid: arfoundation-migration-guide-5-x
---
# Migration guide

This guide covers the differences between AR Foundation 4.x and 5.x.

## `ARSubsystems` package is merged into `ARFoundation`

Until now, the interfaces for AR-related subsystem were provided by [AR Subsystems package](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.2?subfolder=/manual/). These interfaces have been migrated to AR Foundation package. However, all these AR-related subsystems are still using the same namespace `Unity.XR.ARSubsystems`.

### Adapting an existing project

The breaking change here is mostly package dependency. In previous versions, `ARFoundation` package was dependent on `ARSubsytems` package. This dependency is now removed. This means previous apps which have AR Subsystems package as an explicit dependency can now replace it with AR Foundation package.

To make the transition easier, we are still publishing an empty `ARSubsystems` package with dependency on `ARFoundation`. This will ensure that the project which has an explicit dependency on `ARSubsystems` package continues to work as expected.

## Texture Importer ##

`TextureImporterInternals.GetSourceTextureDimensions` has been removed. Use `TextureImporter.GetSourceTextureWidthAndHeight` instead.

## XRSubsystem ##

The [XRSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSubsystem%601) is deprecated. Use [SubsystemWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemWithProvider) instead. This is the new Subsystem base class in Unity core and it requires an implementation of [SubsystemDescriptorWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemDescriptorWithProvider) and [SubsystemProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemProvider).

- Implementing a subsystem using deprecated APIs:

```c#
public class TestSubsystemDescriptor : SubsystemDescriptor<TestSubsystem>
{ }

public class TestSubsystem : XRSubsystem<TestSubsystemDescriptor>
{
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override void OnDestroyed() { }
}
```

- Implementing a subsystem using the new APIs:

```c#
public class TestSubsystemDescriptor : SubsystemDescriptorWithProvider<TestSubsystem, TestSubsystemProvider>
{ }

public class TestSubsystem : SubsystemWithProvider<TestSubsystem, TestSubsystemDescriptor, TestSubsystemProvider>
{ }

public class TestSubsystemProvider : SubsystemProvider<TestSubsystem>
{
    public override void Start() { }

    public override void Stop() { }

    public override void Destroy() { }
}
```

## XR Origin ##

`ARSessionOrigin` is now deprecated and will be replaced with `XROrigin`. In order to prepare your projects for the eventual removal of `ARSessionOrigin`, make sure to follow these steps:

- Replace all references in custom scripts from `ARSessionOrigin` to `XROrigin`.
- Once the upgrade is made to `XROrigin`, change all references to properties in `ARSessionOrigin` (now `XROrigin`) from camelCase to PascalCase.
- Remove all `ARSessionOrigin` components in the project and replace them with `XROrigin`.
- If you want to convert an existing object with `ARSessionOrigin` attached, make sure that you parent the camera under the camera offset object.

![XR Origin Hierarchy](images/xr-origin-hierarchy.png "XR Origin Hierarchy")

For more about XR Origin, see the [XR Core Utilities Package documentation](xref:xr-core-utils-xr-origin).
