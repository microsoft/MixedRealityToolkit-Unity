// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [AddComponentMenu("MRTK/Data Binding/Theme Selector")]
    public class ThemeSelector : MonoBehaviour
    {

        [Tooltip("A scriptable object that provides the theme to use for MRTK UX Controls")]
        [SerializeField, Experimental]
        private ScriptableObject[] themeProfiles;

        [Tooltip("The ThemeProvider instance to modify.")]
        [SerializeField]
        private DataSourceThemeProvider themeProvider;

        [Tooltip("The current theme.")]
        [SerializeField]
        private int currentTheme = 0;
        public int CurrentTheme
        {
            get
            {
                return currentTheme;
            }

            set
            {
                SetTheme(currentTheme);
            }
        }

        /// <summary>
        /// Set the theme to specified profile in the list of theme profiles.
        /// </summary>
        /// <param name="whichTheme">Index for theme to select and make currently active theme.</param>
        public void SetTheme(int whichTheme)
        {
            if (themeProvider != null && whichTheme < themeProfiles.Length)
            {
                currentTheme = whichTheme;
                themeProvider.SetTheme(themeProfiles[whichTheme]);
            }
        }

        private void OnStart()
        {
            SetTheme(currentTheme);
        }   

        /// <summary>
        /// A Unity Editor only event function that is called when the script is loaded or a value changes in the Unity Inspector.
        /// </summary>
        private void OnValidate()
        {
            if (CurrentTheme < 0)
            {
                CurrentTheme = 0;
            }
            if (CurrentTheme >= themeProfiles.Length)
            {
                CurrentTheme = themeProfiles.Length - 1;
            }
            SetTheme(currentTheme);
        }
    }
}
#pragma warning restore CS1591