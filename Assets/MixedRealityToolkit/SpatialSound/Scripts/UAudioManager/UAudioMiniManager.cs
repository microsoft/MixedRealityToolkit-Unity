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
            for (int i = 0; i < Events.Length; i++)
            {
                PlayEvent(this.Events[i]);
            }
        }

        /// <summary>
        /// Play a single MiniAudioEvent on the predetermined source(s)
        /// </summary>
        /// <param name="audioEvent">The MiniAudioEvent to play</param>
        private void PlayEvent(MiniAudioEvent audioEvent)
        {
            if (audioEvent.PrimarySource == null)
            {
                Debug.LogErrorFormat(this, "Emitter on object \"{0}\" is null! Cannot play sound.", audioEvent.Name);
                return;
            }

            if (audioEvent.IsContinuous())
            {
                if (audioEvent.SecondarySource == null)
                {
                    Debug.LogErrorFormat(this, "Secondary emitter on event \"{0}\" is null! Cannot play continuous sound.", audioEvent.Name);
                }
            }

            ActiveEvent tempEvent = new ActiveEvent(audioEvent, audioEvent.PrimarySource.gameObject, audioEvent.PrimarySource, audioEvent.SecondarySource);

            // Do this last. The base class owns this event once we pass it to PlayContainer, and may dispose it if it cannot be played.
            PlayContainer(tempEvent);
        }

        /// <summary>
        /// Sets the mute flag for all AudioSource components in the event
        /// </summary>
        public void SetMute(bool mute)
        {
            for (int i = 0; i < Events.Length; i++)
            {
                Events[i].PrimarySource.mute = mute;
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

            for (int i = 0; i < this.Events.Length; i++)
            {
                Events[i].PrimarySource.pitch = newPitch;
            }
        }
    }
}