// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    [RequireComponent(typeof(AudioSource))]    
    public class AudioEmitter : MonoBehaviour 
    {
        [Tooltip("Time, in seconds, between audio influence updates.  0 indicates to update every frame.")]
        [Range(0.0f, 1.0f)]
        public float UpdateInterval = 0.25f;

        [Tooltip("Maximum distance, in m, to look when attempting to find the user and any influencers.")]
        [Range(1.0f, 50.0f)]
        public float MaxDistance = 20.0f;

        [Tooltip("Maximum number of objects that will be considered when looking for influencers.")]
        [Range(5, 25)]
        [SerializeField]
        private int MaxObjects = 10;

        // Time of last audio processing update.
        private DateTime lastUpdate = DateTime.MinValue;

        private AudioSource audioSource;
        private float initialAudioSourceVolume;

        private RaycastHit[] hits;

	    private void Awake() 
        {           
            audioSource = gameObject.GetComponent<AudioSource>();
            initialAudioSourceVolume = audioSource.volume;

            // Pre-allocate the array that will be used to collect RaycastHit structures.
            hits = new RaycastHit[MaxObjects];
        }
	
        List<IAudioInfluencer> previousInfluencers = new List<IAudioInfluencer>();
	    private void Update() 
        {
            // Audio influences do not need to be updated every frame.
            DateTime now = DateTime.Now;

            if ((UpdateInterval * 1000.0f) <= (now - lastUpdate).Milliseconds)
            {
                audioSource.volume = initialAudioSourceVolume;

                List<IAudioInfluencer> influencers = GetInfluencers();
                foreach (IAudioInfluencer influencer in influencers)
                {
                    influencer.ApplyEffect(gameObject, audioSource);
                }

                List<IAudioInfluencer> influencersToRemove = new List<IAudioInfluencer>();
                foreach (IAudioInfluencer prev in previousInfluencers)
                {
                    if (!influencers.Contains(prev))
                    {
                        influencersToRemove.Add(prev);
                    }
                }
                RemoveInfluencers(influencersToRemove);

                previousInfluencers = influencers;
                lastUpdate = now;
            }
	    }

        private void RemoveInfluencers(List<IAudioInfluencer> influencers)
        {
            foreach (IAudioInfluencer influencer in influencers)
            {
                influencer.RemoveEffect(gameObject, audioSource);
            }
        }

        private List<IAudioInfluencer> GetInfluencers()
        {
            List<IAudioInfluencer> influencers = new List<IAudioInfluencer>();

            // For influencers that take effect only when between the emitter and the user, perform a raycast
            // from the user in our directiond.
            Vector3 direction = (gameObject.transform.position - Camera.main.transform.position).normalized;
            float distance = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position);

            int count = Physics.RaycastNonAlloc(Camera.main.transform.position,
                                                direction,
                                                hits,
                                                distance,
                                                Physics.DefaultRaycastLayers,
                                                QueryTriggerInteraction.Ignore);
            
            for (int i = 0; i < count; i++)
            {
                IAudioInfluencer ai = hits[i].collider.gameObject.GetComponentInParent<IAudioInfluencer>();
                if (ai != null)
                {
                    influencers.Add(ai);
                }
            }

            return influencers;
        }
    }
}