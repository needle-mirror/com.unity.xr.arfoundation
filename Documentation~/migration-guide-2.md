# Migration Guide

This will guide you through the changes coming from AR Foundation 2.x to 3.x.

## Image Tracking

The image tracking manager `ARTrackedImageManager` has a `referenceLibrary` property on it to set the reference image library (the set of images to detect in the environment). Previously, this was an `XRReferenceImageLibrary`. Now, it is an `IReferenceImageLibrary`, and `XRReferenceImageLibrary` implements `IReferenceImageLibrary`. If you had code that was setting the `referenceLibrary` property to an `XRReferenceImageLibrary`, it should continue to work as before. However, if you previoulsy treated the `referenceLibrary` as an `XRReferenceImageLibrary`, you will have to attempt to cast it to a `XRReferenceImageLibrary`.

In the Editor, it will always be an `XRReferenceImageLibrary`. However, at runtime with image tracking enabled, `ARTrackedImageManager.referenceLibrary` will return a new type, `RuntimeReferenceImageLibrary`. This still behaves like an `XRReferenceImageLibrary` (e.g., you can enumerate its reference images), and it may also have additional functionality (see `MutableRuntimeReferenceImageLibrary`).

## Background shaders

The `ARCameraBackground` has been updated to support the lightweight render pipeline (LWRP) and Universal Render Pipelines (UniversalRP) when those packages are present. This involved a breaking change to the `XRCameraSubsystem`: the property `shaderName` is now `cameraMaterial`. It is unlikely most developers would need to access this directly. The shader name was only used by ARFoundation to construct the background material. That functionality has moved to the subsystem.