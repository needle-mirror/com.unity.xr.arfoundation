---
uid: arfoundation-raycasts-single
---
# Single ray casts

Single ray casts occur once. Use single ray casts for discrete, one time user interactions with the environment. For example, placement of a virtual object on a detected surface to preview what it looks like in a user's room.

 There are two `Raycast` methods on the [ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager) that perform single ray casts. Refer to [Optional feature platform support](xref:arfoundation-raycasts-platform-support#optional-features-support-table) to check whether your target platform supports each type of ray casting method.

| Method                                              | Description                                                             |
|:--------------------------------------------------- | :---------------------------------------------------------------------  |
| [Viewport based ray cast](#viewport-based-ray-cast) | Casts a ray from a two-dimensional pixel position on the screen (a screen point). |
| [World based ray cast](#world-based-ray-cast)       | Casts an arbitrary ray (a position and direction).                      |

## Viewport based ray cast

The viewport based [Raycast](xref:UnityEngine.XR.ARFoundation.ARRaycastManager.Raycast(UnityEngine.Vector2,List{UnityEngine.XR.ARFoundation.ARRaycastHit},UnityEngine.XR.ARSubsystems.TrackableType)) method casts a ray from a two-dimensional pixel position on the screen:

[!code-cs[ARRaycastManager_Raycast_screenPoint](../../../Runtime/ARFoundation/ARRaycastManager.cs#ARRaycastManager_Raycast_screenPoint)]

You can, for example, pass a touch position directly:

[!code-cs[raycast_using_touch](../../../Tests/CodeSamples/RaycastSamples.cs#raycast_using_touch)]

## World based ray cast

The world based [Raycast](xref:UnityEngine.XR.ARFoundation.ARRaycastManager.Raycast(UnityEngine.Ray,List{UnityEngine.XR.ARFoundation.ARRaycastHit},UnityEngine.XR.ARSubsystems.TrackableType)) method takes an arbitrary [Ray](xref:UnityEngine.Ray) (a position and direction):

[!code-cs[ARRaycastManager_Raycast_ray](../../../Runtime/ARFoundation/ARRaycastManager.cs#ARRaycastManager_Raycast_ray)]

## Parameters

The following table describes the common parameters of the `Raycast` methods:

| Parameter       | Description |
| :-------------- | :---------- |
| `hitResults`    | Contents are replaced with the ray cast results, if successful. Results are sorted by distance in closest-first order. |
| `trackableType` | (Optional) The types of trackables to cast against. |

## Determine what the ray cast hit

If the ray cast hits something, `hitResults` is populated with a `List` of [ARRaycastHits](xref:UnityEngine.XR.ARFoundation.ARRaycastHit).

Use the [hitType](xref:UnityEngine.XR.ARFoundation.ARRaycastHit.hitType) to determine what kind of thing the ray cast hit. If it hit a [trackable](xref:UnityEngine.XR.ARFoundation.ARTrackable), such as a [plane](xref:UnityEngine.XR.ARFoundation.ARPlane), then the [ARRaycastHit.trackable](xref:UnityEngine.XR.ARFoundation.ARRaycastHit.trackable) property can be cast to that type of trackable:

[!code-cs[raycasthit_trackable](../../../Tests/CodeSamples/RaycastSamples.cs#raycasthit_trackable)]
