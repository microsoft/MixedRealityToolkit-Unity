// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Example of building an event system for Interactable that still uses ReceiverBase events
    /// </summary>
    public class InteractableReceiver : ReceiverBaseMonoBehavior
    {
        // list of events added to this interactable
        [HideInInspector]
        public List<InteractableEvent> Events = new List<InteractableEvent>();

        protected virtual void Awake()
        {
            SetupEvents();
        }

        /// <summary>
        /// set up only one event
        /// </summary>
        protected virtual void SetupEvents()
        {
            if (Events.Count > 0)
            {
                InteractableTypesContainer interactableTypes = InteractableEvent.GetEventTypes();
                Events[0].Receiver = InteractableEvent.GetReceiver(Events[0], interactableTypes);
                Events[0].Receiver.Host = this;
            }
        }

        /// <summary>
        /// A state has changed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public override void OnStateChange(InteractableStates state, Interactable source)
        {
            base.OnStateChange(state, source);
            if (Events.Count > 0)
            {
                if (Events[0].Receiver != null)
                {
                    Events[0].Receiver.OnUpdate(state, source);
                }
            }
        }

        /// <summary>
        /// click happened
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="pointer"></param>
        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            base.OnClick(state, source, pointer);

            if (Events.Count > 0)
            {
                if (Events[0].Receiver != null)
                {
                    Events[0].Receiver.OnClick(state, source, pointer);
                }
            }
        }

        /// <summary>
        /// voice command happened
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public override void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            base.OnVoiceCommand(state, source, command, index, length);

            if (Events.Count > 0)
            {
                if (Events[0].Receiver != null)
                {
                    Events[0].Receiver.OnVoiceCommand(state, source, command, index, length);
                }
            }
        }
    }
}
