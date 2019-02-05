// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events
{
    public interface IInteractableHandler
    {
        void OnStateChange(InteractableStates state, Interactable source);

        /// <summary>
        /// A voice command was called
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1);

        /// <summary>
        /// A click event happened
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null);
    }
}
