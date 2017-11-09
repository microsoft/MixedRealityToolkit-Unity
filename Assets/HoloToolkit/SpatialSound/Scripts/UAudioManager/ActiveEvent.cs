// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Currently active AudioEvents along with their AudioSource components for instance limiting events
    /// </summary>
    public class ActiveEvent : IDisposable
    {
        private AudioSource primarySource = null;
        public AudioSource PrimarySource
        {
            get
            {
                return primarySource;
            }
            private set
            {
                primarySource = value;
                if (primarySource != null)
                {
                    primarySource.enabled = true;
                }
            }
        }

        private AudioSource secondarySource = null;
        public AudioSource SecondarySource
        {
            get
            {
                return secondarySource;
            }
            private set
            {
                secondarySource = value;
                if (secondarySource != null)
                {
                    secondarySource.enabled = true;
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                return
                    (primarySource != null && primarySource.isPlaying) ||
                    (secondarySource != null && secondarySource.isPlaying);
            }
        }

        public GameObject AudioEmitter
        {
            get;
            private set;
        }

        public string MessageOnAudioEnd
        {
            get;
            private set;
        }

        public AudioEvent AudioEvent = null;
        public bool IsStoppable = true;
        public float VolDest = 1;
        public float AltVolDest = 1;
        public float CurrentFade = 0;
        public bool PlayingAlt = false;
        public bool IsActiveTimeComplete = false;
        public float ActiveTime = 0;
        public bool CancelEvent = false;

        public ActiveEvent(AudioEvent audioEvent, GameObject emitter, AudioSource primarySource, AudioSource secondarySource, string messageOnAudioEnd = null)
        {
            this.AudioEvent = audioEvent;
            AudioEmitter = emitter;
            PrimarySource = primarySource;
            SecondarySource = secondarySource;
            MessageOnAudioEnd = messageOnAudioEnd;
            SetSourceProperties();
        }

        public static AnimationCurve SpatialRolloff;

        /// <summary>
        /// Set the volume, spatialization, etc., on our AudioSources to match the settings on the event to play.
        /// </summary>
        private void SetSourceProperties()
        {
            Action<Action<AudioSource>> forEachSource = (action) =>
            {
                action(PrimarySource);
                if (SecondarySource != null)
                {
                    action(SecondarySource);
                }
            };

            AudioEvent audioEvent = this.AudioEvent;
            switch (audioEvent.Spatialization)
            {
                case SpatialPositioningType.TwoD:
                    forEachSource((source) =>
                    {
                        source.spatialBlend = 0f;
                        source.spatialize = false;
                    });
                    break;
                case SpatialPositioningType.ThreeD:
                    forEachSource((source) =>
                    {
                        source.spatialBlend = 1f;
                        source.spatialize = false;
                    });
                    break;
                case SpatialPositioningType.SpatialSound:
                    forEachSource((source) =>
                    {
                        source.spatialBlend = 1f;
                        source.spatialize = true;
                    });
                    break;
                default:
                    Debug.LogErrorFormat("Unexpected spatialization type: {0}", audioEvent.Spatialization.ToString());
                    break;
            }

            if (audioEvent.Spatialization == SpatialPositioningType.SpatialSound)
            {
                forEachSource((source) =>
                {
                    SpatialSoundSettings.SetRoomSize(source, audioEvent.RoomSize);
                    source.rolloffMode = AudioRolloffMode.Custom;
                    source.maxDistance = audioEvent.MaxDistanceAttenuation3D;
                    source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioEvent.AttenuationCurve);
                });
            }
            else
            {
                forEachSource((source) =>
                {
                    if (audioEvent.Spatialization == SpatialPositioningType.ThreeD)
                    {
                        source.rolloffMode = AudioRolloffMode.Custom;
                        source.maxDistance = audioEvent.MaxDistanceAttenuation3D;
                        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioEvent.AttenuationCurve);
                        source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, audioEvent.SpatialCurve);
                        source.SetCustomCurve(AudioSourceCurveType.Spread, audioEvent.SpreadCurve);
                        source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, audioEvent.ReverbCurve);
                    }
                    else
                    {
                        source.rolloffMode = AudioRolloffMode.Logarithmic;
                    }
                });
            }

            if (audioEvent.AudioBus != null)
            {
                forEachSource((source) => source.outputAudioMixerGroup = audioEvent.AudioBus);
            }

            float pitch = 1f;

            if (audioEvent.PitchRandomization != 0)
            {
                pitch = UnityEngine.Random.Range(audioEvent.PitchCenter - audioEvent.PitchRandomization, audioEvent.PitchCenter + audioEvent.PitchRandomization);
            }
            else
            {
                pitch = audioEvent.PitchCenter;
            }
            forEachSource((source) => source.pitch = pitch);

            float vol = 1f;
            if (audioEvent.FadeInTime > 0)
            {
                forEachSource((source) => source.volume = 0f);
                this.CurrentFade = audioEvent.FadeInTime;
                if (audioEvent.VolumeRandomization != 0)
                {
                    vol = UnityEngine.Random.Range(audioEvent.VolumeCenter - audioEvent.VolumeRandomization, audioEvent.VolumeCenter + audioEvent.VolumeRandomization);
                }
                else
                {
                    vol = audioEvent.VolumeCenter;
                }
                this.VolDest = vol;
            }
            else
            {
                if (audioEvent.VolumeRandomization != 0)
                {
                    vol = UnityEngine.Random.Range(audioEvent.VolumeCenter - audioEvent.VolumeRandomization, audioEvent.VolumeCenter + audioEvent.VolumeRandomization);
                }
                else
                {
                    vol = audioEvent.VolumeCenter;
                }
                forEachSource((source) => source.volume = vol);
            }

            float pan = audioEvent.PanCenter;
            if (audioEvent.PanRandomization != 0)
            {
                pan = UnityEngine.Random.Range(audioEvent.PanCenter - audioEvent.PanRandomization, audioEvent.PanCenter + audioEvent.PanRandomization);
            }
            forEachSource((source) => source.panStereo = pan);
        }

        /// <summary>
        /// Sets the pitch value for the primary source.
        /// </summary>
        /// <param name="newPitch">The value to set the pitch, between 0 (exclusive) and 3 (inclusive).</param>
        public void SetPitch(float newPitch)
        {
            if (newPitch <= 0 || newPitch > 3)
            {
                Debug.LogErrorFormat("Invalid pitch {0} set for event", newPitch);
                return;
            }

            this.PrimarySource.pitch = newPitch;
        }

        public void Dispose()
        {
            if (this.primarySource != null)
            {
                this.primarySource.enabled = false;
                this.primarySource = null;
            }

            if (this.secondarySource != null)
            {
                this.secondarySource.enabled = false;
                this.secondarySource = null;
            }
        }

        /// <summary>
        /// Creates a flat animation curve to negate Unity's distance attenuation when using Spatial Sound
        /// </summary>
        public static void CreateFlatSpatialRolloffCurve()
        {
            if (SpatialRolloff != null)
            {
                return;
            }
            SpatialRolloff = new AnimationCurve();
            SpatialRolloff.AddKey(0, 1);
            SpatialRolloff.AddKey(1, 1);
        }
    }
}