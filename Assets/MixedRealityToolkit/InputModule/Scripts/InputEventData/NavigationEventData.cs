// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Describes an input event that involves content navigation.
    /// </summary>
    public class NavigationEventData : InputEventData
    {
        /// <summary>
        /// The amount of manipulation that has occurred. Sent in the form of
        /// a normalized offset of a hand. 
        /// </summary>
        public Vector3 NormalizedOffset { get; private set; }

        public NavigationEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, Vector3 normalizedOffset)
        {
            BaseInitialize(inputSource, sourceId, tag);
            NormalizedOffset = normalizedOffset;
        }
    }
}