// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// UAudioManagerBase provides the base functionality for UAudioManager classes.
    /// </summary>
    /// <typeparam name="TEvent">The type of AudioEvent being managed.</typeparam>
    /// <remarks>The TEvent type specified must derive from AudioEvent.</remarks>
    public partial class UAudioManagerBase<TEvent> : MonoBehaviour where TEvent : AudioEvent, new()
    {
        [SerializeField]
        protected TEvent[] events = null;

        protected const float InfiniteLoop = -1;
        protected List<ActiveEvent> activeEvents;

#if UNITY_EDITOR
        public TEvent[] EditorEvents { get { return events; } set { events = value; } }
        public List<ActiveEvent> ProfilerEvents { get { return activeEvents; } }
#endif

        protected void Awake()
        {
            activeEvents = new List<ActiveEvent>();
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
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                StopEvent(activeEvents[i]);
            }
        }

        /// <summary>
        /// Fades out all of the events over fadeTime and stops once completely faded out.
        /// </summary>
        /// <param name="fadeTime">The amount of time, in seconds, to fade between current volume and 0.</param>
        public void StopAllEvents(float fadeTime)
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                StartCoroutine(StopEventWithFadeCoroutine(activeEvents[i], fadeTime));
            }
        }

        /// <summary>
        /// Stops all events on a single emitter.
        /// </summary>
        public void StopAllEvents(GameObject emitter)
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                if (activeEvents[i].AudioEmitter == emitter)
                {
                    StopEvent(activeEvents[i]);
                }
            }
        }

        /// <summary>
        /// Stops all events on one AudioSource.
        /// </summary>
        public void StopAllEvents(AudioSource emitter)
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                if (activeEvents[i].PrimarySource == emitter)
                {
                    StopEvent(activeEvents[i]);
                }
            }
        }

        /// <summary>
        /// Linearly interpolates the volume property on all of the AudioSource components in the ActiveEvents.
        /// </summary>
        private void UpdateEmitterVolumes()
        {
            // Move through each active event and change the settings for the AudioSource components to smoothly fade volumes.
            for (int i = 0; i < activeEvents.Count; i++)
            {
                ActiveEvent currentEvent = this.activeEvents[i];

                // If we have a secondary source (for crossfades) adjust the volume based on the current fade time for each active event.
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

                // Adjust the volume of the main source based on the current fade time for each active event.
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

                // If there is no time left in the fade, make sure we are set to the destination volume.
                if (currentEvent.currentFade > 0)
                {
                    currentEvent.currentFade -= Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Determine which rules to follow for container playback, and begin the appropriate function.
        /// </summary>
        /// <param name="activeEvent">The event to play.</param>
        protected void PlayContainer(ActiveEvent activeEvent)
        {
            if (activeEvent.audioEvent.container.sounds.Length == 0)
            {
                Debug.LogErrorFormat(this, "Trying to play container \"{0}\" with no clips.", activeEvent.audioEvent.container);

                // Clean up the ActiveEvent before we discard it, so it will release its AudioSource(s).
                activeEvent.Dispose();
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
                    Debug.LogErrorFormat(this, "Trying to play container \"{0}\" with an unknown AudioContainerType \"{1}\".", activeEvent.audioEvent.container, activeEvent.audioEvent.container.containerType);

                    // Clean up the ActiveEvent before we discard it, so it will release its AudioSource(s).
                    activeEvent.Dispose();
                    break;
            }
        }

        /// <summary>
        /// Begin playing a non-continuous container, loop if applicable.
        /// </summary>
        private void StartOneOffEvent(ActiveEvent activeEvent)
        {
            if (activeEvent.audioEvent.container.looping)
            {
                StartCoroutine(PlayLoopingOneOffContainerCoroutine(activeEvent));
                activeEvent.activeTime = InfiniteLoop;
            }
            else
            {
                PlayOneOffContainer(activeEvent);
            }

            StartCoroutine(RecordEventInstanceCoroutine(activeEvent));
        }

        /// <summary>
        /// Play a non-continuous container.
        /// </summary>
        private float PlayOneOffContainer(ActiveEvent activeEvent)
        {
            AudioContainer currentContainer = activeEvent.audioEvent.container;

            // Fading or looping overrides immediate volume settings.
            if (activeEvent.audioEvent.fadeInTime == 0 && !activeEvent.audioEvent.container.looping)
            {
                activeEvent.volDest = activeEvent.PrimarySource.volume;
            }

            // Simultaneous sounds.
            float clipTime = 0;

            if (currentContainer.containerType == AudioContainerType.Simultaneous)
            {
                clipTime = PlaySimultaneousClips(currentContainer, activeEvent);
            }
            // Sequential and Random sounds.
            else
            {
                clipTime = PlaySingleClip(currentContainer, activeEvent);
            }

            activeEvent.activeTime = clipTime;
            return clipTime;
        }

        /// <summary>
        /// Play all clips in container simultaneously
        /// </summary>
        private float PlaySimultaneousClips(AudioContainer currentContainer, ActiveEvent activeEvent)
        {
            float tempDelay = 0;
            float finalActiveTime = 0f;

            if (currentContainer.looping)
            {
                finalActiveTime = InfiniteLoop;
            }

            for (int i = 0; i < currentContainer.sounds.Length; i++)
            {
                tempDelay = PlayClipAndGetTime(currentContainer.sounds[i], activeEvent.PrimarySource, activeEvent);

                if (finalActiveTime != InfiniteLoop)
                {
                    float estimatedActiveTimeNeeded = GetActiveTimeEstimate(currentContainer.sounds[i], activeEvent, tempDelay);

                    if (estimatedActiveTimeNeeded == InfiniteLoop || estimatedActiveTimeNeeded > finalActiveTime)
                    {
                        finalActiveTime = estimatedActiveTimeNeeded;
                    }
                }
            }

            return finalActiveTime;
        }

        /// <summary>
        /// Play one sound from a container based on container behavior.
        /// </summary>
        /// <param name="currentContainer"></param>
        /// <param name="activeEvent"></param>
        /// <returns>The estimated ActiveTime for the clip, or InfiniteLoop if the container and/or clip are set to loop.</returns>
        private float PlaySingleClip(AudioContainer currentContainer, ActiveEvent activeEvent)
        {
            float tempDelay = 0;
            if (currentContainer.containerType == AudioContainerType.Random)
            {
                currentContainer.currentClip = Random.Range(0, currentContainer.sounds.Length);
            }
            UAudioClip currentClip = currentContainer.sounds[currentContainer.currentClip];

            // Trigger sound and save the delay (in seconds) to add to the total amount of time the event will be considered active.
            tempDelay = PlayClipAndGetTime(currentClip, activeEvent.PrimarySource, activeEvent);

            // Ready the next clip in the series if sequence container.
            if (currentContainer.containerType == AudioContainerType.Sequence)
            {
                currentContainer.currentClip++;
                if (currentContainer.currentClip >= currentContainer.sounds.Length)
                {
                    currentContainer.currentClip = 0;
                }
            }

            // Return active time based on looping or clip time.
            return GetActiveTimeEstimate(currentClip, activeEvent, tempDelay);
        }

        /// <summary>
        /// Repeatedly trigger the one-off container based on the loop time.
        /// </summary>
        private IEnumerator PlayLoopingOneOffContainerCoroutine(ActiveEvent activeEvent)
        {
            while (!activeEvent.cancelEvent)
            {
                float tempLoopTime = PlayOneOffContainer(activeEvent);
                float eventLoopTime = activeEvent.audioEvent.container.loopTime;

                // Protect against containers looping every frame by defaulting to the length of the audio clip.
                if (eventLoopTime != 0)
                {
                    tempLoopTime = eventLoopTime;
                }

                yield return new WaitForSeconds(tempLoopTime);
            }
        }

        /// <summary>
        /// Choose a random sound from a container and play, calling the looping coroutine to constantly choose new audio clips when current clip ends.
        /// </summary>
        /// <param name="audioContainer">The audio container.</param>
        /// <param name="emitter">The emitter to use.</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        private void PlayContinuousRandomContainer(AudioContainer audioContainer, AudioSource emitter, ActiveEvent activeEvent)
        {
            audioContainer.currentClip = Random.Range(0, audioContainer.sounds.Length);
            UAudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];

            activeEvent.PrimarySource.volume = 0f;
            activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
            activeEvent.altVolDest = 0f;
            activeEvent.currentFade = audioContainer.crossfadeTime;

            float waitTime = (tempClip.sound.length / emitter.pitch) - activeEvent.audioEvent.container.crossfadeTime;

            // Ignore clip delay since container is continuous.
            PlayClipAndGetTime(tempClip, emitter, activeEvent);
            activeEvent.activeTime = InfiniteLoop;
            StartCoroutine(RecordEventInstanceCoroutine(activeEvent));
            audioContainer.currentClip++;
            if (audioContainer.currentClip >= audioContainer.sounds.Length)
            {
                audioContainer.currentClip = 0;
            }
            StartCoroutine(ContinueRandomContainerCoroutine(audioContainer, activeEvent, waitTime));
        }

        /// <summary>
        /// Coroutine for "continuous" random containers that alternates between two sources to crossfade clips for continuous playlist looping.
        /// </summary>
        /// <param name="audioContainer">The audio container.</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        /// <param name="waitTime">The time in seconds to wait before switching AudioSources for crossfading.</param>
        /// <returns>The coroutine.</returns>
        private IEnumerator ContinueRandomContainerCoroutine(AudioContainer audioContainer, ActiveEvent activeEvent, float waitTime)
        {
            while (!activeEvent.cancelEvent)
            {
                yield return new WaitForSeconds(waitTime);

                audioContainer.currentClip = Random.Range(0, audioContainer.sounds.Length);
                UAudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];

                // Play on primary source.
                if (activeEvent.playingAlt)
                {
                    activeEvent.PrimarySource.volume = 0f;
                    activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
                    activeEvent.altVolDest = 0f;
                    activeEvent.currentFade = audioContainer.crossfadeTime;
                    waitTime = (tempClip.sound.length / activeEvent.PrimarySource.pitch) - audioContainer.crossfadeTime;
                    PlayClipAndGetTime(tempClip, activeEvent.PrimarySource, activeEvent);
                }
                // Play on secondary source.
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
        /// Play the current clip in a container, and call the coroutine to constantly choose new audio clips when the current clip ends.
        /// </summary>
        /// <param name="audioContainer">The audio container.</param>
        /// <param name="emitter">The emitter to use.</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        private void PlayContinuousSequenceContainer(AudioContainer audioContainer, AudioSource emitter, ActiveEvent activeEvent)
        {
            UAudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];

            activeEvent.PrimarySource.volume = 0f;
            activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
            activeEvent.altVolDest = 0f;
            activeEvent.currentFade = audioContainer.crossfadeTime;

            float waitTime = (tempClip.sound.length / emitter.pitch) - activeEvent.audioEvent.container.crossfadeTime;

            // Ignore clip delay since the container is continuous.
            PlayClipAndGetTime(tempClip, emitter, activeEvent);
            activeEvent.activeTime = InfiniteLoop;
            StartCoroutine(RecordEventInstanceCoroutine(activeEvent));
            audioContainer.currentClip++;

            if (audioContainer.currentClip >= audioContainer.sounds.Length)
            {
                audioContainer.currentClip = 0;
            }

            StartCoroutine(ContinueSequenceContainerCoroutine(audioContainer, activeEvent, waitTime));
        }

        /// <summary>
        /// Coroutine for "continuous" sequence containers that alternates between two sources to crossfade clips for continuous playlist looping.
        /// </summary>
        /// <param name="audioContainer">The audio container.</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        /// <param name="waitTime">The time in seconds to wait before switching AudioSources to crossfading.</param>
        /// <returns>The coroutine.</returns>
        private IEnumerator ContinueSequenceContainerCoroutine(AudioContainer audioContainer, ActiveEvent activeEvent, float waitTime)
        {
            while (!activeEvent.cancelEvent)
            {
                yield return new WaitForSeconds(waitTime);
                UAudioClip tempClip = audioContainer.sounds[audioContainer.currentClip];
                if (tempClip.sound == null)
                {
                    Debug.LogErrorFormat(this, "Sound clip in event \"{0}\" is null!", activeEvent.audioEvent.name);
                    waitTime = 0;
                }
                else
                {
                    // Play on primary source.
                    if (activeEvent.playingAlt)
                    {
                        activeEvent.PrimarySource.volume = 0f;
                        activeEvent.volDest = activeEvent.audioEvent.volumeCenter;
                        activeEvent.altVolDest = 0f;
                        activeEvent.currentFade = audioContainer.crossfadeTime;
                        waitTime = (tempClip.sound.length / activeEvent.PrimarySource.pitch) - audioContainer.crossfadeTime;
                        PlayClipAndGetTime(tempClip, activeEvent.PrimarySource, activeEvent);
                    }
                    // Play on secondary source.
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
        /// Play a single clip on an AudioSource; if looping forever, return InfiniteLoop for the event time.
        /// </summary>
        /// <param name="audioClip">The audio clip to play.</param>
        /// <param name="emitter">The emitter to use.</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        /// <returns>The amount of delay, if any, we are waiting before playing the clip. A looping clip will always return InfiniteLoop.</returns>
        private float PlayClipAndGetTime(UAudioClip audioClip, AudioSource emitter, ActiveEvent activeEvent)
        {
            if (audioClip.delayCenter == 0)
            {
                emitter.PlayClip(audioClip.sound, audioClip.looping);

                if (audioClip.looping)
                {
                    return InfiniteLoop;
                }

                return 0;
            }
            else
            {
                float rndDelay = Random.Range(audioClip.delayCenter - audioClip.delayRandomization, audioClip.delayCenter + audioClip.delayRandomization);

                StartCoroutine(PlayClipDelayedCoroutine(audioClip, emitter, rndDelay, activeEvent));

                if (audioClip.looping)
                {
                    return InfiniteLoop;
                }

                return rndDelay;
            }
        }

        /// <summary>
        /// Coroutine for playing a clip after a delay (in seconds).
        /// </summary>
        /// <param name="audioClip">The clip to play.</param>
        /// <param name="emitter">The emitter to use.</param>
        /// <param name="delay">The amount of time in seconds to wait before playing audio clip.</param>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        /// <returns>The coroutine.</returns>
        private IEnumerator PlayClipDelayedCoroutine(UAudioClip audioClip, AudioSource emitter, float delay, ActiveEvent activeEvent)
        {
            yield return new WaitForSeconds(delay);

            if (this.activeEvents.Contains(activeEvent))
            {
                emitter.PlayClip(audioClip.sound, audioClip.looping);
            }
        }

        /// <summary>
        /// Stop audio sources in an event, and clean up instance references.
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        protected void StopEvent(ActiveEvent activeEvent)
        {
            if (activeEvent.PrimarySource != null)
            {
                activeEvent.PrimarySource.Stop();
            }

            if (activeEvent.SecondarySource != null)
            {
                activeEvent.SecondarySource.Stop();
            }

            activeEvent.cancelEvent = true;
            RemoveEventInstance(activeEvent);
        }

        /// <summary>
        /// Coroutine for fading out an AudioSource, and stopping the event once fade is complete.
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        /// <param name="fadeTime">The amount of time, in seconds, to completely fade out the sound.</param>
        /// <returns>The coroutine.</returns>
        protected IEnumerator StopEventWithFadeCoroutine(ActiveEvent activeEvent, float fadeTime)
        {
            if (activeEvent.isStoppable)
            {
                activeEvent.isStoppable = false;
                activeEvent.volDest = 0f;
                activeEvent.altVolDest = 0f;
                activeEvent.currentFade = fadeTime;

                yield return new WaitForSeconds(fadeTime);

                if (activeEvent.PrimarySource != null)
                {
                    activeEvent.PrimarySource.Stop();
                }

                if (activeEvent.SecondarySource != null)
                {
                    activeEvent.SecondarySource.Stop();
                }

                activeEvent.cancelEvent = true;
                RemoveEventInstance(activeEvent);
            }
        }

        /// <summary>
        /// Keep an event in the "activeEvents" list for the amount of time we think it will be playing, plus the instance buffer.
        /// This is mostly done for instance limiting purposes.
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        /// <returns>The coroutine.</returns>
        private IEnumerator RecordEventInstanceCoroutine(ActiveEvent activeEvent)
        {
            // Unity has no callback for an audioclip ending, so we have to estimate it ahead of time.
            // Changing the pitch during playback will alter actual playback time.
            activeEvents.Add(activeEvent);

            // Only return active time if sound is not looping/continuous.
            if (activeEvent.activeTime > 0)
            {
                yield return new WaitForSeconds(activeEvent.activeTime);

                // Mark this event so it no longer counts against the instance limit.
                activeEvent.isActiveTimeComplete = true;

                // Since the activeTime estimate may not be enough time to complete the clip (due to pitch changes during playback, or a negative instanceBuffer value, for example)
                // wait here until it is finished, so that we don't cut off the end.
                if (activeEvent.IsPlaying)
                {
                    yield return null;
                }

            }
            // Otherwise, continue at next frame.
            else
            {
                yield return null;
            }

            if (activeEvent.activeTime != InfiniteLoop)
            {
                RemoveEventInstance(activeEvent);
            }
        }

        /// <summary>
        /// Remove event from the currently active events.
        /// </summary>
        /// <param name="activeEvent">The persistent reference to the event as long as it is playing.</param>
        private void RemoveEventInstance(ActiveEvent activeEvent)
        {
            activeEvents.Remove(activeEvent);

            // Send message notifying user that sound is complete
            if (!string.IsNullOrEmpty(activeEvent.MessageOnAudioEnd))
            {
                activeEvent.AudioEmitter.SendMessage(activeEvent.MessageOnAudioEnd);
            }

            activeEvent.Dispose();
        }

        /// <summary>
        /// Return the number of instances matching the name eventName for instance limiting check.
        /// </summary>
        /// <param name="eventName">The name of the event to check.</param>
        /// <returns>The number of instances of that event currently active.</returns>
        protected int GetInstances(string eventName)
        {
            int tempInstances = 0;

            for (int i = 0; i < activeEvents.Count; i++)
            {
                var eventInstance = activeEvents[i];

                if (!eventInstance.isActiveTimeComplete && eventInstance.audioEvent.name == eventName)
                {
                    tempInstances++;
                }
            }

            return tempInstances;
        }

        /// <summary>
        /// Calculates the estimated active time for an ActiveEvent playing the given clip.
        /// </summary>
        /// <param name="audioClip">The clip being played.</param>
        /// <param name="activeEvent">The event being played.</param>
        /// <param name="additionalDelay">The delay before playing in seconds.</param>
        /// <returns>The estimated active time of the event based on looping or clip time. If looping, this will return InfiniteLoop.</returns>
        private static float GetActiveTimeEstimate(UAudioClip audioClip, ActiveEvent activeEvent, float additionalDelay)
        {
            if (audioClip.looping || activeEvent.audioEvent.container.looping || additionalDelay == InfiniteLoop)
            {
                return InfiniteLoop;
            }
            else
            {
                float pitchAdjustedClipLength = activeEvent.PrimarySource.pitch != 0 ? (audioClip.sound.length / activeEvent.PrimarySource.pitch) : 0;

                // Restrict non-looping ActiveTime values to be non-negative.
                return Mathf.Max(0.0f, pitchAdjustedClipLength + activeEvent.audioEvent.instanceTimeBuffer + additionalDelay);
            }
        }
    }
}