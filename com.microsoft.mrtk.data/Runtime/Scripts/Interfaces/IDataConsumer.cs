// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Interface for all data consumers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A data consumer is the intermediary between a data source and a view used to
    /// present information. The view will typically be a prefab where specific pieces of information identify which
    /// datum will be used to modify that presented information at runtime.
    /// </para>
    /// <para>
    /// Key Concepts:
    /// </para>
    /// <para> 
    /// Key Path (string) - See <see cref="IDataSource"/> for more information. A key path identifies a specific datum in the data source.
    /// That datum may be a primitive, or an entire data subset.
    /// </para>
    /// <para>
    /// Change Set - A grouping of data changes. All data source notifications of changed data will be within a Change Set.
    /// This allows for multiple related changes to be grouped into one atomic update of a data view. This is critical when
    /// multiple data items influence a single piece of presented information, like a single sentence with 3 pieces of variable
    /// information.
    /// </para>
    /// <para>
    /// Data Change Notifications - All changes to a data source are provide via change notifications. Every data consumer can
    /// sign up to listen or any number of data changes to specific data items within the data source. Also multiple consumers
    /// can listen to the same data source.
    /// </para>
    /// <para>
    /// For data that represents a collection or list of items, a data consumer can request specific subsets of that list.
    /// This allows for pagination and for virtualization, which is critical for supporting large data sets.
    /// </para>
    /// </remarks>
    public interface IDataConsumer
    {
        /// <summary>
        /// Prepare this data consumer for use and attach to the provided external resources.
        /// </summary>
        /// <remarks>
        /// <para>       
        /// The use of this method is typically associated with a prefab item in a list when it is added to a list. It is called on
        /// each DataConsumer in the prefab when re-using a prefab that's newly instantiated or retrieved from an object pool.
        /// </para>
        /// <para>
        /// Multiple data sources allows data to come from several places, typically useful for binding both data and themes.
        /// </para>
        /// <para>
        /// This is typically not called from application level code or derived classes. It is public to allow <see cref="DataConsumerCollection"/> and similar
        /// aggregate consumers to attach any spawned child data consumers.
        /// </para>
        /// <para>
        /// The <see cref="DataConsumerGOBase.AttachDataConsumer"/> function allows derived classes to perform additional attach tasks.
        /// </para>
        /// </remarks>
        /// <param name="dataSources">Which data sources to attach to for attaching a run-time loaded object, keyed to string DataType.</param>
        /// <param name="controller">The data controller to use during the attach process.</param>
        /// <param name="resolvedKeyPathPrefix">The path prefix for this object to be appended to front of all local key paths.</param>
        void Attach(Dictionary<string, IDataSource> dataSources, IDataController controller, string resolvedKeyPathPrefix = null);

        /// <summary>
        /// Detach this data consumer and prepare to return to object pool.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Detach from external resources before it is disabled.
        /// </para>
        /// <para>
        /// This is typically not called by application level code. It is public to allow <see cref="DataConsumerCollection"/> and similar
        /// aggregate consumers to clean up any spawned child data consumers.
        /// </para>
        /// <para>
        /// The <see cref="DataConsumerGOBase.AttachDataConsumer"/> function allows derived classes to perform additional detach tasks.
        /// </para>
        /// </remarks>
        void Detach();

        /// <summary>
        /// Tell whether attached to a DataSource.
        /// </summary>
        /// <returns><see langword="true"/> if currently attached to a data source.</returns>
        bool IsAttached();

        /// <summary>
        /// Mark beginning of a related set of data changes.
        /// </summary>
        /// <remarks>
        /// This bracketing of data change sets is particularly useful when a single asset, like a text string, is impacted by more than one piece of changed data, like
        /// a sentence that has two embedded variables.
        /// </remarks>
        /// <param name="dataSource">The data source that is starting a data change set.</param>
        void DataChangeSetBegin(IDataSource dataSource);

        /// <summary>
        /// Notification of a data change.
        /// </summary>
        /// <remarks>
        /// For each registered <paramref name="keyPath"/>, this will be called if the data at that <paramref name="keyPath"/> has changed. If the <paramref name="keyPath"/> represents a complex data structure, like an
        /// entire collection of entries, in concept it will notify if any of that data has changed. This currently is not optimized and will be called if there is
        /// a wholesale update of the dataset, but not necessarily detecting if there are actually any changes in the subset represented by this <paramref name="keyPath"/>.
        /// </remarks>
        /// <param name="dataSource">The data source that sent this notification.</param>
        /// <param name="keyPath">The resolved <paramref name="keyPath"/> for the data that has changed.</param>
        /// <param name="value">The value for the data represented by the <paramref name="keyPath"/> and related to the type of change that occurred.</param>
        /// <param name="changeType">The type of data change that has occurred.</param>
        void NotifyDataChanged(IDataSource dataSource, string keyPath, object value, DataChangeType changeType);

        /// <summary>
        /// Mark beginning of a related set of data changes.
        /// </summary>
        /// <remarks>
        /// This bracketing of data change sets is particularly useful when a single asset, like a text string, is impacted by more than one piece of changed data, like
        /// a sentence that has two embedded variables.
        /// </remarks>
        /// <param name="dataSource">The data source that is ending a data change set.</param>
        void DataChangeSetEnd(IDataSource dataSource);
    }
}
#pragma warning restore CS1591