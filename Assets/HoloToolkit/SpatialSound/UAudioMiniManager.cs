using System.Threading;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class UAudioMiniManager : UAudioManagerBase<MiniAudioEvent>
    {
        /// <summary>
        /// Plays all of the Audio Events in the manager
        /// </summary>
        public void PlayAll()
        {
            for (int i = 0; i < this.events.Length; i++)
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
            if (AudioEvent.IsContinuous(audioEvent))
            {
                if (audioEvent.secondarySource == null)
                {
                    Debug.LogErrorFormat(this, "Secondary emitter on event \"{0}\" is null! Cannot play continuous sound.", audioEvent.name);
                }
            }

            ActiveEvent tempEvent = CreateNewActiveEvent(audioEvent);
            tempEvent.PrimarySource = audioEvent.primarySource;
            tempEvent.SecondarySource = audioEvent.secondarySource;
            SetSourceProperties(tempEvent.PrimarySource, tempEvent);
            PlayContainer(tempEvent);
        }

        /// <summary>
        /// Sets the mute flag for all AudioSource components in the event
        /// </summary>
        public void SetMute(bool mute)
        {
            for (int i = 0; i < this.events.Length; i++)
            {
                this.events[i].primarySource.mute = mute;
            }
        }

        /// <summary>
        /// Sets the pitch value for all AudioSource components in the event
        /// </summary>
        /// <param name="newPitch"></param>
        public void SetPitch(float newPitch = 1)
        {
            for (int i = 0; i < this.events.Length; i++)
            {
                this.events[i].primarySource.pitch = newPitch;
            }
        }
    }
}