---
uid: arfoundation-samples-geo-anchors
---
# Geo anchors sample

The `ARKit Geo Anchors` sample demonstrates ARKit's [ARGeoAnchors feature](https://developer.apple.com/documentation/arkit/argeoanchor?language=objc) (Apple developer documentation).

## Requirements

This sample requires an iOS device running iOS 14.0 or later, an A12 chip or later, location services enabled, and cellular capability.

## Geo anchors scene

ARKit's ARGeoAnchors aren't yet supported by AR Foundation. This sample shows how you can access this feature with some Objective-C code.

The geo anchors sample uses a custom [ConfigurationChooser](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.configurationChooser) to instruct the Apple ARKit XR Plug-in to use an ARGeoTrackingConfiguration.

This sample also shows how to interpret the [nativePtr](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.nativePtr) provided by the [XRSessionSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem) as an ARKit [ARSession](https://developer.apple.com/documentation/arkit/arsession?language=objc) pointer.

[!include[](../../snippets/apple-arkit-trademark.md)]
