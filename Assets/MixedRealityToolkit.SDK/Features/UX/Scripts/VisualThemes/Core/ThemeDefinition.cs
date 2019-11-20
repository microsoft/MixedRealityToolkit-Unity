// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Defines configuration properties and settings to use when initializing a class extending InteractableThemeBase
    /// </summary>
    [System.Serializable]
    public struct ThemeDefinition
    {
        /// <summary>
        /// Defines the type of Theme to associate with this definition. Type must be a class that extends InteractableThemeBase
        /// </summary>
        public Type ThemeType
        {
            get
            {
                if (Type == null)
                {
                    if (string.IsNullOrEmpty(AssemblyQualifiedName))
                    {
                        var className = ClassName;
                        // Temporary workaround
                        // This is to fix a bug in RC2.1 where the AssemblyQualifiedName was never actually saved.
                        var correctType = TypeCacheUtility.GetSubClasses<ReceiverBase>().Where(s => s?.Name == className);
                        if (!correctType.Any())
                        {
                            return null;
                        }

                        AssemblyQualifiedName = correctType.First().AssemblyQualifiedName;
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

        // Unity cannot serialize System.Type, thus must save AssemblyQualifiedName
        // Field here for Runtime use
        [NonSerialized]
        private Type Type;

        [FormerlySerializedAs("Name")]
        [SerializeField]
        private string ClassName;

        [SerializeField]
        private string AssemblyQualifiedName;

        [FormerlySerializedAs("Properties")]
        [FormerlySerializedAs("StateProperties")]
        [SerializeField]
        private List<ThemeStateProperty> stateProperties;
        /// <summary>
        /// List of properties with values defined per state index (Example list of colors for different states)
        /// </summary>
        public List<ThemeStateProperty> StateProperties
        {
            get { return stateProperties; }
            set { stateProperties = value; }
        }

        [FormerlySerializedAs("CustomSettings")]
        [FormerlySerializedAs("CustomProperties")]
        [SerializeField]
        private List<ThemeProperty> customProperties;
        /// <summary>
        /// List of single-value properties defined for the entire Theme engine regardless of the current state
        /// </summary>
        public List<ThemeProperty> CustomProperties
        {
            get { return customProperties; }
            set { customProperties = value; }
        }

        [FormerlySerializedAs("Easing")]
        [SerializeField]
        private Easing easing;
        /// <summary>
        /// Object to configure easing between values. Type of Theme Engine, as defined by the ThemeType property, must have IsEasingSupported set to true
        /// </summary>
        public Easing Easing
        {
            get { return easing; }
            set { easing = value; }
        }

        /// <summary>
        /// Utility function to generate the default ThemeDefinition configuration for the provided type of Theme engine
        /// </summary>
        /// <typeparam name="T">type of Theme Engine to build default configuration for</typeparam>
        /// <returns>Default ThemeDefinition configuration for the provided them type</returns>
        public static ThemeDefinition? GetDefaultThemeDefinition<T>() where T : InteractableThemeBase
        {
            return GetDefaultThemeDefinition(typeof(T));
        }

        /// <summary>
        /// Utility function to generate the default ThemeDefinition configuration for the provided type of Theme engine
        /// </summary>
        /// <param name="themeType">type of Theme Engine to build default configuration for</param>
        /// <returns>Default ThemeDefinition configuration for the provided them type</returns>
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
