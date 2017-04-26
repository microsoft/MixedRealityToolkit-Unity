// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.Prototyping
{
    public class CycleShowHideObjects : CycleArray<GameObject>
    {
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
