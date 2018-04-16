// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.InputSources;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.InputSystem.EventData
{
    /// <summary>
    /// Describes an input event that involves a tap.
    /// </summary>
    public class ClickEventData : InputEventData
    {
        /// <summary>
        /// Number of Clicks, Taps, or Presses that triggered the event.
        /// </summary>
        public int Count { get; private set; }

        public ClickEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, int count, object[] tags = null)
        {
            BaseInitialize(inputSource, tags);
            Count = count;
        }

        public void Initialize(IInputSource inputSource, int count, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            Count = count;
        }

        public void Initialize(IInputSource inputSource, int count, InputType inputType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, inputType, handedness, tags);
            Count = count;
        }
    }
}