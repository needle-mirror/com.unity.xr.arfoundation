---
uid: arfoundation-samples-use
---
# Set up samples

Understand how to clone and open the AR Foundation Samples project in Unity, and build the project to your target device.

To use the AR Foundation sample app, [clone the samples](#clone-samples) from GitHub, configure project settings, then [build and run on a target device](#build-run).

## Requirements

For this version of AR Foundation (<span class="short_version">X.Y</span>), use the `main` branch of the AR Foundation Samples repository.

<a id="clone-samples"></a>

## Clone and open samples

To clone and open the AR Foundation Samples project:

1. Navigate to the [AR Foundation Samples](https://github.com/Unity-Technologies/arfoundation-samples) repository in GitHub and [Clone the repository](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository) (GitHub documentation).
2. Open the Unity Hub and select **Add** > **Add project from disk** to open the file explorer.
3. Navigate to the root of the cloned repository and select **Add Project**.
4. Click on the project to open it in the Unity Editor.
5. You can now edit the scenes to meet your needs from the `Assets/Scenes` folder, or continue to [Build and run on device](#build-run).

> [!IMPORTANT]
> If the Unity Hub shows the warning symbol next to the project, you don't have the correct Unity Editor version installed. This can cause errors in the AR Foundation Samples project. To fix the **Missing Editor Version** warning, click on the warning symbol, select the **Missing Version** of the Editor and **Install Version**.

<a id="build-run"></a>

## Build and run on device

To build and run the sample on your device for the first time:

1. Connect your device to your computer.
2. Open the **Build Profiles** window (**File** > **Build Profiles**).
3. Select your target platform from the **Platforms** menu.
4. Optionally, click **Open Scene List** to select specific scenes you want to build. By default, Unity builds all scenes in the project.
5. Under **Platform Settings**, select your device from the **Run Device** dropdown.
6. Press **Build and Run**.
7. In the dialog window, choose a location and file name for your build, and select **Save**.

If the build is successful, Unity will install the application on your target device.

In future builds, you can build and run to device directly from **File** > **Build and Run** if you haven't changed your target device.

> [!NOTE]
> Check the documentation for your target platform to understand whether there are any specific project settings that you must configure for your chosen platform.

### Build on Meta Quest devices

This repository is configured with the Google ARCore XR Plug-in enabled by default on the Android platform. To build for Meta Quest, disable the Google ARCore provider in the Android tab of **Project Settings** > **XR Plug-in Management**, then follow the Meta Quest [Project setup](xref:meta-openxr-project-settings) instructions.
