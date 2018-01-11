// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Event dispatched when a hold gesture is detected.
    /// </summary>
    public class HoldEventData : BaseInputEventData
    {
        public HoldEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag)
        {
            BaseInitialize(inputSource, sourceId, tag);
        }
    }
}