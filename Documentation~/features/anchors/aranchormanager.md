---
uid: arfoundation-anchors-aranchormanager
---
# AR Anchor Manager component

The [ARAnchorManager](xref:UnityEngine.XR.ARFoundation.ARAnchorManager) component is a type of [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers) that tracks anchors. As a trackable manager, it creates GameObjects in your scene for each tracked anchor.

![AR Anchor Manager component](../../images/ar-anchor-manager.png)<br/>*AR Anchor Manager component*

| Property | Description |
| :------- | :---------- |
| **trackables Changed** | Invoked when trackables have changed (been added, updated, or removed). |
| **Anchor Prefab** | If not `null`, this prefab is instantiated for each tracked anchor. If the prefab doesn't contain an [AR Anchor component](xref:arfoundation-anchors-aranchor), `ARAnchorManager` will add one. |

## Get started

Add an AR Anchor Manager component to your XR Origin GameObject to enable your app to create and track anchors. If your scene doesn't contain an XR Origin GameObject, first follow the [Scene setup](xref:arfoundation-scene-setup) instructions.

If the device doesn't [support](xref:arfoundation-anchors-platform-support) anchors, the AR Anchor Manager component will disable itself during `OnEnable`.

<a id="create-an-anchor"/>

## Create an anchor

While enabled, the AR Anchor Manager component allows you to create anchors in four different ways:

1. **C# scripting — TryAddAnchorAsync** (most widely supported)

    On all platforms that support anchors, you can use [TryAddAnchorAsync](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryAddAnchorAsync(UnityEngine.Pose)) with C# async/await syntax as shown in the follwing code example:

    [!code-cs[TryAddAnchorAsync](../../../Tests/Runtime/CodeSamples/ARAnchorManagerSamples.cs#TryAddAnchorAsync)]

2. **C# scripting — AttachAnchor**

    Some platforms [support](xref:arfoundation-anchors-platform-support#optional-features-support-table) the ability to attach anchors to other trackables such as planes. The following code sample demonstrates how to check for support, then attach an anchor to a pose on a plane's surface with [AttachAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.AttachAnchor(UnityEngine.XR.ARFoundation.ARPlane,UnityEngine.Pose)).

    [!code-cs[AttachAnchor](../../../Tests/Runtime/CodeSamples/ARAnchorManagerSamples.cs#AttachAnchor)]

3. **C# scripting — TryLoadAnchorAsync**

    Some platforms [support](xref:arfoundation-anchors-platform-support#optional-features-support-table) the ability to save anchors from one AR session and load them in subsequent AR sessions. Refer to [Persistent anchors](xref:arfoundation-anchors-persistent) for more information.

4. **Add an AR Anchor component to a GameObject**

    When an AR Anchor component is enabled at runtime, it will use the AR Anchor Manager in your scene to attempt to add itself as an anchor. If you choose to create anchors with the AR Anchor component, it is important to understand the limitations of this approach.

    > [!IMPORTANT]
    > When you enable an AR Anchor component at runtime, it makes a request to the AR Anchor Manager to add itself as an anchor. This request can fail, and if it fails, the AR Anchor component deactivates its GameObject and is not tracked.
    >
    > You should use `ARAnchorManager.trackablesChanged` to verify that the anchor was successfully added before you parent any content to a newly enabled AR Anchor component.

## Parent your content to the anchor

The most common use case for anchors is to place virtual content in the physical world. After you create an anchor, you can [Instantiate](https://docs.unity3d.com/ScriptReference/Object.Instantiate.html) a prefab as a child of that anchor, or call [Transform.SetParent](https://docs.unity3d.com/ScriptReference/Transform.SetParent.html) to reparent a GameObject in your scene to the anchor GameObject.

<a id="anchor-life-cycle-events"/>

## Anchor life cycle events

While enabled, the AR Anchor Manager component will get changes reported by the [XRAnchorSubsystem](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem) every frame. If any anchors were added, updated, or removed, AR Anchor Manager will invoke its [trackablesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackablesChanged) event with the relevant information.

You can subscribe to `trackablesChanged` using either the Inspector or C# scripting.

1. **Use the Inspector**

    a. Create a public method on a `MonoBehaviour` or `ScriptableObject` with a single paramater of type [ARTrackablesChangedEventArgs\<ARAnchor\>](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1), as shown in the following example code:

    [!code-cs[AnchorsChanged](../../../Tests/Runtime/CodeSamples/ARTrackableManagerSamples.cs#AnchorsChanged)]

    b. Select your XR Origin GameObject, then click the **Add (+)** button on the AR Anchor Manager component's **trackables Changed** property.

    c. Using the Object picker (⊙), select either a GameObject that contains an instance of your component or an instance of your ScriptableObject, whichever is applicable.

    ![A trackablesChanged event is shown in the Inspector with a subscribed MonoBehavior](../../images/ar-trackable-manager-trackables-changed.png)<br/>*Subscribe to the trackablesChanged event*

    d. In the dropdown, select your class name and the name of your method. The method name appears in the **Dynamic** section of the methods list.

2. **Use C# scripting**

    a. Create a public method with a single parameter of type [ARTrackablesChangedEventArgs\<ARAnchor\>](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1) as shown in the preceding step 1a.

    b. Use the following example code to subscribe to the `trackablesChanged` event:

    [!code-cs[AnchorsSubscribe](../../../Tests/Runtime/CodeSamples/ARTrackableManagerSamples.cs#AnchorsSubscribe)]

<a id="remove-an-anchor"/>

## Remove an anchor

While enabled, the AR Anchor Manager component allows you to remove anchors in two ways:

1. **C# scripting**

    You can use the [TryRemoveAnchor](xref:UnityEngine.XR.ARFoundation.ARAnchorManager.TryRemoveAnchor(UnityEngine.XR.ARFoundation.ARAnchor)) API, passing the `ARAnchor` that you wish to remove. When you remove an anchor this way, the next `ARAnchorManager.trackablesChanged` event reports the `ARAnchor` component in its [removed](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1.removed) list.

2. **Destroy the AR Anchor component**

    When you [Destroy](xref:UnityEngine.Object.Destroy(UnityEngine.Object)) an AR Anchor component, it will use the AR Anchor Manager component in your scene to remove itself from the anchor subystem. When you remove an anchor this way, the next `ARAnchorManager.trackablesChanged` event contains a null `ARAnchor` component in its [removed](xref:UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs`1.removed) list, because you destroyed the component before this event was invoked.

## Visualize anchors in the scene

By default, the AR Anchor Manager doesn't render any geometry in the scene when anchors are detected. To enable anchor visualization, set a prefab as the AR Anchor Manager's **Anchor Prefab**.

The [AR Foundation Samples](https://github.com/Unity-Technologies/arfoundation-samples) GitHub repository contains a prefab that you can use to get started:

| Prefab | Description |
| :----- | :---------- |
| [AR Anchor Debug Visualizer](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Prefabs/AR%20Anchor%20Debug%20Visualizer.prefab) | Visualize anchors with a Transform gizmo, and optionally visualize additional information such as the anchor's [trackableId](xref:UnityEngine.XR.ARFoundation.ARTrackable`2.trackableId), [sessionId](xref:UnityEngine.XR.ARFoundation.ARAnchor.sessionId), [trackingState](xref:UnityEngine.XR.ARFoundation.ARTrackable`2.trackingState), and whether the anchor is attached to a plane. |
