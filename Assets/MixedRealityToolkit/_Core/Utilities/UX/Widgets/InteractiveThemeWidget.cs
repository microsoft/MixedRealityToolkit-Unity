// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.UX.Widgets
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
        public override void SetState(ButtonStateEnum state)
        {
            base.SetState(state);
            mThemeUpdated = false;
        }

        /// <summary>
        /// Find a InteractiveThemeColor by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public InteractiveThemeColor GetColorTheme(string tag)
        {
            // search locally
            InteractiveThemeColor[] colorThemes = InteractiveHost.GetComponentsInChildren<InteractiveThemeColor>();
            InteractiveThemeColor theme = FindColorTheme(colorThemes, tag);

            // search globally
            if (theme == null)
            {
                colorThemes = FindObjectsOfType<InteractiveThemeColor>();
                theme = FindColorTheme(colorThemes, tag);
            }

            if (!mThemeUpdated) mThemeUpdated = theme != null;

            return theme;
        }

        // compare theme tags
        private InteractiveThemeColor FindColorTheme(InteractiveThemeColor[] colorThemes, string tag)
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
        /// Find a InteractiveThemeVector3 by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public InteractiveThemeVector3 GetVector3Theme(string tag)
        {
            // search locally
            InteractiveThemeVector3[] vector3Themes = InteractiveHost.GetComponentsInChildren<InteractiveThemeVector3>();
            InteractiveThemeVector3 theme = FindVector3Theme(vector3Themes, tag);

            // search globally
            if (theme == null)
            {
                vector3Themes = FindObjectsOfType<InteractiveThemeVector3>();
                theme = FindVector3Theme(vector3Themes, tag);
            }

            if (!mThemeUpdated) mThemeUpdated = theme != null;

            return theme;
        }

        // compare theme tags
        public InteractiveThemeVector3 FindVector3Theme(InteractiveThemeVector3[] vector3Themes, string tag)
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
        /// Find a InteractiveThemeTexture by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public InteractiveThemeTexture GetTextureTheme(string tag)
        {
            // search locally
            InteractiveThemeTexture[] textureThemes = InteractiveHost.GetComponentsInChildren<InteractiveThemeTexture>();
            InteractiveThemeTexture theme = FindTextureTheme(textureThemes, tag);

            // search globally
            if (theme == null)
            {
                textureThemes = FindObjectsOfType<InteractiveThemeTexture>();
                theme = FindTextureTheme(textureThemes, tag);
            }

            if (!mThemeUpdated) mThemeUpdated = theme != null;

            return theme;
        }

        // compare theme tags
        public InteractiveThemeTexture FindTextureTheme(InteractiveThemeTexture[] textureThemes, string tag)
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
