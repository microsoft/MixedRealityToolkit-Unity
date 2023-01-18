// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Test DataController that logs a message event to the console
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Data Binding/Data Controller Test")]
    public class DataControllerTest : DataControllerGOBase
    {
        [Tooltip("A keypath for the data item that will be set to the message.")]
        [SerializeField]
        protected string keyPathToReceiveMessage = "dataControllerMessage";


        [Tooltip("A string format for constructing the desired output message.")]
        [SerializeField]
        protected string formatString = "DataController received command {0} for object {1}.";

        private IDataSource _dataSource;

        private void OnEnable()
        {
            GetNearestDataSource("data");
        }

        private void GetNearestDataSource(string dataSourceTypeToFind)
        {
            IDataSourceProvider[] components = GetComponentsInParent<IDataSourceProvider>();
            foreach (IDataSourceProvider providerToCheck in components)
            {
                string[] dataSourceTypes = providerToCheck.GetDataSourceTypes();
                foreach (string dataSourceTypeToCheck in dataSourceTypes)
                {
                    // find and add first occurrence of each unique DataSource data type like "data" or "theme"
                    if (dataSourceTypeToFind == dataSourceTypeToCheck)
                    {
                        _dataSource = providerToCheck.GetDataSource(dataSourceTypeToCheck);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Process the specified command with the specified bound datum and optional parameters.
        /// </summary>
        /// <param name="command">Which command to issue, as a string.</param>
        /// <param name="data">A data reference specifying the data to be acted upon, usually a unique identifier.</param>
        /// <param name="optionalParameters">Optional parameters, or null if none.</param>
        public override void ProcessCommand(string command, object data, Dictionary<string, object> optionalParameters = null)
        {
            if (_dataSource != null)
            {
                string message = string.Format(formatString, command, data.ToString());

                _dataSource.SetValue(keyPathToReceiveMessage, message, true);

                Debug.Log(message);
            }
        }
    }
}
