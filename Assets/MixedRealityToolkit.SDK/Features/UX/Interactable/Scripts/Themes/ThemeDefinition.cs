// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The main settings found in Themes
    /// </summary>
    [System.Serializable]
    public struct ThemeDefinition
    {
        [FormerlySerializedAs("Name")]
        public string ClassName;

        public string AssemblyQualifiedName;

        public Type Type;

        //public InteractableThemeBase Theme;

        /// <summary>
        /// Per state properties
        /// </summary>
        [FormerlySerializedAs("Properties")]
        public List<ThemeStateProperty> StateProperties;

        [FormerlySerializedAs("CustomSettings")]
        public List<ThemeProperty> CustomProperties;

        public Easing Easing;

        //public ThemeTarget ThemeTarget;

        // TODO: Troy - Comments
        public static ThemeDefinition? GetDefaultThemeDefinition<T>()
        {
            return GetDefaultThemeDefinition(typeof(T));
        }

        public static ThemeDefinition? GetDefaultThemeDefinition(Type themeType)
        {
            var theme = InteractableThemeBase.CreateTheme(themeType);
            if (theme != null)
            {
                return theme.GetDefaultThemeDefinition();
            }

            return null;
        }
    }
}
