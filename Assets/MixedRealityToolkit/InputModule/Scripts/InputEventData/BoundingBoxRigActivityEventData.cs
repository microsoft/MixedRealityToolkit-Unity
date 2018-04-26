// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine.EventSystems;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Describes activity of a bounding box rig events.
    /// </summary>
    public class BoundingBoxRigActivityEventData : BaseInputEventData
    {
        /// <summary>
        /// The bounding box rigged object.
        /// </summary>
        public GameObject BoundingBoxRiggedObject { get; private set; }

        public BoundingBoxRigActivityEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource source, uint sourceId, object tag, GameObject boundingBoxRiggedObject)
        {
            BaseInitialize(source, SourceId, tag);
            BoundingBoxRiggedObject = boundingBoxRiggedObject;
        }
    }
}