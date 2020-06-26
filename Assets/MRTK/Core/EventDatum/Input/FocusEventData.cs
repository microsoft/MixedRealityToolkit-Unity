// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Describes an Input Event associated with a specific pointer's focus state change.
    /// </summary>
    public class FocusEventData : BaseEventData
    {
        /// <summary>
        /// The pointer associated with this event.
        /// </summary>
        public IMixedRealityPointer Pointer { get; private set; }

        /// <summary>
        /// The old focused object.
        /// </summary>
        public GameObject OldFocusedObject { get; private set; }

        /// <summary>
        /// The new focused object.
        /// </summary>
        public GameObject NewFocusedObject { get; private set; }

        /// <inheritdoc />
        public FocusEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        public void Initialize(IMixedRealityPointer pointer)
        {
            Reset();
            Pointer = pointer;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        public void Initialize(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            Reset();
            Pointer = pointer;
            OldFocusedObject = oldFocusedObject;
            NewFocusedObject = newFocusedObject;
        }
    }
}

