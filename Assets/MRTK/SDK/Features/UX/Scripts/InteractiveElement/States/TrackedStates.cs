// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The scriptable object container for the states tracked within an Interactive Element.
    /// </summary>
    [CreateAssetMenu(fileName = "TrackedStates", menuName = "Mixed Reality Toolkit/Interactive Element/Tracked States")]
    public class TrackedStates : ScriptableObject
    {
        [SerializeField]
        [Tooltip("A list of the interaction states that will be tracked.")]
        private List<InteractionState> states = new List<InteractionState>(); 

        /// <summary>
        /// A list of the interaction states that will be tracked.
        /// </summary>
        public List<InteractionState> States
        {
            get { return states; }
            set { states = value; }
        }

        public TrackedStates()
        {
            // Add default states to the States list initially
            States.Add(new InteractionState("Default"));
            States.Add(new InteractionState("Focus"));
        }

        /// <summary>
        /// Check if a state is currently being tracked.  If the state is in the States list then the 
        /// state is being tracked.
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>True if the state is being tracked, false otherwise</returns>
        public bool IsStateTracked(string stateName)
        {
            if (States.Exists((state) => state.Name == stateName))
            {
                Debug.LogError($"The {stateName} state is already being tracked and cannot be added to states.");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
