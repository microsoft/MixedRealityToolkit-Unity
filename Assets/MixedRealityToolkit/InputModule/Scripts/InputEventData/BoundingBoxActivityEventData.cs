// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Describes activity of a bounding box rig events.
    /// </summary>
    public class BoundingBoxActivityEventData : BaseInputEventData
    {
        public BoundingBoxActivityEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource source, uint sourceId, object tag)
        {
            BaseInitialize(source, SourceId, tag);
        }
    }
}