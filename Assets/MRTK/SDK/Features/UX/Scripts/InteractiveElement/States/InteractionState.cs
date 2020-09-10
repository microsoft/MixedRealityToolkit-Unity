// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The container that represents a single state that is tracked within an Interactive Element.  The Tracked States 
    /// scriptable object contains a list of Interaction States to track.
    /// </summary>
    [System.Serializable]
    public class InteractionState
    {
        /// <summary>
        /// Constructor for an Interaction State.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        public InteractionState(string stateName)
        {
            Name = stateName;
        }

        [SerializeField]
        [Tooltip("The name of the state")]
        private string stateName;

        /// <summary>
        /// The name of the state.
        /// </summary>
        public string Name
        {
            get => stateName;
            internal set => stateName = value;
        }

        [SerializeField]
        [Tooltip("The value of the state. The value will be 0 if the state is off, 1 if the state is on.")]
        private int stateValue = 0;

        /// <summary>
        /// The value of the state. The value will be 0 if the state is off, 1 if the state is on.
        /// </summary>
        public int Value
        {
            get => stateValue;
            internal set => stateValue = value; 
        }        
        
        [SerializeField]
        [Tooltip("The active value of the state. If the state is currently active the value will be 1, 0 if the " +
            " state is not active.")]
        private int active = 0;

        /// <summary>
        /// The active value of the state. If the state is currently active the value will be 1, 0 if the 
        /// state is not active.
        /// </summary>
        public int Active
        {
            get => active;
            internal set => active = value; 
        }

        [SerializeField]
        [Tooltip("The event configuration of a state. The event configuration is an already defined scriptable object that contains the " +
            "serialized Unity Events with specific event data for the state. For example, the FocusEvents contains serialized" +
            "OnFocusOn and OnFocusOff Unity Events with Focus Event Data. These events are exposed in the inspector." +
            "The event configuration of a state is null when an associated event configuration does not exist" +
            "for the specific state. For example, the Default state does not have an associated event configuration therefore EventConfiguration " +
            "will always be null.")]
        private BaseInteractionEventConfiguration eventConfiguration = null;

        /// <summary>
        /// The event configuration of a state. The event configuration is an already defined scriptable object that contains the 
        /// serialized Unity Events with specific event data for the state. For example, the FocusEvents contains serialized
        /// OnFocusOn and OnFocusOff Unity Events with Focus Event Data. These events are exposed in the inspector.
        /// The event configuration of a state is null when an associated event configuration does not exist
        /// for the specific state. For example, the Default state does not have an associated event configuration therefore EventConfiguration 
        /// will always be null.
        /// </summary>
        public BaseInteractionEventConfiguration EventConfiguration
        {
            get => eventConfiguration;
            internal set => eventConfiguration = value;
        }
    }
}
