// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        internal int firstItemToPlace = 0;

        [SerializeField]
        internal int maximumItemsToPlace = 999999;

        [SerializeField]
        internal string requestId = "";


        internal int _firstVisiblItem = 0;
        internal int _visibleItemCount = 0;

        internal IDataConsumerCollection _dataConsumerCollection;

        internal Dictionary<int, GameObject> _gameObjectsByIndex = new Dictionary<int, GameObject>();


        internal void AddGameObject( int index, GameObject go )
        {
            _gameObjectsByIndex[index] = go;
        }

        internal void RemoveGameObject( int index )
        {            
            _dataConsumerCollection.ReturnGameObjectForReuse(index, _gameObjectsByIndex[index]);

            _gameObjectsByIndex.Remove(index);
        }

        internal void RemoveVisibleGameObjects()
        {
            int maxItem = firstItemToPlace + GetItemPlacementCount();
            for( int index = firstItemToPlace; index < maxItem; index++ )
            {
                RemoveGameObject(index);
            }
        }



        public void SetDataConsumerCollection(IDataConsumerCollection dataConsumerCollection)
        {
            _dataConsumerCollection = dataConsumerCollection;
        }

        public void ScrollNextPage ()
        {
            int totalItems = _dataConsumerCollection?.GetCollectionItemCount() ?? 0;

            if (firstItemToPlace < totalItems - GetItemPlacementCount())
            {
                RemoveVisibleGameObjects();

                firstItemToPlace += GetItemPlacementCount();
                if (firstItemToPlace > totalItems - GetItemPlacementCount())
                {
                    firstItemToPlace = totalItems - GetItemPlacementCount();
                }

                _dataConsumerCollection?.RequestCollectionItems(this, firstItemToPlace, GetItemPlacementCount(), requestId);

            }

        }

        internal virtual int GetItemPlacementCount()
        {
            return maximumItemsToPlace;
        }


        public void ScrollPreviousPage()
        {
            if (firstItemToPlace > 0)
            {
                RemoveVisibleGameObjects();
                firstItemToPlace -= GetItemPlacementCount();
                if (firstItemToPlace < 0) firstItemToPlace = 0;
                _dataConsumerCollection?.RequestCollectionItems(this, firstItemToPlace, GetItemPlacementCount(), requestId);
            }
        }

        public void ScrollNextItem()
        {
            firstItemToPlace++;
        }

        public void ScrollPreviousItem()
        {
            if (firstItemToPlace > 0) firstItemToPlace--;
        }

        public virtual void StartPlacement() {
            maximumItemsToPlace = _dataConsumerCollection.GetCollectionItemCount();
        }

        public virtual void PlaceItem(string requestId, int indexRangeStart, int indexRangeCount, int itemIndex, GameObject itemGO)
        {
            if (itemIndex >= firstItemToPlace && itemIndex < firstItemToPlace + GetItemPlacementCount() )
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


        internal virtual void PredictivelyLoadItems()
        {

        }


        public virtual void NotifyCollectionDataChanged() {
            // default behavior is to ask for all items in the collection with empty string as request ID.
            _dataConsumerCollection?.RequestCollectionItems(this, firstItemToPlace, GetItemPlacementCount(), requestId);
        }

    }
}
