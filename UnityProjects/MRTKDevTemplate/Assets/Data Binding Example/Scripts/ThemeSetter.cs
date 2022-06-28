// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [AddComponentMenu("MRTK/Examples/Data Binding/Theme Setter")]
    public class ThemeSetter : MonoBehaviour
    {
        [Tooltip("The data source to use for setting the current theme.")]
        [SerializeField]
        protected DataSourceGOBase dataSource;

        [Tooltip("A list of valid theme names.")]
        [SerializeField]
        protected string[] themeNames;

        [Tooltip("Keypath for variable to receive theme name.")]
        [SerializeField]
        protected string keyPathForTheme;

        [SerializeField]
        protected DataSourceThemeTest themes;

        protected int _currentTheme = 0;

        public void NextTheme()
        {
            if (++_currentTheme >= themeNames.Length)
            {
                _currentTheme = 0;
            }

            UpdateThemeName();
            themes.ChangeTheme(_currentTheme);
        }

        public void SetTheme(int themeIdx)
        {
            if (themeIdx >= themeNames.Length)
            {
                themeIdx = 0;
            }
            _currentTheme = themeIdx;

            UpdateThemeName();
            themes.ChangeTheme(_currentTheme);
        }

        public void UpdateThemeName()
        {
            if (themeNames.Length > 0)
            {
                dataSource.SetValue(keyPathForTheme, themeNames[_currentTheme]);
            }
        }
    }
}
