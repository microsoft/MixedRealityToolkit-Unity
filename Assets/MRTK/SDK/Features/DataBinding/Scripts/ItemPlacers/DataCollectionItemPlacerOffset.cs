// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{

    /// <summary>
    /// A simple data collection item placer that will place each item at a specific
    /// offset from the previous object, first in the x, then y, then z directions, 
    /// using the offsets provided in the inspector.  The starting point is reset
    /// each time a new placement session is started using StartPlacement().
    /// 
    /// </summary>
    public class DataCollectionItemPlacerOffset : DataCollectionItemPlacerGOBase
    {


        [Tooltip("Place each item in a collection at successive offsets relative to parent gameobject, with the first item spawning at 0,0,0.")]
        [SerializeField]
        internal Vector3 itemOffset;

        [Tooltip("How many items to show in the x dimension using the x item offset.")]
        [SerializeField]
        internal int xCount = 4;

        [Tooltip("How many items to show in the y dimension using the y item offset.")]
        [SerializeField]
        internal int yCount = 3;

        [Tooltip("How many items to show in the y dimension using the y item offset.")]
        [SerializeField]
        internal int zCount = 1;

        internal Vector3 _itemPlacerPositionOffset;



        public override void StartPlacement()
        {
            base.StartPlacement();
            _itemPlacerPositionOffset = new Vector3(0, 0, 0);
        }

        public override void PlaceVisibleItem( string requestId, int indexRangeStart, int indexRangeCount, int itemIndex, GameObject itemGO)
        {
            itemIndex -= indexRangeStart;

            _itemPlacerPositionOffset.x = itemOffset.x * (itemIndex % xCount);
            _itemPlacerPositionOffset.y = itemOffset.y * ((itemIndex / xCount) % yCount);
            _itemPlacerPositionOffset.z = itemOffset.z * (itemIndex / (xCount * yCount));

            itemGO.transform.position = itemGO.transform.position + _itemPlacerPositionOffset;
        }


        internal override int GetItemPlacementCount()
        {
            return xCount * yCount * zCount;
        }



    }
}
