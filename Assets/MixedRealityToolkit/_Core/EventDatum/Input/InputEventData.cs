// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that has a source id.
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        /// <summary>
        /// Handedness of the <see cref="IMixedRealityInputSource"/>.
        /// </summary>
        public Handedness Handedness { get; private set; } = Handedness.None;

        /// <summary>
        /// The KeyCode of the <see cref="IMixedRealityInputSource"/>.
        /// </summary>
        public KeyCode KeyCode { get; private set; } = KeyCode.None;

        /// <summary>
        /// The InputType of the <see cref="IMixedRealityInputSource"/>.
        /// </summary>
        public InputAction InputAction { get; private set; } = new InputAction(0,"None");

        /// <inheritdoc />
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        public void Initialize(IMixedRealityInputSource inputSource)
        {
            BaseInitialize(inputSource);
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="keyCode"></param>
        public void Initialize(IMixedRealityInputSource inputSource, KeyCode keyCode)
        {
            BaseInitialize(inputSource);
            KeyCode = keyCode;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputType"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputAction inputAction)
        {
            BaseInitialize(inputSource);
            InputAction = inputAction;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness)
        {
            BaseInitialize(inputSource);
            Handedness = handedness;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="keyCode"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, KeyCode keyCode)
        {
            BaseInitialize(inputSource);
            Handedness = handedness;
            KeyCode = keyCode;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputType"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, InputAction inputAction)
        {
            BaseInitialize(inputSource);
            Handedness = handedness;
            InputAction = inputAction;
        }
    }
}
