---
uid: arfoundation-raycasts-tracked
---
# Tracked ray casts

Tracked (or persistent) ray casts continuously project a ray from a two-dimensional pixel position on the screen (a screen point). Tracked ray casts enable continuous user interaction with the physical environment. This enables you to implement features that depend on understanding the physical environment over time and relative to a user's position. For example, you might use tracked ray casts in an app where the user paints a detected wall (a plane) with virtual spray paint. Tracked ray casts continuously track where the user is aiming the paint, and determine when the sprayed paint hits a wall in the physical environment.

Tracked ray casts are a type of [trackable](xref:UnityEngine.XR.ARFoundation.ARTrackable). The [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast) component contains the data associated with a tracked ray cast. Each [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast) continues to update automatically until you remove it or disable the [ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager). Refer to [AR Raycast component](xref:arfoundation-raycasts-arraycast) to understand the `AR Raycast` component and its lifecycle.

Conceptually, tracked ray casts are similar to repeating the same ray cast query each frame, but platforms with direct support for this feature can provide better results. Not all platforms that support ray casting support tracked ray casts. To check whether your platform supports tracked ray casts, refer to [Optional feature platform support](xref:arfoundation-raycasts-platform-support#optional-features-support-table).

## Add a tracked ray cast

To add a tracked ray cast, call [AddRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycastManager.AddRaycast(UnityEngine.Vector2,System.Single)) on the [ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager) component from script code. When you call `AddRaycast`, the `ARRaycastManager` creates a new GameObject with an `AR Raycast` component on it.

You must create tracked ray casts from a screen point:

[!code-cs[ARRaycastManager_AddRaycast_screenPoint](../../../Runtime/ARFoundation/ARRaycastManager.cs#ARRaycastManager_AddRaycast_screenPoint)]

## Ray cast lifecycle events

While enabled, the AR Raycast Manager component gets changes reported by the [XRRaycastSubsystem](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystem) every frame. If any ray casts were added, updated, or removed, AR Raycast Manager invokes its [trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged) event with the relevant information.

You can subscribe to `trackablesChanged` using either the **Inspector** or C# scripting.

1. **Use the Inspector**

    a. Create a public method on a `MonoBehaviour` or `ScriptableObject` with a single parameter of type [ARTrackablesChangedEventArgs\<ARRaycast\>](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1), as shown in the following example code:

    [!code-cs[RacyastsChanged](../../../Tests/CodeSamples/ARTrackableManagerSamples.cs#RaycastsChanged)]

    b. Select your XR Origin GameObject, then click the **Add (+)** button on the AR Raycast Manager component's **trackables Changed** property.

    c. Using the Object picker (⊙), select either a GameObject that contains an instance of your component or an instance of your ScriptableObject, whichever is applicable.

    ![A trackablesChanged event is shown in the Inspector with a subscribed MonoBehavior](../../images/ar-trackable-manager-trackables-changed.png)<br/>*Subscribe to the trackablesChanged event*

    d. In the dropdown, select your class name and the name of your method. The method name appears in the **Dynamic** section of the methods list.

2. **Use C# scripting**

    a. Create a public method with a single parameter of type [ARTrackablesChangedEventArgs\<ARRaycast\>](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1) as shown in the preceding step 1a.

    b. Use the following example code to subscribe to the `trackablesChanged` event:

    [!code-cs[RaycastsSubscribe](../../../Tests/CodeSamples/ARTrackableManagerSamples.cs#RaycastsSubscribe)]

## Remove a ray cast

You can use the [RemoveRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycastManager.RemoveRaycast(UnityEngine.XR.ARFoundation.ARRaycast)) method, passing the `ARRaycast` that you wish to remove. When you remove a ray cast this way, the next `ARRaycastManager.trackablesChanged` event reports the `ARRaycast` component in its [removed](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1.removed) list.

## Visualize a ray cast

When you create a new [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast), AR Foundation creates a new GameObject with an AR Raycast component on it. By default, the AR Raycast Manager doesn't render a visualization of ray casts. You can optionally provide a prefab in the **Raycast Prefab** field of the AR Raycast Manager that is instantiated for each `ARRaycast` to extend the default behavior. For example, you could provide a custom prefab to visualize a line for each `ARRaycast`.
