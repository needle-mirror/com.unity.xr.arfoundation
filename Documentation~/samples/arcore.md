---
uid: arfoundation-samples-arcore
---
# ARCore samples

The AR Foundation Samples repository provides sample scenes to demonstrate features specific to [ARCore](xref:arcore-manual). You can open these samples in Unity from the `Assets/Scenes/ARCore` folder.

## ARCore session recording scene

The `AR Core Session Recording` scene demonstrates the [Session recording and playback](xref:arcore-session-recording) functionality on ARCore.

This feature allows you to record the sensor and camera telemetry during a live session, and then replay it at a later time. When replayed, ARCore runs on the target device using the recorded telemetry rather than live data. Refer to the [ARCoreSessionRecorder](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/ARCore/ARCoreSessionRecorder.cs) script for example code.
