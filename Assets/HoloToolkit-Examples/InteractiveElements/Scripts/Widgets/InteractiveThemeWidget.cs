// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// A version of InteractiveWidget that uses an InteractiveTheme to define each state
    /// </summary>
    public class InteractiveThemeWidget : InteractiveWidget
    {
        /// <summary>
        /// Find a ColorInteractiveTheme by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public ColorInteractiveTheme GetColorTheme(string tag)
        {
            // search locally
            ColorInteractiveTheme[] colorThemes = InteractiveHost.GetComponentsInChildren<ColorInteractiveTheme>();
            ColorInteractiveTheme theme = FindColorTheme(colorThemes, tag);

            // search globally
            if (theme == null)
            {
                colorThemes = FindObjectsOfType<ColorInteractiveTheme>();
                theme = FindColorTheme(colorThemes, tag);
            }

            return theme;
        }

        // compare theme tags
        private ColorInteractiveTheme FindColorTheme(ColorInteractiveTheme[] colorThemes, string tag)
        {
            for (int i = 0; i < colorThemes.Length; ++i)
            {
                if (colorThemes[i].Tag == tag)
                {
                    return colorThemes[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Find a Vector3InteractiveTheme by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Vector3InteractiveTheme GetVector3Theme(string tag)
        {
            // search locally
            Vector3InteractiveTheme[] vector3Themes = InteractiveHost.GetComponentsInChildren<Vector3InteractiveTheme>();
            Vector3InteractiveTheme theme = FindVector3Theme(vector3Themes, tag);

            // search globally
            if (theme == null)
            {
                vector3Themes = FindObjectsOfType<Vector3InteractiveTheme>();
                theme = FindVector3Theme(vector3Themes, tag);
            }

            return theme;
        }

        // compare theme tags
        public Vector3InteractiveTheme FindVector3Theme(Vector3InteractiveTheme[] vector3Themes, string tag)
        {
            for (int i = 0; i < vector3Themes.Length; ++i)
            {
                if (vector3Themes[i].Tag == tag)
                {
                    return vector3Themes[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Find a TextureInteractiveTheme by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public TextureInteractiveTheme GetTextureTheme(string tag)
        {
            // search locally
            TextureInteractiveTheme[] textureThemes = InteractiveHost.GetComponentsInChildren<TextureInteractiveTheme>();
            TextureInteractiveTheme theme = FindTextureTheme(textureThemes, tag);

            // search globally
            if (theme == null)
            {
                textureThemes = FindObjectsOfType<TextureInteractiveTheme>();
                theme = FindTextureTheme(textureThemes, tag);
            }

            return theme;
        }

        // compare theme tags
        public TextureInteractiveTheme FindTextureTheme(TextureInteractiveTheme[] textureThemes, string tag)
        {
            for (int i = 0; i < textureThemes.Length; ++i)
            {
                if (textureThemes[i].Tag == tag)
                {
                    return textureThemes[i];
                }
            }

            return null;
        }
    }
}
