// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A simple data source that fetches JSON data from a specified URL every N seconds.
    ///
    /// This will trigger notification changes by the base class once the data has been
    /// parsed.
    ///
    /// Note: The theme can be changed by pressing theme buttons at http://TryMRTK.com
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Data Binding/Data Source Json Theme Test")]
    public class DataSourceJsonThemeTest : DataSourceGOJson
    {
        [Tooltip("URL for a json data source")]
        [SerializeField]
        private string url = "http://TryMRTK.com/api/data";

        [Tooltip("How many seconds between fetching the data source and notifying all consumer of changes.")]
        [SerializeField]
        private float secondsBetweenFetches = 1.0f;

        [Tooltip("Which theme selector is used to update the actual theme.")]
        [SerializeField]
        private ThemeSelector themeSelector;

        protected float _time = 0.0f;
        private int lastTheme;

        protected override void InitializeDataSource()
        {
            _time = secondsBetweenFetches;
        }

        protected void JsonFetchSuccess(string jsonText, object requestRef)
        {
            int currentTheme = Convert.ToInt32(GetValue("theme"));
            if (currentTheme != lastTheme && themeSelector != null)
            {
                lastTheme = currentTheme;
                themeSelector.SetTheme(currentTheme);
            }
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if (_time >= secondsBetweenFetches)
            {
                _time -= secondsBetweenFetches;

                StartCoroutine(StartJsonRequest(url, JsonFetchSuccess));
            }
        }
    }
}
