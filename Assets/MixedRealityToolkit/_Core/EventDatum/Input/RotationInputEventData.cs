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
    /// Describes an input event that involves an Input Source's spatial rotation.
    /// </summary>
    public class RotationInputEventData : InputEventData
    {
        /// <summary>
        /// The Rotation of the Input.
        /// </summary>
        public Quaternion Rotation { get; protected set; } = Quaternion.identity;

        public RotationInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        public void Initialize(IMixedRealityInputSource inputSource, MixedRealityInputAction inputAction, Quaternion rotation)
        {
            Initialize(inputSource, inputAction);
            Rotation = rotation;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, MixedRealityInputAction inputAction, Quaternion rotation)
        {
            Initialize(inputSource, handedness, inputAction);
            Rotation = rotation;
        }
    }
}
