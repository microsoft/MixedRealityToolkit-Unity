// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves content navigation.
    /// </summary>
    public class FocusEventData : InputEventData
    {
        /// <summary>
        /// The object that lost focus. 
        /// </summary>
        public GameObject PreviousObject { get; private set; }

        /// <summary>
        /// The object that gained focus. 
        /// </summary>
        public GameObject NewObject { get; private set; }

        public FocusEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, GameObject previousObject, GameObject newObject)
        {
            BaseInitialize(inputSource, sourceId);
            PreviousObject = previousObject;
            NewObject = newObject;
        }
    }
}