// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Base class for Data Sources that are accessible through a Game Object MonoBehaviour proxy
    /// </summary>
    /// <remarks>
    /// This class encapsulates a Data Source object so that it can exist as a MonoBehaviour, which
    /// is important for using the inspector to connect Data Consumers to a Data Source.
    ///
    /// Note that this class does NOT inherit from DataSourceBase since it is a proxy.
    ///
    /// By implementing this as a proxy, most of code for any Data Source concrete implementation
    /// can exist as pure C# with no Unity dependencies.
    /// </remarks>
    public abstract class DataSourceGOBase : MonoBehaviour, IDataSource, IDataSourceProvider
    {
        [Tooltip("(Optional) Data source type. Can be used by data consumers to automatically find and attach to the correct data source. E.g. This is useful for differentiating between 'data' and 'theme' data sources.")]
        [SerializeField, Experimental]
        protected string dataSourceType;

        [Tooltip("(Optional) DataKeyPathMapper that translates between local view key paths and data source key paths. This is useful for re-using prefabs.")]
        [SerializeField]
        protected DataKeyPathMapperGODictionary keyPathMapper = null;

        /// </inheritdoc/>
        public string DataSourceType
        {
            get => dataSourceType;
            set => dataSourceType = value;
        }

        /// <summary>
        /// If ever requested, cache the data source types that can be provided by this DataSourceProvider.
        /// </summary>
        private string[] _dataSourceTypes = null;

        /// </inheritdoc/>
        public IDataController DataController { get => _dataController; set => _dataController = value; }

        protected IDataSource DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    Initialize();
                }
                return _dataSource;
            }
        }

        private IDataSource _dataSource;
        private IDataController _dataController;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Allocate the correct data source
        /// </summary>
        /// <remarks>
        /// Attach whatever data source is desired here in your derived class if not already allocated.
        /// This method passes most IDataSource calls directly to the IDataSource provided in your
        /// overridden method.
        ///
        /// Further initialization can occur when InitializeDataSource() is called.
        /// </remarks>
        public virtual IDataSource AllocateDataSource()
        {
            // Override this method to allocate an appropriate data source if not already allocated.
            return null;
        }

        /// <summary>
        /// IDataSourceProvider method to get this data source.
        /// </summary>
        /// <remarks>
        /// Attach whatever data source is desired here in your derived class if not already allocated.
        /// Further initialization can occur when InitializeDataSource() is called.
        ///
        /// NOTE: This does not return _dataSource because then any logic in the subclass of this class will
        /// never execute for any methods that overrides an IDataSource interface method.
        /// </remarks>
        public virtual IDataSource GetDataSource(string dataSourceType = null)
        {
            return this;
        }


        /// </inheritdoc/>
        public string[] GetDataSourceTypes()
        {
            // Most data sources only provide one type of data, so the IDataSourceProvider embedded
            // in each data source will almost always only return one data source.
            // However, a data source provider may provide access to more than one data source when
            // it is not directly associated with a single data source.  An example is the DataSourceProviderSingleton.
            _dataSourceTypes ??= new string[] { DataSourceType };

            return _dataSourceTypes;
        }

        /// </inheritdoc/>
        protected virtual void InitializeDataSource()
        {
            // override for additional initialization.
        }

        /// </inheritdoc/>
        public string ResolveKeyPath(string resolvedKeyPathPrefix, string localKeyPath)
        {
            if (DataSource != null)
            {
                return DataSource.ResolveKeyPath(resolvedKeyPathPrefix, localKeyPath);
            }
            else
            {
                return null;
            }
        }

        /// </inheritdoc/>
        public void SetDataKeyPathMapper(IDataKeyPathMapper keyPathMapper)
        {
            if (DataSource != null)
            {
                DataSource.SetDataKeyPathMapper(keyPathMapper);
            }
        }

        /// </inheritdoc/>
        public virtual object GetValue(string resolvedKeyPath)
        {
            if (DataSource != null)
            {
                return DataSource.GetValue(resolvedKeyPath);
            }
            else
            {
                return null;
            }
        }

        /// </inheritdoc/>
        public virtual void SetValue(string resolvedKeyPath, object newValue, bool isAtomicChange = false)
        {
            if (DataSource != null)
            {
                DataSource.SetValue(resolvedKeyPath, newValue, isAtomicChange);
            }
        }

        /// </inheritdoc/>
        public void AddDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            if (DataSource != null)
            {
                DataSource.AddDataConsumerListener(resolvedKeyPath, dataConsumer);
            }
        }

        /// </inheritdoc/>
        public void RemoveDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            if (DataSource != null)
            {
                DataSource.RemoveDataConsumerListener(resolvedKeyPath, dataConsumer);
            }
        }

        /// </inheritdoc/>
        public void DataChangeSetBegin()
        {
            if (DataSource != null)
            {
                DataSource.DataChangeSetBegin();
            }
        }

        /// </inheritdoc/>
        public void DataChangeSetEnd()
        {
            if (DataSource != null)
            {
                DataSource.DataChangeSetEnd();
            }
        }

        /// </inheritdoc/>
        public virtual bool IsCollectionAtKeyPath(string resolvedKeyPath)
        {
            if (DataSource != null)
            {
                return DataSource.IsCollectionAtKeyPath(resolvedKeyPath);
            }
            else
            {
                return false;
            }
        }

        /// </inheritdoc/>
        public virtual int GetCollectionCount(string resolvedKeyPath)
        {
            if (DataSource != null)
            {
                return DataSource.GetCollectionCount(resolvedKeyPath);
            }
            else
            {
                return 0;
            }
        }

        /// </inheritdoc/>
        public virtual string GetNthCollectionKeyPathAt(string resolvedKeyPath, int n)
        {
            if (DataSource != null)
            {
                return DataSource.GetNthCollectionKeyPathAt(resolvedKeyPath, n);
            }
            else
            {
                return null;
            }
        }

        /// </inheritdoc/>
        public IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            if (DataSource != null)
            {
                return DataSource.GetCollectionKeyPathRange(resolvedKeyPath, rangeStart, rangeCount);
            }
            else
            {
                return null;
            }
        }

        /// </inheritdoc/>
        public void NotifyDataChanged(string resolvedKeyPath, object value, DataChangeType dataChangeType, bool isAtomicChange)
        {
            if (DataSource != null)
            {
                DataSource.NotifyDataChanged(resolvedKeyPath, value, dataChangeType, isAtomicChange);
            }
        }

        /// </inheritdoc/>
        public void NotifyAllChanged(DataChangeType dataChangeType = DataChangeType.DatumModified, IDataConsumer whichDataConsumer = null)
        {
            if (DataSource != null)
            {
                DataSource.NotifyAllChanged(dataChangeType, whichDataConsumer);
            }
        }

        /// </inheritdoc/>
        public bool IsDataAvailable()
        {
            if (DataSource != null)
            {
                return DataSource.IsDataAvailable();
            }
            else
            {
                return false;
            }
        }

        private void Initialize()
        {
            if (_dataSource == null)
            {
                _dataSource = AllocateDataSource();
                if (keyPathMapper != null && _dataSource != null)
                {
                    _dataSource.SetDataKeyPathMapper(keyPathMapper);
                }

                _dataSource.DataSourceType = dataSourceType;

                // one time initialization of a data source
                InitializeDataSource();
            }
        }
    }
}
