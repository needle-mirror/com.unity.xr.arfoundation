---
uid: arfoundation-meshing-classification
---
# Mesh classification

Understand the mesh classification feature.

In AR Foundation 6.4 and newer, you can enable mesh classification on [supported platforms](xref:arfoundation-meshing-platform-support) to apply semantic labels of detected meshes. Enable mesh classification to improve understanding of the real-world environment.

## Uses for mesh classification

Mesh classification enables you to use semantic labels of meshes to improve your app's understanding of the physical environment. You can use these semantic labels to improve features of your application that rely on understanding of the physical environment, for example:

* Accurate physics
* Object placement
* Navigation

## Enable mesh classification

There are two ways to enable mesh classification:

1. Open the **ARMeshManager** in the **Inspector** window, and enable **Classification**.
2. Use the [submeshClassificationEnabled](xref:UnityEngine.XR.ARFoundation.ARMeshManager.submeshClassificationEnabled) API.

> [!NOTE]
> Some platforms might require a restart of the `XRMeshSubsystem` to enable mesh classification via scripting.

Once you have enabled mesh classification, you can use [TryGetSubmeshClassifications](xref:UnityEngine.XR.ARFoundation.ARMeshManager.TryGetSubmeshClassifications) to obtain classifications using Mesh ID.

## Mesh classification reference

AR Foundation provides the following mesh classifications:

| **Name** | **Description** |
| :------- | :-------------- |
| **Ceiling** | Mesh classification as a ceiling. |
| **Door** | Mesh classification as a door. |
| **Floor** | Mesh classification as a floor. |
| **Other** | Generic other mesh classification. |
| **Seat** | Mesh classification as a seat/chair. |
| **Table** | Mesh classification as a table. |
| **Unknown** | Unknown/no mesh classification. |
| **Wall** | Mesh classification as a wall. |
| **Window** | Mesh classification as a window. |
