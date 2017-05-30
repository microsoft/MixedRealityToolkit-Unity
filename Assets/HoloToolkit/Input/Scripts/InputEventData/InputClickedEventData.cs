// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a tap.
    /// </summary>
    public class InputClickedEventData : InputEventData
    {
        /// <summary>
        /// Number of taps that triggered the event.
        /// </summary>
        public int TapCount { get; private set; }

        public InputClickedEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, InteractionPressKind buttonKind, int tapCount)
        {
            Initialize(inputSource, sourceId, tag, buttonKind);
            TapCount = tapCount;
        }
    }
}