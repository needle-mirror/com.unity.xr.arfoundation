---
uid: arfoundation-custom-background-shaders
---
# Custom background shaders

Any custom background shaders to render onto XR device screens will use a display matrix to transform the CPU side image texture coordinates. Reference this manual page for the [display matrix format and derivation](xref:arfoundation-display-matrix-format-and-derivation).

## XR vertex shader operation

The vertex shaders will perform the following mathematical operation, where "t<sub>i</sub>" are the texture coordinates of the CPU side image, "D" is the display matrix, and "t<sub>d</sub>" are the texture coordinates of XR device's screen:

![A vector-matrix multiplication equation. It is a 1 by 4 row vector times a 4 by 4 matrix, which equals another 1 by 4 row vector. The components of the first 1 by 4 row vector are "t i dot x", "t i dot y", 1, 0. "t i" are the texture coordinates of the image. The 4 by 4 matrix is the display matrix called "D." The components of the second 1 by 4 row vector are "t d dot x", "t d dot y", 1, 0. "t_d" are the texture coordinates of the device screen.](https://media.github.cds.internal.unity3d.com/user/7904/files/a362c2c6-80e3-40b3-8841-4aaddda29790)<br/>*Use the display matrix and image texture coordinates to find the output device coordinates*

### GLSL example code
For GLSL vertex shaders, the following line will transform image texture coordinates to device screen coordinates:
<pre>
outputTextureCoord = (vec4(gl_MultiTexCoord0.x, gl_MultiTexCoord0.y, 1.0f, 0.0f) * _UnityDisplayTransform).xy;
</pre>

where `_UnityDisplayTransform` is the display matrix and `gl_MultiTexCoord0` are the 2D texture coordinates of the image.

Note, the GLSL `*` operator is overloaded to use a row vector when a matrix is premultiplied by a vector. Refer to this doc for more information: https://en.wikibooks.org/wiki/GLSL_Programming/Vector_and_Matrix_Operations#Operators
<br/>

### HLSL example code
For HLSL vertex shaders, the following line will transform image texture coordinates to device screen coordinates:
<pre>
outputTextureCoord = mul(float3(v.texcoord, 1.0f), _UnityDisplayTransform).xy;
</pre>

where `_UnityDisplayTransform` is the display matrix and `v` are the 2D texture coordinates of the image.

Note, the HLSL function `mul()` is overloaded to use a row vector when a matrix is premultiplied by a vector and to still perform the operation without the `0` in the last index of the row vector, as it is not necessary for the final output texture coordinates. Refer to this doc for more information: https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-mul