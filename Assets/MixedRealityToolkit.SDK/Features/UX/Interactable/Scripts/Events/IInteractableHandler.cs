// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public interface IInteractableHandler
    {
        void OnStateChange(InteractableStates state, Interactable source);

        /// <summary>
        /// A voice command was called
        /// </summary>
        void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1);

        /// <summary>
        /// A click event happened
        /// </summary>
        void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null);
    }
}
