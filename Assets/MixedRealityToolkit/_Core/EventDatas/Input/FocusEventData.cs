// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatas.Input
{
    /// <summary>
    /// Event dispatched associated with a specific pointer.
    /// </summary>
    public class FocusEventData : BaseEventData
    {
        /// <summary>
        /// The pointer associated with this event.
        /// </summary>
        public IPointer Pointer { get; private set; }

        /// <summary>
        /// The old focused object.
        /// </summary>
        public GameObject OldFocusedObject { get; private set; }

        /// <summary>
        /// The new focused object.
        /// </summary>
        public GameObject NewFocusedObject { get; private set; }

        public FocusEventData(UnityEngine.EventSystems.EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IPointer pointer)
        {
            Reset();
            Pointer = pointer;
        }

        public void Initialize(IPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            Reset();
            Pointer = pointer;
            OldFocusedObject = oldFocusedObject;
            NewFocusedObject = newFocusedObject;
        }
    }
}

