// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data pool of objects that can be used for object re-use.
    ///
    /// This is designed to reduce memory allocations and hence
    /// the frequency and duration of garbage collections. It can
    /// also reduce the instantiation time of a prefab when populating
    /// large lists.
    /// </summary>
    ///
    public interface IDataObjectPool
    {
        /// <summary>
        /// Establish the maximum number of items to allow in the data object pool.
        /// </summary>
        /// <remarks>
        /// This maximum size is for the combination of unused objects and prefetched objects.
        /// </remarks>
        /// <param name="maxSize">The new maximum size.</param>
        /// <param name="resizeNow">Whether to resize all at once, or as items are added/removed.</param>
        void SetMaximumPoolSize(int maxSize, bool resizeNow);

        /// <summary>
        /// Determine if the pool is currently empty.
        /// </summary>
        /// <remarks>
        /// It is considered empty if it contains no unused objects and no prefetched objects.</remarks>
        /// <returns>True if empty.</returns>
        bool IsEmpty();

        /// <summary>
        /// Determine if the pool is currently full and can't accept more objects.
        /// </summary>
        /// <remarks>
        /// It is considered full if it contains the maximum allowed combination of unused and prefetched objects.
        ///
        /// Note that if there are prefetched objects, it may be possible to reuse these.
        /// </remarks>
        /// <returns>True if full.</returns>
        bool IsFull();

        /// <summary>
        /// Get the maximum allowed number of unused and prefetched objects.
        /// </summary>
        /// <returns>The maximum allowed objects.</returns>
        int MaximumObjectsCount();

        /// <summary>
        /// Get the number of prefetched objects in the pool.
        /// </summary>
        /// <returns>Number of prefetched objects.</returns>
        int PrefetchedObjectsCount();

        /// <summary>
        /// Check if an object is already in the prefab prefetch pool.
        /// </summary>
        /// <param name="id">The identifier for the object to check.</param>
        /// <returns>True if object with specified id was found in prefetch pool.</returns>
        bool ObjectIsPrefetched(int id);

        /// <summary>
        /// Get the number of unused objects in the pool.
        /// </summary>
        /// <remarks>
        /// This count does not include any prefetched objects. Note that it is possible
        /// to forfeit a prefetched object and re-use it at any time.
        /// </remarks>
        /// <returns>Number of prefetched objects.</returns>
        int UnusedObjectsCount();


        /// <summary>
        /// Return an object to the pool in a reset, unused state.
        /// </summary>
        /// <remarks>
        /// If this method returns false, there was no space for the object and it is the caller's responsibility
        /// to destroy the object.
        ///
        /// To place a pre-initialized object prefab into the pool, use AddPrefetchedObjectToPool instead.
        /// </remarks>
        /// <param name="objectToReturn">The object to return to the pool.</param>
        /// <returns>True if there was space for the prefab.</returns>
        bool ReturnObjectToPool(object objectToReturn);

        /// <summary>
        /// Get an object from the pool, with option to forfeit a prefetched object
        /// </summary>
        /// <remarks>
        /// If canReusePrefetchedObject is true and there are no unused objects, then it will forfeit one of
        /// the prefetched objects to satisfy the request.
        ///
        /// Note that there is not currently a least-recently-used, or least likely to be used algorithm for
        /// forfeiting a pre-fetched object.
        /// </remarks>
        /// <param name="canReusePrefetchObject">If no unused items are found, allow re-use of an existing prefetched item.</param>
        /// <returns>The object to re-use.</returns>
        object GetObjectFromPool(bool canReusePrefetchObject = true);

        /// <summary>
        /// Add a prefetched object to the pool with the specified ID.
        /// </summary>
        /// <remarks>
        /// A typical scenario would first call TryGetPrefetchedObject to get either an already prefetched
        /// object or if it does not exist, to get an unused object to then prefetch. Then, once
        /// prefetched, add it to the object pool using this method.
        ///
        /// The object can either be added as the newest or the oldest. Adding one as the oldest is useful when
        /// you may want to speculatively add an existing item for reuse even though no one has asked for that item to be
        /// prefetched.
        /// </remarks>
        /// <param name="id">The id of object to add.</param>
        /// <param name="objectToReturn">The prefetched object to add.</param>
        /// <param name="asNewest">If true, add as newest item. If false, add as oldest item.</param>
        /// <returns>True if the object could be added to the pool. False if not added, in which case caller should dispose of or destroy appropriately.</returns>
        bool AddPrefetchedObjectToPool(int id, object objectToReturn, bool asNewest = true);

        /// <summary>
        /// Return all prefectehd objects to the pool for reuse.
        /// </summary>
        /// <remarks>
        /// This is useful for situations where a collection is being entirely reset or reorganized where
        /// the ids associated with gameobjects are all incorrect.
        /// </remarks>
        /// <param name="processObject">Apply an action to each object being returned. This allows them to be reset.</param>
        void ReturnAllPrefetchedObjects(Action<int, object> processObject);

        /// <summary>
        /// Get prefetched object for specified id and if not found,
        /// return a generic non-prefetched object instead.
        /// </summary>
        /// <param name="id">The id of the prefetched object to attempt to retrieve.</param>
        /// <param name="returnedObject">Either prefetched or generic object</param>
        /// <returns>True if prefetched object found. False if not found and generic object returned.</returns>
        bool TryGetPrefetchedObject(int id, out object returnedObject);
    }
}
