// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{

    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Sources/Data Source Provider Singleton")]
    public class DataSourceProviderSingleton : MonoBehaviour, IDataSourceProvider
    {
        [Tooltip("Array of data sources that this provider can provide.")]
        [SerializeField, Experimental]
        private DataSourceGOBase[] dataSources;

        // Check to see if we're about to be destroyed.
        private static bool _shuttingDown = false;
        protected static IDataSourceProvider _instance;
        private Dictionary<string, IDataSource> _typeToDataSourceLookup;
        private string[] _dataSourceTypes;

        /// <summary>
        /// Get a datasource for a specific data source type
        /// </summary>
        /// <param name="dataSourceType">Type name assign to desired data source, typically "data" or "theme", but can be any name chosen for that data source.</param>
        /// <returns>Interface to the desired data source.</returns>
        public IDataSource GetDataSource(string dataSourceType)
        {
            _typeToDataSourceLookup.TryGetValue(dataSourceType, out IDataSource dataSourceOut);
            return dataSourceOut;
        }

        /// <summary>
        /// Get an array of data source types that can be provided by this provider
        /// </summary>
        /// <returns>Array of dataSourceType strings</returns>
        public string[] GetDataSourceTypes()
        {
            // Only build this array if it's requested.
            if (_dataSourceTypes == null)
            {
                _dataSourceTypes = new string[_typeToDataSourceLookup.Keys.Count];
                _typeToDataSourceLookup.Keys.CopyTo(_dataSourceTypes, 0);
            }

            return _dataSourceTypes;
        }

        private void InitializeSingleton()
        {
            _dataSourceTypes = null;

            if (_typeToDataSourceLookup == null)
            {
                _typeToDataSourceLookup = new Dictionary<string, IDataSource>();
            }
            else
            {
                _typeToDataSourceLookup.Clear();
            }

            if (dataSources != null)
            {
                foreach (IDataSource dataSource in dataSources)
                {
                    _typeToDataSourceLookup[dataSource.DataSourceType] = dataSource;
                }
            }

            _instance = this as IDataSourceProvider;
            _shuttingDown = false;
        }

        /// <summary>
        /// Indicates if the singleton has been initialized and ready to go
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                if (_shuttingDown)
                {
                    return false;
                }

                return _instance != null;
            }
        }

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static IDataSourceProvider Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    return null;
                }

                return _instance;
            }
        }

        private void OnValidate()
        {
            InitializeSingleton();
        }

        private void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void OnDestroy()
        {
            _shuttingDown = true;
            _instance = null;
        }
    }
}
