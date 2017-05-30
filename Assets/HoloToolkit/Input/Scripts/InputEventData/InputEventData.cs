// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that has a source id and a press kind. 
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        /// <summary>
        /// Button type that initiated the event.
        /// </summary>
        public InteractionPressKind PressKind { get; private set; }

        public InputEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, InteractionPressKind pressKind)
        {
            BaseInitialize(inputSource, sourceId, tag);
            PressKind = pressKind;
        }
    }
}