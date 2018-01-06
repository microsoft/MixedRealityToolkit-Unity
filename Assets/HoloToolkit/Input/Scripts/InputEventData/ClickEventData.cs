// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a tap.
    /// </summary>
    public class ClickEventData : InputEventData
    {
        /// <summary>
        /// Number of Clicks or Taps that triggered the event.
        /// </summary>
        public int ClickCount { get; private set; }

        public ClickEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, int clickCount, object[] tags = null)
        {
            Initialize(inputSource, tags);
            ClickCount = clickCount;
        }

        public void Initialize(IInputSource inputSource, int clickCount, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            ClickCount = clickCount;
        }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, int clickCount, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, pressType, handedness, tags);
            ClickCount = clickCount;
        }
#endif
    }
}