---
uid: arfoundation-occlusion-workflow
---
# Configure occlusion in your project

Configure occlusion in your project.

Similar to other AR Foundation [managers](xref:arfoundation-managers), the [AR Occlusion Manager](xref:arfoundation-occlusion-manager) component is the starting point to integrate occlusion in your app. However, unlike other managers, AR Occlusion Manager requires the help of additional components.

The components necessary to configure AR Foundation occlusion currently differ depending on whether your app targets mobile phone devices or headsets. The following sections outline how to configure occlusion in your project for each class of devices.

## Configure occlusion for OpenXR headsets

As noted in [Occlusion platform support](xref:arfoundation-occlusion-platform-support), AR Foundation occlusion is currently supported on Meta Quest and Android XR devices.

If your app targets supported OpenXR devices, enable occlusion by adding the following components to your XR Origin's **Main Camera** GameObject:

* [AR Occlusion Manager](xref:arfoundation-occlusion-manager)
* [AR Shader Occlusion](xref:arfoundation-shader-occlusion)

> [!NOTE]
> If your scene doesn't contain an XR Origin, review the AR Foundation [Scene setup](xref:arfoundation-scene-setup) instructions first, then return to this page.

Once you have added the relevant occlusion components to your scene, you can configure properties for each component using the **Inspector** window. Refer to each component's documentation for more information.

### Occlusion shader

The AR Shader Occlusion component requires you to supply a customizable occlusion shader and use that shader on all occludable Materials in your project. Refer to [AR Shader Occlusion](xref:arfoundation-shader-occlusion) for more information.

## Configure occlusion for mobile phones or XR Simulation

For iOS, Android, or XR Simulation, enable occlusion by adding the following components to your XR Origin's **Main Camera** GameObject:

* [AR Occlusion Manager](xref:arfoundation-occlusion-manager)
* [AR Camera Background](xref:arfoundation-camera-components)

While the AR Camera Background component is necessary for occlusion to work correctly on these platforms, AR Camera Background doesn't offer any occlusion-related configuration options in the **Inspector**. The AR Occlusion Manager component is the only configurable occlusion component for mobile phones and XR Simulation.

> [!WARNING]
> If you attach both the AR Shader Occlusion and AR Camera Background components to your camera, AR Shader Occlusion controls occlusion functionality and disables the occlusion portion of AR Camera Background. If your app targets both headsets and mobile devices, consider either creating separate scenes for each device type or adding the AR Shader Occlusion component at runtime on OpenXR platforms.

## Additional resources

* [Occlusion samples](xref:arfoundation-samples-occlusion)

[!include[](../../snippets/apple-arkit-trademark.md)]
