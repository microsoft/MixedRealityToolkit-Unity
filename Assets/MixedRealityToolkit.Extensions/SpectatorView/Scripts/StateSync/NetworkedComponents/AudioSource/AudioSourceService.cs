// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class AudioSourceService : SynchronizedComponentService<AudioSourceService, RemoteAudioSource>, IAssetCache
    {
        public static readonly ShortID ID = new ShortID("AUD");

        public override ShortID GetID() { return ID; }

        private const int DSPBufferSize = 1024;
        private const AudioSpeakerMode SpeakerMode = AudioSpeakerMode.Stereo;

        private AudioClipAssetCache audioClipAssets;
        private AudioMixerGroupAssetCache audioMixerGroupAssets;

        protected override void Awake()
        {
            base.Awake();

            audioClipAssets = AudioClipAssetCache.LoadAssetCache<AudioClipAssetCache>();
            audioMixerGroupAssets = AudioMixerGroupAssetCache.LoadAssetCache<AudioMixerGroupAssetCache>();
        }

        private void Start()
        {
            SynchronizedSceneManager.Instance.RegisterService(this, new SynchronizedComponentDefinition<SynchronizedAudioSource>(typeof(AudioSource)));
        }

        public Guid GetAudioClipId(AudioClip clip)
        {
            return audioClipAssets?.GetAssetId(clip) ?? Guid.Empty;
        }

        public AudioClip GetAudioClip(Guid assetId)
        {
            return audioClipAssets?.GetAsset(assetId);
        }

        public Guid GetAudioMixerGroupId(AudioMixerGroup group)
        {
            return audioMixerGroupAssets?.GetAssetId(group) ?? Guid.Empty;
        }

        public AudioMixerGroup GetAudioMixerGroup(Guid assetId)
        {
            return audioMixerGroupAssets?.GetAsset(assetId);
        }

        public void UpdateAssetCache()
        {
            AudioClipAssetCache.GetOrCreateAssetCache<AudioClipAssetCache>().UpdateAssetCache();
            AudioMixerGroupAssetCache.GetOrCreateAssetCache<AudioMixerGroupAssetCache>().UpdateAssetCache();
        }

        public void ClearAssetCache()
        {
            AudioClipAssetCache.GetOrCreateAssetCache<AudioClipAssetCache>().ClearAssetCache();
            AudioMixerGroupAssetCache.GetOrCreateAssetCache<AudioMixerGroupAssetCache>().ClearAssetCache();
        }
    }
}