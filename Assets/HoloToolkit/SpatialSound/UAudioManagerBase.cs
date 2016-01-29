using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
	public class UAudioManagerBase<TEvent> : MonoBehaviour where TEvent : AudioEvent, new()
	{
        [SerializeField]
        protected TEvent[] events = null;
        protected const float InfiniteLoop = -1;
        protected List<ActiveEvent> activeEvents;

#if UNITY_EDITOR
        public TEvent[] EditorEvents { get { return this.events; } set { this.events = value; } }
        public List<ActiveEvent> ProfilerEvents { get { return this.activeEvents; } }
#endif

        protected void Awake()
        {
            this.activeEvents = new List<ActiveEvent>();
        }

        private void Update()
		{
            UpdateEmitterVolumes();
		}

        protected void OnDestroy()
        {
            StopAllEvents();
        }

        /// <summary>
        /// Stops all ActiveEvents
        /// </summary>
        public void StopAllEvents()
        {
            for (int i = this.activeEvents.Count - 1; i >= 0; i--)
            {
                StopEvent(this.activeEvents[i]);
            }
        }

        /// <summary>
        /// Fades out all of the events over fadeTime and stops once completely faded out
        /// </summary>
        /// <param name="fadeTime">The amount of time to fade between current volume and 0</param>
        public void StopAllEvents(float fadeTime)
        {
            for (int i = this.activeEvents.Count - 1; i >= 0; i--)
            {
                StartCoroutine(StopEventWithFadeCoroutine(this.activeEvents[i], fadeTime));
                //CoroutineEx.Run(StopEventWithFadeCoroutine(this.activeEvents[i], fadeTime), this.activeEvents[i].cancelSource.Token);
            }
        }

        /// <summary>
		/// Stops all events on a single emitter
		/// </summary>
		public void StopAllEvents(GameObject emitter)
        {
            for (int i = this.activeEvents.Count - 1; i >= 0; i--)
            {
                if (this.activeEvents[i].PrimarySource.gameObject == emitter)
                {
                    StopEvent(this.activeEvents[i]);
                }
            }
        }

        /// <summary>
        /// Stops all events on one AudioSource
        /// </summary>
        public void StopAllEvents(AudioSource emitter)
        {
            for (int i = this.activeEvents.Count - 1; i >= 0; i--)
            {
                if (this.activeEvents[i].PrimarySource == emitter)
                {
                    StopEvent(this.activeEvents[i]);
                }
            }
        }

        /// <summary>
        /// Linearly interpolates the volume property on all of the AudioSource components in the ActiveEvents
        /// </summary>
        private void UpdateEmitterVolumes()
        {
            //Move through each active event and change the settings for the AudioSource components to smoothly fade volumes
            for (int i = 0; i < this.activeEvents.Count; i++)
            {
                ActiveEvent currentEvent = this.activeEvents[i];
                //If we have a secondary source (for crossfades) adjust the volume based on the current fade time for each active event
                if (currentEvent.SecondarySource != null && currentEvent.SecondarySource.volume != currentEvent.altVolDest)
                {
                    if (Mathf.Abs(currentEvent.altVolDest - currentEvent.SecondarySource.volume) < Time.deltaTime / currentEvent.currentFade)
                    {
                        currentEvent.SecondarySource.volume = currentEvent.altVolDest;
                    }
                    else
                    {
                        currentEvent.SecondarySource.volume += (currentEvent.altVolDest - currentEvent.SecondarySource.volume) * Time.deltaTime / currentEvent.currentFade;
                    }

                }
                //Adjust the volume of the main source based on the current fade time for each active event
                if (currentEvent.PrimarySource != null && currentEvent.PrimarySource.volume != currentEvent.volDest)
                {
                    if (Mathf.Abs(currentEvent.volDest - currentEvent.PrimarySource.volume) < Time.deltaTime / currentEvent.currentFade)
                    {
                        currentEvent.PrimarySource.volume = currentEvent.volDest;
                    }
                    else
                    {
                        currentEvent.PrimarySource.volume += (currentEvent.volDest - currentEvent.PrimarySource.volume) * Time.deltaTime / currentEvent.currentFade;
                    }
                }
                //If there is no time left in the fade, make sure we are set to the destination volume
                if (currentEvent.currentFade > 0)
                {
                    currentEvent.currentFade -= Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Creates a new ActiveEvent
        /// </summary>
        /// <param name="audioEvent">The AudioEvent to be played</param>
        /// <returns>The newly created ActiveEvent</returns>
        protected ActiveEvent CreateNewActiveEvent(TEvent audioEvent)
        {
            ActiveEvent newEvent = new ActiveEvent();
            newEvent.audioEvent = audioEvent;
            return newEvent;
        }

        /// <summary>
        /// Determine which rules to follow for container playback, and begin the appropriate function
        /// </summary>
        /// <param name="activeEvent">The event to play.</param>
        protected void PlayContainer(ActiveEvent activeEvent)
		{
			if (activeEvent.audioEvent.container.sounds.Length == 0)
			{
                Debug.LogErrorFormat(this, "Trying to play container \"{0}\" with no clips.", activeEvent.audioEvent.container);
				return;
			}

			switch (activeEvent.audioEvent.container.containerType)
			{
				case AudioContainerType.Random:
                    StartOneOffEvent(activeEvent);
					break;
				case AudioContainerType.Simultaneous:
                    StartOneOffEvent(activeEvent);
                    break;
				case AudioContainerType.Sequence:
                    StartOneOffEvent(activeEvent);
                    break;
				case AudioContainerType.ContinuousSequence:
					PlayContinuousSequenceContainer(activeEvent.audioEvent.container, activeEvent.PrimarySource, activeEvent);
					break;
				case AudioContainerType.ContinuousRandom:
					PlayContinuousRandomContainer(activeEvent.audioEvent.container, activeEvent.PrimarySource, activeEvent);
					break;
				default:
					break;
			}
		}

        /// <summary>
        /// Begin playing a non-continuous container, loop if applicable
        /// </summary>
        private void StartOneOffEvent(ActiveEvent activeEvent)
        {
            
            if (activeEvent.audioEvent.container.looping)
            {
                StartCoroutine(PlayLoopingOneOffContainerCoroutine(activeEvent));
                //CoroutineEx.Run(PlayLoopingOneOffContainerCoroutine(activeEvent), activeEvent.cancelSource.Token);
                activeEvent.activeTime = InfiniteLoop;
            }
            else
            {
                PlayOneOffContainer(activeEvent);
            }
            StartCoroutine(RecordEventInstanceCoroutine(activeEvent));
            //CoroutineEx.Run(RecordEventInstanceCoroutine(activeEvent), CoroutineEx.StepOption.StepOnStart, activeEvent.cancelSource.Token);
        }

        /// <summary>
        /// Play a non-continuous container
        /// </summary>
        private void PlayOneOffContainer(ActiveEvent activeEvent)
        {
            AudioContainer currentContainer = activeEvent.audioEvent.container;
            //Fading or looping overrides immediate volume settings
            if (activeEvent.audioEvent.fadeInTime == 0 && !activeEvent.audioEvent.container.looping)
            {
                activeEvent.volDest = activeEvent.PrimarySource.volume;
            }

            //Simultaneous sounds
            if (currentContainer.containerType == AudioContainerType.Simultaneous)
            {
                activeEvent.activeTime = PlaySimultaneousClips(currentContainer, activeEvent);
            }
            //Sequence and Random sounds
            else
            {
                activeEvent.activeTime = PlaySingleClip(currentContainer, activeEvent);
            }
        }

        /// <summary>
        /// Play all clips in container simultaneously
        /// </summary>
        private float PlaySimultaneousClips(AudioContainer currentContainer, ActiveEvent activeEvent)
        {
            float tempDelay = 0;
            float finalDelay = 0f;
            if (currentContainer.looping)
            {
                finalDelay = InfiniteLoop;
            }
            for (int i = 0; i < currentContainer.sounds.Length; i++)
            {
                tempDelay = PlayClipAndGetTime(currentContainer.sounds[i], activeEvent.PrimarySource, activeEvent);
                if (finalDelay != InfiniteLoop)
                {
                    if (tempDelay == InfiniteLoop)
                    {
                        finalDelay = InfiniteLoop;
                    }
                    else if (tempDelay > finalDelay)
                    {
                        finalDelay = tempDelay;
                    }
                }
            }
            return finalDelay;
        }

        /// <summary>
        /// Play one sound from a container based on container behavior
        /// </summary>
        private float PlaySingleClip(AudioContainer currentContainer, ActiveEvent activeEvent)
        {
            float tempDelay = 0;
            if (currentContainer.containerType == AudioContainerType.Random)
            {
                currentContainer.currentClip = Random.Range(0, currentContainer.sounds.Length);
            }
            AudioClip currentClip = currentContainer.sounds[currentContainer.currentClip];

            //Trigger sound and save the delay (in seconds) to add to the total amount of time the event will be considered active
            tempDelay = PlayClipAndGetTime(currentClip, activeEvent.PrimarySource, activeEvent);

            //Ready the next clip in the series if sequence container
            if (currentContainer.containerType == AudioContainerType.Sequence)
            {
                currentContainer.currentClip++;
                if (currentContainer.currentClip >= currentContainer.sounds.Length)
                {
                    currentContainer.currentClip = 0;
                }
            }

            //Return active time based on looping or clip time
            if (currentClip.looping || currentContainer.looping)
            {
                return InfiniteLoop;
            }
            else
            {
                return (currentClip.sound.length / activeEvent.PrimarySource.pitch) + activeEvent.audioEvent.instanceBuffer + tempDelay;
            }
        }

        /// <summary>
        /// Repeatedly trigger the one-off container based on the loop time
        /// </summary>
        private IEnumerator PlayLoopingOneOffContainerCoroutine(ActiveEvent activeEvent)
        {
            while (!activeEvent.stopLoop)
            {
                PlayOneOffContainer(activeEvent);
                yield return new WaitForSeconds(activeEvent.audioEvent.container.loopTime);
            }
        }

        /// <summary>
        /// Choose a random sound from a container and play, calling the looping coroutine to constantly choose new audio clips when current clip ends
        /// </summary>
        /// <param name="audioContainer">The audio container</param>
        /// <param name="emitter">The emitter to use</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        private void PlayContinuousRandomContainer(AudioContainer audioContainer, AudioSource emitter, ActiveEvent activeEvent)
		{
			audioContainer.currentClip = Random.Range(0, audioContainer.sounds.Length);
			AudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];
			activeEvent.PrimarySource.volume = 0f;
			activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
			activeEvent.altVolDest = 0f;
			activeEvent.currentFade = audioContainer.crossfadeTime;
			float waitTime = (tempClip.sound.length / emitter.pitch) - activeEvent.audioEvent.container.crossfadeTime;
			//Ignore clip delay since container is continuous
			PlayClipAndGetTime(tempClip, emitter, activeEvent);
			activeEvent.activeTime = InfiniteLoop;
            StartCoroutine(RecordEventInstanceCoroutine(activeEvent));
			//CoroutineEx.Run(RecordEventInstanceCoroutine(activeEvent), CoroutineEx.StepOption.StepOnStart, activeEvent.cancelSource.Token);
			audioContainer.currentClip++;
			if (audioContainer.currentClip >= audioContainer.sounds.Length)
			{
				audioContainer.currentClip = 0;
			}
            StartCoroutine(ContinueRandomContainerCoroutine(audioContainer, activeEvent, waitTime));
			//CoroutineEx.Run(ContinueRandomContainerCoroutine(audioContainer, activeEvent, waitTime), activeEvent.cancelSource.Token);
		}

        /// <summary>
        /// Coroutine for "continuous" random containers that alternates between two sources to crossfade clips for continuous playlist looping
        /// </summary>
        /// <param name="audioContainer">The audio container</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        /// <param name="waitTime">The time in seconds to wait before switching AudioSources for crossfading</param>
        /// <returns></returns>
        private IEnumerator ContinueRandomContainerCoroutine(AudioContainer audioContainer, ActiveEvent activeEvent, float waitTime)
		{
			while (true)
			{
                yield return new WaitForSeconds(waitTime);
				audioContainer.currentClip = Random.Range(0, audioContainer.sounds.Length);
				AudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];
				//Play on first source
				if (activeEvent.playingAlt)
				{
					activeEvent.PrimarySource.volume = 0f;
					activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
					activeEvent.altVolDest = 0f;
					activeEvent.currentFade = audioContainer.crossfadeTime;
					waitTime = (tempClip.sound.length / activeEvent.PrimarySource.pitch) - audioContainer.crossfadeTime;
					PlayClipAndGetTime(tempClip, activeEvent.PrimarySource, activeEvent);
				}
				//Play on alt source
				else
				{
					activeEvent.SecondarySource.volume = 0f;
					activeEvent.altVolDest = activeEvent.audioEvent.volumeCenter;
					activeEvent.volDest = 0f;
					activeEvent.currentFade = audioContainer.crossfadeTime;
					waitTime = (tempClip.sound.length / activeEvent.SecondarySource.pitch) - audioContainer.crossfadeTime;
					PlayClipAndGetTime(tempClip, activeEvent.SecondarySource, activeEvent);
				}
				activeEvent.playingAlt = !activeEvent.playingAlt;
			}
		}

        /// <summary>
        /// Play the current clip in a container, and call the coroutine to constantly choose new audio clips when the current clip ends
        /// </summary>
        /// <param name="audioContainer">The audio container</param>
        /// <param name="emitter">The emitter to use</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        private void PlayContinuousSequenceContainer(AudioContainer audioContainer, AudioSource emitter, ActiveEvent activeEvent)
		{
			AudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];
			activeEvent.PrimarySource.volume = 0f;
			activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
			activeEvent.altVolDest = 0f;
			activeEvent.currentFade = audioContainer.crossfadeTime;
			float waitTime = (tempClip.sound.length / emitter.pitch) - activeEvent.audioEvent.container.crossfadeTime;
			//Ignore clip delay since container is continuous
			PlayClipAndGetTime(tempClip, emitter, activeEvent);
			activeEvent.activeTime = InfiniteLoop;
            StartCoroutine(RecordEventInstanceCoroutine(activeEvent));
			//CoroutineEx.Run(RecordEventInstanceCoroutine(activeEvent), CoroutineEx.StepOption.StepOnStart, activeEvent.cancelSource.Token);
			audioContainer.currentClip++;
			if (audioContainer.currentClip >= audioContainer.sounds.Length)
			{
				audioContainer.currentClip = 0;
			}
            StartCoroutine(ContinueSequenceContainerCoroutine(audioContainer, activeEvent, waitTime));
			//CoroutineEx.Run(ContinueSequenceContainerCoroutine(audioContainer, activeEvent, waitTime), activeEvent.cancelSource.Token);
		}

        /// <summary>
        /// Coroutine for "continuous" sequence containers that alternates between two sources to crossfade clips for continuous playlist looping
        /// </summary>
        /// <param name="audioContainer">The audio container</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        /// <param name="waitTime">The time in seconds to wait before switching AudioSources to crossfading</param>
        /// <returns></returns>
        private IEnumerator ContinueSequenceContainerCoroutine(AudioContainer audioContainer, ActiveEvent activeEvent, float waitTime)
		{
			while (true)
			{
                yield return new WaitForSeconds(waitTime);
				//yield return Yield.WaitForSeconds(waitTime);
				AudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];
                if (tempClip.sound == null)
                {
                    Debug.LogErrorFormat(this, "Sound clip in event \"{0}\" is null!", activeEvent.audioEvent.name);
                    waitTime = 0;
                }
                else
                {
                    //Play on first source
                    if (activeEvent.playingAlt)
                    {
                        activeEvent.PrimarySource.volume = 0f;
                        activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
                        activeEvent.altVolDest = 0f;
                        activeEvent.currentFade = audioContainer.crossfadeTime;
                        waitTime = (tempClip.sound.length / activeEvent.PrimarySource.pitch) - audioContainer.crossfadeTime;
                        PlayClipAndGetTime(tempClip, activeEvent.PrimarySource, activeEvent);
                    }
                    //Play on alt source
                    else
                    {
                        activeEvent.SecondarySource.volume = 0f;
                        activeEvent.altVolDest = activeEvent.audioEvent.volumeCenter;
                        activeEvent.volDest = 0f;
                        activeEvent.currentFade = audioContainer.crossfadeTime;
                        waitTime = (tempClip.sound.length / activeEvent.SecondarySource.pitch) - audioContainer.crossfadeTime;
                        PlayClipAndGetTime(tempClip, activeEvent.SecondarySource, activeEvent);
                    }
                }
                audioContainer.currentClip++;
                if (audioContainer.currentClip >= audioContainer.sounds.Length)
                {
                    audioContainer.currentClip = 0;
                }
                activeEvent.playingAlt = !activeEvent.playingAlt;
            }
		}

        /// <summary>
        /// Play a single clip on an AudioSource; if looping forever, return InfiniteLoop for the event time
        /// </summary>
        /// <param name="audioClip">The audio clip to play</param>
        /// <param name="emitter">The emitter to use</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        /// <returns></returns>
        private float PlayClipAndGetTime(AudioClip audioClip, AudioSource emitter, ActiveEvent activeEvent)
		{
			if (audioClip.delayCenter == 0)
			{
				PlayClipInstant(audioClip, emitter);
				if (audioClip.looping)
				{
					return InfiniteLoop;
				}
				return audioClip.sound.length;
			}
			else
			{
				float rndDelay = Random.Range(audioClip.delayCenter - audioClip.delayRandomization, audioClip.delayCenter + audioClip.delayRandomization);
                StartCoroutine(PlayClipDelayedCoroutine(audioClip, emitter, rndDelay, activeEvent));
				//CoroutineEx.Run(PlayClipDelayedCoroutine(audioClip, emitter, rndDelay, activeEvent), activeEvent.cancelSource.Token);
				if (audioClip.looping)
				{
					return InfiniteLoop;
				}
				return rndDelay;
			}
		}

        /// <summary>
        /// Play a sound with no delay
        /// </summary>
        /// <param name="audioClip">The audio clip to play</param>
        /// <param name="emitter">The emitter to use</param>
        private void PlayClipInstant(AudioClip audioClip, AudioSource emitter)
		{
			if (audioClip.looping)
			{
				PlayClipLooping(audioClip.sound, emitter);
			}
			else
			{
				emitter.PlayOneShot(audioClip.sound);
			}
		}

        /// <summary>
        /// Coroutine for playing a clip after a delay (in seconds)
        /// </summary>
        /// <param name="audioClip">The audio clip to play</param>
        /// <param name="emitter">The emitter to use</param>
        /// <param name="delay">The amount of time in seconds to wait before playing audio clip</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        /// <returns></returns>
        private IEnumerator PlayClipDelayedCoroutine(AudioClip audioClip, AudioSource emitter, float delay, ActiveEvent activeEvent)
		{
            yield return new WaitForSeconds(delay);
			//yield return Yield.WaitForSeconds(delay);
			if (this.activeEvents.Contains(activeEvent))
			{
				if (audioClip.looping)
				{
					PlayClipLooping(audioClip.sound, emitter);
				}
				else
				{
					emitter.PlayOneShot(audioClip.sound);
				}
			}
		}

        /// <summary>
        /// Assign an AudioClip to an emitter for seamless looping and play
        /// </summary>
        /// <param name="audioClip">The audio clip to play</param>
        /// <param name="emitter">The emitter to use</param>
        private void PlayClipLooping(UnityEngine.AudioClip audioClip, AudioSource emitter)
		{
			emitter.clip = audioClip;
			emitter.loop = true;
			emitter.Play();
		}

        /// <summary>
        /// Stop audio sources in an event, and clean up instance references
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
		protected void StopEvent(ActiveEvent activeEvent)
		{
			//TODO: Stop the event's coroutine - ie. StopCoroutine(activeEvent);
            if (activeEvent.PrimarySource != null)
            {
                activeEvent.PrimarySource.Stop();
            }
			if (activeEvent.SecondarySource != null)
			{
				activeEvent.SecondarySource.Stop();
			}
			RemoveEventInstance(activeEvent);
		}

        /// <summary>
        /// Coroutine for fading out an AudioSource, and stopping the event once fade is complete
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        /// <param name="fadeTime">The amount of time in seconds to completely fade out the sound</param>
        /// <returns></returns>
        protected IEnumerator StopEventWithFadeCoroutine(ActiveEvent activeEvent, float fadeTime)
		{
            if (activeEvent.isStoppable)
            {
                activeEvent.isStoppable = false;
                activeEvent.volDest = 0f;
                activeEvent.altVolDest = 0f;
                activeEvent.currentFade = fadeTime;
                yield return new WaitForSeconds(fadeTime);
                //TODO: Stop the event's coroutine - ie. StopCoroutine(activeEvent);
                activeEvent.PrimarySource.Stop();
                if (activeEvent.SecondarySource != null)
                {
                    activeEvent.SecondarySource.Stop();
                }
                RemoveEventInstance(activeEvent);
            }
		}

        /// <summary>
        /// Set the volume, spatialization, etc. on an AudioSource to match the settings on the event to play
        /// </summary>
        /// <param name="emitter"></param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        protected void SetSourceProperties(AudioSource emitter, ActiveEvent activeEvent)
		{
            //SetSpatializerProperties(emitter);
            AudioEvent audioEvent = activeEvent.audioEvent;
			if (audioEvent.spatialization == PositioningType.TwoD)
			{
                emitter.spatialize = false;
				emitter.spatialBlend = 0f;
			}
			else if (audioEvent.spatialization == PositioningType.ThreeD)
			{
                emitter.spatialize = false;
				emitter.spatialBlend = 1f;
			}
            else
            {
                emitter.spatialize = true;
                emitter.spatialBlend = 1f;
            }
            if (audioEvent.bus != null)
            {
                emitter.outputAudioMixerGroup = audioEvent.bus;
            }
			float rndPitch = 1f;
			if (audioEvent.pitchRandomization != 0)
			{
				rndPitch = Random.Range(audioEvent.pitchCenter - audioEvent.pitchRandomization, audioEvent.pitchCenter + audioEvent.pitchRandomization);
			}
			else
			{
				rndPitch = audioEvent.pitchCenter;
			}
			emitter.pitch = rndPitch;

			float rndVol = 1f;
			if (audioEvent.fadeInTime > 0)
			{
				emitter.volume = 0f;
				activeEvent.currentFade = audioEvent.fadeInTime;
				if (audioEvent.volumeRandomization != 0)
				{
					rndVol = Random.Range(audioEvent.volumeCenter - audioEvent.volumeRandomization, audioEvent.volumeCenter + audioEvent.volumeRandomization);
				}
				else
				{
					rndVol = audioEvent.volumeCenter;
				}
				activeEvent.volDest = rndVol;
			}
			else
			{
				if (audioEvent.volumeRandomization != 0)
				{
					rndVol = Random.Range(audioEvent.volumeCenter - audioEvent.volumeRandomization, audioEvent.volumeCenter + audioEvent.volumeRandomization);
				}
				else
				{
					rndVol = audioEvent.volumeCenter;
				}
				emitter.volume = rndVol;
			}
		}

        /// <summary>
        /// TESTING FUNCTION: Sets the properties on the MS HRTF plugin
        /// </summary>
        /// <param name="emitter">The AudioSource that is using the MS HRTF Spatializer</param>
        private void SetSpatializerProperties(AudioSource emitter)
        {
            //Spatializer plugin: element 0 is between a value of 0 and 1
            //WHAT ARE THOSE NUMBERS EVEN?
            float spatialTest = 0f;
            int spatialNumber = 0;
            if (emitter.GetSpatializerFloat(spatialNumber, out spatialTest))
            {
                Debug.Log("Spatializer: " + spatialTest.ToString());
            }
            else
            {
                Debug.Log("Couldn't read spatializer float " + spatialNumber.ToString());
            }
            if (emitter.SetSpatializerFloat(spatialNumber, 2))
            {
                Debug.Log("Set float!");
            }
            else
            {
                Debug.LogFormat("Couldn't set float {0}!", spatialNumber);
            }
        }

        /// <summary>
        /// Keep an event in the "activeEvents" list for the amount of time we think it will be playing, plus the instance buffer
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        /// <returns></returns>
        private IEnumerator RecordEventInstanceCoroutine(ActiveEvent activeEvent)
		{
			//Unity has no callback for an audioclip ending, so we have to calculate it ahead of time
			//Changing the pitch during playback will alter actual playback time
			this.activeEvents.Add(activeEvent);
			//Only return active time if sound is not looping/continuous
			if (activeEvent.activeTime > 0)
			{
                yield return new WaitForSeconds(activeEvent.activeTime);
				//yield return Yield.WaitForSeconds(activeEvent.activeTime);
			}
			//Otherwise, continue at next frame
			else
			{
                yield return new WaitForEndOfFrame();
				//yield return Yield.WaitForNextFrame;
			}
			if (activeEvent.activeTime != InfiniteLoop)
			{
                RemoveEventInstance(activeEvent);
			}
		}

        /// <summary>
        /// Remove event from the currently active events
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing</param>
        private void RemoveEventInstance(ActiveEvent activeEvent)
		{
			this.activeEvents.Remove(activeEvent);
            activeEvent.stopLoop = true;
		}

        /// <summary>
        /// Return the number of instances matching the name eventName for instance limiting check
        /// </summary>
        /// <param name="eventName">The event to check</param>
        /// <returns>The number of instances currently active for the given event</returns>
        protected int GetInstances(string eventName)
		{
			int tempInstances = 0;
			for (int i = 0; i < this.activeEvents.Count; i++)
			{
				if (this.activeEvents[i].audioEvent.name == eventName)
				{
					tempInstances++;
				}
			}
			return tempInstances;
		}
	}
}