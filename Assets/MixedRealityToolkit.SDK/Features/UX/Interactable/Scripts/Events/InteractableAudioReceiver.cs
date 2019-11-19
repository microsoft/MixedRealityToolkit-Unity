// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Add audio clip to play onClick or on Voice Command
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/InteractableAudioReceiver")]
    public class InteractableAudioReceiver : ReceiverBase
    {
        /// <summary>
        /// AudioClip to play when event is selected
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.AudioClip, Label = "Audio Clip", Tooltip = "Assign an audioclip to play on click")]
        public AudioClip AudioClip;

        /// <inheritdoc />
        public override bool HideUnityEvents => true;

        private State lastState;

        /// <summary>
        /// Creates and AudioReceiver, which plays sounds on Click
        /// </summary>
        public InteractableAudioReceiver(UnityEvent ev) : base(ev, "AudioEvent")
        {
        }

        /// <summary>
        /// Called on update, check to see if the state has changed sense the last call
        /// </summary>
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            if (state.CurrentState() != lastState)
            {
                // the state has changed, do something new
                lastState = state.CurrentState();
            }
        }

        /// <summary>
        /// assign the clip to the audio source and play
        /// </summary>
        private void PlayAudio(Interactable source)
        {
            AudioSource audioSource = source.GetComponent<AudioSource>();
            if(audioSource == null)
            {
                audioSource = source.gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = AudioClip;
            audioSource.Play();
        }

        /// <summary>
        /// click happened
        /// </summary>
        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            base.OnClick(state, source);
            PlayAudio(source);
        }

        /// <summary>
        /// voice command called
        /// </summary>
        public override void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            base.OnVoiceCommand(state, source, command, index, length);
            PlayAudio(source);
        }
    }
}
