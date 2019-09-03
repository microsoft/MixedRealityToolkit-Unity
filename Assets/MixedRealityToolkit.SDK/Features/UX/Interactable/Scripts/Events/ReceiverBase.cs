// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Windows.Markup;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The base class for all receivers that attach to Interactables
    /// </summary>
    public abstract class ReceiverBase
    {
        public string Name;

        public bool HideUnityEvents;
        protected UnityEvent uEvent;

        /// <summary>
        /// Each Receiver has a base Event it raises, (in addition to others).
        /// </summary>
        public UnityEvent Event { get => uEvent; set => uEvent = value; }

        public MonoBehaviour Host;

        /// <summary>
        /// Constructs an interaction receiver that will raise unity event when triggered.
        /// </summary>
        /// <param name="ev">Unity event to invoke. Add more events in deriving class.</param>
        /// <param name="name">Name of the unity event that will get invoked (visible in editor).</param>
        public ReceiverBase(UnityEvent ev, String name)
        {
            uEvent = ev;
            Name = name;
        }

        /// <summary>
        /// The state has changed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public abstract void OnUpdate(InteractableStates state, Interactable source);

        /// <summary>
        /// A voice command was called
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public virtual void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            // voice command called
        }

        /// <summary>
        /// A click event happened
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="pointer"></param>
        public virtual void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            // click called
        }
    }
}
