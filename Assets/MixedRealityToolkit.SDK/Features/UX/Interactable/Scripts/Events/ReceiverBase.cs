// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The base class for all receivers that attach to Interactables
    /// </summary>
    public abstract class ReceiverBase
    {
        /// <summary>
        /// Name of Event Receiver
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Defines whether Unity Events should be hidden in inspector for this type of EventReceiver
        /// </summary>
        public virtual bool HideUnityEvents => false;

        protected UnityEvent uEvent;
        /// <summary>
        /// Each Receiver has a base Event it raises, (in addition to others).
        /// </summary>
        public UnityEvent Event { get => uEvent; set => uEvent = value; }

        /// <summary>
        /// Targeted component for Event Receiver at runtime
        /// </summary>
        public MonoBehaviour Host { get; set; }

        /// <summary>
        /// Constructs an interaction receiver that will raise unity event when triggered.
        /// </summary>
        /// <param name="ev">Unity event to invoke. Add more events in deriving class.</param>
        /// <param name="name">Name of the unity event that will get invoked (visible in editor).</param>
        public ReceiverBase(UnityEvent ev, string name)
        {
            uEvent = ev;
            Name = name;
        }

        /// <summary>
        /// The state has changed
        /// </summary>
        public abstract void OnUpdate(InteractableStates state, Interactable source);

        /// <summary>
        /// A voice command was called
        /// </summary>
        public virtual void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
        }

        /// <summary>
        /// A click event happened
        /// </summary>
        public virtual void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
        }
    }
}
