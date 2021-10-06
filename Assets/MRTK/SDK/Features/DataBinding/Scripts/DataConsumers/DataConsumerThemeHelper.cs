using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{

    /// <summary>
    /// A theme data consumer used to help another primary data consumer derived from DataConsumeThemableBase<T> 
    /// that is designed to manage both dynamically bound data received from a data-centric DataSource, and
    /// then theme that dynamic data. As an example, a numeric status can be used to look up an appropriate
    /// status icon for the current status.  That icon can then be further themed to adopt the desired, branding or 
    /// other specific look and feel appropriate for the currently active theme.
    /// </summary>
    public class DataConsumerThemeHelper : DataConsumerGOBase
    {
        [Tooltip("(Optional) Specific theme data soource to use for retrieving theme information. If not provided, the first matching data source of specified type will be used.")]
        [SerializeField]
        protected DataSourceGOBase themeDataSource;

        [Tooltip("Theme key path for data that will be used by the element this is attached to, typically another data consumer on the same game object.")]
        [SerializeField]
        protected string themeKeyPath;


        public IDataConsumerThemable DataConsumerThemable { get; set; }


        protected override void InitializeDataConsumer()
        {
            if (this.dataSourceType == null || this.dataSourceType == "")
            {
                this.dataSourceType = "theme";      // make default "theme" to differentiate from "data" data source types
            }
        }


        protected override void AttachDataConsumer()
        {
            AddKeyPathListener(themeKeyPath);
        }


        /// <summary>
        /// Receive changed object, determine its, type and process appropriately
        /// </summary>
        /// <remarks>
        /// The object can be any of a number of types and loaded accordingly:
        /// 
        /// int                     Use as index to select Nth entry in ValueToObjectInfo
        /// T                       Directly use the value to replace the managed variable of that type
        /// "resource://<<path>>"   Use path to load a Unity Resource
        /// "file://<<path>>"       Use path to load a streaming asset
        /// other string            Use string value to find entry by value in ValueToObjectInfo
        /// 
        /// </remarks>
        /// <param name="dataSource"></param>
        /// <param name="resolvedKeyPath"></param>
        /// <param name="localKeyPath"></param>
        /// <param name="inValue"></param>
        /// <param name="dataChangeType"></param>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object inValue, DataChangeType dataChangeType)
        {
            if (DataConsumerThemable != null)
            {
                DataConsumerThemable.ProcessThemeDataChanged(this, resolvedKeyPath, localKeyPath, inValue, dataChangeType);
            }
        }
    }
}