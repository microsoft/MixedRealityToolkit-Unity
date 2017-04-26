// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Links to the GridLayout to kick off layouts and update content status
    /// Manages the data that feeds into the layout
    /// </summary>
    public class GridLayoutController : MonoBehaviour
    {
        public GridLayout Grid;
        public int OffsetIndex;

        public virtual void SetupTile(GameObject tile, int id, int column, int row)
        {
            GridLayoutRenderer layoutRenderer = tile.GetComponent<GridLayoutRenderer>();
            int currentIndex = id + OffsetIndex;

            layoutRenderer.Controller = this;
            layoutRenderer.GridId = id;
            layoutRenderer.DataIndex = currentIndex;
            layoutRenderer.ColumnId = column;
            layoutRenderer.RowId = row;

            // get the custom renderer and format data and render

        }

        public virtual void ItemClicked(GridLayoutRenderer renderer, bool selected)
        {

        }
    }
}
