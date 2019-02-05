// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Utilities.InspectorFields;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events
{
    /// <summary>
    /// Add audio clip to play onClick or on Voice Command
    /// </summary>
    public class InteractableAudioReceiver : ReceiverBase
    {
        [InspectorField(Type = InspectorField.FieldTypes.AudioClip, Label = "Audio Clip", Tooltip = "Assign an audioclip to play on click")]
        public AudioClip AudioClip;

        private State lastState;
        
        public InteractableAudioReceiver(UnityEvent ev) : base(ev)
        {
            Name = "AudioEvent";
            HideUnityEvents = true; // hides Unity events in the receiver - meant to be code only
        }
        
        /// <summary>
        /// Called on update, check to see if the state has changed sense the last call
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
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
        /// <param name="source"></param>
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
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="pointer"></param>
        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            base.OnClick(state, source);
            PlayAudio(source);
        }

        /// <summary>
        /// voice command called
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public override void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            base.OnVoiceCommand(state, source, command, index, length);
            PlayAudio(source);
        }
    }
}
