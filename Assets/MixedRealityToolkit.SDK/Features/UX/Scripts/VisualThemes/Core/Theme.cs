// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme scriptableObject for loading theme settings
    /// </summary>
    [CreateAssetMenu(fileName = "Theme", menuName = "Mixed Reality Toolkit/Theme", order = 1)]
    public class Theme : ScriptableObject
    {
        [FormerlySerializedAs("Settings")]
        [SerializeField]
        private List<ThemeDefinition> definitions;

        /// <summary>
        /// List of Theme Definition configurations. Each definition defines what type of Theme Engine to create and how to configure it
        /// </summary>
        public List<ThemeDefinition> Definitions
        {
            get { return definitions; }
            set
            {
                definitions = value;
                ValidateDefinitions();
            }
        }

        [FormerlySerializedAs("States")]
        [SerializeField]
        private States states;
        /// <summary>
        /// Associated States object to use with this theme. Defines the states available for each Theme to utilize
        /// </summary>
        public States States
        {
            get { return states; }
            set
            {
                states = value;
                ValidateDefinitions();
            }
        }

        /// <summary>
        /// Stores historical values of different ThemeDefinition selections. Useful for editor design
        /// Each item in list corresponds to item in Definitions list property
        /// Each Dictionary keeps track of last used ThemeDefinition configuration for a given Theme type (type must extend from InteractableThemeBase)
        /// </summary>
        public List<Dictionary<Type, ThemeDefinition>> History = new List<Dictionary<Type, ThemeDefinition>>();

        /// <summary>
        /// Helper function to convert States scriptableobject into an array of available State values 
        /// </summary>
        /// <returns>Array of available State values for currently assigned States property in this Theme</returns>
        [System.Obsolete("Use States.StateList instead")]
        public State[] GetStates()
        {
            if (States != null)
            {
                return States.StateList.ToArray();
            }

            return new State[0];
        }

        /// <summary>
        /// Validate list of ThemeDefinitions with current States object
        /// </summary>
        public void ValidateDefinitions()
        {
            if (Definitions != null && States != null)
            {
                int numOfStates = States.StateList.Count;
                foreach (var definition in Definitions)
                {
                    // For each theme property with values per possible state
                    // ensure the number of values matches the number of states
                    foreach (ThemeStateProperty p in definition.StateProperties)
                    {
                        if (p.Values.Count != numOfStates)
                        {
                            // Need to fill property with default values to match number of states
                            if (p.Values.Count < numOfStates)
                            {
                                for (int i = p.Values.Count - 1; i < numOfStates; i++)
                                {
                                    p.Values.Add(p.Default.Copy());
                                }
                            }
                            else
                            {
                                // Too many property values, remove to match number of states
                                for (int i = p.Values.Count - 1; i >= numOfStates; i--)
                                {
                                    p.Values.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
