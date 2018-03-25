// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Describes placement of objects events.
    /// </summary>
    public class PlacementEventData : BaseInputEventData
    {
        public PlacementEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag)
        {
            BaseInitialize(inputSource, sourceId, tag);
        }
    }
}
