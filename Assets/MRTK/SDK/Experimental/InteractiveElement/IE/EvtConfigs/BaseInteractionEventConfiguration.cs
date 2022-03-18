// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// Base class for interaction event configuration. An event configuration maps to a single Interaction State.
    /// </summary>
    [System.Serializable]
    public class BaseInteractionEventConfiguration : IStateEventConfig
    {
        [SerializeField, HideInInspector]
        private string stateName = null;

        /// <summary>
        /// The name of the state associated with this event configuration.
        /// </summary>
        public string StateName
        {
            get => stateName;
            set => stateName = value;
        }

        /// <summary>
        /// The associated runtime event receiver for this event configuration.
        /// </summary>
        public BaseEventReceiver EventReceiver { get; set; } = null;
    }
}
