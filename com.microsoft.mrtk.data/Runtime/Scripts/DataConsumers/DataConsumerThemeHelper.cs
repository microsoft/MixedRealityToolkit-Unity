// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Theme Helper")]
    public class DataConsumerThemeHelper : DataConsumerGOBase
    {
        [Tooltip("(Optional) Specific theme data source to use for retrieving theme information. If not provided, the first matching data source of specified type will be used.")]
        [SerializeField]
        protected DataSourceGOBase themeDataSource;

        [Tooltip("Theme key path for data that will be used by the element this is attached to, typically another data consumer on the same game object.")]
        [SerializeField]
        protected string themeKeyPath;

        public string ThemeKeyPath
        {
            get
            {
                return themeKeyPath;
            }
            set
            {
                themeKeyPath = value;
                if (IsAttached())
                {
                    // TODO add logic for removal of old keypath if there was one.
                    AddKeyPathListener(themeKeyPath);
                }
            }
        }

        /// <summary>
        /// At least one component was found that this data consumer
        /// will manage.
        /// </summary>
        /// <returns>Returns true if components to manage were found during initialization.</returns>
        public bool HasComponentsToManage()
        {
            return FindComponentsToManage().Count > 0;
        }

        public IDataConsumerThemable DataConsumerThemable { get; set; }

        /// <summary>
        /// Get the data source that is used for theming data
        /// </summary>
        /// <returns>An IDataSource interface of the theming data source.</returns>
        public IDataSource GetThemeDataSource()
        {
            if (DataSources == null)
            {
                return null;
            }

            // Is it explicitly provided?

            if (themeDataSource != null)
            {
                return themeDataSource;
            }

            // Is there one of the specified data type strings?

            if (DataSourceTypes.Length > 0)
            {
                foreach (string dataType in DataSourceTypes)
                {
                    if (DataSources.ContainsKey(dataType))
                    {
                        return DataSources[dataType];
                    }
                }
            }

            // Is there one called "theme"?

            if (DataSources.Count > 0)
            {
                if (DataSources.ContainsKey("theme"))
                {
                    return DataSources["theme"];
                }
            }

            return null;
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
        /// <param name="dataSource">Which data source called this method.</param>
        /// <param name="resolvedKeyPath">Fully resolved keypath for datum that changed.</param>
        /// <param name="localKeyPath">Local keypath for the datum that changed.</param>
        /// <param name="inDataValue">The current value of the datum</param>
        /// <param name="dataChangeType">The type of change that has occurred.</param>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object inValue, DataChangeType dataChangeType)
        {
            if (DataConsumerThemable != null)
            {
                DataConsumerThemable.ProcessThemeDataChanged(this, resolvedKeyPath, localKeyPath, inValue, dataChangeType);
            }
        }

        /// <inheritdoc/>
        protected override void InitializeDataConsumer()
        {
            if (DataSourceTypes != null && DataSourceTypes.Length == 0)
            {
                DataSourceTypes = new string[] { "theme" };      // make default "theme" to differentiate from "data" data source types
            }
        }

        /// <inheritdoc/>
        protected override void AttachDataConsumer()
        {
            AddKeyPathListener(themeKeyPath);
        }
    }
}
