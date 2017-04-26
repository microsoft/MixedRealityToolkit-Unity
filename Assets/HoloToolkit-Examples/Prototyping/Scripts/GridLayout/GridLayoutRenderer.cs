// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Exists within the prefab with links all all the elements that need to be updated with data.
    /// </summary>
    public class GridLayoutRenderer : MonoBehaviour
    {
        public GridLayoutController Controller;
        public int GridId;
        public int RowId;
        public int ColumnId;
        public int DataIndex;

        public virtual void OnClick(bool selected)
        {
            Controller.ItemClicked(this, selected);
        }
    }
}
