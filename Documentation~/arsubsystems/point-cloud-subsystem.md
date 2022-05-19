---
uid: arsubsystems-point-cloud-subsystem
---
# XR point cloud subsystem

The point cloud subsystem is an interface into depth information detected in the scene. This refers to feature points, which are unique features detected in the environment that can be correlated between multiple frames. A set of feature points is called a point cloud.

The point cloud subsystem is a type of [tracking subsystem](xref:arsubsystems-manual#tracking-subsystems). Its trackable is [XRPointCloud](xref:UnityEngine.XR.ARSubsystems.XRPointCloud).

Some providers only have one `XRPointCloud`, while others might have several. Check your provider's documentation for more details.
