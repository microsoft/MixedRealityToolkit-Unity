// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.MixedReality.Toolkit.Utilities;

// Included only to log error message during debug
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Data
{

    /// <summary>
    /// Base implementation of the IDataSource interface. This base class
    /// contains no Unity specific code and can be used as the base for 
    /// implementing other concrate data source classes.
    /// 
    /// To make any data source available as a MonoBehavior, see
    /// DataSourceGOBase.
    /// 
    /// </summary>

    public class DataSourceBase : IDataSource
    {

        protected IDataKeyPathMapper _dataKeyPathMapper = null;

        protected Dictionary<string, List<IDataConsumer>> _keyPathToDataConsumers = new Dictionary<string, List<IDataConsumer>>();
        protected HashSet<IDataConsumer> _dataConsumers = new HashSet<IDataConsumer>();
        protected Regex _findArrayIndexRegex = new Regex( @"\[(\d+)\]" );

        public IDataController DataController { get => _dataController; set => _dataController = value; }
        private IDataController _dataController;


        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>
        public void SetDataKeyPathMapper(IDataKeyPathMapper keyPathMapper)
        {
            _dataKeyPathMapper = keyPathMapper;
        }


        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>
        public string ResolveKeyPath(string resolvedKeyPathPrefix, string localKeyPath)
        {
            if (resolvedKeyPathPrefix == null)
            {
                resolvedKeyPathPrefix = "";
            }

            if (_dataKeyPathMapper != null)
            {
                localKeyPath = _dataKeyPathMapper.GetDataKeyPathFromViewKeyPath(localKeyPath);
            }


            if (IsCollectionAtKeyPath(resolvedKeyPathPrefix))
            {
                // TODO: How do we want to handle multidimentional arrays, eg. threedmodels[10].transform.position[1][2]
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

        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>

        public object GetValue(string resolvedKeyPath)
        {
            if (IsDataSourceAvailable())
            {
                return GetValueInternal(resolvedKeyPath);
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>

        public void SetValue(string resolvedKeyPath, object newValue, bool isAtomicChange = false)
        {
            if (IsDataSourceAvailable())
            {
                SetValueInternal(resolvedKeyPath, newValue);
                NotifyDataChanged(resolvedKeyPath, newValue, DataChangeType.DatumModified, isAtomicChange);
            }
        }

 
        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>

        public virtual void DataChangeSetBegin()
        {
            foreach (IDataConsumer dataConsumer in _dataConsumers)
            {
                dataConsumer.DataChangeSetBegin(this);

            }
        }

        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>

        public virtual void DataChangeSetEnd()
        {
            foreach (IDataConsumer dataConsumer in _dataConsumers)
            {
                dataConsumer.DataChangeSetEnd(this);
            }
        }

        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>

        public void AddDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {

            if (!_keyPathToDataConsumers.ContainsKey(resolvedKeyPath))
            {
                _keyPathToDataConsumers[resolvedKeyPath] = new List<IDataConsumer>();
            }

            if (_keyPathToDataConsumers[resolvedKeyPath].Contains(dataConsumer))
            {
                DebugUtilities.LogVerbose("Attempting to add the same consumer listener to the same keypath: " + resolvedKeyPath);
            }
            else
            {
                _keyPathToDataConsumers[resolvedKeyPath].Add(dataConsumer);
                _dataConsumers.Add(dataConsumer);
            }

            // TODO: This is for dynamically added collection items, but could cause
            //       unintented side effects. Need better solution.
            object value = GetValue(resolvedKeyPath);

            if ( value != null)
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


        public virtual void OnCollectionListenerAdded(string resolvedKeyPath, object collection)
        {
            // no default action. Override for further collection listener setup.
        }

        public virtual void OnCollectionListenerRemoved(string resolvedKeyPath)
        {
            // no default action. Override for further collection listener setup.
        }



        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>

        public virtual void RemoveDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            if (_keyPathToDataConsumers.ContainsKey(resolvedKeyPath))
            {
                _keyPathToDataConsumers[resolvedKeyPath].Remove(dataConsumer);

                try
                {
                    if (IsCollectionAtKeyPath(resolvedKeyPath))
                    {
                        OnCollectionListenerRemoved(resolvedKeyPath);
                    }

                }
                catch (ArgumentOutOfRangeException)
                {
                    // ObservableCollection only reports a removed item after it's been removed, which
                    // so any attempt to access that object like to check if it's a collection will
                    // cause an exception.
                }
            }
        }


        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>
        // Dictionary data source does not support collections
        public virtual bool IsCollectionAtKeyPath(string resolvedKeyPath)
        {
            return false;
        }


        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>
        public virtual int GetCollectionCount(string resolvedKeyPath)
        {
            return 0;
        }


        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>
        public virtual string GetNthCollectionKeyPathAt(string resolvedKeyPath, int n)
        {
            return null;
        }


        /// <summary>
        /// Implements IDataSource method. See IDataSource.cs for more information
        /// </summary>
        public virtual IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            return null;
        }


        public virtual object GetValueInternal(string resolvedKeyPath)
        {
            // override to return actual value from your custom data source
            return null;
        }

        public virtual void SetValueInternal(string resolvedKeyPath, object newValue)
        {
            // Default provided since not all data sources need to support setting of values
            //
            // NOTE: Remember to call NotifyDataChanged when overriding this method
            // NotifyDataChanged(keyPath, newValue, DataChangeType.DatumModified);
        }


        public void NotifyDataChanged(string resolvedKeyPath, object value, DataChangeType changeType, bool isAtomicChange )
        {
            if (_keyPathToDataConsumers.ContainsKey(resolvedKeyPath))
            {
                if ( isAtomicChange )
                {
                    DataChangeSetBegin();   // TODO: optimize to only send to consumers that will actually be notified
                }

                List<IDataConsumer> dataConsumers = _keyPathToDataConsumers[resolvedKeyPath];
                foreach (IDataConsumer dataConsumer in dataConsumers)
                {
                    dataConsumer.NotifyDataChanged(this, resolvedKeyPath, value, changeType);
                }

                if (isAtomicChange )
                {
                    DataChangeSetEnd();  // TODO: optimize to only send to consumers that will actually be notified
                }
            }
        }


        public void NotifyAllChanged( DataChangeType dataChangeType = DataChangeType.DatumModified)
        {
            if (IsDataSourceAvailable())
            {
                DataChangeSetBegin();
                List< KeyValuePair<string, List<IDataConsumer>>> dataConsumersKeyValuesCopy = new List<KeyValuePair<string, List<IDataConsumer>>>();

                foreach (KeyValuePair<string, List<IDataConsumer>> dataConsumersKeyValue in _keyPathToDataConsumers)
                {
                    dataConsumersKeyValuesCopy.Add(dataConsumersKeyValue);
                }

                    foreach (KeyValuePair<string, List<IDataConsumer>> dataConsumersKeyValue in dataConsumersKeyValuesCopy)
                {
                    List<IDataConsumer> dataConsumers = dataConsumersKeyValue.Value;

                    foreach (IDataConsumer dataConsumer in dataConsumers)
                    {
                        dataConsumer.NotifyDataChanged(this, dataConsumersKeyValue.Key, GetValue(dataConsumersKeyValue.Key), dataChangeType);
                    }
                }

                DataChangeSetEnd();
            }
        }

        /// <summary>
        /// Get a path where all array indices have been removed.
        /// </summary>
        /// <remarks>
        /// This can be used to identify a list regardless of 
        /// which entery in one or nested lists it may have come from.
        /// </remarks>
        /// <param name="resolvedKeyPath">The keypath to modify.</param>
        /// <returns></returns>
        public string GetDeindexedKeyPath(string resolvedKeyPath)
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

      
        protected virtual bool IsDataSourceAvailable()
        {
            // override if an async datasource may not yet have finished loading
            return true;
        }

    }
}
