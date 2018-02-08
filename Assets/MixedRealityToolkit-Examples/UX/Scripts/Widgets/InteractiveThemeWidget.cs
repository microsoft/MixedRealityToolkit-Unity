// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.UX.Themes;

namespace MixedRealityToolkit.Examples.UX.Widgets
{
    /// <summary>
    /// A version of InteractiveWidget that uses an InteractiveTheme to define each state
    /// </summary>
    public abstract class InteractiveThemeWidget : InteractiveWidget
    {
        // checks if the theme has changed since the last SetState was called.
        protected bool mThemeUpdated;

        /// <summary>
        /// Sets the themes based on the Theme Tags
        /// </summary>
        public abstract void SetTheme();

        /// <summary>
        /// If the themes have changed since the last SetState was called, update the widget
        /// </summary>
        public void RefreshIfNeeded()
        {
            if (mThemeUpdated)
            {
                SetState(State);
            }
        }

        /// <summary>
        /// Sets the state of the widget
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            mThemeUpdated = false;
        }

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

            if (!mThemeUpdated) mThemeUpdated = theme != null;

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

            if (!mThemeUpdated) mThemeUpdated = theme != null;

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

            if (!mThemeUpdated) mThemeUpdated = theme != null;

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
