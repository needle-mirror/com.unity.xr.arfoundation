---
uid: arfoundation-install
---
# Install AR Foundation

AR Foundation is an official Unity package available via the [Package Manager](https://learn.unity.com/tutorial/the-package-manager).

## Editor compatibility

AR Foundation 6.3 is compatible with Unity 6 (6000.0) or newer.

> [!NOTE]
> You can visit the Unity Forum for more information on the [Unity 6 New Naming Convention](https://forum.unity.com/threads/unity-6-new-naming-convention.1558592/).

### Older Editor versions

If your project requires an older version of the Editor, you can use the following supported versions of AR Foundation:

| Editor version | AR Foundation version |
| :------------: | :-------------------: |
| 2022.3 (Enterprise license only) | 5.2 |
| 6000.0 | 6.0 |
| 6000.2 | 6.2 |

# Required packages

The AR Foundation package contains interfaces for AR features, but doesn't implement any AR features itself. To use AR Foundation on a target platform, you also need a separate *provider plug-in* package for that platform. A provider plug-in is a separate package containing AR Foundation feature implementations for a given platform.

Unity officially supports the following provider plug-ins with this version of AR Foundation:

| Platform | Plug-in | Version |
| :------- | :------ | :------ |
| **Android**      | [Google ARCore XR Plug-in](xref:arcore-manual) | 6.3 |
| **iOS**          | [Apple ARKit XR Plug-in](xref:arkit-manual) | 6.3 |
| **visionOS**     | [Apple visionOS XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.visionos@latest) | 2.4 |
| **Hololens 2**   | [OpenXR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest)| 1.16 |
| **Meta Quest**   | [Unity OpenXR: Meta](xref:meta-openxr-manual) | 2.3 |
| **Android XR**   | [Unity OpenXR: Android XR](xref:androidxr-openxr-manual) | 1.1 |

To use AR Foundation on a device, you must install at least one provider plug-in, either from the Package Manager or by going to **Project Settings** > **XR Plug-in Management** as shown below.

![The XR Plug-in Management category of the Project Settings window displays an interface for downloading AR Foundation provider plug-ins for supported platforms](../images/enable-arcore-plugin.png)<br/>*XR Plug-in Management*

> [!NOTE]
> Google also maintains a separate product, [ARCore Extensions for AR Foundation](https://developers.google.com/ar/develop/unity-arf), that you can use to access additional ARCore capabilities.

## Provider project settings

Some provider plug-ins require that you set specific project settings for AR to function properly. Refer to each provider's documentation for specific instructions:

| Provider plug-in | Setup instructions |
| :--------------- | :----------------- |
| [Google ARCore XR Plug-in](xref:arcore-manual) | [Project configuration](xref:arcore-project-config) (ARCore) |
| [Apple ARKit XR Plug-in](xref:arkit-manual) | [Project configuration](xref:arkit-project-config) (ARKit) |
| [Unity OpenXR: Meta](xref:meta-openxr-manual)| [Get started](xref:meta-openxr-get-started) (OpenXR Meta) |
| [Unity OpenXR: Android XR](xref:androidxr-openxr-manual) | [Get started](xref:androidxr-openxr-get-started) (OpenXR AndroidXR) |

## Third-party plug-ins

It is possible to develop custom provider plug-ins for AR Foundation. If you are using a third-party provider plug-in, see your plug-in documentation for more specific instructions regarding installation and use.

[!include[](../snippets/apple-arkit-trademark.md)]
