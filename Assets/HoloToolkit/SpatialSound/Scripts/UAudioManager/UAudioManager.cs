// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The UAudioManager class is a singleton that provides organization and control of an application's AudioEvents.
    /// Designers and coders can share the names of the AudioEvents to enable rapid iteration on the application's
    /// sound similar to how XAML is used for user interfaces.
    /// </summary>
    public partial class UAudioManager : UAudioManagerBase<AudioEvent>
    {
        [Tooltip("The maximum number of AudioEvents that can be played at once. Zero (0) indicates there is no limit.")]
        [SerializeField]
        private int globalEventInstanceLimit = 0;

        [Tooltip("The desired behavior when the instance limit is reached.")]
        [SerializeField]
        private AudioEventInstanceBehavior globalInstanceBehavior = AudioEventInstanceBehavior.KillOldest;

        /// <summary>
        /// Optional transformation applied to the audio event emitter passed to calls to play event.
        /// This allows events to be redirected to a different emitter.
        /// </summary>
        /// <remarks>This class is a singleton, the last transform set will be applied to all audio
        /// emitters when their state changes (from stopped to playing, volume changes, etc).</remarks>
        public Func<GameObject, GameObject> AudioEmitterTransform { get; set; }

        /// <summary>
        /// Dictionary for quick lookup of events by name.
        /// </summary>
        private Dictionary<string, AudioEvent> eventsDictionary;

        private static UAudioManager instance;
        public static UAudioManager Instance
        {
            get { return instance ?? (instance = FindObjectOfType<UAudioManager>()); }
        }

        protected new void Awake()
        {
            base.Awake();

            CreateEventsDictionary();
        }

        /// <summary>
        /// Plays an AudioEvent.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent.</param>
        /// <remarks>The AudioEvent is attached to the same GameObject as this script.</remarks>
        public void PlayEvent(string eventName)
        {
            PlayEvent(eventName, gameObject);
        }

        /// <summary>
        /// Plays an AudioEvent.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent.</param>
        /// <param name="emitter">The GameObject on which the AudioEvent is to be played.</param>
        /// <param name="messageOnAudioEnd">The Message to Send to the GameObject when the sound has finished playing.</param>
        public void PlayEvent(string eventName, GameObject emitter, string messageOnAudioEnd = null)
        {
            PlayEvent(eventName, emitter, null, null, messageOnAudioEnd);
        }

        /// <summary>
        /// Plays an AudioEvent.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent.</param>
        /// <param name="primarySource">The AudioSource component to use as the primary source for the event.</param>
        /// <param name="secondarySource">The AudioSource component to use as the secondary source for the event.</param>
        public void PlayEvent(string eventName, AudioSource primarySource, AudioSource secondarySource = null)
        {
            PlayEvent(eventName, primarySource.gameObject, primarySource, secondarySource);
        }

        /// <summary>
        /// Plays an AudioEvent.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent.</param>
        /// <param name="emitter">The GameObject on which the AudioEvent is to be played.</param>
        /// <param name="primarySource">The AudioSource component to use as the primary source for the event.</param>
        /// <param name="secondarySource">The AudioSource component to use as the secondary source for the event.</param>
        /// <param name="messageOnAudioEnd">The Message to Send to the GameObject when the sound has finished playing.</param>
        private void PlayEvent(string eventName, GameObject emitter, AudioSource primarySource, AudioSource secondarySource, string messageOnAudioEnd = null)
        {
            if (!CanPlayNewEvent())
            {
                return;
            }

            emitter = ApplyAudioEmitterTransform(emitter);

            if (emitter == null)
            {
                //if emitter is null, use the uAudioManager GameObject(2dSound)
                emitter = gameObject;
            }

            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning("Audio Event string is null or empty!");
                return;
            }

            AudioEvent currentEvent;

            if (!eventsDictionary.TryGetValue(eventName, out currentEvent))
            {
                Debug.LogFormat("Could not find event \"{0}\"", eventName);
                return;
            }

            // If the instance limit has been reached...
            if (currentEvent.InstanceLimit != 0 && GetInstances(eventName) >= currentEvent.InstanceLimit)
            {
                if (currentEvent.AudioEventInstanceBehavior == AudioEventInstanceBehavior.KillNewest)
                {
                    // Do not play the event.
                    Debug.LogFormat(this, "Instance limit reached, not playing event \"{0}\"", eventName);
                    return;
                }
                else
                {
                    // Top the oldest instance of this event.
                    KillOldestInstance(eventName);
                }
            }

            if (primarySource == null)
            {
                primarySource = GetUnusedAudioSource(emitter);
            }

            if (currentEvent.IsContinuous() && secondarySource == null)
            {
                secondarySource = GetUnusedAudioSource(emitter);
            }

            PlayEvent(currentEvent, emitter, primarySource, secondarySource, messageOnAudioEnd);
        }

        /// <summary>
        /// Plays an AudioEvent.
        /// </summary>
        /// <param name="audioEvent">The AudioEvent to play.</param>
        /// <param name="emitter">The GameObject on which the AudioEvent is to be played.</param>
        /// <param name="primarySource">The AudioSource component to use as the primary source for the event.</param>
        /// <param name="secondarySource">The AudioSource component to use as the secondary source for the event.</param>
        /// <param name="messageOnAudioEnd">The Message to Send to the GameObject when the sound has finished playing.</param>
        private void PlayEvent(AudioEvent audioEvent,
                            GameObject emitter,
                            AudioSource primarySource,
                            AudioSource secondarySource,
                            string messageOnAudioEnd = null)
        {
            ActiveEvent tempEvent = new ActiveEvent(audioEvent, emitter, primarySource, secondarySource, messageOnAudioEnd);

            // The base class owns this event once we pass it to PlayContainer, and may dispose it if it cannot be played.
            PlayContainer(tempEvent);
        }

        /// <summary>
        /// Stop event by gameObject.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="gameObjectToStop"></param>
        /// <param name="fadeOutTime"></param>
        public void StopEventsOnGameObject(string eventName, GameObject gameObjectToStop, float fadeOutTime = 0f)
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                ActiveEvent activeEvent = activeEvents[i];

                if (activeEvent.AudioEmitter == gameObjectToStop)
                {
                    StopEvent(activeEvent.audioEvent.Name, gameObjectToStop, fadeOutTime);
                }
            }
        }


        /// <summary>
        /// Stops all events by name.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent.</param>
        /// <param name="fadeOutTime">The amount of time in seconds to completely fade out the sound.</param>
        public void StopAllEvents(string eventName, GameObject emitter = null, float fadeOutTime = 0f)
        {

            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                ActiveEvent activeEvent = activeEvents[i];

                if (activeEvent.audioEvent.Name == eventName)
                {
                    if (fadeOutTime > 0)
                    {
                        StartCoroutine(StopEventWithFadeCoroutine(activeEvent, fadeOutTime));
                    }
                    else
                    {
                        StartCoroutine(StopEventWithFadeCoroutine(activeEvent, activeEvent.audioEvent.FadeOutTime));
                    }
                }
            }
        }

        /// <summary>
        /// Stops all.
        /// </summary>
        /// <param name="fadeOutTime">The amount of time in seconds to completely fade out the sound.</param>
        public void StopAll(GameObject emitter = null, float fadeOutTime = 0f)
        {
            foreach (ActiveEvent activeEvent in activeEvents)
            {
                if (fadeOutTime > 0)
                {
                    StartCoroutine(StopEventWithFadeCoroutine(activeEvent, fadeOutTime));
                }
                else
                {
                    StartCoroutine(StopEventWithFadeCoroutine(activeEvent, activeEvent.audioEvent.FadeOutTime));
                }
            }
        }



        /// <summary>
        /// Stops an AudioEvent.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent.</param>
        /// <param name="emitter">The GameObject on which the AudioEvent will stopped.</param>
        /// <param name="fadeTime">The amount of time in seconds to completely fade out the sound.</param>
        public void StopEvent(string eventName, GameObject emitter = null, float fadeOutTime = 0f)
        {
            emitter = ApplyAudioEmitterTransform(emitter);
            if (emitter == null)
            {
                //if emitter is null, use the uaudiomanager gameobject(2dsound)
                emitter = gameObject;
            }

            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                ActiveEvent activeEvent = activeEvents[i];

                if (activeEvent.audioEvent.Name == eventName && activeEvent.AudioEmitter == emitter)
                {
                    //if there's no fade specified, use the fade stored in the event
                    if (fadeOutTime > 0f)
                    {
                        StartCoroutine(StopEventWithFadeCoroutine(activeEvent, fadeOutTime));
                    }
                    else
                    {
                        StartCoroutine(StopEventWithFadeCoroutine(activeEvent, activeEvents[i].audioEvent.FadeOutTime));

                    }
                }
            }
        }

        /// <summary>
        /// Sets the pitch value on active AudioEvents.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvents.</param>
        /// <param name="newPitch">The value to set the pitch, between 0 (exclusive) and 3 (inclusive).</param>
        public void SetPitch(string eventName, float newPitch)
        {
            if (newPitch <= 0 || newPitch > 3)
            {
                Debug.LogErrorFormat(this, "Invalid pitch {0} set for event \"{1}\"", newPitch, eventName);
                return;
            }

            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                ActiveEvent activeEvent = activeEvents[i];
                if (activeEvent.audioEvent.Name == eventName)
                {
                    activeEvent.SetPitch(newPitch);
                }
            }
        }

        /// <summary>
        /// Sets an AudioEvent's container loop frequency
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent.</param>
        /// <param name="newLoopTime">The new loop time in seconds.</param>
        public void SetLoopingContainerFrequency(string eventName, float newLoopTime)
        {
            AudioEvent currentEvent;

            if (!eventsDictionary.TryGetValue(eventName, out currentEvent))
            {
                Debug.LogErrorFormat(this, "Could not find event \"{0}\"", eventName);
                return;
            }

            if (newLoopTime <= 0)
            {
                Debug.LogErrorFormat(this, "Invalid loop time set for event \"{0}\"", eventName);
                return;
            }

            currentEvent.Container.loopTime = newLoopTime;
        }

        /// <summary>
        /// Sets the volume for active AudioEvents.
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvents.</param>
        /// <param name="emitter">The GameObject associated, as the audio emitter, for the AudioEvents.</param>
        /// <param name="volume">The new volume.</param>
        public void ModulateVolume(string eventName, GameObject emitter, float volume)
        {
            emitter = ApplyAudioEmitterTransform(emitter);

            if (emitter == null)
            {
                return;
            }

            for (int i = 0; i < activeEvents.Count; i++)
            {
                ActiveEvent activeEvent = activeEvents[i];

                if (activeEvents[i].audioEvent.Name == eventName && activeEvents[i].AudioEmitter == emitter)
                {
                    activeEvent.volDest = volume;
                    activeEvent.altVolDest = volume;
                    activeEvent.currentFade = 0;
                }
            }
        }

        /// <summary>
        /// Get an available AudioSource.
        /// </summary>
        /// <param name="emitter">The audio emitter on which the AudioSource is desired.</param>
        /// <param name="currentEvent">The current audio event.</param>
        /// <returns></returns>
        private AudioSource GetUnusedAudioSource(GameObject emitter, ActiveEvent currentEvent = null)
        {
            // Get or create valid AudioSource.
            AudioSourcesReference sourcesReference = emitter.GetComponent<AudioSourcesReference>();
            if (sourcesReference != null)
            {
                List<AudioSource> sources = sourcesReference.AudioSources;
                for (int s = 0; s < sources.Count; s++)
                {
                    if (!sources[s].isPlaying && !sources[s].enabled)
                    {
                        if (currentEvent == null)
                        {
                            return sources[s];
                        }
                        else if (sources[s] != currentEvent.PrimarySource)
                        {
                            return sources[s];
                        }
                    }
                }
            }
            else
            {
                sourcesReference = emitter.AddComponent<AudioSourcesReference>();
            }

            return sourcesReference.AddNewAudioSource();
        }

        /// <summary>
        /// Checks to see if a new AudioEvent can be played.
        /// </summary>
        /// <returns>True if a new AudioEvent can be played, otherwise false.</returns>
        /// <remarks>If the global instance behavior is set to AudioEventInstanceBehavior.KillOldest,
        /// the oldest event will be stopped to allow a new event to be played.</remarks>
        private bool CanPlayNewEvent()
        {
            if (globalEventInstanceLimit == 0 || activeEvents.Count < globalEventInstanceLimit)
            {
                return true;
            }
            else
            {
                if (globalInstanceBehavior == AudioEventInstanceBehavior.KillOldest)
                {
                    StopEvent(activeEvents[0]);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Stops the first (oldest) instance of an event with the matching name
        /// </summary>
        /// <param name="eventName">The name associated with the AudioEvent to stop.</param>
        private void KillOldestInstance(string eventName)
        {
            for (int i = 0; i < activeEvents.Count; i++)
            {
                ActiveEvent tempEvent = activeEvents[i];

                if (tempEvent.audioEvent.Name == eventName)
                {
                    StopEvent(tempEvent);
                    return;
                }
            }
        }

        /// <summary>
        /// Applies the registered transform to an audio emitter.
        /// </summary>
        /// <param name="emitter"></param>
        /// <returns></returns>
        /// <remarks>If there is no registered transform, the GameObject specified in the
        /// emitter parameter will be returned.</remarks>
        private GameObject ApplyAudioEmitterTransform(GameObject emitter)
        {
            if (AudioEmitterTransform != null)
            {
                emitter = AudioEmitterTransform(emitter);
            }

            return emitter;
        }

        /// <summary>
        /// Create the Dictionary for quick lookup of AudioEvents.
        /// </summary>
        private void CreateEventsDictionary()
        {
            eventsDictionary = new Dictionary<string, AudioEvent>(events.Length);

            for (int i = 0; i < events.Length; i++)
            {
                AudioEvent tempEvent = events[i];
                eventsDictionary.Add(tempEvent.Name, tempEvent);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Sort Events")]
        private void AlphabetizeEventList()
        {
            Array.Sort<AudioEvent>(events);
        }
#endif
    }
}