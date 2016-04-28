// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The different rules for how audio should be played back.
    /// </summary>
    public enum AudioContainerType
    {
        Random,
        Sequence,
        Simultaneous,
        ContinuousSequence,
        ContinuousRandom
    }

    /// <summary>
    /// Defines the behavior for when the instance limit is reached for a particular event.
    /// </summary>
    public enum AudioEventInstanceBehavior
    {
        KillOldest,
        KillNewest
    }

    /// <summary>
    /// The different types of spatial positioning.
    /// </summary>
    public enum SpatialPositioningType
    {
        TwoD,           // Stereo
        ThreeD,         // 3D audio
        SpatialSound,   // Microsoft Spatial Sound
    }

    /// <summary>
    /// The AudioEvent class is the main component of UAudioManager and contains settings and a container for playing audio clips.
    /// </summary>
    [System.Serializable]
    public class AudioEvent : IComparable, IComparable<AudioEvent>
    {
        [Tooltip("The name of this AudioEvent.")]
        public string name = "_NewAudioEvent";

        [Tooltip("How this sound is to be positioned.")]
        public SpatialPositioningType spatialization = SpatialPositioningType.TwoD;

        [Tooltip("The size of the Microsoft Spatial Sound room.  Only used when positioning is set to SpatialSound.")]
        public SpatialSoundRoomSizes roomSize = SpatialSoundSettings.DefaultSpatialSoundRoom;

        [Tooltip("The minimum gain, in decibels.  Only used when positioning is set to SpatialSound.")]
        [Range(SpatialSoundSettings.MinimumGainDecibels, SpatialSoundSettings.MaximumGainDecibels)]
        public float minGain = SpatialSoundSettings.DefaultMinGain;

        [Tooltip("The maximum gain, in decibels.  Only used when positioning is set to SpatialSound.")]
        [Range(SpatialSoundSettings.MinimumGainDecibels, SpatialSoundSettings.MaximumGainDecibels)]
        public float maxGain = SpatialSoundSettings.DefaultMaxGain;

        [Tooltip("The distance, in meters at which the gain is 0 decibels.  Only used when positioning is set to SpatialSound.")]
        [Range(SpatialSoundSettings.MinimumUnityGainDistanceMeters, SpatialSoundSettings.MaximumUnityGainDistanceMeters)]
        public float unityGainDistance = SpatialSoundSettings.DefaultUnityGainDistance;

        [Tooltip("The AudioMixerGroup to use when playing.")]
        public UnityEngine.Audio.AudioMixerGroup bus = null;

        [Tooltip("The default or center pitch around which randomization can be done.")]
        [Range(-3.0f, 3.0f)]
        public float pitchCenter = 1.0f;

        /// <summary>
        /// The amount in either direction from Pitch Center that the pitch can randomly vary upon playing the event.
        /// </summary>
        /// <remarks>The supported range is 0.0f - 2.0f.</remarks>
        [HideInInspector]
        public float pitchRandomization = 0.0f;

        [Tooltip("The default or center volume level around which randomization can be done.")]
        [Range(0.0f, 1.0f)]
        public float volumeCenter = 1.0f;

        /// <summary>
        /// The amount in either direction from Volume Center that the volume can randomly vary upon playing the event.
        /// </summary>
        /// <remarks>The supported range is 0.0f - 0.5f.</remarks>
        [HideInInspector]
        public float volumeRandomization = 0.0f;

        [Tooltip("The default or center panning. Only used when positioning is set to 2D.")]
        [Range(-1.0f, 1.0f)]
        public float panCenter = 0;

        /// <summary>
        /// The amount in either direction from Pan Center that panning can randomly vary upon playing the event.
        /// </summary>
        /// <remarks>The supported range is 0.0f - 0.5f.</remarks>
        [HideInInspector]
        public float panRandomization = 0.0f;

        [Tooltip("Time, in seconds, for the audio to fade from 0 to the selected volume.  Does not apply to continuous containers in which the Crossfade TGime property is used.")]
        public float fadeInTime = 0.0f;

        [Tooltip("The maximum number of instances that should be allowed at a time for this event. Any new instances will be suppressed.")]
        public int instanceLimit = 0;

        [Tooltip("The amount of time in seconds that an event will remain active past when the sound ends. Useful for limiting the instances of an event beyond the clip play time.")]
        public float instanceTimeBuffer = 0.0f;

        [Tooltip("The behavior when the instance limit is reached.")]
        public AudioEventInstanceBehavior instanceBehavior = AudioEventInstanceBehavior.KillOldest;

        /// <summary>
        /// Contains the sounds associated with this AudioEvent.
        /// </summary>
        public AudioContainer container = new AudioContainer();

        /// <summary>
        /// Is this AudioEvent's container a continuous container?
        /// </summary>
        /// <returns>True if this AudioEvent's container is one of the continuous types (random or sequential), otherwise false.</returns>
        public bool IsContinuous()
        {
            return container.containerType == AudioContainerType.ContinuousRandom ||
                   container.containerType == AudioContainerType.ContinuousSequence;
        }

        /// <summary>
        /// Compares this AudioEvent with another object.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>An integer that indicates whether this AudioEvent precedes (-1), follows (1),
        /// or appears in the same position (0) in the sort order as the AudioEvent being compared.</returns>
        /// <remarks>If the specified object is not an AudioEvent, the return value is 1.</remarks>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            AudioEvent tempEvent = obj as AudioEvent;
            if (tempEvent != null)
            {
                return CompareTo(tempEvent);
            }
            else
            {
                throw new ArgumentException("Object is not an AudioEvent");
            }
        }

        /// <summary>
        /// Compares this AudioEvent with another AudioEvent.
        /// </summary>
        /// <param name="other">The AudioEvent to compare against.</param>
        /// <returns>An integer that indicates whether this AudioEvent precedes (-1), follows (1),
        /// or appears in the same position (0) in the sort order as the AudioEvent being compared.</returns>
        public int CompareTo(AudioEvent other)
        {
            if (other == null) return 1;
            return string.Compare(name, other.name);
        }
    }
}