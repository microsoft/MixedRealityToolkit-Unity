// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an input event that involves an Input Source's spatial position.
    /// </summary>
    public class PositionInputEventData : InputEventData
    {
        /// <summary>
        /// The Position of the Input.
        /// </summary>
        public Vector3 Position { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public PositionInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, MixedRealityInputAction inputAction, Vector3 position)
        {
            Initialize(inputSource, inputAction);
            Position = position;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, MixedRealityInputAction inputAction, Vector3 position)
        {
            Initialize(inputSource, handedness, inputAction);
            Position = position;
        }
    }
}
