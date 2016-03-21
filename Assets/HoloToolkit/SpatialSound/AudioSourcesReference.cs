// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The AudioSourcesReference class encapsulates a cache of references to audio source components on a given 
    /// local audio emitter game object. Used primarily by UAudioManager, it improves performance by bypassing 
    /// having to requery for list of attached components on each use.
    /// </summary>
    public class AudioSourcesReference : MonoBehaviour
    {
        private List<AudioSource> audioSources;
        public List<AudioSource> AudioSources
        {
            get
            {
                return audioSources;
            }
        }

        public AudioSource AddNewAudioSource()
        {
            var source = this.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.dopplerLevel = 0f;
            source.enabled = false;
            audioSources.Add(source);
            return source;
        }

        private void Awake()
        {
            audioSources = new List<AudioSource>();
            foreach (AudioSource audioSource in GetComponents<AudioSource>())
            {
                audioSources.Add(audioSource);
            }
        }

        private void OnDestroy()
        {
            // AudioSourcesReference created all these components and nothing else should use them.
            foreach (AudioSource audioSource in audioSources)
            {
                Object.Destroy(audioSource);
            }

            audioSources = null;
        }
    }
}