// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a select press.
    /// </summary>
    public class SelectPressedEventData : BaseInputEventData
    {
        /// <summary>
        /// The amount, from 0.0 to 1.0, that the select was pressed.
        /// </summary>
        public double PressedAmount { get; private set; }

        public SelectPressedEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, double pressedAmount)
        {
            BaseInitialize(inputSource, sourceId, tag);
            PressedAmount = pressedAmount;
        }
    }
}