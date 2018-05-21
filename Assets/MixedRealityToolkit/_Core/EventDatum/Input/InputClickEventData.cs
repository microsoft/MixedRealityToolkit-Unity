// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that involves a tap, click, or touch.
    /// </summary>
    public class InputClickEventData : InputEventData
    {
        /// <summary>
        /// Number of Clicks, Taps, or Presses that triggered the event.
        /// </summary>
        public int Count { get; private set; }

        /// <inheritdoc />
        public InputClickEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        public void Initialize(IMixedRealityInputSource inputSource, int count)
        {
            BaseInitialize(inputSource);
            Count = count;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        /// <param name="handedness"></param>
        public void Initialize(IMixedRealityInputSource inputSource, int count, Handedness handedness)
        {
            Initialize(inputSource, handedness);
            Count = count;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        /// <param name="inputType"></param>
        /// <param name="handedness"></param>
        public void Initialize(IMixedRealityInputSource inputSource, int count, InputAction inputAction, Handedness handedness)
        {
            Initialize(inputSource, handedness, inputAction);
            Count = count;
        }
    }
}