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
        public Type ThemeType
        {
            get
            {
                if (Type == null)
                {
                    if (string.IsNullOrEmpty(AssemblyQualifiedName))
                    {
                        return null;
                    }

                    Type = Type.GetType(AssemblyQualifiedName);
                }

                return Type;
            }
            set
            {
                if (!value.IsSubclassOf(typeof(InteractableThemeBase)))
                {
                    Debug.LogWarning($"Cannot assign type {value} that does not extend {typeof(InteractableThemeBase)} to ThemeDefinition");
                    return;
                }

                if (Type != value)
                {
                    Type = value;
                    ClassName = Type.Name;
                    AssemblyQualifiedName = Type.AssemblyQualifiedName;
                }
            }
        }

        [SerializeField]
        private Type Type;

        // Outdated variables. Kept only for temporary backward compatibility
        [SerializeField]
        [FormerlySerializedAs("Name")]
        private string ClassName;

        [SerializeField]
        private string AssemblyQualifiedName;

        /// <summary>
        /// Per state properties
        /// </summary>
        [FormerlySerializedAs("Properties")]
        public List<ThemeStateProperty> StateProperties;

        [FormerlySerializedAs("CustomSettings")]
        public List<ThemeProperty> CustomProperties;

        public Easing Easing;

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
