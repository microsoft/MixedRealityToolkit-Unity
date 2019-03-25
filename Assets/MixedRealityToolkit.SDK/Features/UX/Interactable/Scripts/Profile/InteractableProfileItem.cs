// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The foundation of profiles that exist on an Interactable
    /// Profiles pair themes with the objects they manipulate, based on state changes
    /// </summary>

    [System.Serializable]
    public class InteractableProfileItem
    {
        public GameObject Target;
        public List<Theme> Themes;
        public bool HadDefaultTheme;

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

        /// <summary>
        /// Get a new theme instance and load it with settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="host"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static InteractableThemeBase GetTheme(InteractableThemePropertySettings settings, GameObject host)
        {
            Type themeType = Type.GetType(settings.AssemblyQualifiedName);
            InteractableThemeBase theme = (InteractableThemeBase)Activator.CreateInstance(themeType);
            theme.Init(host, settings);
            return theme;
        }
    }
}
