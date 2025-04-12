---
uid: arfoundation-samples-collaboration-data
---
# Collaboration data sample

The `AR Collaboration Data` sample demonstrates the ARKit [Collaboration data](https://developer.apple.com/documentation/arkit/arcollaborationdata) feature.

A collaborative session is an ARKit-specific feature. This feature enables multiple devices to share session information in real time. Each device will periodically produce `ARCollaborationData` which should be sent to all other devices in the collaborative session. ARKit will share each participant's pose and all reference points. Other types of trackables, such as detected planes, aren't shared.

## Requirements

The collaboration data sample requires iOS 13 or newer.

## Collaboration data scene

Refer to [CollaborativeSession.cs](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/ARKit/ARCollaborationData/CollaborativeSession.cs) to understand the example code.

There are two types of collaboration data:

| Type          | Description                                                              |
| :------------ | :----------------------------------------------------------------------- |
| Critical      | Available periodically and should be sent to all other devices reliably. |
| Optional      | Available nearly every frame and may be sent unreliably. Includes data about the device's location. |

### Networking in collaborative sessions

ARKit support for collaborative sessions doesn't include networking. The developer must manage the connection and send data to other participants in the collaborative session.

This sample uses Apple's [MultipeerConnectivity Framework](https://developer.apple.com/documentation/multipeerconnectivity) (Apple developer documentation). Refer to [Multipeer](https://github.com/Unity-Technologies/arfoundation-samples/tree/main/Assets/Scripts/Runtime/Multipeer) to understand the implementation from this sample.

To create reference points, tap on the screen. Reference points are created when the tap results in a ray cast which hits a point in the point cloud.

[!include[](../../snippets/apple-arkit-trademark.md)]
