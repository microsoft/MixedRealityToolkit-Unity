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


        protected int m_firstVisibelItem = 0;
        protected int m_numVisibleItems = 0;

        protected IDataConsumerCollection _dataConsumerCollection;

        protected Dictionary<int, GameObject> _gameObjectsByIndex = new Dictionary<int, GameObject>();


        protected void AddGameObject( int index, GameObject go )
        {
            _gameObjectsByIndex[index] = go;
        }

        protected void RemoveGameObject( int indexAsId )
        {            
            _dataConsumerCollection.ReturnGameObjectForReuse(indexAsId, _gameObjectsByIndex[indexAsId]);

            _gameObjectsByIndex.Remove(indexAsId);
        }

        protected void RemoveVisibleGameObjects()
        {
            int maxItem = m_firstVisibelItem + m_numVisibleItems;
            for( int indexAsId = m_firstVisibelItem; indexAsId < maxItem; indexAsId++ )
            {
                RemoveGameObject(indexAsId);
            }
        }



        public void SetDataConsumerCollection(IDataConsumerCollection dataConsumerCollection)
        {
            _dataConsumerCollection = dataConsumerCollection;
        }

        public void ScrollNextPage ()
        {
            if (m_firstVisibelItem < GetTotalItemCount() - GetAvailableItemCount())
            {
                RemoveVisibleGameObjects();

                m_firstVisibelItem += GetAvailableItemCount();
                RequestVisibleItems();
            }

        }


        public void ScrollPreviousPage()
        {
            if (m_firstVisibelItem > 0)
            {
                RemoveVisibleGameObjects();
                m_firstVisibelItem -= GetVisibleItemCount();
                if (m_firstVisibelItem < 0)
                {
                    m_firstVisibelItem = 0;
                }

                RequestVisibleItems();
            }
        }


        protected void RequestVisibleItems()
        {
            m_numVisibleItems = Math.Min(GetAvailableItemCount(), GetTotalItemCount() - m_firstVisibelItem);

            if (m_numVisibleItems > 0)
            {
                _dataConsumerCollection?.RequestCollectionItems(this, m_firstVisibelItem, m_numVisibleItems, requestId);
            }
        }


        protected virtual int GetTotalItemCount()
        {
            return _dataConsumerCollection?.GetCollectionItemCount() ?? 0;
        }

        protected virtual int GetVisibleItemCount()
        {
            // override this to return the actual currently visible item count
            return maximumVisibleItems;
        }

        protected virtual int GetAvailableItemCount()
        {
            return Math.Min(GetTotalItemCount(), GetVisibleItemCount());
        }

        
        public void ScrollNextItem()
        {
            if (m_firstVisibelItem < GetTotalItemCount() - GetAvailableItemCount() )
            {
                RemoveGameObject(m_firstVisibelItem);
                m_firstVisibelItem++;
                int newItemIndex = m_firstVisibelItem + m_numVisibleItems - 1;
                _dataConsumerCollection?.RequestCollectionItems(this, newItemIndex, 1, requestId);
            }
        }

        public void ScrollPreviousItem()
        {
            if (m_firstVisibelItem > 0)
            {
                RemoveGameObject(m_firstVisibelItem + m_numVisibleItems - 1);
                m_firstVisibelItem--;
                _dataConsumerCollection?.RequestCollectionItems(this, m_firstVisibelItem, 1, requestId);
            }
        }

        public virtual void StartPlacement() {
            maximumVisibleItems = _dataConsumerCollection.GetCollectionItemCount();
        }

        public virtual void PlaceItem(string requestId, int indexRangeStart, int indexRangeCount, int itemIndex, GameObject itemGO)
        {
            if (itemIndex >= m_firstVisibelItem && itemIndex < m_firstVisibelItem + GetAvailableItemCount() )
            {
                itemGO.SetActive(true);
                AddGameObject(itemIndex, itemGO);
                PlaceVisibleItem( requestId, indexRangeStart, indexRangeCount, itemIndex, itemGO);
            }
        }

        public abstract void PlaceVisibleItem(string requestId, int indexRangeStart, int indexRangeCount, int itemIndex, GameObject itemGO);

        public virtual void EndPlacement() {
            PredictivelyLoadItems();
        }


        protected virtual void PredictivelyLoadItems()
        {

        }


        public virtual void NotifyCollectionDataChanged(DataChangeType dataChangeType) {
            // default behavior is to ask for all items in the collection with empty string as request ID.
            if (dataChangeType == DataChangeType.CollectionItemAdded)
            {
                if (m_numVisibleItems < GetAvailableItemCount())
                {
                    // If adding items and num visible is not currently the maximum possible visible, then
                    // let's add the new item to what's visible.
                    int firstItem = m_firstVisibelItem + m_numVisibleItems;
                    int numItems = GetAvailableItemCount() - m_numVisibleItems;
                    m_numVisibleItems += numItems;
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

    }
}
