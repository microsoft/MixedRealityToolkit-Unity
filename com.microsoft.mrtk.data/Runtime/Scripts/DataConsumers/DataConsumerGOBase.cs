// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Base class for Data Consumers that must derive from a Game Object MonoBehaviour.
    /// </summary>
    ///
    /// <remarks>
    /// This class encapsulates as much of the basic logic that is needed to serve as a Data Consumer,
    /// without getting into the specifics that might deviate based on the types of views being
    /// managed.
    ///
    /// Although this may change in future implementations, since an IDataConsumer does not currently
    /// need to be discoverable by other game objects, this base class
    /// does not attempt to proxy a full IDataConsumer interface. It does however pass through many of the
    /// IDataConsumer methods to a non-Unity base class to reduce the mix of Unity and business logic.
    /// </remarks>
    [Serializable]
    public abstract class DataConsumerGOBase : MonoBehaviour, IDataConsumer
    {
        [Tooltip("Data source types used to identify an appropriate data source for automatic attachment. If no type is provided, the first data source found in parents will be used.")]
        [SerializeField, Experimental]
        private string[] dataSourceTypes;
        /// <summary>
        /// Data source types used to identify an appropriate data source for
        /// automatic attachment. If no type is provided, the first data source
        /// found in parents will be used.
        /// </summary>
        public string[] DataSourceTypes
        {
            get
            {
                return dataSourceTypes;
            }
            set
            {
                dataSourceTypes = value;
                if (IsAttached())
                {
                    Detach();
                    SelfAttach();
                }
            }
        }

        [Tooltip("For items in collections, it can be useful to ignore the prefix prepended to local keypaths, such as when it is used to look up theme data in a separate DataSource.")]
        [SerializeField]
        /// <summary>
        /// For items in collections, it can be useful to ignore the prefix
        /// prepended to local keypaths, such as when it is used to look up
        /// theme data in a separate DataSource.
        /// </summary>
        protected bool ignoreKeyPathPrefix = false;

        [Tooltip("Specifies if this data consumer should include inactive components when managing components in child game objects.")]
        [SerializeField]
        private bool includeInactiveComponentsInChildren = false;

        [Tooltip("If True, indicates the hierarchy and managed components in this object will not change. This will use the cached types and component references to scan for after they are attained a first time. This avoids an extensive amount of GetComponent<> in each Attach(), which is super expensive especially for DataConsumerText.")]
        [SerializeField]
        private bool isFixedHierarchyWillUseCachedValues = true;

        /// <summary>
        /// If True, indicates the hierarchy and managed components in this
        /// object will not change. This will use the cached types and
        /// component references to scan for after they are attained a first
        /// time. This avoids an extensive amount of GetComponent<> in each
        /// Attach(), which is super expensive especially for DataConsumerText.
        /// </summary>
        public bool IsFixedHierarchyWillUseCachedValues => isFixedHierarchyWillUseCachedValues;

        protected Dictionary<string, IDataSource> _dataSourcesByType = null;

        public string ResolvedKeyPathPrefix { get; set; } = "";

        [Tooltip("If True, will ensure that the attachment happens only once, and will not be detached. This is useful to optimize items that will never be recycled. This optimization is disabled automatically for collections.")]
        [SerializeField]
        private bool autoAttachAndDetach = true;
        /// <summary>
        /// If True, will ensure that the attachment happens only once, and
        /// will not be detached. This is useful to optimize items that will
        /// never be recycled. This optimization is disabled automatically for
        /// collections.
        /// </summary>
        public bool AutoAttachAndDetach
        {
            get => autoAttachAndDetach;
            set
            {
                if (autoAttachAndDetach == value) { return; }
                autoAttachAndDetach = value;
                if (autoAttachAndDetach == false) { return; }

                if (enabled && !_attached)
                {
                    StartCoroutine(DelaySelfAttach());
                }
                else if (!enabled && _attached)
                {
                    Detach();
                }
            }
        }

        protected readonly struct KeyPathInfo
        {
            public string LocalKeyPath { get; }
            public IDataSource DataSource { get; }

            public KeyPathInfo(string localKeyPath, IDataSource dataSource)
            {
                LocalKeyPath = localKeyPath;
                DataSource = dataSource;
            }
        }

        protected Dictionary<string, KeyPathInfo> _resolvedToLocalKeyPathLookup = new Dictionary<string, KeyPathInfo>();
        protected IDataController _dataController;

        protected bool _attached = false;
        protected bool _externalAttached = false;

        protected bool _isQuitting = false;
        protected HashSet<Component> _componentsToManage = new HashSet<Component>();

        /// <summary>
        /// Note that runtime changes to this will not be reflected in the serialized data for this Component.
        /// </summary>
        public Dictionary<string, IDataSource> DataSources
        {
            get { return _dataSourcesByType; }
            private set { _dataSourcesByType = value; }
        }

        public IDataController DataController
        {
            get { return _dataController; }
            set { _dataController = value; }
        }

        #region Abstract methods

        protected abstract void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType);

        #endregion Abstract methods

        #region Unity MonoBehaviour methods

        /// <summary>
        /// Unity's OnEnable() method.
        /// </summary>
        ///
        /// <remarks>
        /// Note that this should rarely be overridden but is declared virtual for circumstances
        /// where this is required. If overridden, make sure to call this default behavior.
        ///
        /// Any initialization should be accomplished by overriding IniitalizeDataConsumer().
        /// </remarks>
        public virtual void OnEnable()
        {
            if (!_attached && autoAttachAndDetach)
            {
                StartCoroutine(DelaySelfAttach());
            }
        }

        private void Awake()
        {
            InitializeDataConsumer();
        }

        protected virtual void OnDisable()
        {
            if (IsAttached() && autoAttachAndDetach)
            {
                Detach();
            }
        }

        #endregion

        #region IDataConsumer interface methods

        /// <inheritdoc/>
        public void Attach(Dictionary<string, IDataSource> dataSources, IDataController dataController, string resolvedKeyPathPrefix = null)
        {
            if (_attached)
            {
                CheckAndUpdateAttach(dataSources, dataController, resolvedKeyPathPrefix);
            }
            else
            {
                AttachAllResources(dataSources, dataController, resolvedKeyPathPrefix);
            }
        }

        /// <inheritdoc/>
        public bool IsAttached()
        {
            return _attached;
        }

        /// <inheritdoc/>
        public void Detach()
        {
            if (_attached)
            {
                _attached = false;
                _externalAttached = false;

                DetachDataConsumer();

                foreach (KeyValuePair<string, KeyPathInfo> kv in _resolvedToLocalKeyPathLookup)
                {
                    string resolvedKeyPath = kv.Key;
                    IDataSource dataSource = kv.Value.DataSource;

                    dataSource.RemoveDataConsumerListener(resolvedKeyPath, this as IDataConsumer);
                }

                _resolvedToLocalKeyPathLookup.Clear();
                ResolvedKeyPathPrefix = "";
                DataSources.Clear();
                DataController = null;
                _componentsToManage.Clear();
            }
            else if (!_isQuitting)
            {
                Debug.LogWarning("Attempting to detach while not attached.");
            }
        }


        /// <inheritdoc/>
        public virtual void DataChangeSetBegin(IDataSource dataSource)
        {
            // no default behavior. Override if needed.
        }


        /// <inheritdoc/>
        public virtual void DataChangeSetEnd(IDataSource dataSource)
        {
            // no default behavior. Override if needed.
        }

        /// <inheritdoc/>
        public void NotifyDataChanged(IDataSource dataSource, string resolvedKeyPath, object value, DataChangeType dataChangeType)
        {
            if (_resolvedToLocalKeyPathLookup.ContainsKey(resolvedKeyPath))
            {
                KeyPathInfo keyPathInfo = _resolvedToLocalKeyPathLookup[resolvedKeyPath];

                ProcessDataChanged(dataSource, resolvedKeyPath, keyPathInfo.LocalKeyPath, value, dataChangeType);
            }
        }


        #endregion IDataConsumer interface methods


        #region Methods intended to be overridden

        /// <summary>
        /// Perform any one time initialization here that is specific to this data consumer.
        /// </summary>
        protected virtual void InitializeDataConsumer()
        {
            // override if any additional onetime initialization required such as setting delegates.
        }

        /// <summary>
        /// Perform any attach logic here that is specific to this data consumer.
        /// </summary>
        /// <remarks>
        /// This usually is a place to allocate resources that are needed while attached but can otherwise
        /// be discarded to save resources when not attached.
        /// </remarks>
        protected virtual void AttachDataConsumer()
        {
            // meant to be overridden
        }

        /// <summary>
        /// Perform any detach logic here that is specific to this data consumer.
        /// </summary>
        /// <remarks>
        /// This usually frees up any resources that were established in AttachDataConsumer.
        /// </remarks>
        protected virtual void DetachDataConsumer()
        {
        }

        /// <summary>
        /// For consumers that manage one or more components for modification based on data received,
        /// this is a convenience method that is called once per component of the specified types
        /// declared in GetComponentTypes().
        ///
        /// Note that if you do not manage specific components, no need to override
        /// this method. Instead, register keypath listeners via a different
        /// method specified to the need.
        /// </summary>
        /// <param name="component">The found Component to process for keypaths.</param>
        protected virtual void AddVariableKeyPathsForComponent(Component component)
        {
            // No default behavior, but also not needed if not a component based binding
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
            Type[] types = Array.Empty<Type>();
            return types;
        }

        /// <summary>
        /// Report whether this data consumer should manage components on child game objects
        /// </summary>
        /// <returns>True if components or other resources on child game objects should be managed.</returns>
        protected virtual bool ManageChildren()
        {
            return true;
        }


        /// <summary>
        /// Allows for using GetComponentInChildren with the optional second parameter, if non-active components should be considered.
        /// Defaults to only consider ACTIVE components -- Should be overridden if this behavior is not desired by data consumers.
        /// </summary>
        protected virtual bool ShouldIncludeInactiveComponentsInChildren()
        {
            return includeInactiveComponentsInChildren;
        }


        #endregion

        #region Protected and private methods


        protected void CheckAndUpdateAttach(Dictionary<string, IDataSource> dataSources, IDataController dataController, string resolvedKeyPathPrefix = null)
        {
            if (_attached)
            {
                // These are checked in most efficient to least efficient order for efficiency
                if (!IsSameDataController(dataController) || !IsSameKeyPathPrefix(resolvedKeyPathPrefix) || !IsSameDataSources(dataSources))
                {
                    // things have changed, so we should play it safe by detaching and then re-attaching

                    if (_externalAttached)
                    {
                        Debug.LogWarning("Already attached externally and attaching again before detaching with different data.  Will automatically detach to correct. KeyPath: " + resolvedKeyPathPrefix);
                    }

                    // if no new prefix provided, preserve any existing prefix through the Detach()
                    resolvedKeyPathPrefix ??= ResolvedKeyPathPrefix;

                    Detach();
                    AttachAllResources(dataSources, dataController, resolvedKeyPathPrefix);
                }
            }
        }

        protected bool IsSameKeyPathPrefix(string keyPathPrefix)
        {
            return System.Object.ReferenceEquals(keyPathPrefix, ResolvedKeyPathPrefix) ||
                    (keyPathPrefix != null && ResolvedKeyPathPrefix != null && keyPathPrefix.Equals(ResolvedKeyPathPrefix));
        }

        protected bool IsSameDataController(IDataController dataController)
        {
            return System.Object.ReferenceEquals(dataController, DataController);
        }

        protected bool IsSameDataSources(Dictionary<string, IDataSource> dataSources)
        {
            if (DataSources.Count != DataSources.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, IDataSource> kv in dataSources)
            {
                if (!DataSources.TryGetValue(kv.Key, out IDataSource dataSourceToTry))
                {
                    if (!System.Object.ReferenceEquals(dataSourceToTry, kv.Value))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        protected void AttachAllResources(Dictionary<string, IDataSource> dataSources, IDataController dataController, string resolvedKeyPathPrefix = null)
        {
            _attached = true;
            _externalAttached = true; // Note: if this is a self attach case, this is immediately disabled again in SelfAttach()

            if (resolvedKeyPathPrefix != null && !ignoreKeyPathPrefix)
            {
                ResolvedKeyPathPrefix = resolvedKeyPathPrefix;
            }

            AttachDataSources(dataSources);
            DataController = dataController;

            AttachDataConsumer();
            AddKeyPaths(FindComponentsToManage());

            NotifyThisConsumerKeypathsAdded();
        }

        protected void AttachDataSources(Dictionary<string, IDataSource> dataSources)
        {
            DataSources ??= new Dictionary<string, IDataSource>();

            if (dataSourceTypes != null && dataSourceTypes.Length > 0)
            {
                foreach (string dataSourceType in dataSourceTypes)
                {
                    if (dataSourceTypes.Length == 0 || dataSources.ContainsKey(dataSourceType))
                    {
                        DataSources[dataSourceType] = dataSources[dataSourceType];
                    }
                }
            }
            else if (!ReferenceEquals(dataSources, DataSources))
            {
                foreach (KeyValuePair<string, IDataSource> kv in dataSources)
                {
                    DataSources[kv.Key] = kv.Value;
                }
            }
        }

        protected void NotifyThisConsumerKeypathsAdded()
        {
            foreach (IDataSource dataSource in DataSources.Values)
            {
                dataSource.DataChangeSetBegin();
            }

            foreach (KeyValuePair<string, KeyPathInfo> kv in _resolvedToLocalKeyPathLookup)
            {
                string resolvedKeyPath = kv.Key;
                IDataSource dataSource = kv.Value.DataSource;
                if (dataSource.IsDataAvailable())
                {
                    dataSource.NotifyDataChanged(resolvedKeyPath, dataSource.GetValue(resolvedKeyPath), DataChangeType.DatumAdded, false);
                }
            }

            foreach (IDataSource dataSource in DataSources.Values)
            {
                dataSource.DataChangeSetEnd();
            }
        }

        protected virtual IDataSource GetBestDataSourceForKeyPath(string resolvedKeyPathPrefix, string localKeyPath, out string fullyResolvedKeyPath)
        {
            IDataSource firstDataSource = null;

            fullyResolvedKeyPath = null;
            foreach (IDataSource dataSource in DataSources.Values)
            {
                if (firstDataSource == null)
                {
                    firstDataSource = dataSource;
                }

                // TODO: Refactor this for a better way to differentiate between "data" data sources that may need a
                // keypath prefix, and "theme" data sources that probably don't need a keypath prefix.  ADO #1939

                if (dataSource.DataSourceType == "data")
                {
                    fullyResolvedKeyPath = dataSource.ResolveKeyPath(resolvedKeyPathPrefix, localKeyPath);
                }
                else
                {
                    fullyResolvedKeyPath = localKeyPath;
                }
                try
                {
                    if (dataSource.GetValue(fullyResolvedKeyPath) != null)
                    {
                        return dataSource;
                    }
                }
                catch
                {
                    // exception means resolved keypath was invalid. So not likely a good data source.
                }
            }

            return firstDataSource;
        }

        protected virtual IDataSource GetBestDataSource(string dataTypeHint = "data")
        {
            if (DataSources != null)
            {
                if (dataTypeHint != null && DataSources.ContainsKey(dataTypeHint))
                {
                    return DataSources[dataTypeHint];
                }

                // Is there one of the specified data type strings?

                if (dataSourceTypes.Length > 0)
                {
                    foreach (string dataType in dataSourceTypes)
                    {
                        if (DataSources.ContainsKey(dataType))
                        {
                            return DataSources[dataType];
                        }
                    }
                }

                foreach (IDataSource dataSource in DataSources.Values)
                {
                    if (dataSource != null)
                    {
                        return dataSource;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Add a key path to the data source so that this object will be notified when it has changed.
        /// </summary>
        /// <param name="localKeyPath">Local key path prior to any key path mapping or resolving.</param>
        /// <returns>The fully resolved keypath for the specified local keypath.</returns>
        protected string AddKeyPathListener(string localKeyPath, IDataSource specificDataSource = null)
        {
            string fullyResolvedKeyPath = null;

            if (specificDataSource == null)
            {
                specificDataSource = GetBestDataSourceForKeyPath(ResolvedKeyPathPrefix, localKeyPath, out fullyResolvedKeyPath);
            }

            if (specificDataSource != null)
            {
                fullyResolvedKeyPath ??= specificDataSource.ResolveKeyPath(ResolvedKeyPathPrefix, localKeyPath);

                if (fullyResolvedKeyPath != null)
                {
                    _resolvedToLocalKeyPathLookup[fullyResolvedKeyPath] = new KeyPathInfo(localKeyPath, specificDataSource);
                    specificDataSource.AddDataConsumerListener(fullyResolvedKeyPath, this as IDataConsumer);
                    return fullyResolvedKeyPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Populate the caches of components that are being managed by this DataConsumer, based on the GetComponentTypes() provided.
        /// Separation of this method from the FindVariablesToManage() allows for calling this in Awake before the data sources may be present/ready if desired.
        /// </summary>
        protected HashSet<Component> FindComponentsToManage()
        {
            // For perf, reuse a common set so allocs only occur first time during initialization.
            _componentsToManage ??= new HashSet<Component>();

            if (_componentsToManage.Count == 0)
            {
                Type[] componentTypesToScan = GetComponentTypes();

                foreach (Type componentType in componentTypesToScan)
                {
                    if (ManageChildren())
                    {
                        FindUnmanagedComponentsInChildren(gameObject, componentType);
                    }
                    else
                    {
                        FindComponentsOnGameObject(gameObject, componentType);
                    }
                }
            }

            return _componentsToManage;
        }

        protected void FindComponentsOnGameObject(GameObject gameObject, Type componentType)
        {
            Component[] componentsToScanForVariables = gameObject.GetComponents(componentType) as Component[];
            foreach (Component component in componentsToScanForVariables)
            {
                if (ComponentMeetsAllQualifications(component))
                {
                    _componentsToManage.Add(component);
                }
            }
        }

        protected void FindUnmanagedComponentsInChildren(GameObject currentGameObject, Type componentTypeToManage)
        {
            FindComponentsOnGameObject(currentGameObject, componentTypeToManage);

            if (currentGameObject.transform.childCount > 0)
            {
                foreach (Transform childTransform in currentGameObject.transform)
                {
                    GameObject childGameObject = childTransform.gameObject;
                    // Only check deeper on this branch if this type of DataConsumer is not already found on this game object.
                    Component thisDataConsumerType = childGameObject.GetComponent(GetType());
                    if (thisDataConsumerType == null)
                    {
                        FindUnmanagedComponentsInChildren(childGameObject, componentTypeToManage);
                    }
                }
            }
        }

        /// <summary>
        /// Once components have been identified, add keypaths for them.
        /// </summary>
        /// <remarks>
        /// If there is a 1:1 or 1:many relationship between keypaths
        /// and components, the default behavior breaks down the work for you.
        ///
        /// However, if there is a many:many relationship, then overriding this
        /// method makes more sense to avoid creating multiple listeners per keypath.
        /// </remarks>
        /// <param name="componentsToManage">The found components to be managed.</param>
        protected virtual void AddKeyPaths(HashSet<Component> componentsToManage)
        {
            AddVariableKeyPathsForAllComponents(componentsToManage);
        }

        protected void AddVariableKeyPathsForAllComponents(ISet<Component> components)
        {
            foreach (Component component in components)
            {
                AddVariableKeyPathsForComponent(component);
            }
        }

        protected virtual bool ComponentMeetsAllQualifications(Component component)
        {
            return true;
        }

        /// <summary>
        /// If no data source is provided directly, search through this object and its parents in game object
        /// hierarchy for the data source to use with this data consumer.
        /// </summary>
        protected Dictionary<string, IDataSource> FindDataSources(string[] dataSourceTypes, Dictionary<string, IDataSource> defaultDataSources = null)
        {
            Dictionary<string, IDataSource> dataSourcesOut = defaultDataSources;

            if (defaultDataSources == null || defaultDataSources.Count == 0)
            {
                dataSourcesOut ??= new Dictionary<string, IDataSource>();

                FindDataSourcesInParents(dataSourceTypes, dataSourcesOut);
                FindDataSourcesInSingleton(dataSourceTypes, dataSourcesOut);
            }
            return dataSourcesOut;
        }


        /// <summary>
        /// Search through this object and its parents in game object
        /// hierarchy for the data source to use with this data consumer.
        /// </summary>
        /// <param name="dataSourceTypes">The desired data source types.</param>
        /// <param name="dataSourcesInOut">The current dictionary of data sources to add on to.</param>
        protected void FindDataSourcesInParents(string[] dataSourceTypes, Dictionary<string, IDataSource> dataSourcesInOut)
        {
            GameObject currentGO = gameObject;

            // walk from current GameObject up to root GameObject looking for an IDataSourceProvider
            while (currentGO != null)
            {
                Component[] dataSourceComponents = currentGO.GetComponents(typeof(IDataSourceProvider));
                foreach (Component dataSourceComponent in dataSourceComponents)
                {
                    if ((dataSourceComponent as MonoBehaviour).enabled)
                    {
                        IDataSourceProvider dataSourceProvider = dataSourceComponent as IDataSourceProvider;
                        AddDataSourcesFromProvider(dataSourceProvider, dataSourceTypes, dataSourcesInOut);
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


        /// <summary>
        /// Search through this object and its parents in game object
        /// hierarchy for the data source to use with this data consumer.
        /// </summary>
        /// <param name="dataSourceTypes">The desired data source types.</param>
        /// <param name="dataSourcesInOut">The current dictionary of data sources to add on to.</param>
        protected void FindDataSourcesInSingleton(string[] dataSourceTypes, Dictionary<string, IDataSource> dataSourcesInOut)
        {
            if (DataSourceProviderSingleton.IsInitialized)
            {
                AddDataSourcesFromProvider(DataSourceProviderSingleton.Instance, dataSourceTypes, dataSourcesInOut);
            }
        }

        /// <summary>
        /// Given a data source provider, add each data source that has a data source type not already added to the dictionary of data sources
        /// </summary>
        /// <param name="dataSourceProvider">The provider to scan.</param>
        /// <param name="dataSourceTypes">The desired data source types.</param>
        /// <param name="dataSourcesInOut">The current dictionary of data sources to add on to.</param>
        private void AddDataSourcesFromProvider(IDataSourceProvider dataSourceProvider, string[] dataSourceTypes, Dictionary<string, IDataSource> dataSourcesInOut)
        {
            string[] dataSourceTypesToCheck = dataSourceProvider.GetDataSourceTypes();
            foreach (string dataSourceTypeToCheck in dataSourceTypesToCheck)
            {
                // find and add first occurrence of each unique DataSource data type like "data" or "theme"
                if ((dataSourceTypes == null || dataSourceTypes.Length == 0 || dataSourceTypes.Contains(dataSourceTypeToCheck)) && !dataSourcesInOut.ContainsKey(dataSourceTypeToCheck))
                {
                    dataSourcesInOut[dataSourceTypeToCheck] = dataSourceProvider.GetDataSource(dataSourceTypeToCheck);
                }
            }
        }

        /// <summary>
        /// If no data source is provided directly, search through this object and its parents in game object
        /// hierarchy for the data source to use with this data consumer.
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
        /// TODO: Clean this up -- just for testing but need delay to allow Source to be enabled prior.
        /// </summary>
        /// <returns>IEnumerator so that it can be used as a Coroutine.</returns>
        private IEnumerator DelaySelfAttach()
        {
            yield return new WaitForEndOfFrame();
            SelfAttach();
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
                Dictionary<string, IDataSource> dataSources = FindDataSources(dataSourceTypes, DataSources);
                IDataController dataController = null;

                // ToDo: this should be more generalized than hard coded data type of "data"
                if (dataSources.ContainsKey("data"))
                {
                    dataController = dataSources["data"].DataController;
                }

                if (dataController == null)
                {
                    dataController = FindNearestDataController(DataController);
                }

                Attach(dataSources, dataController);
                _externalAttached = false;
            }
            else
            {
                if (!_externalAttached)
                {
                    Debug.LogWarning("Attempting to self attach when already self attached.");
                }
            }
        }

        private void OnApplicationQuit()
        {
            // This is currently only used for logging warnings
            _isQuitting = true;
        }

        #endregion protected and private methods
    }
}
