// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The UAudioMiniManager class organization and control of a GameObject's MiniAudioEvents.
    /// </summary>
    public partial class UAudioMiniManager : UAudioManagerBase<MiniAudioEvent>
    {
        /// <summary>
        /// Plays all of the Audio Events in the manager
        /// </summary>
        public void PlayAll()
        {
            for (int i = 0; i < events.Length; i++)
            {
                PlayEvent(this.events[i]);
            }
        }

        /// <summary>
        /// Play a single MiniAudioEvent on the predetermined source(s)
        /// </summary>
        /// <param name="audioEvent">The MiniAudioEvent to play</param>
        private void PlayEvent(MiniAudioEvent audioEvent)
        {
            if (audioEvent.primarySource == null)
            {
                Debug.LogErrorFormat(this, "Emitter on object \"{0}\" is null! Cannot play sound.", audioEvent.name);
                return;
            }

            if (audioEvent.IsContinuous())
            {
                if (audioEvent.secondarySource == null)
                {
                    Debug.LogErrorFormat(this, "Secondary emitter on event \"{0}\" is null! Cannot play continuous sound.", audioEvent.name);
                }
            }

            ActiveEvent tempEvent = new ActiveEvent(audioEvent, audioEvent.primarySource.gameObject, audioEvent.primarySource, audioEvent.secondarySource);

            // Do this last. The base class owns this event once we pass it to PlayContainer, and may dispose it if it cannot be played.
            PlayContainer(tempEvent);
        }

        /// <summary>
        /// Sets the mute flag for all AudioSource components in the event
        /// </summary>
        public void SetMute(bool mute)
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i].primarySource.mute = mute;
            }
        }

        /// <summary>
        /// Sets the pitch value for all AudioSource components in the event
        /// </summary>
        /// <param name="newPitch">The value to set the pitch, between 0 (exclusive) and 3 (inclusive).</param>
        public void SetPitch(float newPitch)
        {
            if (newPitch <= 0 || newPitch > 3)
            {
                Debug.LogErrorFormat(this, "Invalid pitch {0} set", newPitch);
                return;
            }

            for (int i = 0; i < this.events.Length; i++)
            {
                events[i].primarySource.pitch = newPitch;
            }
        }
    }
}