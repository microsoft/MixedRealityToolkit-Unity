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

### Triplanar mapping
Triplanar mapping is a technique to programmatically texture a mesh. Often used in terrain, meshes without UVs, or difficult to unwrap shapes. This implementation supports world or local space projection, the specification of blending smoothness, and normal map support. Note, each texture used requires 3 texture samples, so please use sparingly in performance critical situations.

![triplanar](https://user-images.githubusercontent.com/13305729/47942385-f9b71080-deae-11e8-8b4f-29a3594d8e96.gif)

A checkbox has also been added to control albedo optimizations. As an optimization albedo operations are disabled when no albedo texture is specified. To control this (as requested by this blog post: http://dotnetbyexample.blogspot.com/2018/10/workaround-remote-texture-loading-does.html) Simply check this box:

![albedoassignment](https://user-images.githubusercontent.com/13305729/47942430-28cd8200-deaf-11e8-8df7-d80a51485047.png)

The lighting model has been tweaked to match the Unity standard shader a little closer. And, the example comparison scene has been updated to reflect this:

![matrixcompare](https://user-images.githubusercontent.com/13305729/47942465-4ef32200-deaf-11e8-8a8b-f850d7eaf015.gif)

Fresnel lighting on back facing polygons should now look more correct. (Thank you Gerrit Lochmann for the fix).

Finally, normal map scale is now supported and a few extra material gallery examples have been added.

![newmaterials](https://user-images.githubusercontent.com/13305729/48226180-ec3cd300-e353-11e8-87f7-42b952a2c742.gif)

