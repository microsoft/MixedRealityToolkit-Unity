// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Base class for Data Sources that are accessible through a Game Object MonoBehaviour proxy 
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// This class encapsulates a Data Source object so that it can exist as a MonoBehaviour, which
    /// is important for using the inspector to connect Data Consumers to a Data Source.
    /// 
    /// By implementing this as a proxy, most of code for any Data Source concrete implmementation
    /// can exist as pure C# with no Unity dependencies.
    /// 
    /// </remarks>

    public abstract class DataSourceGOBase : MonoBehaviour, IDataSource, IDataSourceProvider
    {
        [Tooltip("Optional DataKeyPathMapper that translates between local view key paths and data source key paths. This is useful for re-using prefabs.")]
        [SerializeField]
        private DataKeyPathMapperGODictionary keyPathMapper;

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

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_dataSource == null)
            {
                _dataSource = AllocateDataSource();
                if (keyPathMapper != null && _dataSource != null)
                {
                    _dataSource.SetDataKeyPathMapper(keyPathMapper as IDataKeyPathMapper);
                }

                // one time initialization of a data source
                InitializeDataSource();
            }
        }

        protected void Start()
        {
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        /// <summary>
        /// Allocate the correct data source
        /// </summary>
        /// <remarks> 
        /// 
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
        public virtual IDataSource GetDataSource()
        {
            return this;
        }


        protected virtual void InitializeDataSource()
        {
            // override for additional initialization.
        }

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

        public void SetDataKeyPathMapper( IDataKeyPathMapper keyPathMapper )
        {
            if (DataSource != null)
            {
                DataSource.SetDataKeyPathMapper(keyPathMapper);
            }
        }
    
        public virtual object GetValue(string resolvedKeyPath)
        {
            if (DataSource != null )
            {
                return DataSource.GetValue(resolvedKeyPath);
            }
            else
            {
                return null;
            }
        }


        public virtual void SetValue(string resolvedKeyPath, object newValue, bool isAtomicChange = false)
        {
            if (DataSource != null)
            {
                DataSource.SetValue(resolvedKeyPath, newValue, isAtomicChange);
            }
        }


        public void AddDataConsumerListener( string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            if (DataSource != null)
            {
                DataSource.AddDataConsumerListener(resolvedKeyPath, dataConsumer);
            }
        }


        public void RemoveDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer)
        {
            if (DataSource != null)
            {
                DataSource.RemoveDataConsumerListener(resolvedKeyPath, dataConsumer);
            }
        }

        public void DataChangeSetBegin()
        {
            if (DataSource != null)
            {
                DataSource.DataChangeSetBegin();
            }
        }

        public void DataChangeSetEnd()
        {
            if (DataSource != null)
            {
                DataSource.DataChangeSetEnd();
            }
        }


        // Dictionary data source does not support collections
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

        public void NotifyDataChanged( string resolvedKeyPath, object value, DataChangeType dataChangeType, bool isAtomicChange )
        {
            if (DataSource != null)
            {
                DataSource.NotifyDataChanged(resolvedKeyPath, value, dataChangeType, isAtomicChange);
            }
        }

        public void NotifyAllChanged()
        {
            if (DataSource != null)
            {
                DataSource.NotifyAllChanged();
            }
        }
    }
}
