// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// list of theme settings to virtualize theme values
    /// </summary>
    public struct ProfileSettings
    {
        public List<ThemeSettings> ThemeSettings;
    }

    /// <summary>
    /// List of settings that are copied from themes
    /// </summary>
    public struct ThemeSettings
    {
        public List<InteractableThemePropertySettings> Settings;
    }

    /// <summary>
    /// A way to cache some serializes values to pass between buttons and handlers
    /// </summary>
    [System.Serializable]
    public class ThemeTarget
    {
        public List<InteractableThemeProperty> Properties;
        public GameObject Target;
        public State[] States;
    }

    /// <summary>
    /// The main settings found in Themes
    /// </summary>
    [System.Serializable]
    public struct InteractableThemePropertySettings
    {
        public string Name;
        public string AssemblyQualifiedName;
        public Type Type;
        public InteractableThemeBase Theme;
        public List<InteractableThemeProperty> Properties;
        public List<InteractableThemeProperty> History;
        public Easing Easing;
        public bool NoEasing;
        public bool IsValid;
        public ThemeTarget ThemeTarget;
    }
}
