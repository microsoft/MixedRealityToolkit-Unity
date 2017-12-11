// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Event dispatched associated with a specific pointer.
    /// </summary>
    public class FocusChangedEventData : BaseEventData
    {
        /// <summary>
        /// The pointer associated with this event.
        /// </summary>
        public IPointingSource Focuser { get; private set; }
        public GameObject OldFocusedObject { get; private set; }
        public GameObject NewFocusedObject { get; private set; }

        public FocusChangedEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IPointingSource focuser, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            Reset();
            Focuser = focuser;
            OldFocusedObject = oldFocusedObject;
            NewFocusedObject = newFocusedObject;
        }
    }
}
