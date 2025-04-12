---
uid: arfoundation-simulation-xr-environment-view
---
# XR Environment view

Go to **Window** > **XR** > **AR Foundation** > **XR Environment** to open the XR Environment view. The XR Environment view lets you create and edit XR Simulation environments, as well as select and preview the currently active environment. When you enter Play Mode, XR Simulation runs your app in the active environment as if that environment were a physical space.

![The XR Environment view](../images/xr-environment-view-annotated.png)<br/>*The XR Environment view*

| **Label** | **Element** | **Description** |
| :-------: | :---------- | :-------------- |
| **A** | **XR Environment visibility toggle** | Toggles XR Environment visibility on and off. When XR Environment visibility is off, the XR Environment view behaves like a normal Scene view. |
| **B** | **Environment dropdown** | Click to reveal the following options: <ul><li><strong>Environment list:</strong> Lists all environment Prefabs found in the Assets folder. Click on an environment to select it, making it the active environment. Refresh the list by going to **Assets** > **Refresh XR Environment List**.</li><li><strong>Install sample environments</strong> Installs the sample environment assets to your project. See [Install the sample environments](simulation-getting-started.md#install-the-sample-environments).</li></ul> |
| **C** | **Previous environment**       | Select the previous environment in the Environments list. |
| **D** | **Next environment**           | Select the next environment in the Environments list.  |
| **E** | **Create/edit environment dropdown** | Click to reveal the following options for creating and editing environments: <ul><li><strong>Create environment:</strong> Create and save a new environment using the default XR Simulation environment as a template.</li><li><strong>Duplicate environment</strong> Create and save a copy of the active environment.</li><li><strong>Edit environment</strong> Open the active environment for editing in [Prefab Mode](https://docs.unity3d.com/Manual/EditingInPrefabMode.html).</li></ul> |
| **F** | **Camera starting pose** | Visualizes the initial position and rotation of the Camera in the environment when you enter Play Mode. See [Simulation Environment component](simulation-environments.md#simulation-environment-component) for more information about setting these values. |

Any Prefab under your Assets folder with a `SimulationEnvironment` component on the root GameObject is considered to be a simulation environment and can appear in the Environment list. This list is cached and can be refreshed by going to **Assets** > **Refresh XR Environment List**. See [Install the sample environments](xref:arfoundation-simulation-getting-started#install-the-sample-environments) to install the sample environments, or [Simulation environments](xref:arfoundation-simulation-environments) to create and modify environments.

## XR Environment overlay

The XR Environment overlay is a toolbar [overlay](https://docs.unity3d.com/Manual/overlays.html) that controls the behavior of the XR Environment view.

![XR Environment overlay](../images/xr-environment-overlay.png)<br/>*XR Environment overlay*

When you first open the XR Environment view, the XR Environment overlay and several other overlays are displayed by default, but you can customize which overlays you wish to display or hide. Only the XR Environment overlay is required to use the XR Environment view, and you can display or hide any additional overlays according to your preferences. You can also position and dock an overlay by clicking and dragging its handle (**=**).

To display or hide an overlay, follow the steps below:

1. Click the **⋮** icon in the top right corner of the XR Environment view.
2. Click **Overlay Menu** to open the Overlay menu.

   ![Overlay Menu button](../images/overlay-menu-button.png)

3. Toggle the visibility of overlays using the controls in the Overlay menu.

   ![The Overlay menu contains a visibility toggle for each overlay](../images/overlay-menu.png)
