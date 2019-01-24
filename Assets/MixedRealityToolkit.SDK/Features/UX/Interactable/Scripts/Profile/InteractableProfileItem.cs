// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.InspectorFields;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Profile
{
    /// <summary>
    /// The foundation of profiles that exist on an Interactable
    /// Profiles pair themes with the objects they manipulate, based on state changes
    /// </summary>
    
    [System.Serializable]
    public class InteractableProfileItem
    {
        [System.Serializable]
        public struct ThemeLists
        {
            public List<Type> Types;
            public List<String> Names;
        }

        public GameObject Target;
        public List<Theme> Themes;
        public bool HadDefaultTheme;
        
        /// <summary>
        /// Get a list of themes
        /// </summary>
        /// <returns></returns>
        public static ThemeLists GetThemeTypes()
        {
            List<Type> themeTypes = new List<Type>();
            List<string> names = new List<string>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    TypeInfo info = type.GetTypeInfo();
                    if (info.BaseType != null && (info.BaseType.Equals(typeof(InteractableThemeBase)) || info.BaseType.Equals(typeof(InteractableShaderTheme)) || info.BaseType.Equals(typeof(InteractableColorTheme))))
                    {
                        themeTypes.Add(type);
                        names.Add(type.Name);
                    }
                }
            }
            
            ThemeLists lists = new ThemeLists();
            lists.Types = themeTypes;
            lists.Names = names;
            return lists;
        }

        /// <summary>
        /// Get a new theme instance and load it with settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="host"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static InteractableThemeBase GetTheme(InteractableThemePropertySettings settings, GameObject host, ThemeLists lists)
        {
            int index = InspectorField.ReverseLookup(settings.Name, lists.Names.ToArray());
            Type themeType = lists.Types[index];
            InteractableThemeBase theme = (InteractableThemeBase)Activator.CreateInstance(themeType, host);
            theme.Init(host ,settings);
            return theme;
        }

    }
}
