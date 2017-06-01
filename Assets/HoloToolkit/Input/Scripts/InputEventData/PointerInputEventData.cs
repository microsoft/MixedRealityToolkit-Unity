// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class PointerInputEventData : PointerEventData
    {
        public IInputSource InputSource { get; set; }

        public uint InputSourceId { get; set; }

        public PointerInputEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }
    }
}