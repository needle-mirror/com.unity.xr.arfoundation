---
uid: arfoundation-meshing
---
# Meshing

Learn about and implement meshing in your project.

The AR Foundation meshing feature generates a mesh based on scans of the physical world.

Refer to the following topics to learn more about meshing in AR Foundation:

| **Topic** | **Desription** |
| :-------- | :------------- |
| [Meshing platform support](xref:arfoundation-meshing-platform-support) | Understand which platforms support meshing features. |
| [AR Mesh Manager component](xref:arfoundation-meshing-manager) | Learn about the AR Mesh Manager component. |
| [Configure the mesh prefab and meshing components](xref:arfoundation-meshing-prefab) | Understand how to set the mesh prefab and configure other meshing components. |
| [Mesh classification](xref:arfoundation-meshing-classification) | Understand the mesh classification feature. |

### Classification

If you wish to obtain classifications for subcomponents (e.g. vertices or faces) of incoming meshes, there is script access to obtain classifications by Mesh ID via [TryGetSubmeshClassifications](xref:UnityEngine.XR.ARFoundation.ARMeshManager.TryGetSubmeshClassifications). It must first be enabled via the Submesh Classification Enabled setting in [ARMeshManager](xref:UnityEngine.XR.ARFoundation.ARMeshManager). It can also be programmatically enabled via [submeshClassificationEnabled](xref:UnityEngine.XR.ARFoundation.ARMeshManager.submeshClassificationEnabled) but some platforms may require a restart of the [XRMeshSubsystem](xref:UnityEngine.XR.XRMeshSubsystem) to reflect this change.

Not all platforms support this feature.

## Additional resources

* [Meshing samples](xref:arfoundation-samples-meshing)
