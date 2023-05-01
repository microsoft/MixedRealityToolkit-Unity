// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Base implementation of the IDataSource interface. This base class
    /// contains no Unity specific code and can be used as the base for
    /// implementing other concrete data source classes.
    ///
    /// To make any data source available as a MonoBehavior, see
    /// DataSourceGOBase.
    /// </summary>
    public class DataSourceBase : IDataSource
    {
        protected IDataKeyPathMapper _dataKeyPathMapper = null;
        protected Dictionary<string, List<IDataConsumer>> _keyPathToDataConsumers = new Dictionary<string, List<IDataConsumer>>();
        protected Dictionary<IDataConsumer, int> _dataConsumerKeyPathCounts = new Dictionary<IDataConsumer, int>();
        protected Regex _findArrayIndexRegex = new Regex(@"\[(\d+)\]");

        public IDataController DataController { get => _dataController; set => _dataController = value; }
        private IDataController _dataController;

        /// </inheritdoc/>
        public string DataSourceType { get; set; }

        /// </inheritdoc/>
        public void SetDataKeyPathMapper(IDataKeyPathMapper keyPathMapper)
        {
            _dataKeyPathMapper = keyPathMapper;
        }

        /// </inheritdoc/>
        public string ResolveKeyPath(string resolvedKeyPathPrefix, string localKeyPath)
        {
            resolvedKeyPathPrefix ??= string.Empty;

            if (_dataKeyPathMapper != null)
            {
                localKeyPath = _dataKeyPathMapper.GetDataKeyPathFromViewKeyPath(localKeyPath);
            }

            if (IsCollectionAtKeyPath(resolvedKeyPathPrefix))
            {
                // TODO: How do we want to handle multidimensional arrays, e.g. threedmodels[10].transform.position[1][2]
                //       adding a '.' would not work between [1] and [2]
                return resolvedKeyPathPrefix + '[' + localKeyPath + ']';
            }
            else if (resolvedKeyPathPrefix != "")
            {
                return resolvedKeyPathPrefix + '.' + localKeyPath;
            }
            else
            {
                return localKeyPath;
            }
        }

        /// </inheritdoc/>
        public object GetValue(string resolvedKeyPath)
        {
            if (IsDataAvailable())
            {
                return GetValueInternal(resolvedKeyPath);
            }
            else
            {
                return null;
            }
        }

        /// </inheritdoc/>
        public void SetValue(string resolvedKeyPath, object newValue, bool isAtomicChange = false)
        {
            if (IsDataAvailable())
            {
                SetValueInternal(resolvedKeyPath, newValue);
                NotifyDataChanged(resolvedKeyPath, newValue, DataChangeType.DatumModified, isAtomicChange);
            }
        }

        /// </inheritdoc/>
        public virtual void DataChangeSetBegin()
        {
            foreach (IDataConsumer dataConsumer in _dataConsumerKeyPathCounts.Keys)
            {
                dataConsumer.DataChangeSetBegin(this);
            }
        }

        /// </inheritdoc/>
        public virtual void DataChangeSetEnd()
        {
            foreach (IDataConsumer dataConsumer in _dataConsumerKeyPathCounts.Keys)
            {
                dataConsumer.DataChangeSetEnd(this);
            }
        }

        protected void DataChangeSetBeginForKeyPath(string keyPath)
        {
            List<IDataConsumer> dataConsumers = _keyPathToDataConsumers[keyPath];
            foreach (IDataConsumer dataConsumer in dataConsumers)
            {
                dataConsumer.DataChangeSetBegin(this);
            }
        }

        protected void DataChangeSetEndForKeyPath(string keyPath)
        {
            List<IDataConsumer> dataConsumers = _keyPathToDataConsumers[keyPath];
            foreach (IDataConsumer dataConsumer in dataConsumers)
            {
                dataConsumer.DataChangeSetEnd(this);
            }
        }

        /// </inheritdoc/>
        public void AddDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            if (!_keyPathToDataConsumers.ContainsKey(resolvedKeyPath))
            {
                _keyPathToDataConsumers[resolvedKeyPath] = new List<IDataConsumer>();
            }

            if (_keyPathToDataConsumers[resolvedKeyPath].Contains(dataConsumer))
            {
                // Debug.LogWarning("Attempting to add the same consumer listener to the same keypath: " + resolvedKeyPath);
            }
            else
            {
                _keyPathToDataConsumers[resolvedKeyPath].Add(dataConsumer);
                if (_dataConsumerKeyPathCounts.ContainsKey(dataConsumer))
                {
                    _dataConsumerKeyPathCounts[dataConsumer]++;
                }
                else
                {
                    _dataConsumerKeyPathCounts[dataConsumer] = 1;       // 1st keypath
                }
            }

            // TODO: This is for dynamically added collection items, but could cause
            //       unintended side effects. Need better solution.
            object value = GetValue(resolvedKeyPath);

            if (value != null)
            {
                if (IsCollectionAtKeyPath(resolvedKeyPath))
                {
                    OnCollectionListenerAdded(resolvedKeyPath, value);
                }

                /****
                 * Now down at end of Attach in DataConsumer
                dataConsumer.DataChangeSetBegin(this);
                dataConsumer.NotifyDataChanged(this, resolvedKeyPath, value, DataChangeType.DatumAdded);
                dataConsumer.DataChangeSetEnd(this);
                */
            }
        }

        /// <summary>
        /// Triggered whenever a datum is added that is a collection
        /// </summary>
        /// <remarks>
        /// Note that the collection object may not actually an actual C# collection from the data source. It may
        /// be an IEnumerator that can enumerate the keypaths of the collection items.
        /// </remarks>
        /// <param name="resolvedKeyPath">Fully resolved key path for the collection</param>
        /// <param name="collection">The collection being added.</param>
        public virtual void OnCollectionListenerAdded(string resolvedKeyPath, object collection)
        {
            // no default action. Override for further collection listener setup.
        }


        /// <summary>
        /// Triggered whenever a datum is removed that is a collection
        /// </summary>
        /// <remarks>
        /// Note that the collection object may not actually an actual C# collection from the data source. It may
        /// be an IEnumerator that can enumerate the keypaths of the collection items.
        /// </remarks>
        /// <param name="resolvedKeyPath">Fully resolved key path for the collection</param>
        /// <param name="collection">The collection itself that will be removed.</param>
        public virtual void OnCollectionListenerRemoved(string resolvedKeyPath)
        {
            // no default action. Override for further collection listener setup.
        }


        /// </inheritdoc/>
        public virtual void RemoveDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            if (_keyPathToDataConsumers.ContainsKey(resolvedKeyPath))
            {
                if (IsCollectionAtKeyPath(resolvedKeyPath))
                {
                    OnCollectionListenerRemoved(resolvedKeyPath);
                }

                _keyPathToDataConsumers[resolvedKeyPath].Remove(dataConsumer);

                if (--_dataConsumerKeyPathCounts[dataConsumer] == 0)
                {
                    // all keypaths for this dataconsumer have been removed, so remove data consumer
                    _dataConsumerKeyPathCounts.Remove(dataConsumer);
                }
            }
        }

        /// </inheritdoc/>
        public virtual bool IsCollectionAtKeyPath(string resolvedKeyPath)
        {
            return false;
        }

        /// </inheritdoc/>
        public virtual int GetCollectionCount(string resolvedKeyPath)
        {
            return 0;
        }

        /// </inheritdoc/>
        public virtual string GetNthCollectionKeyPathAt(string resolvedKeyPath, int n)
        {
            return null;
        }

        /// </inheritdoc/>
        public virtual IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            return null;
        }


        /// </inheritdoc/>
        public void NotifyDataChanged(string resolvedKeyPath, object value, DataChangeType changeType, bool isAtomicChange)
        {
            if (_keyPathToDataConsumers.ContainsKey(resolvedKeyPath))
            {
                if (isAtomicChange)
                {
                    DataChangeSetBeginForKeyPath(resolvedKeyPath);
                }

                List<IDataConsumer> dataConsumers = _keyPathToDataConsumers[resolvedKeyPath];
                foreach (IDataConsumer dataConsumer in dataConsumers)
                {
                    dataConsumer.NotifyDataChanged(this, resolvedKeyPath, value, changeType);
                }

                if (isAtomicChange)
                {
                    DataChangeSetBeginForKeyPath(resolvedKeyPath);
                }
            }
        }

        /// </inheritdoc/>
        public void NotifyAllChanged(DataChangeType dataChangeType = DataChangeType.DatumModified, IDataConsumer whichDataConsumer = null)
        {
            if (IsDataAvailable())
            {
                DataChangeSetBegin();
                List<KeyValuePair<string, List<IDataConsumer>>> dataConsumersKeyValuesCopy = new List<KeyValuePair<string, List<IDataConsumer>>>();

                foreach (KeyValuePair<string, List<IDataConsumer>> dataConsumersKeyValue in _keyPathToDataConsumers)
                {
                    dataConsumersKeyValuesCopy.Add(dataConsumersKeyValue);
                }

                foreach (KeyValuePair<string, List<IDataConsumer>> dataConsumersKeyValue in dataConsumersKeyValuesCopy)
                {
                    List<IDataConsumer> dataConsumers = dataConsumersKeyValue.Value;

                    foreach (IDataConsumer dataConsumer in dataConsumers)
                    {
                        if (whichDataConsumer == null || whichDataConsumer == dataConsumer)
                        {
                            dataConsumer.NotifyDataChanged(this, dataConsumersKeyValue.Key, GetValue(dataConsumersKeyValue.Key), dataChangeType);
                        }
                    }
                }

                DataChangeSetEnd();
            }
        }

        /// </inheritdoc/>
        public virtual bool IsDataAvailable()
        {
            // override if an async datasource may not yet have finished loading
            return true;
        }

        /// <summary>
        /// Get the value from the data source based on its keypath.
        /// </summary>
        /// <remarks>
        /// This is separated out from the GetValue() method to ensure that any required base functionality
        /// is performed in addition to the actual retrieval of the value.
        /// </remarks>
        /// <param name="resolvedKeyPath">The fully resolved keypath for the datum to retrieve.</param>
        /// <returns>The retrieved value for the specified keypath.</returns>
        public virtual object GetValueInternal(string resolvedKeyPath)
        {
            // override to return actual value from your custom data source
            return null;
        }

        /// <summary>
        /// Set the value in the data source based on its keypath.
        /// </summary>
        /// <remarks>
        /// This is separated out from the SetValue() method to ensure that any required base functionality
        /// is performed in addition to the actual setting of the value.
        /// </remarks>
        /// <param name="resolvedKeyPath">The fully resolved keypath for the datum to set.</param>
        /// <param name="newValue">The new value to set for the specified keypath.</param>

        public virtual void SetValueInternal(string resolvedKeyPath, object newValue)
        {
            // If setting values in the data source is supported, override to support
            // updating values.
        }

        /// <summary>
        /// Get a path where all array indices have been removed.
        /// </summary>
        /// <remarks>
        /// This can be used to identify a list regardless of
        /// which entry in one or nested lists it may have come from.
        /// </remarks>
        /// <param name="resolvedKeyPath">The keypath to modify.</param>
        /// <returns>Resolved keypath with index values removed.</returns>
        protected string GetDeindexedKeyPath(string resolvedKeyPath)
        {
            if (resolvedKeyPath != "")
            {
                MatchCollection matches = _findArrayIndexRegex.Matches(resolvedKeyPath);

                // TODO: find more efficient way to do this with fewer allocs
                for (int idx = matches.Count - 1; idx >= 0; idx--)
                {
                    Group group = matches[idx].Groups[1];

                    resolvedKeyPath = resolvedKeyPath.Remove(group.Index, group.Value.Length);
                }
            }
            return resolvedKeyPath;
        }
    }
}
