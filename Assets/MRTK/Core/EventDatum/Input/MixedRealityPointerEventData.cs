// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
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
        public void Initialize(IMixedRealityPointer pointer, Handedness handedness, MixedRealityInputAction inputAction, int count = 0)
        {
            Initialize(pointer.InputSourceParent, handedness, inputAction);
            Pointer = pointer;
            Count = count;
        }
    }
}