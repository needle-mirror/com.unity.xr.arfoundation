---
uid: arfoundation-camera-torch-mode
---
# Camera torch mode (flash)

Camera torch mode enables you to activate the device's torch (flashlight) to improve tracking quality in low light conditions.

## Check support

Refer to the [Optional features support table](xref:arfoundation-camera-platform-support#optional-feature-platform-support) to learn which provider plug-ins support camera torch mode.

## Session configuration

The camera torch may not be available in all session configurations. The following sample code shows you how to list the session configurations that support camera torch mode:

[!code-cs[CameraTorchModeSupport](../../../Tests/Runtime/CodeSamples/CameraTorchModeSample.cs#CameraTorchModeSupport)]

## Use the torch

The following sample code shows you how to enable the device's camera torch in a supported session configuration:

[!code-cs[EnableCameraTorch](../../../Tests/Runtime/CodeSamples/CameraTorchModeSample.cs#EnableCameraTorch)]

> [!NOTE]
> The camera torch consumes additional device battery power. Consider warning your users of increased power consumption and giving them an option to disable the torch.