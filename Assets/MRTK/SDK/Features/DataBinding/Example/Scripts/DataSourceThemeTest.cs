// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [Serializable]
    public class DataSourceThemeTest : DataSourceGOBase
    {
        [SerializeField]
        protected ThemeProfile[] availableThemes;

        [SerializeField]
        protected int currentTheme;

        private int _lastTheme = -1;
        private int _lastThemeCount;

        public void Awake()
        {
            if (this.dataSourceType == null || this.dataSourceType == "")
            {
                this.dataSourceType = "theme";      // make default "theme" to differentiate from "data" data source types
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

        public void OnValidate()
        {
            ChangeTheme(currentTheme);
        }

        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeTheme(0);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeTheme(1);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeTheme(2);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.N))
            {
                ChangeTheme( (currentTheme + 1) % availableThemes.Length );
            }

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