# Mixed Reality Toolkit - SDK - UX - Controller support

As part of the Mixed Reality Toolkit SDK, we provide scripts / controls for managing and implementing controllers in your Mixed Reality project.

Currently we provide components for:

## Controller visualizer

Provides a singular function for rendering controller models in a scene, whether it's a generic model for all controller, or controller specific models.
The framework is flexible enough to allow you to provide offsets to rotate and reposition the model as it's drawn
> Scaling isn't affected, it's up to you to pre-scale models appropriate to use in your Mixed Reality Scene

### Controller visualizer usage

Using the visualizer is extremely simple, just add it to an existing GameObject in your scene and provided you have configured your controller correctly, they will simply be instantiated into the scene at runtime when controllers are detected.

![Controller visualizer inspector](../../../../../../../Documentation/Images/ControllerVisualizer/ControllerVisualizerInspector.png)

Check the documentation on configuring Controller Profiles for more details:

* [MixedReality Controller Configuration Profile configuration](../../../../Profiles/MixedRealityControllerConfigurationProfile.md)

### Controller visualizer notes

The controller visualizer is still in active development, new features that will be added in the future include:

* Ability to interrogate a given model to extract attachment nodes
* Ability to use animation configuration for a given model
* Configuration to be able to animate models from actions by animation or change in pose
