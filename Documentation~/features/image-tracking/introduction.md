---
uid: arfoundation-image-tracking-introduction
---
# Introduction to image tracking

Learn about image tracking use cases and the types of image tracking that AR Foundation supports.

Image tracking refers to a device's ability to recognize specific images on flat surfaces, such as posters, playing cards, or digital monitors. On devices that [support](xref:arfoundation-image-tracking-platform-support) moving images, AR Foundation can also track the movement of an image in the environment.

You can use image tracking to locate images in the user's environment and add virtual content to them. For example, you might use image tracking functionality in a digital marketing campaign to detect a product image and provide the user with an on-screen experience displaying a 3D visualization of the product.

To use image tracking, you must know the specific images that you want the user's device to be able to recognize. These known images are called reference images, and you set them up in Unity using special ScriptableObjects called reference image libraries.

## Types of reference image libraries

AR foundation provides support for two types of reference image libraries: static and mutable.

### Static libraries

Static libraries are supported on all platforms that support image tracking. With a static reference image library, you provide a predefined set of reference images before you build your project. Once you've built your app, the reference image libraries are fixed and can't change at runtime.

Static libraries are useful in scenarios where you provide reference images in physical media, for example packaging or printed publications, or in other cases where you don't need to change the reference images you use.

### Mutable libraries

With mutable reference libraries, you can add new reference images to your app at runtime. This type of image tracking is helpful where reference images are likely to change, such as on-screen marketing or scavenger hunt games.

## Image tracking samples

AR Foundation provides two [Image tracking samples](https://github.com/Unity-Technologies/arfoundation-samples/tree/main?tab=readme-ov-file#image-tracking) in the AR Foundation Samples GitHub repository. You can use these samples to learn more about image tracking, or adapt them to meet your project's needs.
