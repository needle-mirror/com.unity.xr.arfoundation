---
uid: arfoundation-samples-point-clouds
---
# Point clouds sample

The `All Point Clouds` sample demonstrates AR Foundation [Point clouds](xref:arfoundation-point-clouds) functionality. You can open this sample in Unity from the `Assets/Scenes/PointClouds` folder.

[!include[](../../snippets/samples-tip.md)]

## Point clouds scene

This sample shows all feature points over time, not just the current frame's feature points. This is unlike the AR Default Point Cloud prefab, which shows just the feature points from the current frame.

The sample uses a slightly modified version of the [ARPointCloudParticleVisualizer](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/PointClouds/ARAllPointCloudPointsParticleVisualizer.cs) component that stores all the feature points in a dictionary. As each feature point has a unique identifier, it can look up the stored point and update its position in the dictionary if it already exists. This is a useful starting point for custom solutions that require the entire map of point cloud points, for example for custom mesh reconstruction techniques.

### UI components

This sample has two UI components:

| Component | Location    | Description                                                                      |
| :-------- |:----------- | :------------------------------------------------------------------------------- |
| Button    | Lower left  | Swap between visualizing **All** points and just those in the **Current Frame**. |
| Text      | Upper right | Displays the number of points in each point cloud. |

[!include[](../../snippets/apple-arkit-trademark.md)]
