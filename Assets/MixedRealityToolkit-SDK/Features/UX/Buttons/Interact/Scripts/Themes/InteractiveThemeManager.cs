// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Themes
{
    public class InteractiveThemeManager : MonoBehaviour
    {
        private static Dictionary<String, ColorInteractiveTheme> ColorThemes;
        private static Dictionary<String, Vector3InteractiveTheme> Vector3Themes;
        private static Dictionary<String, TextureInteractiveTheme> TextureThemes;
        private static Dictionary<String, QuaternionInteractiveTheme> QuaternionThemes;
        private static Dictionary<String, StringInteractiveTheme> StringThemes;
        private static Dictionary<String, BoolInteractiveTheme> BoolThemes;
        private static Dictionary<String, AudioInteractiveTheme> AudioThemes;
        private static Dictionary<String, FloatInteractiveTheme> FloatThemes;


        public static void AddColorTheme(ColorInteractiveTheme theme)
        {
            if (ColorThemes == null)
            {
                ColorThemes = new Dictionary<String, ColorInteractiveTheme>();
            }

            if (!ColorThemes.ContainsKey(theme.Tag))
            {
                ColorThemes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple Color themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static ColorInteractiveTheme GetColorTheme(string tag)
        {
            ColorInteractiveTheme theme = null;

            if (ColorThemes != null)
                ColorThemes.TryGetValue(tag, out theme);

            return theme;
        }
        public static void RemoveColorTheme(string tag)
        {
            if (ColorThemes != null)
                ColorThemes.Remove(tag);
        }

        public static void AddVecter3Theme(Vector3InteractiveTheme theme)
        {
            if (Vector3Themes == null)
            {
                Vector3Themes = new Dictionary<String, Vector3InteractiveTheme>();
            }

            if (!Vector3Themes.ContainsKey(theme.Tag))
            {
                Vector3Themes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple Vector3 themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static Vector3InteractiveTheme GetVector3Theme(string tag)
        {
            Vector3InteractiveTheme theme = null;

            if (Vector3Themes != null)
                Vector3Themes.TryGetValue(tag, out theme);

            return theme;
        }

        public static void RemoveVector3Theme(string tag)
        {
            if (Vector3Themes != null)
                Vector3Themes.Remove(tag);
        }

        public static void AddTextureTheme(TextureInteractiveTheme theme)
        {
            if (TextureThemes == null)
            {
                TextureThemes = new Dictionary<String, TextureInteractiveTheme>();
            }

            if (!TextureThemes.ContainsKey(theme.Tag))
            {
                TextureThemes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple Texture themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static TextureInteractiveTheme GetTextureTheme(string tag)
        {
            TextureInteractiveTheme theme = null;

            if (TextureThemes != null)
                TextureThemes.TryGetValue(tag, out theme);

            return theme;
        }

        public static void RemoveTextureTheme(string tag)
        {
            if (TextureThemes != null)
                TextureThemes.Remove(tag);
        }

        public static void AddQuaternionTheme(QuaternionInteractiveTheme theme)
        {
            if (QuaternionThemes == null)
            {
                QuaternionThemes = new Dictionary<String, QuaternionInteractiveTheme>();
            }

            if (!QuaternionThemes.ContainsKey(theme.Tag))
            {
                QuaternionThemes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple Quaternion themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static QuaternionInteractiveTheme GetQuaternionTheme(string tag)
        {
            QuaternionInteractiveTheme theme = null;

            if (QuaternionThemes != null)
                QuaternionThemes.TryGetValue(tag, out theme);

            return theme;
        }

        public static void RemoveQuaternionTheme(string tag)
        {
            if (QuaternionThemes != null)
                QuaternionThemes.Remove(tag);
        }

        public static void AddStringTheme(StringInteractiveTheme theme)
        {
            if (StringThemes == null)
            {
                StringThemes = new Dictionary<String, StringInteractiveTheme>();
            }

            if (!StringThemes.ContainsKey(theme.Tag))
            {
                StringThemes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple String themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static StringInteractiveTheme GetStringTheme(string tag)
        {
            StringInteractiveTheme theme = null;

            if (StringThemes != null)
                StringThemes.TryGetValue(tag, out theme);

            return theme;
        }

        public static void RemoveStringTheme(string tag)
        {
            if (StringThemes != null)
                StringThemes.Remove(tag);
        }

        public static void AddBoolTheme(BoolInteractiveTheme theme)
        {
            if (BoolThemes == null)
            {
                BoolThemes = new Dictionary<String, BoolInteractiveTheme>();
            }

            if (!BoolThemes.ContainsKey(theme.Tag))
            {
                BoolThemes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple Bool themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static BoolInteractiveTheme GetBoolTheme(string tag)
        {
            BoolInteractiveTheme theme = null;

            if (BoolThemes != null)
                BoolThemes.TryGetValue(tag, out theme);

            return theme;
        }

        public static void RemoveBoolTheme(string tag)
        {
            if (BoolThemes != null)
                BoolThemes.Remove(tag);
        }

        public static void AddAudioTheme(AudioInteractiveTheme theme)
        {
            if (AudioThemes == null)
            {
                AudioThemes = new Dictionary<String, AudioInteractiveTheme>();
            }

            if (!AudioThemes.ContainsKey(theme.Tag))
            {
                AudioThemes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple Audio themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static AudioInteractiveTheme GetAudioTheme(string tag)
        {
            AudioInteractiveTheme theme = null;

            if (AudioThemes != null)
                AudioThemes.TryGetValue(tag, out theme);

            return theme;
        }

        public static void RemoveAudioTheme(string tag)
        {
            if (AudioThemes != null)
                AudioThemes.Remove(tag);
        }

        public static void AddFloatTheme(FloatInteractiveTheme theme)
        {
            if (FloatThemes == null)
            {
                FloatThemes = new Dictionary<String, FloatInteractiveTheme>();
            }

            if (!FloatThemes.ContainsKey(theme.Tag))
            {
                FloatThemes.Add(theme.Tag, theme);
            }
            else
            {
                Debug.Log("Multiple Audio themes with the theme Tag: " + theme.tag + " exists, " + theme + "was not registered!");
            }
        }

        public static FloatInteractiveTheme GetFloatTheme(string tag)
        {
            FloatInteractiveTheme theme = null;

            if (FloatThemes != null)
                FloatThemes.TryGetValue(tag, out theme);

            return theme;
        }

        public static void RemoveFloatTheme(string tag)
        {
            if (FloatThemes != null)
                FloatThemes.Remove(tag);
        }
    }
}