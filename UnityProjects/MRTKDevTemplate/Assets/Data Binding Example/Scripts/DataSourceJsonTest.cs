// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A simple data source that fetches JSON data from a specified URL at a specified rate.
    /// </summary>
    /// <remarks>
    /// This will trigger notification changes by the base class once the data has been
    /// parsed.
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Data Binding/Data Source Json Test")]
    public class DataSourceJsonTest : DataSourceGOJson
    {
        [Tooltip("URL for a json data source")]
        [SerializeField]
        private string url = "https://official-joke-api.appspot.com/rando_joke";

        [Tooltip("How many seconds between fetching the data source and notifying all consumer of changes.")]
        [SerializeField]
        private float secondsBetweenFetches = 15.0f;

        protected float _time = 0.0f;

        protected override void InitializeDataSource()
        {
            _time = secondsBetweenFetches;
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            _time += Time.deltaTime;

            if (_time >= secondsBetweenFetches)
            {
                _time -= secondsBetweenFetches;

                StartCoroutine(StartJsonRequest(url));
            }
        }
    }
}
#pragma warning restore CS1591