---
uid: arfoundation-samples-object-tracking
---
# Object tracking samples

The `Object Tracking` sample demonstrates AR Foundation [Object tracking](xref:arfoundation-object-tracking) functionality. You can open this sample in Unity from the `Assets/Scenes/ObjectTracking` folder.

[!include[](../../snippets/samples-tip.md)]

## Requirements

Object tracking is supported by ARKit only. This sample requires iOS 12 or newer.

To use this sample, you must have a physical object the device can recognize as described in [Reference objects](#reference-objects).

<a id="reference-objects"></a>

## Reference objects

To use object tracking you must use a reference object library. You can [Use Unity's sample reference objects](#use-unitys-sample-reference-objects) or [Create your own reference objects](#create-your-own-reference-objects). To learn more about reference objects, refer to [Object tracking](xref:arfoundation-object-tracking)

### Use Unity's sample reference objects

The sample's reference object library is built using two reference objects. The sample includes printable templates which you can print on `8.5×11` inch paper and fold into a cube and cylinder. You can find these printable templates in the `Assets/Scenes/ObjectTracking/Printable Templates` folder.

### Create your own reference objects

You can scan your own objects and add them to the `XRReferenceObjectLibrary`. Refer to [Create a reference object library](xref:arfoundation-object-tracking#create-library) to understand how to create your own reference library.

## Object tracking scene

The `Object tracking` scene detects a 3D object from a set of reference objects in an `XRReferenceObjectLibrary`.

[!include[](../../snippets/samples-tip.md)]

[!include[](../../snippets/apple-arkit-trademark.md)]
