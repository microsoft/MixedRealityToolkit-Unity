// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Interface for placing items from a list into the gameobject hierarchy in a meaningful way.
    /// </summary>
    ///
    /// <remarks>
    /// An object implementing this interface can be dependency injected into the DataConsumerCollection (or
    /// similar) Component via the Unity inspector.
    ///
    /// In addition to controlling how a list is visualized, it can also be used to manage list pagination, virtualization
    /// and predictive prefetching.
    ///
    /// When a list has changed, this object will be notified. It can then request a range of game objects to be provided for placement,
    /// where each game object has been modified to reflect the data associated with that entry in the list. The game object will
    /// typically be a prefab of arbitrary complexity.
    ///
    /// </remarks>
    public interface IDataCollectionItemPlacer
    {
        /// <summary>
        /// Attach item placer and prepare for use
        /// </summary>
        /// <remarks>
        /// Item placers may be attached / detached at any time and this
        /// is an opportunity to allocate any needed resources
        /// </remarks>
        void Attach();

        /// <summary>
        /// Detach item placer and remove any visible items
        /// </summary>
        /// <remarks>
        /// Item placers may be attached / detached at any time and this
        /// is where any remaining visible items should be removed and
        /// returned for re-use.
        /// </remarks>
        void Detach();

        /// <summary>
        /// set the data consumer for a collection that can provide gameobjects to be placed. This will normally be set by the data consumer
        /// that was assigned this placer during initialization phase.
        /// </summary>
        ///
        /// <param name="dataConsumerCollection">Data consumer for a collection that will provide game objects to place.</param>
        void SetDataConsumerCollection(IDataConsumerCollection dataConsumerCollection);

        /// <summary>
        /// Start placement for the requested range of game objects.
        ///
        /// This can be used to reset any state information in preparation for receiving the new items.
        ///
        /// All PlaceItem() method calls will occur within StartPlacement() and EndPlacement() method calls.
        /// </summary>
        void StartPlacement();

        /// <summary>
        /// Place a game object into the experience previously requested via IDataConsumer.RequestCollectionItems()
        /// </summary>
        ///
        /// <remarks>
        /// All calls to this method will be within a StartPlacement() and an EndPlacement() call.
        ///
        /// The request id will always be the request Id provided to the  IDataConsumer.RequestCollectionItems()
        /// method that triggered the calls to this method. This can be used to associate additional data in a
        /// complex scenario where there may be more than one data consumer associated with this item placer.
        ///
        /// Note that the range start and range count could have been provided via state data associated with the
        /// requestRef, but to simplify the most common uses, it is provided explicitly.
        /// </remarks>
        ///
        /// <param name="requestRef">Any desired private object, initially provided to RequestCollectionItems() </param>
        /// <param name="itemIndex">The absolute item index in the data source array.</param>
        /// <param name="itemKeyPath">The localKeypath identifier for the item at the received index.</param>
        /// <param name="itemGO">The game object created using  the data at the specified item index.</param>
        void PlaceItem(object requestRef, int itemIndex, string itemKeyPath, GameObject itemGO);

        /// <summary>
        /// End placement for the requested range of game objects
        /// </summary>
        void EndPlacement();

        /// <summary>
        /// Notification that the collection being managed by the data consumer for this object has changed.
        /// </summary>
        /// <remarks>
        /// This method will typically trigger a IDataConsumer.RequestCollectionItems() method call to
        /// request all or a subset of the items being managed in the collection.  This back and forth
        /// allows for precise control of paging and virtualization to optimize the presentation of
        /// information.</remarks>
        ///
        /// <param name="dataChangeType">The nature of the data change, typically CollectionItemAdded, or CollectionItemRemoved.</param>
        /// <param name="localKeypath">The keypath of the collection.</param>
        /// <param name="value">A value relevant to the type of change. For a collection item change, this is the index of the item.</param>
        void NotifyCollectionDataChanged(DataChangeType dataChangeType, string localKeypath, object value);

        /// <summary>
        /// Get current page number for first visible item.
        /// </summary>
        /// <returns>The page number (zero based)</returns>
        int GetPageNumber();

        /// <summary>
        /// Get the total number of pages in the collection based on current page size.
        /// </summary>
        /// <returns>Number of items in the collection.</returns>
        int GetTotalPageCount();

        /// <summary>
        /// Scroll forward one visible page of items, if possible, or to end of collection if partial page.
        /// </summary>
        void PageForward();

        /// <summary>
        /// Scroll back one page of visible items, if possible, or to beginning if partial page.
        /// </summary>
        void PageBackward();

        /// <summary>
        /// Scroll forward one item if possible.
        /// </summary>
        void MoveToNextItem();

        /// <summary>
        /// Scroll back one item if possible.
        /// </summary>
        void MoveToPreviousItem();

        /// <summary>
        /// Move visible data window by itemCount items forward or backward
        /// </summary>
        /// <remarks>
        /// Note if objects are not removed immediately, they must be removed later (such as after a transition effect)
        /// using PurgeAllRemovableGameObjects() or PurgeRemovableGameObjectRange()</remarks>
        /// <param name="itemCount">THe number of items to scroll. Negative=previous. Positive=next.</param>
        /// <param name="removeExitingObjectsNow">Remove no longer visible objects immediately and return back to object pool.</param>
        /// <returns>Actual number of items scrolled. Note: Always positive for previous or next.</returns>
        int MoveRelative(int itemCount, bool removeExitingObjectsNow = true);

        /// <summary>
        /// Scroll visible data window to the specified first visible item
        /// </summary>
        /// <remarks>
        /// Note if objects are not removed immediately, they must be removed later (such as after a transition effect)
        /// using PurgeAllRemovableGameObjects() or PurgeRemovableGameObjectRange()
        /// </remarks>
        /// <param name="firstItem">The first visible item to navigate to.</param>
        /// <param name="purgeExitingObjectsNow">Purge no longer visible objects immediately and return back to object pool.</param>
        /// <returns>Actual distance moved from current position.</returns>

        int MoveAbsolute(int firstItem, bool purgeExitingObjectsNow = true);
    }
}
