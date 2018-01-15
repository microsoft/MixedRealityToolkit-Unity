UAudioManager                        {#uaudiomanager}
============

## Description

This script controls most of what you need to do with Unity audio. You can play sounds on any GameObject, and necessary AudioSource components will be added if they are not already preset. For performance purposes, it is recommended to add at least one AudioSource component to an object you intend to emit sound.

## Developers

The API consists of Playing and Stopping events, with a few overloads. They are called through the public static "Instance" member. Each method is documented in the code itself. The documentation can be consulted using this generated Doxygen documentation.

## Unity Integration
The UAudioManager is simply a list of audio events and their behaviors. There are two global properties for the manager:

![](/Images/AudioManagerSection1.png)

- **Global Event Instance Limit**: Limits the total number of events that can be played at once.
- **Global Instance Behavior**: Determines what happens when instance limit is reached. Can either deny new events, or stop the oldest event to make space for new events.

You can add as many events as you'd like using the "Add" button. All events are accessible in a dropdown menu. Each Event contains several properties:

![](/Images/AudioManagerSection2.png)

- **Name:** The name that will be called in code. Each event must have a unique name, otherwise the first event with the matching name will always be played
- **Positioning:** Sounds can be 2D or 3D. SpatialSound will be an option when Unity Native Spatial Sound comes online
- **Room Size:** The HRTF room model to use *Only applies to Spatial Sound*
- **Min Gain:** The total distance attenuation possible *Only applies to Spatial Sound*
- **Max Gain:**	The maximum gain resulting from a sound getting closer than Unity Gain Distance *Only applies to Spatial Sound*
- **Unity Gain Distance:** The distance at which there is no attenuation or gain - the sound plays at the volume authored in the audio file *Only applies to Spatial Sound*
- **Bus:** The Audio Mixer Group through which to route the sound.
- **Fade In Time:**	The amount of time in seconds that the sound will take to fade in from 0 to the selected Volume. *Ignored in continuous containers, where the Crossfade Time property is used*
- **Pitch Center:**	The default or center pitch around which randomization can be done
- **Pitch Randomization:** The amount in either direction from Pitch Center that the pitch can randomly vary upon playing the event
- **Volume Center:** The default or center volume level around which randomization can be done
- **Volume Randomization:** The amount in either direction from Volume Center that the volume can randomly vary upon playing the event
- **Pan Center:** The default or center panning for 2D sounds
- **Pan Randomization:** The amount in either direction from Pan Center that the panning can randomly vary upon playing the event
- **Instance Limit:** The maximum number of instances that should be allowed at a time for any event. Any new instances will be suppressed.
- **Instance Buffer:** The amount of time in seconds that an event will remain active past when the sound ends. Useful for limiting the instances of an event beyond the clip play time.


The main element in an event is the Container.

![](/Images/AudioManagerContainer.png)

There are several container types to choose from:

- **Random:** Randomly selects a sound from the list of sounds inside the container.
- **Sequence:**	Plays the first sound, and steps through each sound every time the event is called.
- S**imultaneous:**	Plays all sounds inside the container at once.
- **Continuous Sequence:** Continuously cycles through all sounds in order, crossfading between them using the Crossfade Time property.
- **Continuous Random:** Continuously cycles through all sounds, randomly selecting a sound each cycle, crossfading between them using the Crossfade Time property.

![](/Images/AudioManagerContainerContinuous.png)

![](/Images/AudioManagerContainerSequence.png)

And one property depending on the container type:

- **Crossfade Time:** The amount of time in seconds that two sounds will blend together. *Only applies to continuous containers*
- **Looping:** Whether the container should repeatedly be played until explicitly stopped. *Only applies to non-continuous containers*

Sounds can be added to a container using the "Add" button. Each sound in a container has a few properties:

![](/Images/AudioManagerSounds.png)

- **Sound:** The AudioClip from the Assets folder.
- **Delay Center:**	The default or center time in seconds that the clip will wait before playing.
- **Delay Randomization:** The amount in either direction from Delay Center that the delay can randomly vary upon playing the sound.
- **Looping:** Whether the clip should seamlessly loop *only one looping sound can be used in an event.


