// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an input event that involves content manipulation.
    /// </summary>
    public class ManipulationEventData : InputEventData
    {
        /// <summary>
        /// The amount of manipulation that has occurred. Usually in the form of
        /// delta position of a hand.
        /// </summary>
        public Vector3 CumulativeDelta { get; private set; }

        /// <inheritdoc />
        public ManipulationEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="cumulativeDelta"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Vector3 cumulativeDelta)
        {
            BaseInitialize(inputSource);
            CumulativeDelta = cumulativeDelta;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="cumulativeDelta"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, Vector3 cumulativeDelta)
        {
            Initialize(inputSource, handedness);
            CumulativeDelta = cumulativeDelta;
        }
    }
}