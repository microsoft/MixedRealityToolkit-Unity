//
// Copyright (C) Microsoft. All rights reserved.
//

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// The runtime event of an Audio Event that is currently playing
    /// </summary>
    public class ActiveEvent : IDisposable
    {
        /// <summary>
        /// The AudioEvent that is being played
        /// </summary>
        public AudioEvent rootEvent { get; private set; }
        /// <summary>
        /// The AudioSource to play the AudioEvent on
        /// </summary>
        public AudioSource source { get; private set; }
        /// <summary>
        /// The AudioClip to play from the event
        /// </summary>
        public AudioClip clip;
        /// <summary>
        /// The text associated with the event, usually for subtitles
        /// </summary>
        internal string text = "";
        /// <summary>
        /// Time in seconds before the audio file will start playing
        /// </summary>
        internal float initialDelay = 0;
        /// <summary>
        /// The volume of the event before parameters are applied
        /// </summary>
        private float eventVolume = 0;
        /// <summary>
        /// The volume that the event is fading to or settled on after fading
        /// </summary>
        private float targetVolume = 1;
        /// <summary>
        /// The pitch value 
        /// </summary>
        private float eventPitch = 1;
        /// <summary>
        /// The amount of time in seconds that the event has been active
        /// </summary>
        private float elapsedTime = 0;
        /// <summary>
        /// The amount of time in seconds that the event will fade in or out
        /// </summary>
        private float targetFadeTime = 0;
        /// <summary>
        /// The amount of time in seconds that the event has been fading in or out
        /// </summary>
        private float currentFadeTime = 0;
        /// <summary>
        /// The previous volume the event was at before starting a fade
        /// </summary>
        private float originVolume = 0;
        /// <summary>
        /// Whether the event is currently fading out to be stopped
        /// </summary>
        private bool fadeStopQueued = false;
        /// <summary>
        /// Whether the event has played a sound yet
        /// </summary>
        private bool hasPlayed = false;
        /// <summary>
        /// Whether the event is using the user's gaze for a parameter
        /// </summary>
        private bool useGaze = false;
        /// <summary>
        /// The AudioParameters in use by the event
        /// </summary>
        private ActiveParameter[] activeParameters;
        /// <summary>
        /// The Transform to use to calculate the user's gaze position
        /// </summary>
        private Transform gazeReference;
        /// <summary>
        /// Whether the event has been disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The time left on the event unless it is stopped or the pitch changes
        /// </summary>
        public float EstimatedRemainingTime { get; private set; }
        /// <summary>
        /// Whether the event has been muted
        /// </summary>
        public bool Muted { get; private set; }
        /// <summary>
        /// Whether the event has been soloed
        /// </summary>
        public bool Soloed { get; private set; }

        /// <summary>
        /// Delegate for event completion callback
        /// </summary>
        public delegate void EventCompleted();

        /// <summary>
        /// Callback when the event stops
        /// </summary>
        public event EventCompleted CompletionCallback;

        /// <summary>
        /// Constructor: Create a new ActiveEvent from an AudioEvent and play it on an AudioSource
        /// </summary>
        /// <param name="eventToPlay">The AudioEvent to play</param>
        /// <param name="source">The AudioSource use for the AudioEvent</param>
        public ActiveEvent(AudioEvent eventToPlay, AudioSource source)
        {
            this.rootEvent = eventToPlay;
            this.source = source;
            this.Soloed = false;
            this.Muted = false;
            this.activeParameters = new ActiveParameter[this.rootEvent.Parameters.Count];
            for (int i = 0; i < this.activeParameters.Length; i++)
            {
                this.activeParameters[i] = new ActiveParameter(this.rootEvent.Parameters[i]);
            }
        }

        /// <summary>
        /// Internal AudioManager use: starts the audio event
        /// </summary>
        public void Play()
        {
            this.rootEvent.SetActiveEventProperties(this);
            if (this.rootEvent.FadeIn > 0)
            {
                this.source.volume = 0;
                this.eventVolume = 0;
                this.originVolume = 0;
                this.currentFadeTime = 0;
                this.targetFadeTime = this.rootEvent.FadeIn;
            }
            else
            {
                this.source.volume = this.targetVolume;
                this.eventVolume = this.targetVolume;
            }

            this.source.pitch = this.eventPitch;
            this.source.clip = this.clip;
            this.EstimatedRemainingTime = this.clip.length / this.eventPitch;
            if (this.initialDelay == 0)
            {
                this.hasPlayed = true;
                this.source.Play();
            }

            this.useGaze = HasGazeProperty();
            if (this.useGaze)
            {
                this.gazeReference = Camera.main.transform;
                UpdateGaze();
            }
            else
            {
                ApplyParameters();
            }
        }

        /// <summary>
        /// Internal AudioManager use: update fade and RTPC values
        /// </summary>
        public void Update()
        {
            if (this.source == null)
            {
                StopImmediate();
                return;
            }

            this.elapsedTime += Time.deltaTime;

            if (!this.hasPlayed && this.elapsedTime >= this.initialDelay)
            {
                this.hasPlayed = true;
                this.source.Play();
            }

            if (this.hasPlayed && this.currentFadeTime < this.targetFadeTime)
            {
                UpdateFade();
            }

            if (!this.source.loop)
            {
                UpdateRemainingTime();
                if (this.hasPlayed && !this.source.isPlaying)
                {
                    StopImmediate();
                }
            }

            if (this.useGaze)
            {
                UpdateGaze();
            }

            UpdateParameters();
            ApplyParameters();
        }

        /// <summary>
        /// Stops the event using the default fade time if applicable
        /// </summary>
        public void Stop()
        {
            if (this.rootEvent.FadeOut <= 0)
            {
                StopImmediate();
            }
            else
            {
                this.targetVolume = 0;
                this.originVolume = this.source.volume;
                this.targetFadeTime = this.rootEvent.FadeOut;
                this.currentFadeTime = 0;
                this.fadeStopQueued = true;
            }
        }

        /// <summary>
        /// Stops the event immediately, ignoring the event's fade time
        /// </summary>
        public void StopImmediate()
        {
            if (this.CompletionCallback != null)
            {
                CompletionCallback();
            }

            if (this.source != null)
            {
                this.source.Stop();
            }

            AudioManager.RemoveActiveEvent(this);
            Dispose();
        }

        /// <summary>
        /// Set the value of a parameter on this event independent of the root parameter's value
        /// </summary>
        /// <param name="localParameter">The AudioParameter set on the root AudioEvent</param>
        /// <param name="newValue">The new local value on this event instance (does not set the AudioParameter value)</param>
        public void SetLocalParameter(AudioParameter localParameter, float newValue)
        {
            bool hasParameter = false;
            for (int i = 0; i < this.activeParameters.Length; i++)
            {
                ActiveParameter tempParameter = this.activeParameters[i];
                if (tempParameter.rootParameter.parameter == localParameter)
                {
                    hasParameter = true;
                    tempParameter.CurrentValue = newValue;
                }
            }

            if (!hasParameter)
            {
                Debug.LogWarningFormat("Audio event {0} does not have parameter {1}", this.rootEvent.name, localParameter.name);
            }
        }

        /// <summary>
        /// Internal AudioManager use: initializes the volume based on the event's AudioOutput
        /// </summary>
        /// <param name="newVolume">The target volume for the AudioSource (not necessarily the current volume)</param>
        public void SetVolume(float newVolume)
        {
            this.targetVolume = newVolume;
        }

        /// <summary>
        /// Offset the volume by the specified amount
        /// </summary>
        /// <param name="volumeDelta">A number between -1 and 1 for volume changes</param>
        public void ModulateVolume(float volumeDelta)
        {
            this.targetVolume += volumeDelta;
        }

        /// <summary>
        /// Overwrite the pitch with a new value
        /// </summary>
        /// <param name="newPitch">Pitch value between -1 and 3</param>
        public void SetPitch(float newPitch)
        {
            this.eventPitch = newPitch;
        }

        /// <summary>
        /// Offset the pitch by the specified amount
        /// </summary>
        /// <param name="pitchDelta">A number between -1 and 3 for pitch changes</param>
        public void ModulatePitch(float pitchDelta)
        {
            this.eventPitch += pitchDelta;
        }

        /// <summary>
        /// Clear memory for the ActiveEvent so it is not handled by the garbage manager
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementation for IDisposable interface
        /// </summary>
        /// <param name="disposing">Flag for whether to clean up object</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    for (int i = 0; i < this.activeParameters.Length; i++)
                    {
                        this.activeParameters[i].Dispose();
                    }
                }

                this.disposed = true;
            }
        }

        #region Private Functions

        /// <summary>
        /// Internal AudioManager use: Sync changes to the parameters on all ActiveEvents
        /// </summary>
        private void UpdateParameters()
        {
            for (int i = 0; i < this.activeParameters.Length; i++)
            {
                EventParameter tempParam = this.activeParameters[i].rootParameter;
                if (tempParam.CurrentValue != tempParam.parameter.CurrentValue)
                {
                    tempParam.SyncParameter();
                }
            }
        }

        /// <summary>
        /// Internal AudioManager use: applies parameter changes
        /// </summary>
        private void ApplyParameters()
        {
            float tempVolume = this.eventVolume;
            float tempPitch = this.eventPitch;
            for (int i = 0; i < this.activeParameters.Length; i++)
            {
                ActiveParameter tempParameter = this.activeParameters[i];
                switch (tempParameter.rootParameter.paramType)
                {
                    case ParameterType.Volume:
                        tempVolume *= tempParameter.CurrentResult;
                        break;
                    case ParameterType.Pitch:
                        tempPitch *= tempParameter.CurrentResult;
                        break;
                }
            }

            this.source.volume = tempVolume;
            this.source.pitch = tempPitch;
        }

        /// <summary>
        /// Internal AudioManager use: update volume on ActiveEvents fading in and out
        /// </summary>
        private void UpdateFade()
        {
            float percentageFaded = (this.currentFadeTime / this.targetFadeTime);

            if (this.targetVolume > this.originVolume)
            {
                this.eventVolume = this.originVolume + ((this.targetVolume - this.originVolume) * percentageFaded);
            }
            else
            {
                this.eventVolume = this.originVolume - ((this.originVolume - this.targetVolume) * percentageFaded);
            }

            this.currentFadeTime += Time.deltaTime;

            if (this.currentFadeTime >= this.targetFadeTime)
            {
                this.eventVolume = this.targetVolume;

                if (this.fadeStopQueued)
                {
                    StopImmediate();
                }
            }

            this.source.volume = this.eventVolume;
        }

        /// <summary>
        /// Recalculate estimated time the event will be active for
        /// </summary>
        private void UpdateRemainingTime()
        {
            this.EstimatedRemainingTime = (this.source.clip.length - this.source.time) / this.source.pitch;

            if (this.EstimatedRemainingTime <= this.rootEvent.FadeOut)
            {
                Stop();
            }
        }

        /// <summary>
        /// Check if any of the parameters on the event use gaze for the value
        /// </summary>
        /// <returns>Whether at least one parameter uses gaze</returns>
        private bool HasGazeProperty()
        {
            for (int i = 0; i < this.rootEvent.Parameters.Count; i++)
            {
                AudioParameter tempParam = this.rootEvent.Parameters[i].parameter;
                if (tempParam.UseGaze)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the user's head position relative to the audio event, and set the property's value
        /// </summary>
        private void UpdateGaze()
        {
            float gazeAngle = 0;
            if (this.gazeReference == null)
            {
                this.gazeReference = Camera.main.transform;
            }

            Vector3 posDelta = this.gazeReference.position - this.source.transform.position;
            gazeAngle = Mathf.Abs(180 - Vector3.Angle(this.gazeReference.forward, posDelta));

            for (int i = 0; i < this.activeParameters.Length; i++)
            {
                ActiveParameter tempParameter = this.activeParameters[i];
                if (tempParameter.rootParameter.parameter.UseGaze)
                {
                    tempParameter.CurrentValue = gazeAngle;
                }
            }
        }

        #endregion

        #region Editor

        /// <summary>
        /// Set whether the defined parameter on the event uses the player's gaze angle
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="gazeParameter"></param>
        public void ToggleGaze(bool toggle, AudioParameter gazeParameter)
        {
            this.useGaze = toggle;

            if (!toggle)
            {
                for (int i = 0; i < this.activeParameters.Length; i++)
                {
                    ActiveParameter tempParameter = this.activeParameters[i];
                    if (tempParameter.rootParameter.parameter == gazeParameter)
                    {
                        tempParameter.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// Toggles whether the event is audible
        /// </summary>
        public void ToggleMute()
        {
            this.Muted = !this.Muted;
            this.source.mute = this.Muted;
        }

        /// <summary>
        /// Sets whether the event is audible
        /// </summary>
        /// <param name="toggle">Whether sound should be made inaudible</param>
        public void SetMute(bool toggle)
        {
            this.Muted = toggle;
            this.source.mute = this.Muted;
        }

        /// <summary>
        /// Toggles whether only this sound (and other soloed sounds) are audible
        /// </summary>
        public void ToggleSolo()
        {
            this.Soloed = !this.Soloed;
            AudioManager.ApplyActiveSolos();
        }

        /// <summary>
        /// Mutes all other non-soloed events
        /// </summary>
        /// <param name="toggle">Whether this event is part of the isolated audible events</param>
        public void SetSolo(bool toggle)
        {
            this.Soloed = toggle;
            AudioManager.ApplyActiveSolos();
        }

        /// <summary>
        /// Internal AudioManager use: applies solo property to AudioSource, ignoring "mute" property
        /// </summary>
        public void ApplySolo()
        {
            this.source.mute = !this.Soloed;
        }

        /// <summary>
        /// Internal AudioManager use: clears all solo properties, and reverts to mute property
        /// </summary>
        public void ClearSolo()
        {
            this.Soloed = false;
            this.source.mute = this.Muted;
        }

        #endregion
    }
}