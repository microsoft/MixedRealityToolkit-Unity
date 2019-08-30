// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The foundation of profiles that exist on an Interactable
    /// Profiles pair ThemeContainers with the objects they manipulate, based on state changes
    /// </summary>
    [System.Serializable]
    public class InteractableProfileItem
    {
        public GameObject Target;
        public List<Theme> Themes;
        public bool HadDefaultTheme;

        // TODO: Troy - Don't I just need InteractableThemeBase?
        /// <summary>
        /// The list of base classes whose derived classes will be included in interactable theme
        /// selection dropdowns.
        /// </summary>
        private static readonly List<Type> candidateThemeTypes = new List<Type>()
        {
            typeof(InteractableThemeBase),
            typeof(InteractableShaderTheme),
            typeof(InteractableColorTheme)
        };

        /// <summary>
        /// Get a list of themes
        /// </summary>
        /// <returns></returns>
        public static InteractableTypesContainer GetThemeTypes()
        {
            return InteractableTypeFinder.Find(candidateThemeTypes, TypeRestriction.DerivedOnly);
        }
    }
}
