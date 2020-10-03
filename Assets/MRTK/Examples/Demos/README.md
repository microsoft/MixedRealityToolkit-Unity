# Mixed Reality Toolkit Examples - Demos

This folder contains demonstrations of the currently implemented features supplied with the Mixed Reality Toolkit.  These include:

## Audio

The MRTK provides 3D spatial audio capabilities and utilities to aid you in the production of 3D audio in a Mixed Reality environment.
This folder contains two demonstration implementations for:

* Audio LoFi Effects
* Audio Occlusion

## Boundary

The MRTK provides several capabilities to both represent and manage the boundary provided by the various implemented systems.

This folder provides a simple demonstration to visualize the boundary reported by the active system, if provided.

## Solvers

Solvers provide many advanced ways to link or place objects within a scene and to attach them to other objects or even the player.
Solvers also allow objects to be aware of their surroundings and moved with / against them in various ways.

This folder contains a simple demonstration scene showing several of the solver options and how they can be applied in a scene.

## Standard shader

The MRTK standard shader is specifically customized for use in Mixed Reality environments and enabling several advanced effects "out of the box".

This folder includes three demonstrations to show off the capabilities of the MRTK Standard Shader and how to configure it for each effect, namely:

* Material Gallery
A full gallery of all the effects supported by the MRTK Standard Shader.

* Standard Material Comparison
A side by side comparison of the Unity Standard Shader and the MRTK Standard Shader.

* Standard materials
A gallery of the standards material assets provided with the MRTK and how they look using the MRTK Standard Shader.

## UX

The Mixed Reality Toolkit provides several aids to build the UX in your Mixed Reality Solution, these currently include:

* Object Collections
Several mechanisms to orientate and place object in a scene relative to each other. E.G. Side by side arrays, spherical arrays and much more.

* Lines
Several default line drawing options for use in your project. Also utilized by the MRTK itself for controller pointer and teleportation use.

> Keep checking back often as more UX controls are added.

## Reading Mode

* Reading Mode Demo
Demonstrates how to alter the HoloLens2 field of view to trade view size for detail.  This is called [Reading Mode](https://docs.microsoft.com/en-us/hololens/hololens2-display).  Reading Mode may be helpful when small details need to be visualized on HoloLens2.  There is also the ability to change the Render Viewport Scale which may increase FPS by having the application render fewer pixels.