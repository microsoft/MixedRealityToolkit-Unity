// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
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

        /// <inheritdoc />
        public NavigationEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="normalizedOffset"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Vector3 normalizedOffset)
        {
            BaseInitialize(inputSource);
            NormalizedOffset = normalizedOffset;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="normalizedOffset"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, Vector3 normalizedOffset)
        {
            Initialize(inputSource, handedness);
            NormalizedOffset = normalizedOffset;
        }
    }
}