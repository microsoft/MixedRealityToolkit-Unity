// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class AudioSourceObserver : ComponentObserver<AudioSource>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            AudioSourceBroadcaster.ChangeType changeType = (AudioSourceBroadcaster.ChangeType)message.ReadByte();

            if (AudioSourceBroadcaster.HasFlag(changeType, AudioSourceBroadcaster.ChangeType.Properties))
            {
                Guid audioClipId = message.ReadGuid();
                Guid audioMixerGroupId = message.ReadGuid();

                attachedComponent.clip = AudioSourceService.Instance.GetAudioClip(audioClipId);
                attachedComponent.outputAudioMixerGroup = AudioSourceService.Instance.GetAudioMixerGroup(audioMixerGroupId);
                attachedComponent.bypassEffects = message.ReadBoolean();
                attachedComponent.bypassListenerEffects = message.ReadBoolean();
                attachedComponent.bypassReverbZones = message.ReadBoolean();
                attachedComponent.dopplerLevel = message.ReadSingle();
                attachedComponent.enabled = message.ReadBoolean();
                attachedComponent.ignoreListenerPause = message.ReadBoolean();
                attachedComponent.ignoreListenerVolume = message.ReadBoolean();
                attachedComponent.loop = message.ReadBoolean();
                attachedComponent.maxDistance = message.ReadSingle();
                attachedComponent.minDistance = message.ReadSingle();
                attachedComponent.mute = message.ReadBoolean();
                attachedComponent.panStereo = message.ReadSingle();
                attachedComponent.pitch = message.ReadSingle();
                attachedComponent.priority = message.ReadInt32();
                attachedComponent.reverbZoneMix = message.ReadSingle();
                attachedComponent.rolloffMode = (AudioRolloffMode)message.ReadByte();
                attachedComponent.spatialBlend = message.ReadSingle();
                attachedComponent.spatialize = message.ReadBoolean();
                attachedComponent.spatializePostEffects = message.ReadBoolean();
                attachedComponent.spread = message.ReadSingle();
                attachedComponent.velocityUpdateMode = (AudioVelocityUpdateMode)message.ReadByte();
            }
            if (AudioSourceBroadcaster.HasFlag(changeType, AudioSourceBroadcaster.ChangeType.Volume))
            {
                attachedComponent.volume = message.ReadSingle();
            }

            if (AudioSourceBroadcaster.HasFlag(changeType, AudioSourceBroadcaster.ChangeType.PlayStarted))
            {
                attachedComponent.Play();
            }
            if (AudioSourceBroadcaster.HasFlag(changeType, AudioSourceBroadcaster.ChangeType.PlayStopped))
            {
                attachedComponent.Stop();
            }
        }
    }
}