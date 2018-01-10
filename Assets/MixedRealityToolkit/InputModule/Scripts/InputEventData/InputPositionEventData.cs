// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    public class InputPositionEventData : InputEventData
    {
        /// <summary>
        /// Two values, from -1.0 to 1.0 in the X-axis and Y-axis, representing where the input control is positioned.
        /// </summary>
        public Vector2 Position;

        public InputPositionEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, InteractionSourcePressInfo pressType, Vector2 position)
        {
            Initialize(inputSource, sourceId, tag, pressType);
            Position = position;
        }
    }
}
