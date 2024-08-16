---
uid: arfoundation-raycasts
---
# Ray casts

The AR Foundation Raycast API allows platforms to cast a [ray](xref:UnityEngine.Ray) and calculate the intersection between that ray and detected [trackables](xref:UnityEngine.XR.ARFoundation.ARTrackable) in the physical environment. You can use this API to cast rays against specific trackable types, without the need to represent these trackables in your scene. The trackable types that you can possibly hit with a ray cast depend on the [Supported trackables](xref:arfoundation-raycasts-platform-support#supported-trackables) of your target platform.

You can use ray casting functionality to enable users to interact with the physical environment. For example, you can implement ray casting in your application so that a user can place virtual objects on detected physical surfaces.

Refer to the following topics to understand how to use AR Foundation ray casting:

| Topic                                                                     | Description                                          |
| :-----------------------------------------------------------------------  | :--------------------------------------------------- |
| [Ray cast platform support](xref:arfoundation-raycasts-platform-support)  | Discover which AR platforms support ray casting features, and the trackables supported by each AR platform. |
| [AR Raycast Manager component](xref:arfoundation-raycasts-raycastmanager) | Understand the AR Raycast Manager component and the types of ray cast methods it provides. |
| [AR Raycast component](xref:arfoundation-raycasts-arraycast)              | Understand the AR Raycast component.                 |
