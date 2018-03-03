# What is the "MixedRealityToolkit/Standard" shader?
The MixedRealityToolkit/Standard shader is a collection of shading techniques for mimicking [**Microsoft's Fluent Design System**](https://fluent.microsoft.com/) within Unity 3D.

The goal of this shader is to have a single, flexible shader that can achieve visuals similar to Unity's Standard Shader, implement Fluent Design System principles, and remain performant on mixed reality devices.

## Example Scene

To explore a Unity scene demonstrating materials which use many of the MixedRealityToolkit/Standard's features open **Scenes\MaterialGallery.unity** within Unity's editor, or deploy to a mixed reality device.

## Limitations
- Only one light source is supported, the directional light (additional light can be achieved using lightmapping).
- Performance when using normal mapping can be slow when a material that uses normal mapping takes up most of the view.
