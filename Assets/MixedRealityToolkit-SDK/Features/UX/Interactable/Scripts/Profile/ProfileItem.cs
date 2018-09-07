using System;
using System.Collections;
using System.Collections.Generic;
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
            }

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
