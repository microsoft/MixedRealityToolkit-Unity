// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using MixedRealityToolkit.InputModule;
using MixedRealityToolkit.Examples.Prototyping;

namespace MixedRealityToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Advances a iCycle component on click
    /// </summary>
    public class CycleClicker : MonoBehaviour, IInputClickHandler
    {

        public GameObject CycleObject;
        private ICycle mCycleComp;

        public void OnInputClicked(InputClickedEventData eventData)
        {
            mCycleComp = CycleObject.GetComponent<ICycle>();

            if (mCycleComp != null)
                mCycleComp.MoveNext();
        }
    }
}
