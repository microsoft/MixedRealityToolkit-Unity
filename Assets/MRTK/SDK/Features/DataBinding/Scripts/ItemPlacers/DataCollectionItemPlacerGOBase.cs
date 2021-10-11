// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;


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
        [Tooltip("(Optional) Private request reference or ID that is provided with every request for collection items to correlate the PlaceItem calls to the original request.")]
        [SerializeField]
        protected string requestRef = "";

        [Tooltip("Enable this for placement strategies that depend on the order of child game objects instead of the index of the item.")]
        [SerializeField]
        protected bool keepGameObjectsInIndexOrder = true;

        [Tooltip("Enable this to advance full page count items even if this causes last page to be partially empty. ")]
        [SerializeField]
        protected bool lastPageCanBePartial = true;

        [Tooltip("Event object to receive a variety of events when collection state changes. ")]
        [SerializeField]
        protected DataCollectionEventsGOBase collectionEvents;

        [Tooltip("Turn on debug messages. ")]
        [SerializeField]
        protected bool debugMode = false;


        protected int _totalItemCount = 0;
        protected int _firstVisibleItem = 0;
        protected int _numVisibleItems = 0;

        // Stats for predictive prefetch
        protected int _lastRequestRangeStart;
        protected int _lastRequestRangeCount;
        protected int _lastRequestDirection = 1;

        // Track last state to ensure events are always fired correctly.
        // middle and not empty could be gleaned from the other 3, but
        // this ensures they are fired correctly every time this
        // component is enabled.

        protected bool _lastEventStateAtStart = false;
        protected bool _lastEventStateAtEnd = false;
        protected bool _lastEventStateInMiddle = false;
        protected bool _lastEventCollectionEmpty = false;
        protected bool _lastEventCollectionNotEmpty = false;

        protected bool _lastEventStateCanGoBackward = false;
        protected bool _lastEventStateCanGoForward = false;

        protected IDataConsumerCollection _dataConsumerCollection;

        protected enum State
        {
            Requested = 0,          // Requested but not yet received
            Visible,                // Currently visible
            Prefetched,             // Prefetched but not yet visible
            Removable,              // Removable
            StashRemovable          // Temporarily stash removables that are still being fetched
        };


        protected class ItemInfo
        {
            public int itemIndex;
            public string itemKeypath;
            public State state;
            public GameObject gameObject;

            public ItemInfo(int index, State theState, string keyPath, GameObject go)
            {
                itemIndex = index;
                state = theState;
                itemKeypath = keyPath;
                gameObject = go;
            }
        };


        protected Dictionary<State, Dictionary<int, ItemInfo>> _itemsByState = new Dictionary<State, Dictionary<int, ItemInfo>>();
        protected Dictionary<int, State> _itemStateByIndex = new Dictionary<int, State>();

        /// <summary>
        /// Item placer is going into attached state either after inialization or after being dormand in game object pool.
        /// </summary>
        /// <remarks>
        /// NOTE: if derived class needs to do more Attach readiness logic, this can be done
        /// by overriding AttachItemPlacer
        /// </remarks>
        /// 
        public void Attach()
        {
            _totalItemCount = 0;

            _firstVisibleItem = 0;
            _numVisibleItems = 0;

            _lastEventStateAtStart = false;
            _lastEventStateAtEnd = false;
            _lastEventStateInMiddle = false;
            _lastEventCollectionEmpty = false;
            _lastEventCollectionNotEmpty = false;
            _lastEventStateCanGoBackward = false;
            _lastEventStateCanGoForward = false;

            FindNearestCollectionEvents();

            foreach (State state in Enum.GetValues(typeof(State)))
            {
                _itemsByState[state] = new Dictionary<int, ItemInfo>();
            }
            AttachItemPlacer();
            if (collectionEvents != null)
            {
                collectionEvents.OnAttach();
            }
        }


        /// <summary>
        /// Item placer is going into detached state either before returning in game object pool or destroy
        /// </summary>
        /// <remarks>
        /// NOTE: if derived class needs to do more Detach readiness logic, this can be done
        /// by overriding AttachItemPlacer
        /// </remarks>
        public void Detach()
        {
            DetachItemPlacer();
            PurgeAllItems();
            if (collectionEvents != null)
            {
                collectionEvents.OnDetach();
            }
        }

        /// <summary>
        /// Perform additional attach prep in derived class
        /// </summary>
        protected virtual void AttachItemPlacer()
        {

        }

 
        /// <summary>
        /// Perform additional detach teardown in derived class
        /// </summary>
        protected virtual void DetachItemPlacer()
        {

        }


        protected void CheckForEventsToTrigger()
        {
            if (collectionEvents != null)
            {
                bool newAtStart = IsAtStart();
                bool newAtEnd = IsAtEnd();
                bool newInMiddle = !newAtStart && !newAtEnd;
                bool newEmpty = GetTotalItemCount() == 0;
                bool newCanGoBackward = !newAtStart;
                bool newCanGoForward = !newAtEnd;

                if (!_lastEventStateAtStart && newAtStart)
                {
                    if (debugMode) { Debug.Log("Collection is at start."); }

                    collectionEvents.OnCollectionAtStart();
                }
                if (!_lastEventStateAtEnd && newAtEnd)
                {
                    if (debugMode) { Debug.Log("Collection is  at end."); }
                    collectionEvents.OnCollectionAtEnd();
                }

                // no in middle, and previously not in middle
                if (newInMiddle && !_lastEventStateInMiddle)
                {
                    if (debugMode) { Debug.Log("Collection is in middle."); }
                    collectionEvents.OnCollectionInMiddle();
                }

                // can go backward in list
                if (newCanGoBackward && !_lastEventStateCanGoBackward)
                {
                    if (debugMode) { Debug.Log("Collection can go backward."); }

                    collectionEvents.OnCollectionCanGoBackward();
                }

                // can go forward in list
                if (newCanGoForward && !_lastEventStateCanGoForward)
                {
                    if (debugMode) { Debug.Log("Collection can go forward."); }
              
                    collectionEvents.OnCollectionCanGoForward();
                }

                if (newEmpty && !_lastEventCollectionEmpty)
                {
                    if (debugMode) { Debug.Log("CCollection is Empty."); }
                    collectionEvents.OnCollectionEmpty();
                } 

                if (!newEmpty && !_lastEventCollectionNotEmpty)
                {
                    if (debugMode) { Debug.Log("Collection is not empty."); }
                    collectionEvents.OnCollectionNotEmpty();
                }

                _lastEventStateAtStart = newAtStart;
                _lastEventStateAtEnd = newAtEnd;
                _lastEventStateInMiddle = newInMiddle;
                _lastEventCollectionEmpty = newEmpty;
                _lastEventCollectionNotEmpty = !newEmpty;
                _lastEventStateCanGoBackward = newCanGoBackward;
                _lastEventStateCanGoForward = newCanGoForward;
            }
        }
        protected bool ItemExists(int indexAsId)
        {
            return _itemStateByIndex.ContainsKey(indexAsId);
        }


        protected ItemInfo FindItem(int indexAsId)
        {
            if (_itemStateByIndex.ContainsKey(indexAsId))
            {
                State state = _itemStateByIndex[indexAsId];
                if (_itemsByState.ContainsKey(state) && _itemsByState[state].ContainsKey(indexAsId))
                {
                    return _itemsByState[state][indexAsId];
                }
            }

            return null;
        }


        protected void AddItem(State state, int indexAsId, string keypath, GameObject go)
        {
            RemoveItem(indexAsId);

            _itemStateByIndex[indexAsId] = state;

            if (_itemsByState[state].ContainsKey(indexAsId) == false)
            {
                ItemInfo itemInfo = new ItemInfo(indexAsId, state, keypath, go);
                _itemsByState[state][indexAsId] = itemInfo;
            }
            else
            {
                DebugUtilities.LogVerbose("Item " + indexAsId + " is already in dictionary for CollectionItemPlacer." );

            }
        }

        protected void RemoveItem(int indexAsId)
        {
            if (_itemStateByIndex.ContainsKey(indexAsId))
            {
                State state = _itemStateByIndex[indexAsId];

                _itemsByState[state].Remove(indexAsId);
                _itemStateByIndex.Remove(indexAsId);
            }
        }

        protected void UpdateItem(int indexAsId, State newState, string itemKeyPath, GameObject newGO)
        {
            ItemInfo itemInfo = FindItem(indexAsId);
            if (itemInfo != null)
            {
                itemInfo.itemKeypath = itemKeyPath;
                itemInfo.gameObject = newGO;
                ChangeItemState(indexAsId, newState);
            }

        }

        protected void ChangeItemState(int indexAsId, State newState)
        {
            if (_itemStateByIndex.ContainsKey(indexAsId))
            {
                State oldState = _itemStateByIndex[indexAsId];
                ItemInfo itemInfo = _itemsByState[oldState][indexAsId];

                itemInfo.state = newState;
                _itemsByState[oldState].Remove(indexAsId);
                _itemsByState[newState][indexAsId] = itemInfo;

                _itemStateByIndex[indexAsId] = newState;
            }

        }


        protected void PurgeAllItems()
        {
            // TODO: Add logic to cancel any items requested but not received.
            PurgePrefetchedItems();
            PurgeAllVisibleAndRemovableItems();
        }


        protected void PurgePrefetchedItems()
        {
            if (_itemsByState.ContainsKey(State.Prefetched))
            {
                Dictionary<int, ItemInfo> prefetchedItems = _itemsByState[State.Prefetched];
                ItemInfo[] prefetchedItemInfos = GetCopyOfStateItems(prefetchedItems);

                foreach (ItemInfo itemInfo in prefetchedItemInfos)
                {
                    if (StashOrReturnItem(itemInfo))
                    {
                        _itemStateByIndex.Remove(itemInfo.itemIndex);
                    }
                }

                prefetchedItems.Clear();
            }
        }
    

        protected void PurgeAllVisibleAndRemovableItems()
        {
            if (_numVisibleItems > 0)
            {
                QueueGameObjectsForRemoval(_firstVisibleItem, _numVisibleItems);
                PurgeAllRemovableGameObjects();
                _numVisibleItems = 0;
                if (collectionEvents != null)
                {
                    collectionEvents.OnCollectionChanged();
                }
            }
        }

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

        public void PurgeAllRemovableGameObjects()
        {
            if (_itemsByState.ContainsKey(State.Removable))
            {
                Dictionary<int, ItemInfo> removableItems = _itemsByState[State.Removable];
                ItemInfo [] itemsInRemovable = GetCopyOfStateItems(removableItems);

                foreach (ItemInfo removableItem in itemsInRemovable)
                {
                    if (StashOrReturnItem(removableItem))
                    {
                        _itemStateByIndex.Remove(removableItem.itemIndex);
                    }
                }

                removableItems.Clear();
            }
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
            if (_itemsByState.ContainsKey(State.Removable))
            {
                int maxItemIdx = firstItemIdx + itemCount;
                Dictionary<int, ItemInfo> removableStateItems = _itemsByState[State.Removable];


                for (int idx = firstItemIdx; idx < maxItemIdx; idx++)
                {
                    if (removableStateItems.ContainsKey(idx))
                    {
                        ItemInfo itemInfo = removableStateItems[idx];

                        if (StashOrReturnItem(itemInfo))
                        {
                            RemoveItem(idx);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Makes copy if keys to allow modification of the dictionary during iteration
        /// </summary>
        /// <param name="itemsByState">one of the state item dicts.</param>
        /// <returns></returns>
        private ItemInfo[] GetCopyOfStateItems(Dictionary<int, ItemInfo> stateItems)
        {
            int numItems = stateItems.Count;
            int curItem = 0;
            ItemInfo[] itemsForState = new ItemInfo[numItems];

            foreach (KeyValuePair<int, ItemInfo> entry in stateItems)
            {
                itemsForState[curItem++] = entry.Value;
            }

            return itemsForState;
        }



        public void QueueGameObjectsForRemoval(int firstItemIdx, int numItems)
        {
            int maxItem = firstItemIdx + numItems;
            for (int indexAsId = firstItemIdx; indexAsId < maxItem; indexAsId++)
            {
                ChangeItemState(indexAsId, State.Removable);
            }
        }



        protected bool StashOrReturnItem(ItemInfo itemInfo)
        {
            if (itemInfo.state == State.Requested || itemInfo.gameObject == null)
            {
                ChangeItemState(itemInfo.itemIndex, State.StashRemovable);
                return false;
            }
            else
            {
                _dataConsumerCollection.ReturnGameObjectForReuse(itemInfo.itemIndex, itemInfo.gameObject);
                return true;
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
        public void PageForward()
        {
            MoveRelative(GetMaxVisibleItemCount());
            if (collectionEvents != null)
            {
                collectionEvents.OnCollectionPagedForward();
            }
        }

        /// <summary>
        /// Scroll back one page if possible, or to beginning if partial page.
        /// </summary>
        public void PageBackward()
        {
            MoveRelative(-GetMaxVisibleItemCount());
            if (collectionEvents != null)
            {
                collectionEvents.OnCollectionPagedBackward();
            }
        }


        /// <summary>
        /// Scroll forward one item if possible.
        /// </summary>
        public void MoveToNextItem()
        {
            MoveRelative(1);
            if (collectionEvents != null)
            {
                collectionEvents.OnCollectionScrolledForward();
            }
        }

        /// <summary>
        /// Scroll back one item if possible.
        /// </summary>
        public void MoveToPreviousItem()
        {
            MoveRelative(-1);
            if (collectionEvents != null)
            {
                collectionEvents.OnCollectionPagedBackward();
            }
        }

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

        public int MoveAbsolute(int firstItem, bool purgeExitingObjectsNow = true)
        {
            return MoveRelative(firstItem - _firstVisibleItem, purgeExitingObjectsNow);
        }

        /// <summary>
        /// Scroll visible data window by itemCount items forward or backward
        /// </summary>
        /// <remarks>
        /// Note if objects are not removed immediately, they must be removed later (such as after a transition effect)
        /// using PurgeAllRemovableGameObjects() or PurgeRemovableGameObjectRange()
        /// </remarks>
        /// <param name="itemCount">THe number of items to scroll. Negative=previous. Positive=next.</param>
        /// <param name="purgeExitingObjectsNow">Purge no longer visible objects immediately and return back to object pool.</param>
        /// <returns>Actual number of items scrolled. Note: Always positive for previous or next.</returns>
        public int MoveRelative(int itemCount, bool purgeExitingObjectsNow = true )
        {
            int actualScrollAmount;
            int firstItemToRequest;
            int numItemsToRequest;
            int newFirstVisibleItem = _firstVisibleItem;
            int firstItemToRemove;
            int firstItemToReposition;
            int numItemsToReposition;

            if (itemCount == 0 )
            {
                return 0;
            }


            if (itemCount < 0)
            {
                // scroll previous
                _lastRequestDirection = -1;

                actualScrollAmount = Math.Min(-itemCount, _firstVisibleItem);
                newFirstVisibleItem -= actualScrollAmount;
                firstItemToRequest = _firstVisibleItem - actualScrollAmount;
                numItemsToRequest = actualScrollAmount;
                firstItemToRemove = _firstVisibleItem + _numVisibleItems - actualScrollAmount;

                firstItemToReposition = _firstVisibleItem;
                numItemsToReposition = Math.Max(0, _numVisibleItems - actualScrollAmount);
            }
            else
            {
                // scroll next

                int maxFirstVisibleItem;
                int totalItemCount = GetTotalItemCount();

                _lastRequestDirection = 1;

                if (lastPageCanBePartial)
                {
                    maxFirstVisibleItem = totalItemCount - (totalItemCount % GetItemCountPerPage());
                }
                else
                {
                    maxFirstVisibleItem = totalItemCount - GetMaxVisibleItemCount();
                }

                actualScrollAmount = Math.Min(itemCount, maxFirstVisibleItem - _firstVisibleItem);

                firstItemToRemove = _firstVisibleItem;
                firstItemToRequest = _firstVisibleItem + Math.Max(_numVisibleItems, itemCount);
                newFirstVisibleItem += actualScrollAmount;

                numItemsToRequest = Math.Min(actualScrollAmount, totalItemCount - firstItemToRequest);
                firstItemToReposition = _firstVisibleItem + actualScrollAmount;
                numItemsToReposition = Math.Max(0, _numVisibleItems - actualScrollAmount);
            }

            if (actualScrollAmount > 0)
            {
                QueueGameObjectsForRemoval(firstItemToRemove, actualScrollAmount);
                _firstVisibleItem = newFirstVisibleItem;
               
                RequestItems(firstItemToRequest, numItemsToRequest);

                if (purgeExitingObjectsNow)
                {
                    PurgeRemovableGameObjectRange(firstItemToRemove, actualScrollAmount);
                }


                // for a partial scroll, we need to reposition the ones that are still visible, but are now
                // potentially in a new location
                if (numItemsToReposition > 0)
                {
                    RepositionItems(firstItemToReposition, numItemsToReposition);
                }
            }

            CheckForEventsToTrigger();

            return actualScrollAmount;
        }

        public bool IsVisible(int itemIndex )
        {
            return itemIndex >= _firstVisibleItem && itemIndex < _firstVisibleItem + _numVisibleItems;
        }

        public bool ShouldBeVisible(int itemIndex)
        {
            return itemIndex >= _firstVisibleItem && itemIndex < _firstVisibleItem + GetMaxVisibleItemCount();
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
            return _firstVisibleItem / GetItemCountPerPage();
        }


        /// <summary>
        /// Get nominal number of items per logical page.
        /// </summary>
        /// <remarks>
        /// This method is designed to be overwritten if paging is being used.
        ///
        /// Note that the actual number if items may depend on the nature of the
        /// items (such as the mix of object sizes) or whether fewer items are available than
        /// can be shown.
        /// </remarks>
        /// <returns>The number if items per page.</returns>
        public virtual int GetItemCountPerPage()
        {
            return 1;
        }

        /// <summary>
        /// Get the total number of items in the collection.
        /// </summary>
        /// <returns>Number of items in the collection.</returns>
        public virtual int GetTotalItemCount()
        {
            return _totalItemCount;
        }

        /// <summary>
        /// Get the total number of pages in the collection based on current page size.
        /// </summary>
        /// <returns>Number of items in the collection.</returns>
        public virtual int GetTotalPageCount()
        {
            int itemsPerPage = GetItemCountPerPage();

            return (GetTotalItemCount() + itemsPerPage - 1) / itemsPerPage;
        }

        #region IDataCollectionItemPlacer method implementations

        public virtual void StartPlacement()
        {
            if (collectionEvents != null)
            {
                collectionEvents.OnStartPlacement();
            }

            if (_dataConsumerCollection != null )
            {
                _totalItemCount = _dataConsumerCollection.GetCollectionItemCount();
            }
            else
            {
                _totalItemCount = 0;
            }

            CheckForEventsToTrigger();
        }

        public virtual void PlaceItem(object requestRef, int itemIndex, string itemKeypath, GameObject itemGO)
        {
            ItemInfo itemInfo = FindItem(itemIndex);

            if (itemInfo != null && itemInfo.state == State.Visible && !System.Object.ReferenceEquals(itemGO, itemInfo.gameObject) )
            {
                _dataConsumerCollection.ReturnGameObjectForReuse(itemIndex, itemGO);
                Debug.LogError("Attempting to add object with same index into collection in scene. Returning for reuse. Item index: " + itemIndex);
            }

            if (keepGameObjectsInIndexOrder)
            {
                itemGO.transform.SetSiblingIndex(FindOrderedInsertPoint(itemGO.transform.parent, itemIndex) );
            }

            State currentState;

            if (ShouldBeVisible(itemIndex))
            {
                currentState = State.Visible;
            }
            else if (itemInfo.state == State.StashRemovable || itemInfo.state == State.Removable)
            {
                currentState = State.Removable;
            }
            else
            {
                currentState = State.Prefetched;
            }

            UpdateItem(itemIndex, currentState, itemKeypath, itemGO);

            if (currentState == State.Visible || currentState == State.Prefetched)
            {
                ProcessReceivedItem(requestRef, itemIndex, itemKeypath, itemGO, currentState == State.Visible);
            }
            else if (currentState == State.Removable)
            {
                PurgeRemovableGameObjectRange(itemIndex, 1);
            }

            if (collectionEvents != null)
            {
                collectionEvents.OnItemPlaced();
            }
        }

        
        protected int FindOrderedInsertPoint(Transform parentTransform, int newItemIndex )
        {
            int midSlot = 0;
            int minSlot = 0;
            int maxSlot = parentTransform.childCount - 1;
            int cmp = 0;

            while (minSlot < maxSlot)
            {
                midSlot = (minSlot + maxSlot) / 2;
                Transform transformAtSlot = parentTransform.GetChild(midSlot);

                int valueAtMidSlot = Int32.Parse(transformAtSlot.gameObject.name);
                cmp = valueAtMidSlot - newItemIndex;

                if (cmp < 0)
                {
                    minSlot = midSlot + 1;
                }
                else if (cmp > 0)
                {
                    maxSlot = midSlot;
                }
                else
                {
                    return midSlot;
                }
            }

            if (cmp > 0)
                midSlot++;

            return midSlot + 1;
        }


        protected virtual void RepositionItems(int firstIndexToReposition, int numItems)
        {
            Dictionary<int, ItemInfo> visibleStateItems = _itemsByState[State.Visible];
            for (int itemIndex = firstIndexToReposition; itemIndex < firstIndexToReposition + numItems; itemIndex++)
            {
                if (visibleStateItems.ContainsKey(itemIndex))
                {
                    ItemInfo itemInfo = visibleStateItems[itemIndex];
                    ProcessReceivedItem(requestRef, itemIndex, itemInfo.itemKeypath, itemInfo.gameObject, true );
                }
            }
        }


        public virtual void EndPlacement()
        {
            if (collectionEvents != null)
            {
                collectionEvents.OnEndPlacement();
            }
            PredictivelyLoadItems();
        }



        public virtual void NotifyCollectionDataChanged(DataChangeType dataChangeType, string localKeypath, object value)
        {
            int newTotalItemCount = 0;

            if (_dataConsumerCollection != null)
            {
                newTotalItemCount = _dataConsumerCollection.GetCollectionItemCount();
            }

 
            // default behavior is to ask for all items in the collection with empty string as request ID.
            if (dataChangeType == DataChangeType.CollectionItemAdded)
            {
                _totalItemCount = newTotalItemCount;
                if (_numVisibleItems < GetMaxVisibleItemCount())
                {
                    RequestAnyMissingVisibleItems();
                }
            }
            else if (dataChangeType == DataChangeType.CollectionItemRemoved)
            {
                if (value != null)
                {
                    CollectionItemIdentifier itemIdentifier = value as CollectionItemIdentifier;

                    ItemInfo itemInfo = FindItem(itemIdentifier.indexPosition);
                    if (itemInfo != null)
                    {
                        ProcessRemovedItem(requestRef, itemIdentifier.indexPosition, itemInfo.itemKeypath, itemInfo.gameObject, IsVisible(itemIdentifier.indexPosition));

                        RemoveItem(itemIdentifier.indexPosition);
                        if (_numVisibleItems > _totalItemCount - _firstVisibleItem)
                        {
                            _numVisibleItems = _totalItemCount - _firstVisibleItem;
                        }

                    }
                    _totalItemCount = newTotalItemCount;
                    CheckAndFixVisibleRange();
                }
                else
                {
                    DebugUtilities.LogVerbose("Attempting to remove a collection item without specifying an integer value representing the item's index in the collection. Can't properly remove item from view.");
                }
            }
            else if (dataChangeType == DataChangeType.CollectionReset)
            {
                PurgeAllVisibleAndRemovableItems();
                _totalItemCount = newTotalItemCount;
                CheckAndFixVisibleRange();

                if (collectionEvents != null)
                {
                    collectionEvents.OnCollectionContextSwitch();
                }
            }
            else
            {
                if (_totalItemCount != newTotalItemCount)
                {
                    // For misc mods or deletions, it's safer to just re-fetch all at current scroll position since we don't
                    // necessarily know where a deletion or modification occured.
                    PurgeAllVisibleAndRemovableItems();

                    _totalItemCount = newTotalItemCount;
                    CheckAndFixVisibleRange();
                    RequestAnyMissingVisibleItems();
                }
            }

            if ( collectionEvents != null)
            {
                collectionEvents.OnCollectionChanged();
            }

            // Check to see if any of the other events should be triggered as well.
            CheckForEventsToTrigger();
        }

        #endregion IDataCollectionItemPlacer method implementations

        #region Methods typically overridden

        /// <summary>
        /// Place a visible item into the experience
        /// </summary>
        /// <remarks>
        /// Except in situations where the mere addition of a game object to the collection invokes another
        /// script to manage placement, this class will normally be overridden in a subclass to actually do
        /// the insertion of the specified game object into the scene at the correct transform.  
        /// 
        /// The index range is provided in case this is useful for determining the relative location of this item
        /// to the total number of items requested.  Note that this is the requested items, which generally is not
        /// the entire collection.
        /// </remarks>
        /// <param name="requestRef">Private request reference object provided at the time of the request.</param>
        /// <param name="itemIndex">The index of this item.</param>
        /// <param name="itemKeypath">The keypath of the item at this index.</param>
        /// <param name="itemGO">The game object (usually a prefab) of the item to place.</param>
        /// <param name="isVisible">Is the received item currently within the visible set?</param>
        public virtual void ProcessReceivedItem(object requestRef, int itemIndex, string itemKeypath, GameObject itemGO, bool isVisible)
        {
        }

        /// <summary>
        /// Process the removal of an item that is no longer in the collection
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="requestRef">Private request reference object provided at the time of the request.</param>
        /// <param name="itemIndex">The index of this item.</param>
        /// <param name="itemKeypath">The keypath of the item at this index.</param>
        /// <param name="itemGO">The game object (usually a prefab) of the item.</param>
        /// <param name="isVisible">Is the received item currently within the visible set?</param>

        public virtual void ProcessRemovedItem(object requestRef, int itemIndex, string itemKeypath, GameObject itemGO, bool isVisible)
        {

        }


        #endregion Methods typically overridden


        protected void CheckAndFixVisibleRange()
        {
            bool changed = false;

            if (_firstVisibleItem > _totalItemCount)
            {
                _firstVisibleItem = 0;
                changed = true;
            }
            if (_numVisibleItems > _totalItemCount - _firstVisibleItem)
            {
                _numVisibleItems = _totalItemCount - _firstVisibleItem;
                changed = true;
            }

            if ( changed )
            {
                RequestAnyMissingVisibleItems();
            }
        }


        protected void RequestAnyMissingVisibleItems()
        {
            int _desiredVisibleItems = Math.Min(GetMaxVisibleItemCount(), GetTotalItemCount() - _firstVisibleItem);

            if (_numVisibleItems < _desiredVisibleItems)
            {
                // If adding items and num visible is not currently the maximum possible visible, then
                // let's add the new item to what's visible.
                int firstItem = _firstVisibleItem + _numVisibleItems;
                int numItems = _desiredVisibleItems - _numVisibleItems;
                _numVisibleItems = _desiredVisibleItems;

                RequestItems(firstItem, numItems);
            }
        }


        public void RequestItems(int firstIdx, int count)
        {
            bool someDoNotNeedFetching = false;

            _lastRequestRangeStart = firstIdx;
            _lastRequestRangeCount = count;

            for (int itemIndex = firstIdx; itemIndex < firstIdx + count; itemIndex++)
            {
                ItemInfo itemInfo = FindItem(itemIndex);
                if (itemInfo != null)
                {
                    someDoNotNeedFetching = true;

                    switch (itemInfo.state)
                    {
                        case State.Visible:
                        case State.Requested:
                            break;
                        case State.Removable:
                        case State.StashRemovable:
                            ChangeItemState(itemIndex, State.Prefetched);
                            PlaceItem(requestRef, itemIndex, itemInfo.itemKeypath, itemInfo.gameObject);
                            break;
                        case State.Prefetched:
                            PlaceItem(requestRef, itemIndex, itemInfo.itemKeypath, itemInfo.gameObject);
                            break;

                    }
                }
            }

            if (_dataConsumerCollection != null)
            {
                if (someDoNotNeedFetching)
                {
                    for (int itemIndex = firstIdx; itemIndex < firstIdx + count; itemIndex++)
                    {
                        if (!ItemExists(itemIndex))
                        {
                            AddItem(State.Requested, itemIndex, null, null);
                            _dataConsumerCollection.RequestCollectionItems(this, itemIndex, 1, requestRef);
                        }
                    }
                }
                else
                {
                    for (int itemIndex = firstIdx; itemIndex < firstIdx + count; itemIndex++)
                    {
                        AddItem(State.Requested, itemIndex, null, null);
                    }
                    _dataConsumerCollection.RequestCollectionItems(this, firstIdx, count, requestRef);
                }
            }
        }



        protected virtual void PredictivelyLoadItems()
        {
            if ( _lastRequestRangeCount > 0)
            {
                int firstItem;
                int itemCount;

                if (_lastRequestDirection < 0)
                {
                    firstItem = _lastRequestRangeStart - _lastRequestRangeCount;
                }
                else
                {
                    firstItem = _lastRequestRangeStart + _lastRequestRangeCount;
                }

                itemCount = _lastRequestRangeCount;

                if (firstItem < 0)
                {
                    firstItem += itemCount;
                    itemCount = 0;
                }

                if (itemCount > GetTotalItemCount() - firstItem)
                {
                    itemCount = GetTotalItemCount() - firstItem;
                }

                if (itemCount > 0)
                {
                    _dataConsumerCollection.PrefetchCollectionItems(firstItem, itemCount);
                }
            }
        }


        protected virtual int GetMaxVisibleItemCount()
        {
            // override this to return the actual currently visible item count
            return Math.Min(GetTotalItemCount(), GetItemCountPerPage());
        }

        /// <summary>
        /// Search through game object hierarchy for the nearest IDataCollectionEvents implementation.
        /// </summary>
        /// 
        /// <remarks>
        /// This protected method is unique to collection related Data Consumers. A CollectionEvents is used to
        /// notify other systems that various changes in the state have occured. This is useful for
        /// changing the state of paging and scrolling UX elements and for triggering transition effects.
        /// </remarks>
        protected void FindNearestCollectionEvents()
        {
            if (collectionEvents == null)
            {
                collectionEvents = GetComponentInParent(typeof(IDataCollectionEvents)) as DataCollectionEventsGOBase;
            }
        }

        public bool IsInteger(object value)
        {
            return value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong;
        }
    }
}
