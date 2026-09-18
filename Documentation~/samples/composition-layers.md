---
uid: arfoundation-samples-composition-layers
---
# Composition layer samples

Composition layer samples demonstrate how to render interactive UI on composition layers using the [XR Composition Layers](https://docs.unity3d.com/Packages/com.unity.xr.compositionlayers@latest) package with XR Interaction Toolkit input support. You can open these samples in Unity from the `Assets/Scenes/Composition Layers UI` folder.

## Composition layers vs. world space Canvas UI

Traditional world space uGUI canvases are rendered as part of the eye buffer alongside all other scene geometry. This means they are subject to the same rendering pipeline effects including lens distortion correction, fixed foveated rendering, and temporal reprojection. Text and UI elements are rasterized at the eye buffer resolution.

Composition layers bypass the eye buffer entirely. The UI is rendered to an intermediate texture that the XR runtime composites directly onto the display as a separate layer. This produces sharper text and UI elements because the texture is sampled at the display's native resolution without the quality loss from intermediate rendering steps. Composition layers also support curved cylinder surfaces, which are not possible with standard world space canvases.

## Composition Layers UI scene

The `CompositionLayersUI` scene demonstrates rendering interactive UI on quad and cylinder composition layers. The scene includes a toggle to switch between quad and cylinder layer types at runtime while preserving UI interaction.
