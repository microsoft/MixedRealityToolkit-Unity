## [SpatialSound]()

### [Scripts](Scripts)
Scripts related to the audio features.

**IMPORTANT**: Please make sure to set the MS HRTF Spatializer in your audio settings, in Unity under  
Edit -> Project Settings -> Audio -> Spatializer. You can confirm this setting by adding an AudioSource component to an object and making sure the "Spatialize" checkbox is present.

#### AudioEmitter.cs
A class that encapsulates an AudioSource and supports applying audio influences, for example occlusion.

#### AudioOccluder.cs
Class implementing a simple form of audio occlusion using a low pass filter and volume attenuation.

#### IAudioInfluencer.cs
Interface defining the methods required of an audio influencer.

#### UAudioManager\UAudioManager.cs
1. Allows sound designers to set up audio events with playback behaviors.
2. Plays audio events via singleton API.

**PlayEvent(string eventName)** Plays the event matching eventName on an AudioSource component placed on the GameObject containing the UAudioManager component.

**PlayEvent(string eventName, AudioSource primarySource)** Plays the event matching eventName on the AudioSource primarySource. This should be used if you already have an AudioSource component on which to play the sound, as opposed to the previous function, which will look for one or add it if there is no AudioSource present.

**Global Event Instance Limit** The total number of audio events that can be active at once at any given time.

**Global Instance Behavior** Whether the oldest or newest event should be cancelled to honor the instance limit.

**Name** The name of the audio event to be called in script.

**Positioning** Whether a sound should be played in stereo, 3D or using Spatial Sound.

**Room Size** The room model used for Spatial Sound.

**Min Gain** The lowest attenuation value caused by distance.

**Max Gain** The maximum level boost from the sound being closer than Unity Gain Distance.

**Unity Gain Distance** The distance, in meters, at which the sound is neither boosted nor attenuated.

---
##### [Go back up to the table of contents.](../../../README.md)
---