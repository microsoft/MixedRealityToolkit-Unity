// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Cycles through gameObjects to update the ColorColors and CycleScale on each dot according to the selected index.
    /// </summary>
    public class CycleDots : CycleArray<GameObject>
    {
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            for(int i = 0; i < Array.Length; ++i)
            {
                CycleColors colors = Array[i].GetComponent<CycleColors>();
                CycleUniformScale scale = Array[i].GetComponent<CycleUniformScale>();

                if (colors != null)
                {
                    colors.SetIndex(index == i ? 1 : 0);
                }

                if (scale != null)
                {
                    scale.SetIndex(index == i ? 1 : 0);
                }
            }
        }
    }
}
