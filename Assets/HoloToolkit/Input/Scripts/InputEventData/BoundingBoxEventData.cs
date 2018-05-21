// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes activity of a bounding box rig events.
    /// </summary>
    public class BoundingBoxEventData : BaseEventData
    {
        /// <summary>
        /// The bounding box rigged object.
        /// </summary>
        public GameObject BoundingBoxRiggedObject { get; private set; }

        public BoundingBoxEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(GameObject boundingBoxRiggedObject)
        {
            Reset();

            BoundingBoxRiggedObject = boundingBoxRiggedObject;
        }
    }
}