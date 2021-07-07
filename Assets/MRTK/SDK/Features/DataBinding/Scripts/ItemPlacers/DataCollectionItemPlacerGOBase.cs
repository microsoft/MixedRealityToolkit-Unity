// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Data
{

    /// <summary>
    /// A data collection item placer base implementation that supports paging.
    /// 
    /// This is a base object that can be derived from to support more complex scenarios.
    /// 
    /// A typical item placer will populate a UX elemented designed to present lists or grids of
    /// items, and typically also supports paging and/or scrolling for larger lists.
    /// 
    /// TODO: Make a simpler GO base class that is used in DataConsumerCollection so that it is not
    /// assumed what functionality may be desired in an item placer.
    /// 
    /// </summary>

    
    public abstract class DataCollectionItemPlacerGOBase : MonoBehaviour, IDataCollectionItemPlacer
    {
        [SerializeField]
        protected int maximumVisibleItems = 999999;

        [SerializeField]
        protected string requestId = "";


        protected int _firstVisibleItem = 0;
        protected int _numVisibleItems = 0;

        protected IDataConsumerCollection _dataConsumerCollection;

        protected Dictionary<int, GameObject> _visibleGameObjectsByIndex = new Dictionary<int, GameObject>();
        protected Dictionary<int, GameObject> _removableGameObjectsByIndex = new Dictionary<int, GameObject>();

        /// <summary>
        /// Purge all game objects that have been queued for removal.
        /// </summary>
        /// <remarks>
        /// Normally items are removed as soon as they have been scrolled out of visibility, but
        /// to allow for transition effects, that default behavior for all scroll related methods
        /// can be delayed and the items can be manually purged with this method.
        /// 
        /// NOTE: if objects are not purged, they will indefinitely be referenced by this item placer,
        /// creating an effective memory leak.
        /// 
        /// This is useful for purging all previously visible items, such as
        /// after a transition effect, such as fade out, is done.
        /// </remarks>
        /// <param name="firstItemIdx">Index into collection of first item to purge.</param>
        /// <param name="itemCount">Number of items to purge.</param>
        public void PurgeAllRemovableGameObjects()
        {
            foreach (KeyValuePair<int, GameObject> entry in _removableGameObjectsByIndex)
            {
                _dataConsumerCollection.ReturnGameObjectForReuse(entry.Key, entry.Value);
            }
            _removableGameObjectsByIndex.Clear();
        }

        /// <summary>
        /// Purge a range of game objects that have been queued for removal.
        /// </summary>
        /// <remarks>
        /// Normally items are removed as soon as they have been scrolled out of visibility, but
        /// to allow for transition effects, that default behavior for all scroll related methods
        /// can be delayed and the items can be manually purged with this method.
        /// 
        /// NOTE: if objects are not purged, they will indefinitely be referenced by this item placer,
        /// creating an effective memory leak.
        /// 
        /// This is useful for purging a series of previously visible items, such as
        /// after a transition effect, such as fade out, is done.
        /// </remarks>
        /// <param name="firstItemIdx">Index into collection of first item to purge.</param>
        /// <param name="itemCount">Number of items to purge.</param>
        public void PurgeRemovableGameObjectRange(int firstItemIdx, int itemCount)
        {
            int maxItemIdx = firstItemIdx + itemCount;
            for (int idx = firstItemIdx; idx < maxItemIdx; idx++)
            {
                if (_removableGameObjectsByIndex.ContainsKey(idx))
                {
                    _dataConsumerCollection.ReturnGameObjectForReuse(idx, _removableGameObjectsByIndex[idx]);

                    _removableGameObjectsByIndex.Remove(idx);
                }
            }
        }



        /// <summary>
        /// Set the Data Consumer that is providing items for this item placer.
        /// </summary>
        /// <param name="removeExitingObjectsNow">Release objects going out of visibility back to object pool immediately.</param>
        /// <returns>Actual number of items scrolled.</returns>
        public void SetDataConsumerCollection(IDataConsumerCollection dataConsumerCollection)
        {
            _dataConsumerCollection = dataConsumerCollection;
        }


        /// <summary>
        /// Scroll forward one page if possible, or to end of list if partial page.
        /// </summary>
        public void ScrollNextPage()
        {
            Scroll(GetMaxVisibleItemCount());
        }

        /// <summary>
        /// Scroll back one page if possible, or to beginning if partial page.
        /// </summary>
        public void ScrollPreviousPage()
        {
            Scroll(-GetMaxVisibleItemCount());
        }


        /// <summary>
        /// Scroll forward one item if possible.
        /// </summary>
        public void ScrollNextItem()
        {
            Scroll(1);
        }

        /// <summary>
        /// Scroll back one item if possible.
        /// </summary>
        public void ScrollPreviousItem()
        {
            Scroll(-1);
        }



        /// <summary>
        /// Scroll by itemCount items forward or backward
        /// </summary>
        /// <remarks>
        /// Note if objects are not removed immediately, they must be removed later (such as after a transition effect)
        /// using PurgeAllRemovableGameObjects() or PurgeRemovableGameObjectRange()</remarks>
        /// <param name="itemCount">THe number of items to scroll. Negative=previous. Positive=next.</param>
        /// <param name="removeExitingObjectsNow">Remove no longer visible objects immediately and return back to object pool.</param>
        /// <returns>Actual number of items scrolled. Note: Always positive for previous or next.</returns>
        public int Scroll(int itemCount, bool removeExitingObjectsNow = true)
        {
            int actualScrollAmount = 0;
            int firstItemToRequest;
            int newFirstVisibleItem = _firstVisibleItem;
            int firstItemToRemove;
            int firstItemToReposition = _firstVisibleItem;
            int numItemsToReposition = _numVisibleItems;


            if (itemCount < 0)
            {
                // scroll previous

                actualScrollAmount = Math.Min(-itemCount, _firstVisibleItem);
                newFirstVisibleItem -= actualScrollAmount;
                firstItemToRequest = _firstVisibleItem - actualScrollAmount;
                firstItemToRemove = _firstVisibleItem + _numVisibleItems - actualScrollAmount;
            } 
            else
            {
                // scroll next

                int maxFirstVisibleItem = GetTotalItemCount() - GetMaxVisibleItemCount();

                actualScrollAmount = Math.Min(itemCount, maxFirstVisibleItem - _firstVisibleItem);

                firstItemToRemove = _firstVisibleItem;
                firstItemToRequest = _firstVisibleItem + _numVisibleItems;
                newFirstVisibleItem += actualScrollAmount;
            }

            if (actualScrollAmount > 0)
            {
                QueueGameObjectsForRemoval(firstItemToRemove, actualScrollAmount);
                _firstVisibleItem = newFirstVisibleItem;
                _dataConsumerCollection?.RequestCollectionItems(this, firstItemToRequest, actualScrollAmount, requestId);

                if (removeExitingObjectsNow )
                {
                    PurgeRemovableGameObjectRange(firstItemToRemove, actualScrollAmount);
                }
            }

            // for a partial scroll, we need to reposition the ones that are still visible, but are now
            // potentially in a new location

            if ( firstItemToReposition < firstItemToRemove)
            {
                numItemsToReposition = Math.Min(numItemsToReposition, firstItemToRemove - firstItemToReposition);
            } 
            else
            {
                numItemsToReposition -= actualScrollAmount;
                firstItemToReposition = firstItemToRemove + actualScrollAmount;
            }
            if ( numItemsToReposition > 0)
            {
                RepositionItems(firstItemToReposition, numItemsToReposition);
            }

            return actualScrollAmount;
        }


        /// <summary>
        /// Are visible items at beginning of list
        /// </summary>
        /// <remarks>
        /// If at start of list, then scroll / page previous option is not available.</remarks>
        /// <returns></returns>
        public bool IsAtStart()
        {
            return _firstVisibleItem == 0;
        }


        /// <summary>
        /// Are visible items at the end of list
        /// </summary>
        /// <remarks>
        /// If at end of list, then scroll / page next option is not available.
        /// </remarks>
        /// <returns></returns>
        public bool IsAtEnd()
        {
            return _firstVisibleItem >= GetTotalItemCount() - GetMaxVisibleItemCount();
        }



        /// <summary>
        /// Get the list index of the first visible item
        /// </summary>
        /// <remarks>
        /// Note that this is the current logical start of visible items, but not all of the
        /// data may have been received yet.
        /// </remarks>
        /// <returns>Index number of first logically visible item.</returns>
        public int GetFirstVisibleItem()
        {
            return _firstVisibleItem;
        }

        /// <summary>
        /// Get current page number for first visible item.
        /// </summary>
        /// <returns>The page number (zero based)</returns>
        public int GetPageNumber()
        {
            return _firstVisibleItem / maximumVisibleItems;
        }


        /// <summary>
        /// Get nominal number of items per logical page.
        /// </summary>
        /// <remarks>
        /// Note that the actual number if items may depend on the nature of the
        /// items (such as the mix of object sizes) or whether fewer items are available than
        /// can be shown.
        /// </remarks>
        /// <returns>The number if items per page.</returns>
        public virtual int GetItemCountPerPage()
        {
            return maximumVisibleItems;
        }

        /// <summary>
        /// Get the total number of items in the collection.
        /// </summary>
        /// <returns>Number of items in the collection.</returns>
        public virtual int GetTotalItemCount()
        {
            return _dataConsumerCollection?.GetCollectionItemCount() ?? 0;
        }


        #region IDataCollectionItemPlacer method implementations

        public virtual void StartPlacement() {
            maximumVisibleItems = _dataConsumerCollection.GetCollectionItemCount();
        }

        public virtual void PlaceItem(string requestId, int indexRangeStart, int indexRangeCount, int itemIndex, GameObject itemGO)
        {
            if (itemIndex >= _firstVisibleItem && itemIndex < _firstVisibleItem + GetMaxVisibleItemCount() )
            {
                itemGO.SetActive(true);
                AddGameObject(itemIndex, itemGO);
                PlaceVisibleItem( requestId, indexRangeStart, indexRangeCount, itemIndex, itemGO);
            }
        }


        protected virtual void RepositionItems(int firstIndexToReposition, int numItems )
        {
            for( int itemIndex = firstIndexToReposition; itemIndex < firstIndexToReposition + numItems; itemIndex++ )
            {
                PlaceVisibleItem(requestId, firstIndexToReposition, numItems, itemIndex, _visibleGameObjectsByIndex[itemIndex]);
            }
        }


        public virtual void EndPlacement() {
            PredictivelyLoadItems();
        }



        public virtual void NotifyCollectionDataChanged(DataChangeType dataChangeType) {
            // default behavior is to ask for all items in the collection with empty string as request ID.
            if (dataChangeType == DataChangeType.CollectionItemAdded)
            {
                if (_numVisibleItems < GetMaxVisibleItemCount())
                {
                    // If adding items and num visible is not currently the maximum possible visible, then
                    // let's add the new item to what's visible.
                    int firstItem = _firstVisibleItem + _numVisibleItems;
                    int numItems = GetMaxVisibleItemCount() - _numVisibleItems;
                    _numVisibleItems += numItems;
                    _dataConsumerCollection?.RequestCollectionItems(this, firstItem, numItems, requestId);
                }
            }
            else
            {
                // For misc mods or deletions, it's safer to just re-fetch all at current scroll position since we don't
                // necessarily know where a deletion or modification occured.
                RequestVisibleItems();
            }
        }

        #endregion IDataCollectionItemPlacer method implementations

        #region Abstract methods

        /// <summary>
        /// Place a visible item into the experience
        /// </summary>
        /// <remarks>
        /// This class must be overridden to actually do the insertion of the specified game object into the scene.
        /// 
        /// The index range is provided in case this is useful for determining the relative location of this item
        /// to the total number of items requested.
        /// </remarks>
        /// <param name="requestId">Request ID provided at the time of the request.</param>
        /// <param name="indexRangeStart">The start of the index range of items requested in this batch.</param>
        /// <param name="indexRangeCount">The number of items requested in this batch</param>
        /// <param name="itemIndex">The index of this item.</param>
        /// <param name="itemGO">The game object (usually a prefab) of the item to place.</param>
        public abstract void PlaceVisibleItem(string requestId, int indexRangeStart, int indexRangeCount, int itemIndex, GameObject itemGO);

        #endregion Abstract methods


        protected void RequestVisibleItems()
        {
            _numVisibleItems = Math.Min(GetMaxVisibleItemCount(), GetTotalItemCount() - _firstVisibleItem);

            if (_numVisibleItems > 0)
            {
                _dataConsumerCollection?.RequestCollectionItems(this, _firstVisibleItem, _numVisibleItems, requestId);
            }
        }


        protected virtual void PredictivelyLoadItems()
        {

        }

        protected void AddGameObject(int indexAsId, GameObject go)
        {
            if (_removableGameObjectsByIndex.ContainsKey(indexAsId))
            {
                _removableGameObjectsByIndex.Remove(indexAsId);
            }

            _visibleGameObjectsByIndex[indexAsId] = go;
        }


        protected void QueueGameObjectsForRemoval(int firstItemIdx, int numItems)
        {
            int maxItem = firstItemIdx + numItems;
            for (int indexAsId = firstItemIdx; indexAsId < maxItem; indexAsId++)
            {
                if (_visibleGameObjectsByIndex.ContainsKey(indexAsId) )
                {
                    GameObject objectToRemove = _visibleGameObjectsByIndex[indexAsId];

                    _visibleGameObjectsByIndex.Remove(indexAsId);
                    _removableGameObjectsByIndex[indexAsId] = objectToRemove;
                }
            }
        }


        protected virtual int GetMaxVisibleItemCount()
        {
            // override this to return the actual currently visible item count
            return Math.Min(GetTotalItemCount(), GetItemCountPerPage());
        }
    }
}
