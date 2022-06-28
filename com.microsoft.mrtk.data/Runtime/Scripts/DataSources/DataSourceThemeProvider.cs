// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A helper class that simplifies providing a theme that is
    /// organized as a ScriptableObject. Simply place this higher
    /// in the scene hierarchy than any UX element that you wish
    /// to inherit this theme and it will be found automatically.
    ///
    /// To have one theme profile be used for all application theming
    /// as well as UXComponents themes, this can be accomplished by deriving from
    /// the UXComponentsThemeProfile, or by offering all the same keypaths as
    /// well as any keypaths unique to the application.
    ///
    /// If more comprehensive support for theme data sources is needed,
    /// consider using DataSourceProviderSingletion.
    /// </summary>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Sources/Data Source Theme Provider")]
    public class DataSourceThemeProvider : DataSourceGOBase
    {
        [Tooltip("A scriptable object that provides the theme to use for any specific UI theming needs. This can be unique to the application, or derived from or structured similarly to UXComponentsThemeProfile.")]
        [SerializeField]
        private ScriptableObject themeProfile;

        private DataSourceReflection _dataSourceReflection;

        /// <summary>
        /// Currently set theme profile.
        /// </summary>
        public ScriptableObject ThemeProfile
        {
            get
            {
                return themeProfile;
            }

            set
            {
                SetTheme(themeProfile);
            }
        }

        public override IDataSource AllocateDataSource()
        {
            _dataSourceReflection = new DataSourceReflection(themeProfile);
            return _dataSourceReflection;
        }

        protected override void InitializeDataSource()
        {
            if (DataSourceType == null || DataSourceType == "")
            {
                DataSourceType = "theme";
            }
        }

        public void SetTheme(ScriptableObject themeProfile)
        {
            if (_dataSourceReflection != null && themeProfile != this.themeProfile)
            {
                this.themeProfile = themeProfile;
                _dataSourceReflection.SetDataSourceObject(themeProfile);
                _dataSourceReflection.NotifyAllChanged();
            }
        }
    }
}
