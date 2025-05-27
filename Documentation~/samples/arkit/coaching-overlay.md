---
uid: arfoundation-samples-coaching-overlay
---
# Coaching overlay sample

The `ARKit Coaching Overlay` sample demonstrates the ARKit [Coaching overlay feature](https://developer.apple.com/documentation/arkit/arcoachingoverlayview) (Apple developer documentation).

Coaching overlay is an ARKit-specific feature. This feature overlays a helpful User Interface guiding the user to perform certain actions to achieve a goal, such as finding a horizontal plane.

You can activate the coaching overlay automatically or manually, and you can set its goal.

## Requirements

The coaching overlay sample requires iOS 13 or newer.

## Coaching overlay scene

In this sample, the coaching overlay is set to **Any plane** and activates automatically. This scene displays a special UI on the screen until a plane is found. There is also a button to activate it manually.

The sample includes a MonoBehavior to define the settings of the coaching overlay. Refer to the [ARKitCoachingOverlay](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/ARKit/ARKitCoachingOverlay/ARKitCoachingOverlay.cs) script to understand the settings you can define.

This sample also demonstrates how to subscribe to ARKit session callbacks. Refer to the [CustomSessionDelegate](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/ARKit/ARKitCoachingOverlay/CustomSessionDelegate.cs) script for more information.

[!include[](../../snippets/apple-arkit-trademark.md)]
