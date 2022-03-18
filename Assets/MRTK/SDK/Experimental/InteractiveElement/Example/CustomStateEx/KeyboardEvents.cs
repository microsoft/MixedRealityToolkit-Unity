// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement.Examples
{
    /// <summary>
    /// Example event configuration for the Keyboard state.
    /// </summary>
    public class KeyboardEvents : BaseInteractionEventConfiguration
    {
        public UnityEvent OnKKeyPressed = new UnityEvent();
    }
}