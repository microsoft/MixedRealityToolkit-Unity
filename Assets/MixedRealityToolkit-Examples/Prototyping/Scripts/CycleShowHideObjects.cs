// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Active and detactivate objects based on the selected item in the array.
    /// Only shows the currently selected item.
    /// </summary>
    public class CycleShowHideObjects : CycleArray<GameObject>
    {
        /// <summary>
        /// Show the item by index and hide all others
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            for (int i = 0; i < Array.Length; ++i)
            {
                GameObject item = Array[i];

                if (item != null)
                {
                    item.SetActive(i == Index);
                }
            }
        }
    }
}
