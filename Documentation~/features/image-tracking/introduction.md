---
uid: arfoundation-image-tracking-introduction
---
# Introduction to image tracking

Learn about image tracking use cases and the types of image tracking that AR Foundation supports.

Image tracking refers to a device's ability to recognize specific images on flat surfaces, such as posters, playing cards, or digital monitors. On devices that [support](xref:arfoundation-image-tracking-platform-support) moving images, AR Foundation can also track the movement of an image in the environment.

You can use image tracking to locate images in the user's environment and add virtual content to them. For example, you might use image tracking functionality in a digital marketing campaign to detect a product image and provide the user with an on-screen experience displaying a 3D visualization of the product.

To use image tracking, you must know the specific images that you want the user's device to be able to recognize. These known images are called reference images, and you set them up in Unity using special ScriptableObjects called reference image libraries.

## Types of reference image libraries

AR Foundation supports reference image libraries defined at two different times: edit-time and runtime.

### Edit-time libraries

Every platform that supports image tracking also supports edit-time libraries. With an edit-time reference image library, you provide a predefined set of reference images before you build your project. Once you have built your app, you can't change the reference images.

Edit-time libraries are useful when you provide reference images in physical media, such as packaging or printed publications. They also suit cases where you don't need to change the reference images you use.

### Runtime libraries

With a runtime reference image library, your app defines its reference images while it runs, instead of at edit-time. This is useful if you want users to provide their own images for image tracking. Use a runtime library when you don't know the full set of reference images in advance, or when that set changes from one session to the next.

Not all platforms support runtime libraries. To check which providers support them, refer to the **Runtime library** row in [Image tracking platform support](xref:arfoundation-image-tracking-platform-support#optional-feature-platform-support).

#### Mutable libraries

Mutable libraries are a type of runtime library. You can add new images to these libraries without re-creating them. This type of image tracking is useful where your set of reference images is likely to change, such as in a scavenger hunt game.

#### Immutable libraries

Immutable libraries are another type of runtime library. With immutable reference libraries, you provide the full set of reference images when you create the library, and its contents can't change after that. Immutable libraries are more efficient when your set of reference images doesn't change.

## Image tracking samples

The AR Foundation Samples project provides [Image tracking samples](xref:arfoundation-samples-image-tracking). You can use these samples to learn more about image tracking, or adapt them to meet your project's needs.
