// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Simple test data source that programmatically changes variables in a data source.
    /// </summary>
    /// <remarks>
    /// Using a simple <key,value> store, it's possible to separate data from view
    /// to simplify the integration of generic view prefabs that are populated from
    /// external information.
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Data Binding/Data Source Reflection Test")]
    public class DataSourceReflectionTest : DataSourceTest
    {
        private class StatusInfo
        {
            public string name;
            public Sprite icon;
        }

        /// <summary>
        /// A class to contain data that is to be used as a data source. This is akin to a view model
        /// that will be used to populate a view.
        /// </summary>
        private class TestInfo
        {
            public string firstname;
            public string lastname;
            public string stylesheet;
            public StatusInfo status = new StatusInfo();
            public int score;
        }

        private TestInfo _dataSourceObject = new TestInfo();

        /// <summary>
        /// IDataSourceProvider method used to provide the correct
        /// data source, which in this case is the DataSourceReflection instance with
        /// the specified _dataSourceObject attached to it to provide the actual
        /// data using reflection.
        /// </summary>
        public override IDataSource AllocateDataSource()
        {
            return new DataSourceReflection(_dataSourceObject);
        }
    }
}
