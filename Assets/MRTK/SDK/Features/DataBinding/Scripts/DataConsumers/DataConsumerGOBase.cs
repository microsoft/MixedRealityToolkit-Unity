﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

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


        protected Dictionary<string, string> _resolvedToLocalKeyPathLookup = new Dictionary<string, string>();
        protected IDataSource _dataSource;
        protected IDataController _dataController;

        protected const string _dataBindSpecifierBegin = @"{{";
        protected const string _dataBindSpecifierEnd = @"}}";
        protected Regex _variableRegex = new Regex(_dataBindSpecifierBegin + @"\s*([a-zA-Z0-9\[\]\-._]+)\s*" + _dataBindSpecifierEnd);
        protected bool _attached = false;
        protected bool _externalAttached = false;

        public IDataSource DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        public IDataController DataController
        {
            get { return _dataController; }
            set { _dataController = value; }
        }

        #region Abstract methods


        protected abstract void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType);


        #endregion Abstract methods



        /// <summary>
        /// Unity's OnEnable() method.
        /// </summary>
        /// 
        /// <remarks>
        /// Note that this should rarely be overridden but is declared virtual for circumnstances 
        /// where this is required. If overridden, make sure to call this default behavior.
        /// 
        /// Any initialization should be accomplished by overriding IniitalizeDataConsumer().
        /// </remarks>

        public virtual void OnEnable()
        {
            StartCoroutine(DelaySelfAttach());
            //SelfAttach();
        }

        /// <summary>
        /// TODO: Clean this up -- just for testing but need delay to allow Source to be enabled prior.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelaySelfAttach()
        {
           yield return new WaitForEndOfFrame();
           SelfAttach();
        }

        private void Awake()
        {
            InitializeDataConsumer();
        }

        public virtual void OnDisable()
        {
            Detach();
        }

        /// <summary>
        /// Unity's Start() method.
        /// </summary>
        /// 
        /// <remarks>
        /// Override InitializeDataConsumer for any needed one-time initialization.
        /// </remarks>

        private void Start()
        {
        }




        protected virtual void InitializeDataConsumer()
        {
            // override if any additional onetime initialization required such as setting delegates.
        }


        #region IDataConsumer interface methods

        /// <summary>
        /// Attach to same resources as an associated dataconsumer such as a DataConsumerCOllection
        /// </summary>
        /// <remarks>
        /// This is key to both first time initialization and being attached
        /// by another dataconsumer, typically that has created this one at run
        /// time and also for going back and forth between gameobject data pool and
        /// placement in a collection.
        /// 
        /// NOTE: When you override this, use AttachDataConsumer for additional
        /// attach tasks.
        /// </remarks>
        public void Attach(IDataSource dataSource, IDataController dataController, string resolvedKeyPathPrefix = null)
        {
            if (_attached)
            {
                if (resolvedKeyPathPrefix == null)
                {
                    // preserve through the Detach()
                    resolvedKeyPathPrefix = ResolvedKeyPathPrefix;
                }

                if (_externalAttached)
                {
                    DebugUtilities.LogVerbose("Already attached externally and attaching again before detaching.  Will automatically detach to correct.");
                }

                Detach();
            }

            if (!_attached)
            {
                _attached = true;
                _externalAttached = true; // Note: immediately disabled again in SelfAttach()

                DataSource = dataSource;
                DataController = dataController;

               if (resolvedKeyPathPrefix != null)
               {
                    ResolvedKeyPathPrefix = resolvedKeyPathPrefix;
               }

                AttachDataConsumer();
                FindVariablesToManage();
                DataSource.NotifyAllChanged( DataChangeType.DatumAdded );
            }
        }


        /// <summary>
        /// Self attach is used when this Component is not being attached by another
        /// DataConsumer as would happen when DataConsumerCollection is instantiating
        /// prefabs and adding them to its collection.
        /// </summary>

        protected void SelfAttach()
        {
            if (!_attached)
            {
                IDataSource dataSource = FindNearestDataSource(DataSource);
                IDataController dataController = null;
                if (dataSource != null)
                {
                    dataController = dataSource.DataController;
                }
                if (dataController == null)
                {
                    dataController = FindNearestDataController(DataController);
                }

                Attach(dataSource, dataController);
                _externalAttached = false;
            }
            else
            {
                if (_externalAttached)
                {
                    DebugUtilities.LogVerbose("Attempting to self attach when already externally attached.");

                }
                else
                {
                    DebugUtilities.LogVerbose("Attempting to self attach when already self attached.");
                }
            }
        }


        /// <summary>
        /// When object is disabled, detach from external resoources
        /// </summary>
        /// <remarks>
        /// Note: When you override this, use DetachDataConsumer for additional
        /// detach tasks.
        /// </remarks>
        public void Detach()
        {
            if (_attached)
            {
                _attached = false;
                _externalAttached = false;

                DetachDataConsumer();
                foreach (string resolvedKeyPath in _resolvedToLocalKeyPathLookup.Keys)
                {
                    _dataSource.RemoveDataConsumerListener(resolvedKeyPath, this as IDataConsumer);
                }

                _resolvedToLocalKeyPathLookup.Clear();
                ResolvedKeyPathPrefix = "";
                DataSource = null;
                DataController = null;
            }
            else
            {
                DebugUtilities.LogVerbose("Attempting to detach while not attached.");
            }

        }

        protected virtual void AttachDataConsumer()
        {
            // meant to be overridden
        }

        protected virtual void DetachDataConsumer()
        {

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
        /// Called by the associaed DataSource to report data changes.
        /// </summary>
        /// 
        /// <remarks>
        /// See NotifyDataChanged on the IDataConsumer interface for more detailed information.
        /// </remarks>

        public void NotifyDataChanged(IDataSource dataSource, string resolvedKeyPath, object value, DataChangeType dataChangeType)
        {
            if (_resolvedToLocalKeyPathLookup.ContainsKey(resolvedKeyPath))
            {
                ProcessDataChanged(dataSource, resolvedKeyPath, _resolvedToLocalKeyPathLookup[resolvedKeyPath], value, dataChangeType);
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

        #endregion IDataConsumer interface methods

        #region protected methods


        /// <summary>
        /// For consumers that manage one or more components for modifcation based on data received,
        /// this is a convenience method that is called once per component of the specified types
        /// declared in GetComponentTypes().
        /// 
        /// Note that if you do not need any components, 
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="component"></param>
        protected virtual void AddVariableKeyPathsForComponent(Type componentType, Component component)
        {
            //no default behavoir, but also not needed if not a component based binding
        }


        /// <summary>
        /// If your data consumer modifies components, particularly all components found of a
        /// certain type, then declaring them here will result in AddVariableKeyPathsForComponent
        /// to be called for each found component in this or is specified in child objects.
        /// 
        /// If you do not operate on components, then no need to override this method. Instead
        /// simply override InitializeDataConsumer for one-time initialization and 
        /// override AttachDataConsumer for any setup that should occur each time
        /// your class is enabled, in which you should call AddKeyPathListener() for any keypaths
        /// that you want the datasource to notify of any changes.
        /// </summary>
        /// <returns>List of Component types.</returns>
        protected virtual Type[] GetComponentTypes()
        {
            Type[] types = { };
            return types;
        }


        protected virtual bool ManageChildren()
        {
            return true;
        }

        /// <summary>
        /// Add a key path to the data source so that this object will be notified when it has changed.
        /// </summary>
        /// <param name="localKeyPath">Local key path prior to any key path mapping or resolving.</param>
        /// <returns></returns>
        protected string AddKeyPathListener(string localKeyPath)
        {

            if (_dataSource != null)
            {
                string resolvedKeyPath = _dataSource.ResolveKeyPath(ResolvedKeyPathPrefix, localKeyPath);

                if (resolvedKeyPath != null)
                {
                    _resolvedToLocalKeyPathLookup[resolvedKeyPath] = localKeyPath;
                    _dataSource.AddDataConsumerListener(resolvedKeyPath, this as IDataConsumer);
                }

                return resolvedKeyPath;
            }
            else
            {
                return null;
            }
        }




        /// <summary>
        /// Internal method that finds all managed omponents and registers relevant key paths for each of them.
        /// </summary>
        /// 
        protected void FindVariablesToManage()
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
        /// If no data source is provided directly, search through this object and its parents in game object 
        /// heirarchy for the data source to use with this data consumer.
        /// </summary>
        protected IDataSource FindNearestDataSource(IDataSource defaultDataSource = null)
        {
            if (defaultDataSource == null)
            {
                GameObject currentGO = gameObject;

                while (currentGO != null && defaultDataSource == null)
                {
                    Component[] dataSourceComponents = currentGO.GetComponents(typeof(IDataSourceProvider));
                    foreach (Component dataSourceComponent in dataSourceComponents)
                    {
                        if ((dataSourceComponent as MonoBehaviour).enabled)
                        {
                            IDataSourceProvider dataSourceProvider = dataSourceComponent as IDataSourceProvider;
                            defaultDataSource = dataSourceProvider.GetDataSource();
                            break;
                        }
                    }

                    if (currentGO.transform.parent != null)
                    {
                        currentGO = currentGO.transform.parent.gameObject;
                    }
                    else
                    {
                        currentGO = null;
                    }
                }
            }
            return defaultDataSource;
        }


        /// <summary>
        /// If no data source is provided directly, search through this object and its parents in game object 
        /// heirarchy for the data source to use with this data consumer.
        /// </summary>
        protected IDataController FindNearestDataController(IDataController defaultDataController = null)
        {
            if (defaultDataController == null)
            {
                GameObject currentGO = gameObject;

                while (currentGO != null && defaultDataController == null)
                {
                    Component[] dataControllerComponents = currentGO.GetComponents(typeof(IDataController));
                    foreach (Component dataControllerComponent in dataControllerComponents)
                    {
                        if ((dataControllerComponent as MonoBehaviour).enabled)
                        {
                            defaultDataController = dataControllerComponent as IDataController;
                            break;
                        }
                    }
                    if (currentGO.transform.parent != null)
                    {
                        currentGO = currentGO.transform.parent.gameObject;
                    }
                    else
                    {
                        currentGO = null;
                    }
                }
            }
            return defaultDataController;
        }



        /// <summary>
        /// Returns a pre-allocaed regex object to use for identifying variable key paths in textual strings.
        /// </summary>
        /// 
        /// <remarks>
        /// This is provided to reduce the number of identical regex objects when searching for data embedded variables.</remarks>
        /// <returns></returns>
        protected Regex GetVariableMatchingRegex()
        {
            return _variableRegex;
        }

        #endregion protected methods
    }
}
