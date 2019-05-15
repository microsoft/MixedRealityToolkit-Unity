// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedAudioSource : SynchronizedComponent<AudioSourceService, SynchronizedAudioSource.ChangeType>
    {
        [Flags]
        public enum ChangeType
        {
            None = 0x0,
            PlayStarted = 0x1,
            PlayStopped = 0x2,
            Volume = 0x4,
            Properties = 0x8
        }

        private AudioSource audioSource;
        private bool previousIsPlaying;
        private float previousVolume;
        private AudioSourceProperties previousProperties;

        public Guid AudioClipAssetId
        {
            get { return AudioSourceService.Instance.GetAudioClipId(audioSource.clip); }
        }

        public Guid AudioMixerGroupAssetId
        {
            get { return AudioSourceService.Instance.GetAudioMixerGroupId(audioSource.outputAudioMixerGroup); }
        }

        protected override void Awake()
        {
            base.Awake();

            audioSource = GetComponent<AudioSource>();
        }

        protected override bool HasChanges(ChangeType changeFlags)
        {
            return changeFlags != ChangeType.None;
        }

        protected override ChangeType CalculateDeltaChanges()
        {
            ChangeType changeType = ChangeType.None;
            bool isPlaying = audioSource.isPlaying;
            if (isPlaying != this.previousIsPlaying)
            {
                previousIsPlaying = isPlaying;
                if (isPlaying)
                {
                    changeType = ChangeType.PlayStarted;
                }
                else
                {
                    changeType = ChangeType.PlayStopped;
                }
            }

            if (isPlaying)
            {
                AudioSourceProperties currentProperties = new AudioSourceProperties(this.audioSource);
                if (previousProperties != currentProperties)
                {
                    changeType |= ChangeType.Properties;
                    previousProperties = currentProperties;
                }

                float newVolume = audioSource.volume;
                if (this.previousVolume != newVolume)
                {
                    changeType |= ChangeType.Volume;
                    this.previousVolume = newVolume;
                }
            }

            return changeType;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            if (previousIsPlaying)
            {
                SendDeltaChanges(endpoints, ChangeType.PlayStarted | ChangeType.Properties | ChangeType.Volume);
            }
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, ChangeType changeFlags)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                SynchronizedComponentService.WriteHeader(message, this);

                message.Write((byte)changeFlags);

                if (HasFlag(changeFlags, ChangeType.Properties))
                {
                    message.Write(AudioClipAssetId);
                    message.Write(AudioMixerGroupAssetId);
                    message.Write(previousProperties.bypassEffects);
                    message.Write(previousProperties.bypassListenerEffects);
                    message.Write(previousProperties.bypassReverbZones);
                    message.Write(previousProperties.dopplerLevel);
                    message.Write(previousProperties.enabled);
                    message.Write(previousProperties.ignoreListenerPause);
                    message.Write(previousProperties.ignoreListenerVolume);
                    message.Write(previousProperties.loop);
                    message.Write(previousProperties.maxDistance);
                    message.Write(previousProperties.minDistance);
                    message.Write(previousProperties.mute);
                    message.Write(previousProperties.panStereo);
                    message.Write(previousProperties.pitch);
                    message.Write(previousProperties.priority);
                    message.Write(previousProperties.reverbZoneMix);
                    message.Write((byte)previousProperties.rolloffMode);
                    message.Write(previousProperties.spatialBlend);
                    message.Write(previousProperties.spatialize);
                    message.Write(previousProperties.spatializePostEffects);
                    message.Write(previousProperties.spread);
                    message.Write((byte)previousProperties.velocityUpdateMode);
                }
                if (HasFlag(changeFlags, ChangeType.Volume))
                {
                    message.Write(previousVolume);
                }

                message.Flush();
                SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        public static bool HasFlag(ChangeType changeType, ChangeType flag)
        {
            return (changeType & flag) == flag;
        }
    }

    public struct AudioSourceProperties
    {
        public AudioSourceProperties(AudioSource audioSource)
        {
            enabled = audioSource.enabled;
            loop = audioSource.loop;
            ignoreListenerVolume = audioSource.ignoreListenerVolume;
            ignoreListenerPause = audioSource.ignoreListenerPause;
            velocityUpdateMode = audioSource.velocityUpdateMode;
            panStereo = audioSource.panStereo;
            spatialBlend = audioSource.spatialBlend;
            spatialize = audioSource.spatialize;
            outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            spatializePostEffects = audioSource.spatializePostEffects;
            bypassEffects = audioSource.bypassEffects;
            bypassListenerEffects = audioSource.bypassListenerEffects;
            bypassReverbZones = audioSource.bypassReverbZones;
            dopplerLevel = audioSource.dopplerLevel;
            spread = audioSource.spread;
            priority = audioSource.priority;
            mute = audioSource.mute;
            minDistance = audioSource.minDistance;
            maxDistance = audioSource.maxDistance;
            rolloffMode = audioSource.rolloffMode;
            reverbZoneMix = audioSource.reverbZoneMix;
            clip = audioSource.clip;
            pitch = audioSource.pitch;
        }

        public bool enabled { get; set; }
        public bool loop { get; set; }
        public bool ignoreListenerVolume { get; set; }
        public bool ignoreListenerPause { get; set; }
        public AudioVelocityUpdateMode velocityUpdateMode { get; set; }
        public float panStereo { get; set; }
        public float spatialBlend { get; set; }
        public bool spatialize { get; set; }
        public AudioMixerGroup outputAudioMixerGroup { get; set; }
        public bool spatializePostEffects { get; set; }
        public bool bypassEffects { get; set; }
        public bool bypassListenerEffects { get; set; }
        public bool bypassReverbZones { get; set; }
        public float dopplerLevel { get; set; }
        public float spread { get; set; }
        public int priority { get; set; }
        public bool mute { get; set; }
        public float minDistance { get; set; }
        public float maxDistance { get; set; }
        public AudioRolloffMode rolloffMode { get; set; }
        public float reverbZoneMix { get; set; }
        public AudioClip clip { get; set; }
        public float pitch { get; set; }

        public static bool operator ==(AudioSourceProperties first, AudioSourceProperties second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(AudioSourceProperties first, AudioSourceProperties second)
        {
            return !first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AudioSourceProperties))
            {
                return false;
            }

            AudioSourceProperties other = (AudioSourceProperties)obj;
            return
                other.bypassEffects == bypassEffects &&
                other.bypassListenerEffects == bypassListenerEffects &&
                other.bypassReverbZones == bypassReverbZones &&
                other.clip == clip &&
                other.dopplerLevel == dopplerLevel &&
                other.enabled == enabled &&
                other.ignoreListenerPause == ignoreListenerPause &&
                other.ignoreListenerVolume == ignoreListenerVolume &&
                other.loop == loop &&
                other.maxDistance == maxDistance &&
                other.minDistance == minDistance &&
                other.mute == mute &&
                other.outputAudioMixerGroup == outputAudioMixerGroup &&
                other.panStereo == panStereo &&
                other.pitch == pitch &&
                other.priority == priority &&
                other.reverbZoneMix == reverbZoneMix &&
                other.rolloffMode == rolloffMode &&
                other.spatialBlend == spatialBlend &&
                other.spatialize == spatialize &&
                other.spatializePostEffects == spatializePostEffects &&
                other.spread == spread &&
                other.velocityUpdateMode == velocityUpdateMode;
        }

        public override int GetHashCode()
        {
            return clip == null ? 0 : clip.GetHashCode();
        }
    }
}