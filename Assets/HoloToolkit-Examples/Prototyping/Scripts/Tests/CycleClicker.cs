// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// Advances a iCycle component on click
    /// </summary>
    public class CycleClicker : MonoBehaviour, IPointerHandler
    {

        public GameObject CycleObject;
        private ICycle mCycleComp;

        public void OnPointerUp(PointerEventData eventData) { }

        public void OnPointerDown(PointerEventData eventData) { }

        public void OnPointerClicked(PointerEventData eventData)
        {
            mCycleComp = CycleObject.GetComponent<ICycle>();

            if (mCycleComp != null)
                mCycleComp.MoveNext();
        }
    }
}
