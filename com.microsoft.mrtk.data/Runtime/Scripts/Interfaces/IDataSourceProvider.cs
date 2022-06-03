// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// An object that can provide a data source.
    /// </summary>
    /// <remarks>
    /// This is useful for situations where the lack of multiple inheritance makes
    /// it difficult to directly implement IDataSource in the object, but instead can
    /// lead to the correct data source that's managed externally.
    /// </remarks>
    public interface IDataSourceProvider
    {
        /// <summary>
        /// Get a datasource for a specific data source type, or null to return 1st or only data source.
        /// </summary>
        /// <param name="dataSourceType">Type name assign to desired data source, typically "data" or "theme", but can be any name chosen for that data source.</param>
        /// <returns>Interface to the desired data source.</returns>
        IDataSource GetDataSource(string dataSourceType = null);

        /// <summary>
        /// Get an array of data source types that can be provided by this provider
        /// </summary>
        /// <returns>Array of dataSourceType strings</returns>
        string[] GetDataSourceTypes();
    }
}
