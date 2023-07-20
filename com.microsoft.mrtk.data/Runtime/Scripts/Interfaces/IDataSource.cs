// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Specify the nature of the data change in notifications
    /// </summary>
    public enum DataChangeType
    {
        /// <summary>
        /// A new datum (of arbitrary complexity such as a collection) came into being that did not exist prior to this.
        /// </summary>
        /// <remarks>
        /// When a listener is assigned to a particular object, this event allows for initial observation of that object
        /// </remarks>
        DatumAdded,
                                        
        /// <summary>
        /// An existing datum has been modified.
        /// </summary>
        DatumModified,
        
        /// <summary>
        /// An existing datum has been removed.
        /// </summary>
        DatumRemoved,
        
        /// <summary>
        /// A collection is reset to 0 items.
        /// </summary>
        CollectionReset,
        
        /// <summary>
        /// A new item (of arbitrary complexity) has been added to a collection.
        /// </summary>
        CollectionItemAdded,
        
        /// <summary>
        /// An item in a list has been removed from a collection.
        /// </summary>
        CollectionItemRemoved
    }

    /// <summary>
    ///  Interface for all data sources.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  A data source is any managed set of data
    ///  that can be used to populate a data view via a data consumer. This data
    ///  can be static or dynamic, with any changes being reported to any data consumers
    ///  who have registered to receive change notifications.
    /// </para>
    /// <para>
    /// Key concepts:
    /// </para>
    /// <para>
    /// Key Path (<see langword="string"/>) - a unique accessor to specific data items within a data set.
    ///   Although a Key Path can be any unique identifier per data item, all
    ///   current implementations use the concept of a logical user readable
    ///   specifier that indicates the navigational position of the data of interest
    ///   relative to the entire data set. It is modelled on javascript's concept of
    ///   lists, dictionaries and primitives, such that key paths are correct javascript
    ///   statements for accessing data that can be represented in JSON. The advantage
    ///   of this approach is that it correlates very well with both JSON and XML,
    ///   which are the two most prevalent means of transferring information from back-end services. Example key paths:
    /// </para>
    /// <code>
    ///     temperature
    ///     contacts[10].firstName
    ///     contacts
    ///     contacts[10].addresses[3].city
    ///     [10].title
    ///     kingdom.animal.mammal.aardvark.diet.foodtypes.termites
    /// </code>
    /// <para>
    ///   Given that a key path is an arbitrary string with no required taxonomy, the actual
    ///   data specifiers could be any method of describing what data to retrieve. XML's XPath is an
    ///   example of a viable key path schema. As long as key paths provided by the data consumer
    ///   are consistent with the key paths expected by the data source, everything will work.
    ///   Furthermore Key Path Mappers can be implemented to translate between different schemas.
    /// </para>
    /// <para>
    ///   Resolving a keypath means combining two key paths: 1) a keypath describes how to access a specific
    ///   subset of a larger dataset, such as one entry in a list of many entries, and 2)
    ///   a partial (relative) keypath that represents just a datum within that list or map entry.
    ///   This makes it possible to treat a subset of the data in such a way that it does
    ///   not matter where in a larger data set hierarchy it actually exists. The most critical
    ///   use of this ability is to describe the data of a single entry in a list without worrying about which
    ///   entry in that list the current instance is referencing.
    /// </para>
    /// <para>
    /// Key Path Mapper (<see cref="IDataKeyPathMapper"/>) - A dependency injected helper that can map
    ///   between namespaces used in a data set and name spaces used in a view object, like
    ///   a prefab meant to be re-usable with different data sources, like a contact list. 
    /// </para>
    /// <para>
    /// Data Consumer (<see cref="IDataConsumer"/>) - An object that knows how to consume information being managed by
    ///   a data source and use that data to populate data views.
    /// </para>
    /// <para>
    /// Data Consumer Listener - Data Consumers that have registered to be notified of any changes to a specific
    ///   data item in a dataset. Whenever the data specified has changed (or suspected to have changed), the
    ///   Data Consumer(s) will be notified.
    /// </para>
    /// <para>
    /// Collection - Any subset of the data source that is a repeated array of the same types of information. This
    ///   special consideration of the structure of the data is critical for populating list views where items
    ///   are presented in a list.  Data sources and data consumers can support nested lists, such as a list of keywords
    ///   associated with each photo in a list of photos. The keypath for the keywords would be relative to the photo,
    ///   and the keypath for the photos would be relative to the list, and the keypath of the list would be relative
    ///   to either the nearest parent list, or the root of the data set.
    /// </para>
    /// </remarks>
    public interface IDataSource
    {
        /// <summary>
        /// Get the data source type
        /// </summary>
        /// <remarks>
        /// <para>
        /// The data source type is an arbitrary string type that can be used
        /// by data consumers to find the correct data source to listen to.
        /// </para>
        /// <para>
        /// The value is arbitrary. In a simple use case, it can be used to differentiate
        /// between a "data" and a "theme" data source when theming is in use.
        /// In more sophisticated applications, there can be any
        /// variety of types to ensure data consumers can find and are attached to the correct
        /// data source.
        /// </para>
        /// <para>
        /// CAUTION: Changing the type while attached to data consumers may create unintended
        /// behavior. The setter is provided for setting the type during initialization
        /// prior to attachment by data consumers.
        /// </para>
        /// </remarks>
        string DataSourceType { get; set; }

        /// <summary>
        /// An optional data controller.
        /// </summary>
        /// <remarks>
        /// This data controller will be bound to any DataConsumers that are attached to this data source.
        /// </remarks>
        IDataController DataController { get; set; }

        /// <summary>
        /// Set a key path mapper.
        /// </summary>
        /// <remarks>
        /// If the view (data consumer) uses generic key paths that are not the same as the data source, then this
        /// mapper will translate between data and view key paths. This allows a generic view to be used with a wide
        /// variety of data sources without modification.
        /// </remarks>
        /// <param name="dataKeyPathMapper">Data key path mapper to assign.</param>
        void SetDataKeyPathMapper(IDataKeyPathMapper dataKeyPathMapper);

        /// <summary>
        /// Resolve a key path using a given prefix and local path.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Given a resolved (data source namespace) key path, and a local (data consumer namespace) keypath,
        /// return a fully resolved data source key path for that data item.
        /// </para>
        /// <para>
        /// This <paramref name="resolvedKeyPathPrefix"/> can be an empty string if the <paramref name="localKeyPath"/> represents an
        /// absolute location in the data set. If the <paramref name="localKeyPath"/> is a member of one item in
        /// an array of items, then the prefix will be that of the data container itself containing
        /// the array.
        /// </para>
        /// <para>
        /// If the specified item can not be found in the data source, null is returned.
        /// </para>
        /// </remarks>
        /// <param name="resolvedKeyPathPrefix">The data path prefix.</param>
        /// <param name="localKeyPath">The local keyPat to resolve. Note that this may be mapped by a <see cref="IDataKeyPathMapper"/></param>
        /// <returns>A string that can be used to map to a view data consumer.</returns>
        string ResolveKeyPath(string resolvedKeyPathPrefix, string localKeyPath);

        /// <summary>
        /// Get the value associated with the specified key path and prefix
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned value can be of any arbitrary complexity or data type from primitives to complex nesting of
        /// dictionaries and arrays, to image or audio data.
        /// </para>
        /// <para>
        /// The fullyQualifiedPrefix is provided for lists (or collection of lists) to find the correct instance of a specific local keypath.
        /// If not processing a collection, this can be either <see langword="null"/> or an empty <see langword="string"/>. It is considered fully qualified because it has been resolved
        /// to the namespaces of the data source where each sub-component may have been mapped from a view key path to a data key path.
        /// This means that the path may not be recognizable on casual observation relative to the key path variables
        /// embedded in views and sub-views.
        /// </para>
        /// <para>
        /// Note that a key path for a collection is useful for regenerating an entire collection. If the object is a collection,
        /// <see cref="GetValue(string)"/> returns an <see cref="IEnumerable{T}"/> that can be used to enumerate the key paths of all children in the collection.
        /// </para>
        /// </remarks>
        /// <param name="resolvedKeyPath">Prefix for a collection</param>
        /// <returns>object of arbitrary data type or complexity, but will most often be a primitive type.</returns>
        object GetValue(string resolvedKeyPath);

        /// <summary>
        /// Set the value associated with the specified key path
        /// </summary>
        /// <remarks>
        /// <para>
        /// CAUTION: If isAtomicChange is omitted or passed as false, there MUST be a DataChangeSetBegin() and DataChangeSetEnd() method
        /// calls bracketing this. If not done, certain DataConsumers that only process as a batch (like TMPro) will not update
        /// the presented information.
        /// </para>
        /// <para>
        /// This is currently useful when the data embedding is being used for local, realtime changes of presented information.
        /// </para>
        /// <para>
        /// The fullyQualifiedPrefix is provided for lists (or collection of lists) to find the correct instance of a specific local key path.
        /// If not processing a collection, this can be either null or "". It is considered fully qualified because it has been resolved
        /// to the namespaces of the data source where each sub-component may have been mapped from a view key path to a data key path.
        /// This means that the path may not be recognizable on casual observation relative to the key path variables
        /// embedded in views and sub-views.
        /// </para>
        /// <para>
        /// Although not fully implemented yet, it could also be used to persist changes to a back-end data store of some kind,
        /// such as might be useful for a data entry form.
        /// </para>
        /// </remarks>
        /// <param name="resolvedKeyPath">The fully resolved key path for the variable to change.</param>
        /// <param name="value">The new value to set at that key path.</param>
        /// <param name="isAtomicChange">Whether this is the only change and should be automatically be bracketed by DataChangeSetBegin/End().</param>
        void SetValue(string resolvedKeyPath, object value, bool isAtomicChange = false);

        /// <summary>
        /// Add a new data consumer listener for the specified key path
        /// </summary>
        /// <remarks>
        /// Specify a listener for <paramref name="resolvedKeyPath"/>, such as when the data represented by <paramref name="resolvedKeyPath"/> changes,
        /// the NotifyDataChanged() method of the IDataConsumer will be called.
        /// </remarks>
        /// <param name="resolvedKeyPath">The key path for the data to monitor for changes.</param>
        /// <param name="dataConsumer">The data consumer that is to be notified.</param>
        void AddDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer);

        /// <summary>
        /// Remove a data consumer listener for the specified key path
        /// </summary>
        /// <remarks>
        /// Remove a previously added listener for the specified <paramref name="resolvedKeyPath"/> such that no
        /// further notifications will be provided. Note that this will not impact
        /// other data consumers that may have also registered listeners.
        /// </remarks>
        /// <param name="resolvedKeyPath">The key path to stop monitoring.</param>
        /// <param name="dataConsumer">The data consumer to remove for that key path.</param>

        void RemoveDataConsumerListener(string resolvedKeyPath, IDataConsumer dataConsumer);

        /// <summary>
        /// Determine if  key path represents a data collection
        /// </summary>
        /// <remarks>
        /// When a data consumer is notified of a data change at the specified <paramref name="resolvedKeyPath"/>, this
        /// can be used to determine if that object represents a collection. The collection can
        /// be of any type that can be accessed reliably via an index value. This is useful for
        /// auto-generating and presenting a collection that contains variable data,
        /// like a collection of users, or playlists.
        /// </remarks>
        /// <param name="resolvedKeyPath">The key path of data to be checked.</param>
        /// <returns>A Boolean indication of whether that <paramref name="resolvedKeyPath"/> represents a collection.</returns>
        bool IsCollectionAtKeyPath(string resolvedKeyPath);

        /// <summary>
        /// If a key path represents a collection, return the number of items in that collection.
        /// </summary>
        /// <remarks>
        /// This can be used by a data consumer to pre-allocate resources to receive the
        /// collection, and also to determine the valid index range of 0 to N-1 when using
        /// GetNthCollectionKayPathAt() to retrieve entries in the collection.
        /// </remarks>
        /// <param name="resolvedKeyPath">The key path of data to be checked.</param>
        /// <returns>integer size of the collection. An empty collection is valid.</returns>
        int GetCollectionCount(string resolvedKeyPath);

        /// <summary>
        /// If a key path represents a collection, return a collection key path for the entry at index <paramref name="n"/>.
        /// </summary>
        /// <remarks>
        /// The returned entry can be of arbitrary type and complexity and may even itself contain
        /// collections.
        /// </remarks>
        /// <param name="resolvedKeyPath">The key path of the collection itself.</param>
        /// <param name="n">Get the key path for the entry at this index.</param>
        /// <returns>The fully resolved key path for the nth item in a collection.</returns>
        string GetNthCollectionKeyPathAt(string resolvedKeyPath, int n);

        /// <summary>
        /// Get an enumerable collection of key path strings for the specified collection index range.
        /// </summary>
        /// <remarks>
        /// This is useful for list virtualization where you only fetch the sub-portion of the list that is
        /// currently visible.
        /// </remarks>
        /// <param name="resolvedKeyPath">The key path of the collection itself.</param>
        /// <param name="rangeStart">The index of the first element of the desired range.</param>
        /// <param name="rangeCount">The number of entries to return, (or fewer if end of collection is reached).</param>
        /// <returns>A string enumerable for all the fully resolved key paths in the specified range.</returns>
        IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount);

        /// <summary>
        /// Begin a data change set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default behavior is to notify all registered data consumers that a change set has begun.
        /// </para>
        /// <para>
        /// This is mainly exposed in this interface to allow for nested data sets where a
        /// super data set has child data sets with unknown concrete implementations, and
        /// wants to synchronize set boundaries.
        /// </para>
        /// </remarks>
        void DataChangeSetBegin();

        /// <summary>
        /// End a data change set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default behavior is to notify all registered data consumers that a change set has begun.
        /// </para>
        /// <para>
        /// This is mainly exposed in this interface to allow for nested data sets where a
        /// super data set has child data sets with unknown concrete implementations, and
        /// wants to synchronize set boundaries.
        /// </para>
        /// </remarks>
        void DataChangeSetEnd();

        /// <summary>
        /// Inform data source that its data has changed.
        /// </summary>
        /// <remarks>
        /// This is used in situations where <see cref="SetValue"/> is not being used to modify
        /// the managed data, and the data source is not otherwise aware that the data
        /// has changed. This directly propagates the notification to all data consumers
        /// who are listening for this specific key path.
        /// </remarks>
        /// <param name="keyPath">The resolved keypath of the data that has changed.</param>
        /// <param name="value">The value of the changed item</param>
        /// <param name="changeType">The type of change that has occurred.</param>
        /// <param name="isAtomicChange">Is this the only change notification in a set of related changes.</param>
        void NotifyDataChanged(string keyPath, object value, DataChangeType changeType, bool isAtomicChange);

        /// <summary>
        /// Notify all listeners that data has changed.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method is useful for batching multiple changes before notifying data consumers. An example situation
        /// is when initially populating a static dataset before notifying data consumers of the availability of data. For example:
        /// </para>
        /// <code>
        ///     SetValue( keypath1, value1 );
        ///     ...
        ///     SetValue( keypathn, valuen );
        ///     DataChangeSetBegin();
        ///     NotifyAllChanged();
        ///     DataChangeSetEnd();
        /// </code>
        /// <para>
        /// Note that this currently will be less efficient than triggering changes on individual data items as they occur.
        /// In the current implementation, all listeners will be notified even if that particular data items has not
        /// actually been changed. In future versions, a list will be maintained of all changed items to make this
        /// option more efficient.
        /// </para>
        /// </remarks>
        void NotifyAllChanged(DataChangeType dataChangeType = DataChangeType.DatumModified, IDataConsumer whichConsumer = null);

        /// <summary>
        /// Is data available in the data source.
        /// </summary>
        /// <remarks>
        /// This is useful for data sources that may need time to fetch data and may not be able to provide data yet
        /// for a <see cref="GetValue"/> method call.
        /// </remarks>
        bool IsDataAvailable();
    }
}
#pragma warning restore CS1591