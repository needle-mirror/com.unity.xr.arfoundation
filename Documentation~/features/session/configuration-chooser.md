---
uid: arfoundation-session-configuration-chooser
---
# Configuration Chooser

Understand how to use the Configuration Chooser to configure your AR session.

The [Configuration Chooser](xref:xref:UnityEngine.XR.ARSubsystems.ConfigurationChooser) is an overridable class which you can use to configure your [AR Session](xref:arfoundation-session-arsession). You can use the Configuration Chooser to enable better performance for your application.

## Introduction to configurations

An AR session's [Configuration](xref:UnityEngine.XR.ARSubsystems.Configuration) refers to the setup for the AR session on a specific device. For example, the type of light estimation, or camera facing direction used by the [Camera](xref:arfoundation-camera-components).

As different platforms and devices have different hardware and software capabilities, the configuration of the device can affect the [AR features](xref:UnityEngine.XR.ARSubsystems.Feature) that are available. Additionally, one device might restrict different subsets of its capabilities for simultaneous use depending on the configuration. Use the Configuration Chooser to determine the device configuration you want to use to maximise feature availability, or prioritize high-importance features.

> [!NOTE]
> Your app can only choose one configuration at a time, but can switch between multiple different configurations at runtime.

## Understand available configurations on a target platform

You can create an [ARDebugMenu](xref:UnityEngine.XR.ARFoundation.ARDebugMenu) to understand the available configurations for a specific platform, and identify the active configuration. You can create a configured AR Debug Menu from the Unity Editor, as described in [Debug AR scenes](xref:arfoundation-debug-menu).

The AR Debug Menu displays the available session configurations in the **Configurations** tab, as outlined in [Session Configurations](xref:arfoundation-debug-menu#session-configurations).

## How the default Configuration Chooser chooses a configuration

The default Configuration chooser assigns equal weights to all requested features and selects the configuration that allows the highest number of features. This AR Session uses this configuration to maximise the feature availability on the target device.

To prioritize a specific feature in your app, you can customize the Configuration Chooser to assign more weight to that feature. For example, if your app requires face tracking with the user-facing camera, assign more weight to face tracking and user-facing camera features.

## Create a custom Configuration Chooser

You can create your own Configuration Chooser for greater control over your app's configuration.

To create a custom Configuration Chooser assigned to your AR Session:

1. Select your **AR Session** component from the **Hierarchy** window to view it in the **Inspector**. If you don't have an AR Session, add one as outlined in [Add the AR Session component](xref:arfoundation-session-arsession#add-session).
1. In the **Inspector**, select **Add Component** to open the **Component** menu.
1. Select **New Script** and name the script.
1. In your new script file, implement the [ConfigurationChooser](xref:UnityEngine.XR.ARSubsystems.ConfigurationChooser) class.

Refer to the following section for a sample you can use to get started with creating a custom Configuration Chooser.

## Configuration Chooser sample

The [AR Foundation Samples](https://github.com/Unity-Technologies/arfoundation-samples/tree/main) app provide a sample scene that uses a custom Configuration Chooser. This sample includes a custom Configuration Chooser to choose between the user-facing and world-facing camera. This is useful if your project uses [Face tracking](xref:arfoundation-face-tracking) functionality.

To access this sample, visit [ConfigurationChooser](https://github.com/Unity-Technologies/arfoundation-samples/tree/main/Assets/Scenes/ConfigurationChooser) (GitHub).

## Additional resources

* [What's new in Unity's AR Foundation | Unite Now 2020](https://www.youtube.com/watch?v=jBRxY2KnrUs)
