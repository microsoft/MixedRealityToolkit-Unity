// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// An example of building an Interactable receiver that uses built-in receivers that extend ReceiverBase
    /// This is a mono behavior, place on a gameObject and assign the Interactable
    /// </summary>
    public class InteractableReceiverList : ReceiverBaseMonoBehavior
    {
        // list of events added to this interactable
        [HideInInspector]
        public List<InteractableEvent> Events = new List<InteractableEvent>();

        protected virtual void Awake()
        {
            SetupEvents();
        }

        /// <summary>
        /// starts the event system
        /// </summary>
        protected virtual void SetupEvents()
        {
            InteractableTypesContainer interactableTypes = InteractableEvent.GetEventTypes();

            for (int i = 0; i < Events.Count; i++)
            {
                Events[i].Receiver = InteractableEvent.GetReceiver(Events[i], interactableTypes);
                Events[i].Receiver.Host = this;
            }
        }

        /// <summary>
        /// .A state has changed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public override void OnStateChange(InteractableStates state, Interactable source)
        {
            base.OnStateChange(state, source);

            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnUpdate(state, source);
                }
            }

        }

        /// <summary>
        /// captures click events
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="pointer"></param>
        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            base.OnClick(state, source, pointer);

            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnClick(state, source, pointer);
                }
            }
        }

        /// <summary>
        /// captures voice commands
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="index">index of the voice command</param>
        /// <param name="length">voice command array length</param>
        public override void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            base.OnVoiceCommand(state, source, command, index, length);

            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnVoiceCommand(state, source, command, index, length);
                }
            }
        }
    }
}
