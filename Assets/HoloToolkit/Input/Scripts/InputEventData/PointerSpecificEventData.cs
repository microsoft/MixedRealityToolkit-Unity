// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Event dispatched associated with a specific pointer.
    /// </summary>
    public class PointerSpecificEventData : BaseEventData
    {
        /// <summary>
        /// The pointer associated with this event.
        /// </summary>
        public IPointingSource Pointer { get; private set; }

        public PointerSpecificEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IPointingSource pointer)
        {
            Reset();
            Pointer = pointer;
        }
    }
}
