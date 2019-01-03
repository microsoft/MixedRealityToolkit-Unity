## Basic Usage

From the dropdown menu:
>Mixed Reality Toolkit / Lighting Estimation / Create Estimation Object

That's it! This will create a `GameObject` in your scene with the `Lightingcapture` component on it, this should work on the HoloLens automatically. If you want this example to work in-editor on a laptop that has a camera and a gyrosocope, add the `FollowGyroscope` component to your scene's `Main Camera`.

You can check out the 'Minimum' scene for an example of the bare minimum require for light estimation to work.

## How does it work?

As the user interacts with the environment, this tool creates a Cubemap using the device's camera. This cubemap is then assigned to one of Unity's Reflection Probes for use by shaders for ambient light and reflection calculations!

When the component loads, it takes a single picture from the camera, and wraps it around the entire cubemap! This provides an initial, immidiate estimate of the lighting in the room that can be improved upon over time.

As the user rotates, the component will 'stamp' the current camera image onto the cubemap, and save that rotation to a cache. As the user continues to rotate, the component will check the cache to see if there's already a stamp there, before adding another stamp. Settings can be configured to make stamps expire as the user moves from room to room.

**IMPORTANT NOTE:** On HoloLens, the camera is locked to a specific exposure to accurately reflect lighting changes in each direction! Other devices **do not** do this due to API availability, which leads to a more even, muddy cubemap. So, if your lighting captures don't look great on your non-HoloLens device, that would be why.

## LightingCapture Settings

- **Map Resolution**
Resolution (pixels) per-face of the generated lighting Cubemap.
- **Single Stamp Only**
Should the component only do the initial wraparound stamp? If true, only one picture will be taken, at the very beginning.
- **Stamp FOV Multiplier**
When stamping a camera picture onto the Cubemap, scale it up by this so it covers a little more space. This can mean fewer total stamps needed to complete the Cubemap, at the expense of a less perfect reflection.
- **Stamp Expire Distance**
This is the distance (meters) the camera must travel for a stamp to expire. When a stamp expires, the Camera will take another picture in that direction when given the opportunity. Zero means no expiration.

- **Use Directional Lighting**
Should the system calculate information for a directional light? This will scrape the lower mips of the Cubemap to find the direction and color of the brightest values, and apply it to the scene's light.
- **Max Light Color Saturation**
When finding the primary light color, it will average the brightest 20% of the pixels, and use that color for the light. This sets the cap for the saturation of that color.
- **Light Angle Adjust Per Second**
The light eases into its new location when the information is updated. This is the speed at which it eases to its new destination, measured in degrees per second.

## Shaders

The ambient lighting information works out of the box with Unity's Standard, Legacy, and Mobile shaders, as well as the MRTK Standard shaders! Reflections only work with the Standard and MRTK Standard shaders. Also included is a lightweight `LightEstimation IBL` shader that does normal + diffuse textures with ambient lighting (no reflections).

You may have mixed success with the MRTK shader, we've experienced some issues with it and haven't yet pinned down the root cause. We're currently woking out some of the kinks, and intend to have this behaving properly before a final release!

## Included Tools
### Camera Cubemap Creator

>Mixed Reality Toolkit / Lighting Estimation / Camera Cubemap Creator

This is an editor window for putting together cubemaps outside of runtime for debugging and fixed locations. It's a little buggy, but can be used to make some good stuff! It saves a cubemap format .png to the Asset folder.

### Save Cubemap From Probe

>Mixed Reality Toolkit / Lighting Estimation / Save Cubemap from Probe

If you're in the editor during runtime, and want to save the current Reflection Probe, use this menu item! Also saves a cubemap format .png to the Asset folder.

### Save Cubemap on HoloLens

The 'Demo' scene can save a cubemap .png from the HoloLens to its picture folder using the 'save' command word. This scene requires the MRTK to work, and may have some broken prefabs. You can check the `LightPreviewController.SaveMap()` method for an implementation of the HoloLens save functionality if you want to trigger it from a different command.

This is the best way to get a test cubemap, as the HoloLens' ability to lock the camera exposure will result in a better image!