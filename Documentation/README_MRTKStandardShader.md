# MRTK Standard Shader
![](../Documentation/Images/MRTKStandardShader/MRTK_StandardShader.jpg)

MRTK Standard shading system utilizes a single, flexible shader that can achieve visuals similar to Unity's Standard Shader, implement Fluent Design System principles, and remain performant on mixed reality devices.

## Example scene
You can find the shader examples in the **MaterialGallery** scene under:
[MixedRealityToolkit.Examples/Demos/StandardShader/Scenes/](/Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Scenes)
![materialgallery](https://user-images.githubusercontent.com/13305729/36511641-4eceafac-171c-11e8-991f-40896f75e2ee.png)

## Features
Per pixel metallic, smoothness, emissive, and occlusion control via channel maps. For example:

![channelmap](https://user-images.githubusercontent.com/13305729/43346530-43e88a18-91a6-11e8-8a52-e1c1a60fd8c3.gif)

Built in configurable stencil test support to achieve a wide array of effects. Such as portals:

![stenciltest](https://user-images.githubusercontent.com/13305729/43346556-621f9bc0-91a6-11e8-90fa-0d4015003248.gif)

Instanced color support to give thousands of GPU instanced meshes unique material properties (example scene not in this PR):

![instances](https://user-images.githubusercontent.com/13305729/43346720-243d838e-91a7-11e8-82a3-cbb082052039.gif)

Finally, a scene to compare and test the MRTK/Standard shader against the Unity/Standard shader:

![comparison](https://user-images.githubusercontent.com/13305729/43346748-4640f754-91a7-11e8-927e-60ed4f98e010.gif)

Single pass instanced stereo rendering has also been enabled by default and a few minor MRTK/Standard shader bugs have been fixed.
