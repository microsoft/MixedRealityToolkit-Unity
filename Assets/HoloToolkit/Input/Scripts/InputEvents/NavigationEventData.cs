// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves content navigation.
    /// </summary>
    public class NavigationEventData : InputEventData
    {
        /// <summary>
        /// The amount of manipulation that has occurred. Usually in the form of
        /// delta position of a hand. 
        /// </summary>
        public Vector3 CumulativeDelta { get; private set; }

        public NavigationEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
        {
            BaseInitialize(inputSource, sourceId);
            CumulativeDelta = cumulativeDelta;
        }
    }
}