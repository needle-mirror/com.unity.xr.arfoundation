---
uid: arfoundation-markers-introduction
---
# Introduction to markers

Understand what markers are and how they enable you to align virtual content with physical objects or spaces.

AR markers are visual patterns in the physical environment such as QR codes, Micro QR codes, ArUco markers, or AprilTags that can be reliably detected and tracked by AR devices.

When an AR device camera recognizes a marker, it calculates the marker’s position and orientation (pose) in the real world. This enables the application to precisely align or anchor virtual content with physical objects or spaces.

Some marker types, such as QR codes and Micro QR codes, also carry data encoded in the marker itself. For example, a QR code’s encoded data might contain a URL, a text label, or a serial number that the application can use to trigger digital content, perform look ups, or customize behavior. Other marker types, such as ArUco and AprilTag, don't encode any additional data. Their primary role is to provide a unique visual signature for tracking and alignment.

<!--- TODO: Add image of AR Marker sample scene demonstration --->

## The need for markers

Unlike general tracked images, AR markers are specifically designed for machine readability, making them more robust across various lighting conditions and viewing angles. Additionally, markers like QR codes can encode data, such as a URL or product ID, allowing them to function as both a spatial anchor and a direct source of information. These features make markers a highly dependable tool for synchronizing digital content with specific places and objects in the real world.

AR markers provide an explicit, machine-readable visual target that addresses the following challenges:

- **Robust detection**: markers have well-defined visual characteristics which computer vision algorithms can reliably detect across a wide range of lighting and environments.
- **Accurate positioning**: markers allow the device to compute the exact real-world pose, correcting for drift and enabling precise spatial alignment of digital content.
- **Identity and metadata**: markers can encode information, such as a unique identifier or URL, allowing applications to associate content or logic with a specific physical object or location.

<a id="marker-types"/>

## Marker types

Markers encompass several different types of visual markers. Each type has unique characteristics and is suited for different tasks. You can determine the type of a detected marker by checking its [XRMarkerType](xref:UnityEngine.XR.ARSubsystems.XRMarkerType) property.

The following table compares the supported marker types in AR Foundation:

| Marker Type | Has Encoded Data | Has Marker ID| Common Use Cases | Example Image |
| :---------- | :---------- | :----------- | :--------------- | :------------ |
| **QR Code** | Yes | No | Linking physical objects to digital information, such as launching a website or displaying product details. | ![QR Code](../../images/markers/QRCode-example.png)<br/>*QR Code marker with encoded text "OpenXR"* |
| **Micro QR Code** | Yes | No | Component level traceability in manufacturing or for authenticating small, high-value items. | ![Micro QR Code](../../images/markers/MicroQRCode-example.png)<br/>*Micro QR Code marker with encoded text "OpenXR"* |
| **ArUco** | No | Yes | Fast, real-time tracking of multiple distinct objects. | ![ArUco](../../images/markers/ArUco-example.png)<br/>*ArUco marker with marker ID "42"* |
| **AprilTag** | No | Yes | High precision industrial applications where pose accuracy is critical. | ![April Tag](../../images/markers/AprilTag-example.png)<br/>*April Tag marker with marker ID "43"* |

## Common uses of markers

AR markers are a practical, widely adopted technique across many AR experiences. Markers give users and developers an intuitive, physical reference point for digital information and interactivity in AR applications.

Example uses of markers include:

### Persistent content placement

Ensure digital information, models, or annotations appear at the same physical location each time the device detects a marker.

### Interactive experiences

Trigger content or actions by scanning specific markers, such as opening a product detail page when a device recognizes a product’s code.

### Navigation

Place way points or directions in real space by detecting markers at decision points or along a route.

### Object identification

Recognize and differentiate between multiple objects in the real environment using different marker codes.

### Multi-user shared AR experiences

Synchronize content between multiple devices by aligning to the same physical markers.

### Workflow and maintenance

Provide step-by-step instructions or diagnostics when industrial equipment detects a marker.
