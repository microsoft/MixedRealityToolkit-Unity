Spatial Sound                        {#mainpage}
============

## Description

The UAudioManager is a simple way to set up complex sound playback events. Sound designers often rely on things like randomization to avoid sounds becoming repetitive and fatiguing. This might mean that a different audio clip is played each time, that the volume or pitch varies randomly, or all three. Audio events may even contain multiple sounds playing at once, or a sequence of sounds, which is not possible to do by simply attaching an audio clip to an AudioSource. And since the same sound can often come from multiple objects, setting up each one manually can be tedious. The UAudioManager allows a sound designer to set up the properties of a sound event, and have a programmer play that event on any object.

Also, Spatial Sound in Unity uses the SetSpatializerFloat API to set the spatial properties of the AudioSource component. While these properties are documented, it would require making sure you have the correct order and values of those properties for each project. The UAudioManager incorporates those values into the editor so they are immediately accessible to the sound designer without even having to look up documentation.

Finally, using UAudioManager events, sounds can be easily categorized into the bus system of Unity's audio mixer. For instance, a footstep sound can be set to play on a character bus so that the footsteps of all characters can be controlled at once if the mix needs to highlight or dampen their effect.