using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    [System.Serializable]
    public struct ThemePropertySettings
    {
        public string Name;
        public Type Type;
        public ThemeBase Theme;
        public List<ThemeProperty> Properties;
        public List<ThemeProperty> History;
        public EaseSettings Easing;
        public bool IsValid;
    }

    public struct ProfileSettings
    {
        public List<ThemeSettings> ThemeSettings;
    }

    public struct ThemeSettings
    {
        public List<ThemePropertySettings> Settings;
    }

    [CreateAssetMenu(fileName = "Theme", menuName = "Interactable/Theme", order = 1)]
    public class Theme : ScriptableObject
    {
        public string Name;
        public List<ThemePropertySettings> Settings;
        public List<ThemePropertyValue> CustomSettings;
        public States States;

        public State[] GetStates()
        {
            if (States != null)
            {
                return States.GetStates();
            }

            return new State[0];
        }
    }
}
