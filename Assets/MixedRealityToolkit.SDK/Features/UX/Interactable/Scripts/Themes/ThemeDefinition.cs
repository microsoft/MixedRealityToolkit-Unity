// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    // TODO: Troy -> Remove these containers* => Mark obsolete
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
        public List<ThemeDefinition> Settings;
    }

    /// <summary>
    /// A way to cache some serializes values to pass between buttons and handlers
    /// </summary>
    [System.Serializable]
    public class ThemeTarget
    {
        public List<ThemeStateProperty> Properties;
        public GameObject Target;
        public State[] States;
    }

    /// <summary>
    /// The main settings found in Themes
    /// </summary>
    [System.Serializable]
    public struct ThemeDefinition
    {
        [FormerlySerializedAs("Name")]
        public string ClassName;

        public string AssemblyQualifiedName;

        // TODO: Troy
        // NOT used????
        public Type Type;

        //public InteractableThemeBase Theme;

        /// <summary>
        /// Per state properties
        /// </summary>
        [FormerlySerializedAs("Properties")]
        public List<ThemeStateProperty> StateProperties;

        // TODO: Troy - Delete
        //public List<ThemeProperty> History;

        [FormerlySerializedAs("CustomSettings")]
        public List<ThemeProperty> CustomProperties;

        // TODO: Troy - Delete
        //public List<InteractableCustomSetting> CustomHistory;

        public Easing Easing;

        // TODO: Troy - Rename EasingNotSupported
        public bool NoEasing;

        // TODO: Troy - what is this?
        public bool IsValid;

        //public ThemeTarget ThemeTarget;
    }
}
