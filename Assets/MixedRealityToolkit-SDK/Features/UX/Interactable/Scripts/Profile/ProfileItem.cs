// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    [System.Serializable]
    public class ProfileItem
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

        public void OnUpdate()
        {

        }

        public static ThemeLists GetThemeTypes()
        {
            List<Type> themeTypes = new List<Type>();
            List<string> names = new List<string>();
            
            Assembly assembly = typeof(ThemeBase).GetTypeInfo().Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                TypeInfo info = type.GetTypeInfo();
                if (info.BaseType.Equals(typeof(ThemeBase)) || info.BaseType.Equals(typeof(ShaderTheme)) || info.BaseType.Equals(typeof(ColorTheme)))
                {
                    themeTypes.Add(type);
                    names.Add(type.Name);
                }
            }

            /* works with IL2CPP, but not with .NET
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(ThemeBase)))
                    {
                        themeTypes.Add(type);
                        names.Add(type.Name);
                    }
                }
            }*/

            ThemeLists lists = new ThemeLists();
            lists.Types = themeTypes;
            lists.Names = names;
            return lists;
        }

        public static ThemeBase GetTheme(ThemePropertySettings settings, GameObject host, ThemeLists lists)
        {
            int index = InteractableEvent.ReverseLookup(settings.Name, lists.Names.ToArray());
            Type themeType = lists.Types[index];
            ThemeBase theme = (ThemeBase)Activator.CreateInstance(themeType, host);
            theme.Init(host ,settings);
            return theme;
        }

    }
}
