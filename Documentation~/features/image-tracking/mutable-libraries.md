---
uid: arfoundation-image-tracking-mutable-libraries
---
# Add images to a reference image library at runtime

On [supported platforms](xref:arfoundation-image-tracking-platform-support), use this workflow to create a mutable reference image library and add images at runtime.

If this is your first exposure to reference image libraries, first refer to [Introduction to image tracking](xref:arfoundation-image-tracking-introduction).

## Create a runtime reference image library

When AR Foundation uses a reference image library at runtime, it converts the library from type `XRReferenceImageLibrary`, the Editor-friendly representation, to `RuntimeReferenceImageLibrary`, the runtime representation. You need a `RuntimeReferenceImageLibrary` to use mutable library APIs, and you can create one from an `XRReferenceImageLibrary` using [CreateRuntimeLibrary](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.CreateRuntimeLibrary(UnityEngine.XR.ARSubsystems.XRReferenceImageLibrary)):

[!code-cs[trackedimage_createruntimelibraryfromserialized](../../../Tests/Runtime/CodeSamples/TrackedImageSamples.cs#trackedimage_createruntimelibraryfromserialized)]

> [!NOTE]
> The ordering of the [XRReferenceImage](xref:UnityEngine.XR.ARSubsystems.XRReferenceImage)s in the `RuntimeReferenceImageLibrary` is undefined, so it might not match the order in which the images appeared in the source `XRReferenceImageLibrary`.

Alternatively, to create an empty library, call `CreateRuntimeLibrary` without arguments:

[!code-cs[trackedimage_createemptyruntimelibrary](../../../Tests/Runtime/CodeSamples/TrackedImageSamples.cs#trackedimage_createemptyruntimelibrary)]

## Add reference images at runtime

If your target platform supports mutable libraries, any `RuntimeReferenceImageLibrary` is also a mutable reference library. To add images to the library, simply typecast to `MutableReferenceImageLibrary` as shown in the following code sample:

[!code-cs[trackedimage_addtomutablelibrary](../../../Tests/Runtime/CodeSamples/TrackedImageSamples.cs#trackedimage_addtomutablelibrary)]

[ScheduleAddImageWithValidationJob](xref:UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageWithValidationJob(UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary,UnityEngine.Texture2D,System.String,System.Nullable{System.Single},Unity.Jobs.JobHandle)) returns a [JobHandle](xref:Unity.Jobs.JobHandle) that you can use to determine when the job is complete, as adding an image can be computationally resource-intensive and might require a few frames. You can safely discard the job handle if you don't need to monitor the completion of the job.

Multiple add image jobs can be processed concurrently, regardless of whether image tracking is already running in your app when you schedule the job.

> [!NOTE]
> You can't remove reference images from a reference image library at runtime. To stop tracking a particular reference image, instead use the AR Tracked Image Manager to [set a different library at runtime](xref:arfoundation-image-tracking-manager#set-reference-library-runtime).

### Add reference images from NativeArrays

AR Foundation also supports adding reference images from a `NativeArray` instead of `Texture2D`. [ScheduleAddImageJobImpl](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary.ScheduleAddImageJobImpl(Unity.Collections.NativeSlice{System.Byte},UnityEngine.Vector2Int,UnityEngine.TextureFormat,UnityEngine.XR.ARSubsystems.XRReferenceImage,Unity.Jobs.JobHandle)) accepts a [NativeSlice](xref:Unity.Collections.NativeSlice`1) or pointer. Note that you are responsible for freeing the memory in your `NativeArray` when the job completes. You can do this by scheduling a dependent job that frees the memory:

[!code-cs[trackedimage_DeallocateOnJobCompletion](../../../Tests/Runtime/CodeSamples/TrackedImageSamples.cs#trackedimage_DeallocateOnJobCompletion)]

## Texture import settings

Image detection algorithms rely on accurate knowledge of a reference image's aspect ratio. However, Unity's default texture import setting adjusts texture resolutions to the nearest powers of 2 (PoT). This can have a negative effect on tracking capabilities when tracking non-power of 2 (NPoT) images.

When you import an NPoT image with default settings and add it to a `MutableRuntimeReferenceImageLibrary`, you might see squashing, stretching, or scaling artifacts.

### Configure texture import settings

To ensure images maintain their aspect ratio when imported:

1. Select the images from your `Assets` folder to view the **Import Settings** in the **Inspector**.
2. Under **Advanced** settings, change the **Non-Power of 2** texture import setting to **None**.

>[!NOTE]
> For textures with power-of 2 aspect ratios, **Non-Power of 2** setting will not be selectable as no adjustments are necessary.

## Related resources

* [Unity Job System](https://docs.unity3d.com/Manual/JobSystem.html)
* [Introduction to image tracking](xref:arfoundation-image-tracking-introduction)
* [AR Tracked Image Manager component](xref:arfoundation-image-tracking-manager)
