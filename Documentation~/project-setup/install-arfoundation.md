---
uid: arfoundation-install
---
# Install AR Foundation

AR Foundation is an official Unity package available via the [Package Manager](https://learn.unity.com/tutorial/the-package-manager).

## Editor compatibility

AR Foundation 6.0 is compatible with Unity 6 (6000.0).

> [!NOTE]
> You can visit the Unity Forum for more information on the [Unity 6 New Naming Convention](https://forum.unity.com/threads/unity-6-new-naming-convention.1558592/).

### Older Editor versions

If your project requires an older version of the Editor, you can use the following supported versions of AR Foundation:

| Editor version | AR Foundation version |
| :------------: | :-------------------: |
|     2021.3+    |          5.1          |
|     2020.3+    |          4.2          |

# Required packages

The AR Foundation package contains interfaces for AR features, but doesn't implement any AR features itself. To use AR Foundation on a target platform, you also need a separate *provider plug-in* package for that platform. A provider plug-in is a separate package containing AR Foundation feature implementations for a given platform.

Unity officially supports the following provider plug-ins with this version of AR Foundation:

| AR Platform | Provider plug-in                                                                                        | Version |
| :---------- | :------------------------------------------------------------------------------------------------------ | :------ |
| Android     | [Google ARCore XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arcore@6.0/manual/index.html) |   6.0   |
| iOS         | [Apple ARKit XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit@6.0/manual/index.html)    |   6.0   |
| HoloLens 2  | [OpenXR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.8/manual/index.html)           |   1.8   |

To use AR Foundation on a device, you must install at least one provider plug-in, either from the Package Manager or by going to **Project Settings** > **XR Plug-in Management** as shown below.

![The XR Plug-in Management category of the Project Settings window displays an interface for downloading AR Foundation provider plug-ins for supported platforms](../images/enable-arcore-plugin.png)<br/>*XR Plug-in Management*

> [!NOTE]
> To use additional ARCore functionality such as Cloud Anchors, install Google's [ARCore Extensions for AR Foundation](https://developers.google.com/ar/develop/unity-arf).

## Provider project settings

Some provider plug-ins require that you set specific project settings for AR to function properly. See their documentation for specific instructions:

| Provider plug-in | Setup instructions |
| :--------------- | :----------------- |
| [Google ARCore XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arcore@6.0/manual/index.html) | [Project configuration](https://docs.unity3d.com/Packages/com.unity.xr.arcore@6.0/manual/project-configuration-arcore.html) |
| [Apple ARKit XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit@6.0/manual/index.html) | [Project configuration](https://docs.unity3d.com/Packages/com.unity.xr.arkit@6.0/manual/project-configuration-arkit.html) |


## Third-party plug-ins

It is possible to develop custom provider plug-ins for AR Foundation. If you are using a third-party provider plug-in, see your plug-in documentation for more specific instructions regarding installation and use.

[!include[](../snippets/apple-arkit-trademark.md)]
