// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [Serializable]
    [AddComponentMenu("MRTK/Examples/Data Binding/Data Source Theme Test")]
    public class DataSourceThemeTest : DataSourceGOBase
    {
        [SerializeField]
        protected ThemeProfile[] availableThemes;

        [SerializeField]
        protected int currentTheme;

        private int _lastTheme = -1;
        private int _lastThemeCount;

        protected override void InitializeDataSource()
        {
            if (string.IsNullOrWhiteSpace(dataSourceType))
            {
                dataSourceType = "theme";      // make default "theme" to differentiate from "data" data source types
            }

            _lastThemeCount = availableThemes.Length;
            ChangeTheme(currentTheme);
        }

        public void ChangeTheme(int themeIndex)
        {
            if (themeIndex >= 0 && themeIndex < availableThemes.Length)
            {
                currentTheme = themeIndex;

                DataSourceReflection dataSource = DataSource as DataSourceReflection;

                dataSource.SetDataSourceObject(availableThemes[currentTheme]);
                DataSource.NotifyAllChanged();
            }
        }

        public void NextTheme()
        {
            ChangeTheme((currentTheme + 1) % availableThemes.Length);
        }

        private void OnValidate()
        {
            ChangeTheme(currentTheme);
        }

        private void Update()
        {
            if (_lastTheme != currentTheme)
            {
                DataSource.NotifyAllChanged();
                _lastTheme = currentTheme;
            }

            if (_lastThemeCount == 0 && availableThemes.Length > 0)
            {
                ChangeTheme(0);
            }
        }

        public override IDataSource AllocateDataSource()
        {
            if (availableThemes.Length > 0)
            {
                return new DataSourceReflection(availableThemes[currentTheme]);
            }
            else
            {
                return null;
            }
        }
    }
}
