// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Base class for Data Consumers that must derive from a Game Object MonoBehaviour. 
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// This class encapsulates as much of the basic logic that is needed to serve as a Data Consumer, 
    /// without getting into the specifics that might deviate based on the types of views being 
    /// managed.
    /// 
    /// Although this may change in future implementations, since an IDataConsumer does not currently 
    /// need to be discoverable by other game objects, this base class
    /// does not attempt to proxy a full IDataConsumer interface. It does however pass through many of the
    /// IDataConsumer methods to a non-Unity bsae class to reduce the mix of Unity and business logic.
    /// </remarks>

    [Serializable]
    public abstract class DataConsumerGOBase : MonoBehaviour, IDataConsumer
    {
        public string ResolvedKeyPathPrefix { get; set; } = "";


        internal Dictionary<string, string> _resolvedToLocalKeyPathLookup = new Dictionary<string, string>();
        internal IDataSource _dataSource;

        internal const string _dataBindSpecifierBegin = @"{{";
        internal const string _dataBindSpecifierEnd = @"}}";
        internal Regex _variableRegex = new Regex(_dataBindSpecifierBegin + @"\s*([a-zA-Z0-9\-_]+)\s*" + _dataBindSpecifierEnd);



        public IDataSource DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }


        public virtual void Attach( IDataSource dataSource, string resolvedKeyPathPrefix )
        {
            ResolvedKeyPathPrefix = resolvedKeyPathPrefix;
            DataSource = dataSource;

            InitializeDataConsumer();
            FindVariablesToManage();
        }


        public virtual void Detach()
        {
            foreach ( string resolvedKeyPath in _resolvedToLocalKeyPathLookup.Keys )
            {
                _dataSource.RemoveDataConsumerListener(resolvedKeyPath, this as IDataConsumer);
            }

            _resolvedToLocalKeyPathLookup.Clear();
            ResolvedKeyPathPrefix = "";
        }


        public virtual void DataChangeSetBegin(IDataSource dataSource)
        {
            // no default behavior. Override if needed.
        }



        public virtual void DataChangeSetEnd(IDataSource dataSource)
        {
            // no default behavior. Override if needed.
        }


        /// <summary>
        /// Unity's Awake() method.
        /// </summary>
        /// 
        /// <remarks>
        /// Note that this should rarely be overridden but is declared virtual for circumnstances 
        /// where this is required. If overridden, make sure to call this default behavior.
        /// </remarks>
        
        internal virtual void Awake()
        {
            FindNearestDataSource();
            InitializeDataConsumer();
        }


        /// <summary>
        /// Unity's Start() method.
        /// </summary>
        /// 
        /// <remarks>
        /// Note that this should rarely be overridden but is declared virtual for circumnstances 
        /// where this is required. If overridden, make sure to call this default behavior.
        /// </remarks>
      
        internal virtual void Start()
        {
            FindVariablesToManage();
        }


        internal abstract Type[] GetComponentTypes();

        internal abstract void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object newValue);


        /// <summary>
        /// Called by the associaed DataSource to report data changes.
        /// </summary>
        /// 
        /// <remarks>
        /// See NotifyDataChanged on the IDataConsumer interface for more detailed information.
        /// </remarks>
        
        public void NotifyDataChanged(IDataSource dataSource, string resolvedKeyPath, object newValue)
        {
            if (_resolvedToLocalKeyPathLookup.ContainsKey(resolvedKeyPath))
            {
                ProcessDataChanged(dataSource, resolvedKeyPath, _resolvedToLocalKeyPathLookup[resolvedKeyPath], newValue);
            }
        }

        internal abstract void AddVariableKeyPathsForComponent(Type componentType, Component component);


        internal virtual void InitializeDataConsumer()
        {
            // override if any additional onetime initialization required such as setting delegates.
        }


        internal virtual bool ManageChildren()
        {
            return true;
        }

        /// <summary>
        /// Add a key path to the data source so that this object will be notified when it has changed.
        /// </summary>
        /// <param name="localKeyPath">Local key path prior to any key path mapping or resolving.</param>
        /// <returns></returns>
        internal string AddKeyPath(string localKeyPath)
        {

            if (_dataSource != null)
            {
                string resolvedKeyPath = _dataSource.ResolveKeyPath(this.ResolvedKeyPathPrefix, localKeyPath);


                _resolvedToLocalKeyPathLookup[resolvedKeyPath] = localKeyPath;
                _dataSource.AddDataConsumerListener(resolvedKeyPath, this as IDataConsumer);
                return resolvedKeyPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get all registered keyPaths
        /// </summary>
        /// <remarks>
        /// Get all key paths this data consumer wishes to process. A key path is a specifier of a unique data item (of arbitrary type and complexity) in a data source.
        /// </remarks>
        ///
        /// <returns>IEnumerable for iterating through the returned string keyPaths.</returns>

        public IEnumerable<string> GetDataKeyPaths()
        {
            return _resolvedToLocalKeyPathLookup.Values;
        }


        /// <summary>
        /// Internal method that finds all managed omponents and registers relevant key paths for each of them.
        /// </summary>
        /// 
        internal void FindVariablesToManage()
        {
            Type[] componentTypesToScan = GetComponentTypes();
            Component[] componentsToScanForVariables;

            foreach (Type componentType in componentTypesToScan)
            {
                if (ManageChildren())
                {
                    componentsToScanForVariables = GetComponentsInChildren(componentType) as Component[];
                }
                else
                {
                    componentsToScanForVariables = GetComponents(componentType) as Component[];
                }

                foreach (Component component in componentsToScanForVariables)
                {
                    AddVariableKeyPathsForComponent(componentType, component);
                }
            }
        }


        /// <summary>
        /// Search through this object and its parents in game object heirarchy for the data source to use with this data consumer.
        /// </summary>
        internal void FindNearestDataSource()
        {
            if ( DataSource == null )
            {
                Component[] dataSourceComponents = GetComponentsInParent(typeof(IDataSource), false);
                foreach (Component dataSourceComponent in dataSourceComponents)
                {
                    // TODO: Should we add IsEnabled() to IDataSource to reduce dependency between this and DataSourceGOBase?
                    //       What if someone wants to implement a data source not derived from DataSourceGOBase?

                    DataSourceGOBase dataSource = dataSourceComponent as DataSourceGOBase;
                    if (dataSource.IsEnabled())
                    {
                        DataSource = dataSourceComponent as IDataSource;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Returns a pre-allocaed regex object to use for identifying variable key paths in textual strings.
        /// </summary>
        /// 
        /// <remarks>
        /// This is provided to reduce the number of identical regex objects when searching for data embedded variables.</remarks>
        /// <returns></returns>
        internal Regex GetVariableMatchingRegex()
        {
            return _variableRegex;
        }
    }
}
