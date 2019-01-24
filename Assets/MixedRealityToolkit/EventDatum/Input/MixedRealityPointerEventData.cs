// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that involves a tap, click, or touch.
    /// </summary>
    public class MixedRealityPointerEventData : InputEventData
    {
        /// <summary>
        /// Pointer for the Input Event
        /// </summary>
        public IMixedRealityPointer Pointer { get; private set; }

        /// <summary>
        /// Number of Clicks, Taps, or Presses that triggered the event.
        /// </summary>
        public int Count { get; private set; }

        /// <inheritdoc />
        public MixedRealityPointerEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="handedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        public void Initialize(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null, int count = 0)
        {
            if (inputSource != null)
            {
                Initialize(inputSource, handedness, inputAction);
            }
            else
            {
                Initialize(pointer.InputSourceParent, handedness, inputAction);
            }

            Pointer = pointer;
            Count = count;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="count"></param>
        /// <param name="inputAction"></param>
        /// <param name="handedness"></param>
        public void Initialize(IMixedRealityPointer pointer, Handedness handedness, MixedRealityInputAction inputAction, int count = 0)
        {
            Initialize(pointer.InputSourceParent, handedness, inputAction);
            Pointer = pointer;
            Count = count;
        }
    }
}