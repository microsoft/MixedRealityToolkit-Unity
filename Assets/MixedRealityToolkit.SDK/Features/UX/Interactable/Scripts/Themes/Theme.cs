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
    [CreateAssetMenu(fileName = "Theme", menuName = "Mixed Reality Toolkit/Interactable/Theme", order = 1)]
    public class Theme : ScriptableObject
    {
        /// <summary>
        /// List of Theme Definition configurations. Each definition defines what type of Theme Engine to create and how to configure it
        /// </summary>
        [FormerlySerializedAs("Settings")]
        public List<ThemeDefinition> Definitions;

        /// <summary>
        /// Associated States object to use with this theme. Defines the states available for each Theme to utilize
        /// </summary>
        public States States;

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
        public State[] GetStates()
        {
            if (States != null)
            {
                return States.GetStates();
            }

            return new State[0];
        }
    }
}
