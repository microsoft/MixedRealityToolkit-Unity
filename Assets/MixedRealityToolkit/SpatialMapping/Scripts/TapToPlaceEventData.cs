// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.SpatialMapping
{
    /// <summary>
    /// Describes placement of objects events.
    /// </summary>
    public class TapToPlaceEventData : BaseEventData
    {
        public TapToPlaceEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(GameObject gameObject)
        {
            Reset();
            selectedObject = gameObject;
        }
    }
}
