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
    /// Describes an input event that involves an Input Source's spatial position OR rotation.
    /// </summary>
    public class ThreeDoFInputEventData : InputEventData
    {
        /// <summary>
        /// The Position of the Input.
        /// </summary>
        public Vector3 Position { get; protected set; } = Vector3.zero;

        /// <summary>
        /// The Rotation of the Input.
        /// </summary>
        public Quaternion Rotation { get; protected set; } = Quaternion.identity;

        /// <inheritdoc />
        public ThreeDoFInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputAction inputAction, Vector3 position)
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
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, InputAction inputAction, Vector3 position)
        {
            Initialize(inputSource, handedness, inputAction);
            Position = position;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputAction inputAction, Quaternion rotation)
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
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, InputAction inputAction, Quaternion rotation)
        {
            Initialize(inputSource, handedness, inputAction);
            Rotation = rotation;
        }
    }
}
