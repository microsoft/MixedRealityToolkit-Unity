# MRTK Standard Shader

![Standard shader examples](../Documentation/Images/MRTKStandardShader/MRTK_StandardShader.jpg)

MRTK Standard shading system utilizes a single, flexible shader that can achieve visuals similar to Unity's Standard Shader, implement [Fluent Design System](https://www.microsoft.com/design/fluent/) principles, and remain performant on mixed reality devices.

## Example Scenes

You can find the shader material examples in the **MaterialGallery** scene under:
[MixedRealityToolkit.Examples/Demos/StandardShader/Scenes/](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Scenes) All materials in this scene are using the MRTK/Standard shader.

![materialgallery](../Documentation/Images/MRTKStandardShader/MRTK_MaterialGallery.jpg)

You can find a comparison scene to compare and test the MRTK/Standard shader against the Unity/Standard shader example in the **StandardMaterialComparison** scene under: [MixedRealityToolkit.Examples/Demos/StandardShader/Scenes/](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Scenes)

![comparison](../Documentation/Images/MRTKStandardShader/MRTK_StandardMaterialComparison.gif)

## Architecture

The MRTK/Standard shading system is an "uber shader" that uses [Unity's shader program variant feature](https://docs.unity3d.com/Manual/SL-MultipleProgramVariants.html) to auto-generate optimal shader code based on material properties. When a user selects material properties in the material inspector they only incur performance cost for features they have enabled.

A custom material inspector exists for the MRTK/Standard shader called **MixedRealityStandardShaderGUI.cs**. The inspector automatically enables/disables shader features based on user selection and aides in setting up render state. For more information about each feature please hover over each property in the Unity Editor for a tooltip.

![materialinspector](../Documentation/Images/MRTKStandardShader/MRTK_MaterialInspector.jpg)

## Lighting

The MRTK/Standard uses a simple approximation for lighting. Because this shader does not calculate for physical correctness and energy conservation, it renders quickly and efficient. Blinn-Phong is the primary lighting technique which is blended with Fresnel and image based lighting to approximate physically based lighting. The shader supports the following lighting techniques:

### Directional Light

The shader will respect the direction, color, and intensity of the first Unity Directional Light in the scene (if enabled). Dynamic point lights, spot lights, or any other Unity light will not be considered in real time lighting.

### Spherical Harmonics

The shader will use Light Probes to approximate lights in the scene using [Spherical Harmonics](https://docs.unity3d.com/Manual/LightProbes-TechnicalInformation.html) if enabled. Spherical harmonics calculations are performed per vertex to reduce calculation cost.

### Lightmapping

For static lighting the shader will respect lightmaps built by Unity's [Lightmapping system](https://docs.unity3d.com/Manual/Lightmapping.html) simply mark the renderer as static (or lightmap static) to use lightmaps.

### Hover Light

A Hover Light is a Fluent Design System paradigm that mimics a "point light" hovering near the surface of an object. Often used for far away cursor lighting the application can control the properties of a Hover Light via the [**HoverLight.cs**](xref:Microsoft.MixedReality.Toolkit.Utilities.HoverLight). Up to 3 Hover Lights are supported at a time.

### Proximity Light

A Proximity Light is a Fluent Design System paradigm that mimics a "gradient inverse point light" hovering near the surface of an object. Often used for near cursor lighting the application can control the properties of a Proximity Light via the [**ProximityLight.cs**](xref:Microsoft.MixedReality.Toolkit.Utilities.ProximityLight). Up to 2 Proximity Lights are supported at a time.

## Lightweight Scriptable Render Pipeline Support

The MRTK contains an upgrade path to allow developers to utilize Unity's Lightweight Scriptable Render Pipeline (LWRP) with MRTK shaders. Tested in Unity 2019.1.1f1 and Lightweight RP 5.7.2 package. or instructions on getting started with the LWRP please see [this page](https://docs.unity3d.com/Packages/com.unity.render-pipelines.lightweight@5.10/manual/getting-started-with-lwrp.html).

To perform the MRTK upgrade select: **Mixed Reality Toolkit -> Utilities -> Upgrade MRTK Standard Shader for Lightweight Render Pipeline**

![lwrpupgrade](../Documentation/Images/MRTKStandardShader/MRTK_LWRPUpgrade.jpg)

After the upgrade occurs the MRTK/Standard shader will be altered and any magenta (shader error) materials should be fixed. To verify the upgrade successfully occurred please check the console for: **Upgraded Assets/MixedRealityToolkit/StandardAssets/Shaders/MixedRealityStandard.shader for use with the Lightweight Render Pipeline.**

## Texture Combiner

To improve parity with the Unity Standard shader per pixel metallic, smoothness, emissive, and occlusion values can all be controlled via [channel packing](http://wiki.polycount.com/wiki/ChannelPacking). For example:

![channelmap](../Documentation/Images/MRTKStandardShader/MRTK_ChannelMap.gif)

When you use channel packing, you only have to sample and load one texture into memory instead of four separate ones. When you write your texture maps in a program like Substance or Photoshop, you can pack hand pack them like so:

| Channel | Property             |
|---------|----------------------|
| Red     | Metallic             |
| Green   | Occlusion            |
| Blue    | Emission (Greyscale) |
| Alpha   | Smoothness           |

Or, you can use the MRTK Texture Combiner Tool. To open the tool select: **Mixed Reality Toolkit -> Utilities -> Texture Combiner** which will open the below window:

![texturecombiner](../Documentation/Images/MRTKStandardShader/MRTK_TextureCombiner.jpg)

This windows can be automatically filled out by selecting a Unity Standard shader and clicking "Autopopulate from Standard Material." Or, you can manually specify a texture (or constant value) per red, green, blue, or alpha channel. The texture combination is GPU accelerated and does not require the input texture to be CPU accessible.

## Additional Feature Documentation
Below are extra details on a handful of features details available with the MRTK/Standard shader.

Performant plane, sphere, and box shape clipping with the ability to specify which side of the primitive to clip against (inside or outside).

![primitiveclipping](../Documentation/Images/MRTKStandardShader/MRTK_PrimitiveClipping.gif)

[**ClippingPlane.cs**](xref:Microsoft.MixedReality.Toolkit.Utilities.ClippingPlane), [**ClippingSphere.cs**](xref:Microsoft.MixedReality.Toolkit.Utilities.ClippingSphere), and [**ClippingBox.cs**](xref:Microsoft.MixedReality.Toolkit.Utilities.ClippingBox) can be used to easily control clipping primitive properties.

![primitiveclippinggizmos](../Documentation/Images/MRTKStandardShader/MRTK_PrimitiveClippingGizmos.gif)

Built in configurable stencil test support to achieve a wide array of effects. Such as portals:

![stenciltest](../Documentation/Images/MRTKStandardShader/MRTK_StencilTest.gif)

Instanced color support to give thousands of GPU instanced meshes unique material properties:

![instancedproperties](../Documentation/Images/MRTKStandardShader/MRTK_InstancedProperties.gif)

Triplanar mapping is a technique to programmatically texture a mesh. Often used in terrain, meshes without UVs, or difficult to unwrap shapes. This implementation supports world or local space projection, the specification of blending smoothness, and normal map support. Note, each texture used requires 3 texture samples, so please use sparingly in performance critical situations.

![triplanar](../Documentation/Images/MRTKStandardShader/MRTK_TriplanarMapping.gif)

A checkbox to control albedo optimizations. As an optimization albedo operations are disabled when no albedo texture is specified. To control this (as requested by this blog post: http://dotnetbyexample.blogspot.com/2018/10/workaround-remote-texture-loading-does.html) Simply check this box:

![albedoassignment](../Documentation/Images/MRTKStandardShader/MRTK_AlbedoAssignment.jpg)

Per pixel clipping textures, local edge based anti aliasing, and normal map scaling are supported.

![normalmapscale](../Documentation/Images/MRTKStandardShader/MRTK_NormalMapScale.gif)

Vertex extrusion in world space. Useful for visualizing extruded bounding volumes or transitions in/out meshes.

![normalmapscale](../Documentation/Images/MRTKStandardShader/MRTK_VertexExtrusion.gif)

## See also
- [Interactable](README_Interactable.md)