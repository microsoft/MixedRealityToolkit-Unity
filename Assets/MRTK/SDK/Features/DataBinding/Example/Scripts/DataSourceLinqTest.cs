// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A simple data source that fetches JSON data from a specified URL every N seconds.
    ///
    /// This will trigger notification changes by the base class once the data has been
    /// parsed.
    /// </summary>
    public class DataSourceLinqTest : DataSourceGOBase
    {

        protected override void InitializeDataSource()
        {
        }

        // Update is called once per frame
        void Update()
        {
          
        }
    }
}

