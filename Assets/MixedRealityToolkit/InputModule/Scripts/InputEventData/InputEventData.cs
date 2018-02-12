// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Describes an input event that has a source id and a press kind. 
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        /// <summary>
        /// Button type that initiated the event.
        /// </summary>
        public InteractionSourcePressInfo PressType { get; private set; }

        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, InteractionSourcePressInfo pressType)
        {
            BaseInitialize(inputSource, sourceId, tag);
            PressType = pressType;
        }
    }
}
