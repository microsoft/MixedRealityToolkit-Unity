# Configuration and profiles

Not every single consumer of the MRTK will want it to behave the same way - some will want to have the spatial
mesh running when on AR devices that support it. Some may want the diagnostic visualization on all the time,
and some may only want it on when the user says a voice command. 

The MRTK needs to be configurable in order to support a wide range of those requirements, and it uses a concept
called 'Profiles' to accomplish this.

## What is a profile?

A profile is a serialized asset (an instance of a ScriptableObject) which contains properties that configure
the initialization and runtime behavior of an MRTK service. For example, the MRTK has a camera service, which
will apply different properties to the main camera, depending on whether or not the display is transparent
(like in the case of a HoloLens) or opaque (like in the case of a VR headset). The camera service is given
a [camera profile](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit/Definitions/MixedRealityCameraProfile.cs),
which contains those different transparent vs. opaque settings.

While the camera service may be a simple example of how a profile is used, it is a poor one because of its
simplity - in typical Unity usage, this configuration may just have been exposed as simple fields on the
"Camera" MonoBehaviour itself. If all of the MRTK services had as few options as a camera profile, there
would have been no reason to add the massive complexity introduced by profiles.

Profiles can be more useful in more complicated cases, like in the [InputSystem](https://github.com/microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit/Definitions/InputSystem/MixedRealityInputSystemProfile.cs).
Some of the properties on that profile (such as the MixedRealityInputDataProviderConfiguration entities)
control the objects that will be instantiated at runtime - this is how the input system knows how to create
OpenVR, WMR and Unity input subsystems. This profile is not just a set of properties that configures if
a subfeature is enabled or not - it's also an injection mechanism that the MRTK will use to "new" other classes
at runtime.

Profiles also affect (and are no limited to) things like:

1) What controller mappings are
2) What types of pointers (these are discussed in the [Input System](InputSystem.md) overview)
   are associated with various controllers.

Profiles can be tricky to navigate at first because of deeply nested they can be (especially the Input System).
They can also be confusing because they seem immutable at first (all options are initially greyed out, until
profiles are cloned - this was done to ensure that updates to the MRTK's default profiles wouldn't blow away
client code changes during build-to-build updates).

## Where do I update profiles?

While profiles can be individually updated (by going to the serialized asset of the ScriptableObject),
they are generally accessed through the MRTK inspector of the root MixedRealityToolkit scene object.

![Profile](../../Documentation/Images/Profiles/input_profile.png)

The picture above shows the sheer volume of settings - note that each option on the left will show
the configuration for its corresponding service.

### Camera

Contains per-display type settings. Used to apply different levels of quality, clip, and rendering
settings based on the type of display that the application is run on (i.e. AR vs VR).

### Input

The largest profile for the most complex subsystem of the MRTK. The various subsystems of input will
be covered in the [Input System](InputSystem.md) documentation itself.

### Teleport

This profile controls how the teleporation system works, which is primarily a VR concept.

### Spatial Mapping

This profile controls how the spatial mesh system works (i.e. this system is responsible for starting
the system that will render the spatial meshes on an AR device). Primarily an AR concept.

### Diagnostics

This controls the visual profiler (note that profiler and profile don't refer to the same thing)
that shows a framerate counter, along with basic memory utilization.

### Scene System

This controls a currently not-enabled-by-default system that is designed to make multi-scene
scenarios easier to work with.

### Extensions

This empty-by-default profile is the extension point where consumers can write and then plug in their
own objects that will be instantiated and run by the MRTK runtime.

### Editor

Contains general settings for editor-only behaviors of the MRTK.
