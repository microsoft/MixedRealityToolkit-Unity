// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteAudioSource : RemoteComponent<AudioSource>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedAudioSource.ChangeType changeType = (SynchronizedAudioSource.ChangeType)message.ReadByte();

            if (SynchronizedAudioSource.HasFlag(changeType, SynchronizedAudioSource.ChangeType.Properties))
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
            if (SynchronizedAudioSource.HasFlag(changeType, SynchronizedAudioSource.ChangeType.Volume))
            {
                attachedComponent.volume = message.ReadSingle();
            }

            if (SynchronizedAudioSource.HasFlag(changeType, SynchronizedAudioSource.ChangeType.PlayStarted))
            {
                attachedComponent.Play();
            }
            if (SynchronizedAudioSource.HasFlag(changeType, SynchronizedAudioSource.ChangeType.PlayStopped))
            {
                attachedComponent.Stop();
            }
        }
    }
}