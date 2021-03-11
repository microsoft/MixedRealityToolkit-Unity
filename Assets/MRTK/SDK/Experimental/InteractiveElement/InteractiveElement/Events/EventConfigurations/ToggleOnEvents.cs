// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the ToggleOn InteractionState.
    /// </summary>
    public class ToggleOnEvents : BaseInteractionEventConfiguration
    {
        [SerializeField]
        [Tooltip("Whether on not the toggle is selected when the application starts.")]
        private bool isSelectedOnStart = false;

        /// <summary>
        /// Whether on not the toggle is selected when the application starts.
        /// </summary>
        public bool IsSelectedOnStart
        {
            get => isSelectedOnStart;
            set => isSelectedOnStart = value;
        }

        /// <summary>
        /// A Unity event that is fired when the ToggleOff state is active.
        /// </summary>
        public UnityEvent OnToggleOn = new UnityEvent();
    }
}
