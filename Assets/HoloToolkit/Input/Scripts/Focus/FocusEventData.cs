// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Event dispatched associated with a specific pointer.
    /// </summary>
    public class FocusEventData : BaseEventData
    {
        /// <summary>
        /// The pointer associated with this event.
        /// </summary>
        public IFocuser Focuser { get; private set; }

        public FocusEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IFocuser focuser)
        {
            Reset();
            Focuser = focuser;
        }
    }
}