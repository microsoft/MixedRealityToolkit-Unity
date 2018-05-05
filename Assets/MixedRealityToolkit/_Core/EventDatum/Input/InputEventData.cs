// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
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
        public Handedness Handedness { get; private set; }

        /// <summary>
        /// The KeyCode of the <see cref="IMixedRealityInputSource"/>.
        /// </summary>
        public KeyCode KeyCode { get; private set; }

        /// <summary>
        /// The InputType of the <see cref="IMixedRealityInputSource"/>.
        /// </summary>
        public InputType InputType { get; private set; }

        /// <inheritdoc />
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = Handedness.None;
            KeyCode = KeyCode.None;
            InputType = InputType.None;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="keyCode"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, KeyCode keyCode, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = Handedness.None;
            KeyCode = keyCode;
            InputType = InputType.None;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputType"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputType inputType, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = Handedness.None;
            KeyCode = KeyCode.None;
            InputType = inputType;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = handedness;
            KeyCode = KeyCode.None;
            InputType = InputType.None;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="keyCode"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, KeyCode keyCode, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = handedness;
            KeyCode = keyCode;
            InputType = InputType.None;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputType"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, InputType inputType, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = handedness;
            KeyCode = KeyCode.None;
            InputType = inputType;
        }
    }
}
