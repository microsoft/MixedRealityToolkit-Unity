using UnityEngine;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    public class UAudioManager : UAudioManagerBase<AudioEvent>
    {
        public static UAudioManager Instance;
        /// <summary>
        /// Dictionary for quick lookup of events from the event name
        /// </summary>
        private Dictionary<string, AudioEvent> eventsDictionary;

        protected new void Awake()
        {
            base.Awake();
            if (Instance != null)
            {
                Debug.LogWarningFormat(this, "UAudioManager {0} already exists.", Instance.gameObject.name);
            }
            Instance = this;
            CreateEventsDictionary();
        }

        /// <summary>
        /// Remove the instance reference if object is destroyed
        /// </summary>
        protected new void OnDestroy()
        {
            Instance = null;
            base.OnDestroy();
        }

        /// <summary>
        /// Play the event matching eventName on the specified AudioSource component(s)
        /// </summary>
        /// <param name="eventName">The name of the event to match in the events Dictionary</param>
        /// <param name="primarySource">The AudioSource component to use as the primarySource for the event</param>
        /// <param name="secondarySource">The AudioSource component to use as the secondarySource for the event</param>
        public void PlayEvent(string eventName, AudioSource primarySource, AudioSource secondarySource = null)
        {
            if (primarySource == null)
            {
                Debug.LogErrorFormat(this, "Trying to play event \"{0}\" on null AudioSource. Cancelling.", eventName);
                return;
            }
            AudioEvent currentEvent = this.eventsDictionary[eventName];
            if (currentEvent == null)
            {
                Debug.LogErrorFormat(this, "Could not find event \"{0}\"", eventName);
                return;
            }

            if (currentEvent.instanceLimit == 0 || GetInstances(eventName) < currentEvent.instanceLimit)
            {
                if (AudioEvent.IsContinuous(currentEvent) && secondarySource == null)
                {
                    secondarySource = GetUnusedAudioSource(primarySource.gameObject, primarySource);
                }
                PlayEvent(currentEvent, primarySource, secondarySource);
            }
            else
            {
                Debug.LogFormat(this, "Instance limit reached, not playing event \"{0}\"", eventName);
            }
        }

        /// <summary>
        /// Play the event matching eventName on the specified emitter
        /// </summary>
        /// <param name="eventName">The name of the event to match in the events Dictionary</param>
        /// <param name="emitter">The GameObject on which to find or add AudioSource components to emit the sound</param>
        public void PlayEvent(string eventName, GameObject emitter)
        {
            AudioEvent currentEvent = this.eventsDictionary[eventName];
            if (currentEvent == null)
            {
                Debug.LogErrorFormat(this, "Could not find event \"{0}\"", eventName);
            }
            else if (currentEvent.instanceLimit == 0 || GetInstances(eventName) < currentEvent.instanceLimit)
            {
                PlayEvent(currentEvent, emitter);
            }
            else
            {
                Debug.LogFormat(this, "Instance limit reached, not playing event \"{0}\"", eventName);
            }
        }

        /// <summary>
        /// Plays the event matching eventName on the UAudioManager GameObject
        /// </summary>
        public void PlayEvent(string eventName)
        {
            PlayEvent(eventName, this.gameObject);
        }

        /// <summary>
		/// Stops all events with the name matching eventName
		/// </summary>
		public void StopAllEvents(string eventName)
        {
            for (int i = this.activeEvents.Count - 1; i >= 0; i--)
            {
                if (this.activeEvents[i].audioEvent.name == eventName)
                {
                    StopEvent(this.activeEvents[i]);
                }
            }
        }

        /// <summary>
        /// Stops the event matching eventName on the UAudioManager GameObject
        /// </summary>
        public void StopEvent(string eventName)
        {
            StopEvent(eventName, this.gameObject);
        }

        /// <summary>
        /// Fades out and stops the event matching eventName on the UAudioManager GameObject
        /// </summary>
        public void StopEvent(string eventName, float fadeTime)
        {
            StopEvent(eventName, this.gameObject, fadeTime);
        }

        /// <summary>
        /// Stops an event matching eventName on the specified emitter
        /// </summary>
        public void StopEvent(string eventName, GameObject emitter)
        {
            for (int i = this.activeEvents.Count - 1; i >= 0; i--)
            {
                if (this.activeEvents[i].audioEvent.name == eventName && this.activeEvents[i].PrimarySource.gameObject == emitter)
                {
                    StopEvent(this.activeEvents[i]);
                }
            }
        }

        /// <summary>
        /// Fades out and stops an event matching eventName on the specified emitter
        /// </summary>
        public void StopEvent(string eventName, GameObject emitter, float fadeTime)
        {
            for (int i = this.activeEvents.Count - 1; i >= 0; i--)
            {
                ActiveEvent activeEvent = this.activeEvents[i];
                if (activeEvent.audioEvent.name == eventName && activeEvent.PrimarySource.gameObject == emitter)
                {
                    StartCoroutine(StopEventWithFadeCoroutine(activeEvent, fadeTime));
                    //CoroutineEx.Run(StopEventWithFadeCoroutine(activeEvent, fadeTime), activeEvent.cancelSource.Token);
                }
            }
        }

        /// <summary>
        /// Sets the volume value for the Primary Source on Active Events matching the name eventName
        /// </summary>
        /// <param name="eventName">The name of the Active Event</param>
        /// <param name="newVolume">The value to set the volume, between 0 and 1</param>
        /// <param name="fadeTime">Optional: the amount of time in seconds over which to gradually make the volume change</param>
        public void SetEventVolume(string eventName, float newVolume, float fadeTime = 0)
        {
            if (newVolume < 0 || newVolume > 1)
            {
                Debug.LogErrorFormat(this, "Invalid volume set for event \"{0}\"", eventName);
                return;
            }
            for (int i = 0; i < this.activeEvents.Count; i++)
            {
                ActiveEvent currentEvent = this.activeEvents[i];
                if (currentEvent.audioEvent.name == eventName)
                {
                    currentEvent.volDest = newVolume;
                    currentEvent.currentFade = fadeTime;
                }
            }
        }

        /// <summary>
        /// Sets the volume value for the Primary Source on Active Events matching the name eventName on the GameObject emitter
        /// </summary>
        /// <param name="eventName">The name of the Active Event</param>
        /// <param name="emitter">The GameObject on which the event is playing</param>
        /// <param name="newVolume">The value to set the volume, between 0 and 1</param>
        /// <param name="fadeTime">Optional: the amount of time in seconds over which to gradually make the volume change</param>
        public void SetEventVolume(string eventName, GameObject emitter, float newVolume, float fadeTime = 0)
        {
            if (newVolume < 0 || newVolume > 1)
            {
                Debug.LogErrorFormat(this, "Invalid volume set for event \"{0}\"", eventName);
                return;
            }
            for (int i = 0; i < this.activeEvents.Count; i++)
            {
                ActiveEvent currentEvent = this.activeEvents[i];
                if (currentEvent.audioEvent.name == eventName && currentEvent.PrimarySource.gameObject == emitter)
                {
                    currentEvent.volDest = newVolume;
                    currentEvent.currentFade = fadeTime;
                }
            }
        }

        /// <summary>
        /// Sets the pitch value for the Primary Source on Active Events matching the name eventName
        /// </summary>
        /// <param name="eventName">The name of the Active Event</param>
        /// <param name="newPitch">The value to set the pitch, between 0 (exclusive) and 3 (inclusive)</param>
        public void SetEventPitch(string eventName, float newPitch)
        {
            if (newPitch <= 0 || newPitch > 3)
            {
                Debug.LogErrorFormat(this, "Invalid pitch set for event \"{0}\"", eventName);
                return;
            }
            for (int i = 0; i < this.activeEvents.Count; i++)
            {
                ActiveEvent currentEvent = this.activeEvents[i];
                if (currentEvent.audioEvent.name == eventName)
                {
                    currentEvent.PrimarySource.pitch = newPitch;
                }
            }
        }

        /// <summary>
        /// Change the frequency with which the container for the event matching eventName loops
        /// </summary>
        /// <param name="eventName">The event to modify</param>
        /// <param name="newLoopTime">The new loop time in seconds</param>
        public void SetLoopingContainerFrequency(string eventName, float newLoopTime)
        {
            AudioEvent currentEvent = this.eventsDictionary[eventName];
            if (currentEvent == null)
            {
                Debug.LogErrorFormat(this, "Could not find event \"{0}\"", eventName);
                return;
            }
            if (newLoopTime <= 0)
            {
                Debug.LogErrorFormat(this, "Invalid loop time set for event \"{0}\"", eventName);
                return;
            }
            currentEvent.container.loopTime = newLoopTime;
        }

        /// <summary>
        /// Create the Dictionary for quick lookup of AudioEvents in the manager
        /// </summary>
        private void CreateEventsDictionary()
        {
            this.eventsDictionary = new Dictionary<string, AudioEvent>(this.events.Length);
            for (int i = 0; i < this.events.Length; i++)
            {
                AudioEvent tempEvent = this.events[i];
                this.eventsDictionary.Add(tempEvent.name, tempEvent);
            }
        }

        /// <summary>
        /// Play an AudioEvent on the GameObject that contains this UAudioManager component
        /// </summary>
        /// <param name="audioEvent">The AudioEvent to play</param>
        private void PlayEvent(AudioEvent audioEvent)
        {
            PlayEvent(audioEvent, this.gameObject);
        }

        /// <summary>
        /// Play an AudioEvent on a particular GameObject
        /// </summary>
        /// <param name="audioEvent">The AudioEvent to play</param>
        /// <param name="emitter">The GameObject to use as the emitter via an AudioSource component</param>
        private void PlayEvent(AudioEvent audioEvent, GameObject emitter)
        {
            AudioSource newPrimarySource = GetUnusedAudioSource(emitter);
            if (AudioEvent.IsContinuous(audioEvent))
            {
                AudioSource newSecondarySource = GetUnusedAudioSource(emitter, newPrimarySource);
                PlayEvent(audioEvent, newPrimarySource, newSecondarySource);
            }
            else
            {
                PlayEvent(audioEvent, newPrimarySource);
            }
        }

        /// <summary>
        /// Play an AudioEvent on a particular AudioSource
        /// </summary>
        /// <param name="audioEvent">The AudioEvent to play</param>
        /// <param name="emitter">The AudioSource component to use as an emitter for the AudioEvent</param>
        private void PlayEvent(AudioEvent audioEvent, AudioSource primarySource, AudioSource secondarySource = null)
        {
            ActiveEvent tempEvent = CreateNewActiveEvent(audioEvent);
            tempEvent.PrimarySource = primarySource;
            SetSourceProperties(tempEvent.PrimarySource, tempEvent);
            if (secondarySource != null)
            {
                tempEvent.SecondarySource = secondarySource;
                SetSourceProperties(tempEvent.SecondarySource, tempEvent);
            }
            PlayContainer(tempEvent);
        }

        /// <summary>
        /// Gets an unused AudioSource component on the emitter GameObject, or create a new one if one does not exist
        /// </summary>
        /// <param name="emitter">The GameObject on which to find or add the AudioSource component</param>
        /// <param name="currentEvent">The pre-existing event</param>
        /// <returns>The AudioSource component to be used in an ActiveEvent</returns>
        private AudioSource GetUnusedAudioSource(GameObject emitter, AudioSource primarySource = null)
        {
            //Get or create valid AudioSource
            AudioSource[] sources = emitter.GetComponents<AudioSource>();
            for (int s = 0; s < sources.Length; s++)
            {
                if (!sources[s].isPlaying)
                {
                    if (primarySource == null)
                    {
                        return sources[s];
                    }
                    else if (sources[s] != primarySource)
                    {
                        return sources[s];
                    }
                }
            }
            AudioSource source = emitter.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }
    }
}