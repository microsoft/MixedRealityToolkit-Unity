// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that involves a tap, click, or touch.
    /// </summary>
    public class ClickEventData : InputEventData
    {
        /// <summary>
        /// Number of Clicks, Taps, or Presses that triggered the event.
        /// </summary>
        public int Count { get; private set; }

        /// <inheritdoc />
        public ClickEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, int count, object[] tags = null)
        {
            BaseInitialize(inputSource, tags);
            Count = count;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        /// <param name="handedness"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, int count, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            Count = count;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        /// <param name="inputType"></param>
        /// <param name="handedness"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, int count, InputType inputType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, inputType, tags);
            Count = count;
        }
    }
}