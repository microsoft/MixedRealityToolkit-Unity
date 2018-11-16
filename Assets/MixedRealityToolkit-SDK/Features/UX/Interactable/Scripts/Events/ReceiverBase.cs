// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events
{
    /// <summary>
    /// The base class for all receivers that attach to Interactables
    /// </summary>
    public abstract class ReceiverBase
    {
        public string Name;

        public bool HideUnityEvents;
        protected UnityEvent uEvent;
        public MonoBehaviour Host;

        public ReceiverBase(UnityEvent ev)
        {
            uEvent = ev;
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
        public virtual void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            // voice command called
        }

        /// <summary>
        /// A click event happened
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public virtual void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            // click called
        }
    }
}
