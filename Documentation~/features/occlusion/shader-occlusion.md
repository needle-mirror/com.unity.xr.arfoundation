---
uid: arfoundation-shader-occlusion
---
# AR Shader Occlusion component

The [ARShaderOcclusion](xref:UnityEngine.XR.ARFoundation.ARShaderOcclusion) component exposes depth render texture information directly to global shader memory, instead of working through the [AR Camera Background component](xref:arfoundation-camera-components#ar-camera-background-component). With shader occlusion, you're able to customize occlusion functionality by writing your own shaders. You can use the occlusion and confidence maps to apply techniques such as custom edge smoothing, and visual effects.

Shader occlusion enables occlusion on devices with a Head-mounted Display (HMD). The AR Shader Occlusion component will only function on [OpenXR platforms](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.13/manual/index.html#runtimes). The AR Shader Occlusion component may make available multiple depth render textures if stereo depth information is provided by the platform.

The component subscribes to [AROcclusionManager.frameReceived](xref:UnityEngine.XR.ARFoundation.AROcclusionManager.frameReceived) to receive depth textures, then uses [Graphics.CopyTexture](xref:UnityEngine.Graphics.CopyTexture(UnityEngine.Texture,UnityEngine.Texture)) to copy the textures from the GPU to the CPU. Once it has done so, it calls [Shader.SetGlobalTexture](xref:UnityEngine.Shader.SetGlobalTexture(System.Int32,UnityEngine.Texture)) to store the textures as a global texture for use in shaders.

![AR Shader Occlusion component](../../images/ar-shader-occlusion.png)<br/>*AR Shader Occlusion component.*

## Add the AR Shader Occlusion component to your scene

To enable shader occlusion, add the `ARShaderOcclusion` and [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager) to your [Camera](xref:arfoundation-camera) as outlined in [Managers](xref:arfoundation-managers).

## Shader occlusion samples

![HMD occlusion shader example image](../../images/hmd-occlusion-shader-example.png)<br/>*HMD occlusion shader example image.*

For general information on writing shaders, consult the [Writing shaders](https://docs.unity3d.com/Manual/shader-writing.html) page in the Unity manual.

The AR Foundation Samples app provides an example of how to use HMD occlusion with shaders in the [HMD Occlusion](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scenes/Occlusion/HMDOcclusion/HMDOcclusion.unity) sample scene.
