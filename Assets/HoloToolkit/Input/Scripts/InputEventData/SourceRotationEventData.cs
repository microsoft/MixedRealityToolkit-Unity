// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a source's rotation changing.
    /// </summary>
    public class SourceRotationEventData : BaseInputEventData
    {
        /// <summary>
        /// The new rotation of the source.
        /// </summary>
        public Quaternion Rotation { get; private set; }

        public SourceRotationEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, Quaternion rotation)
        {
            BaseInitialize(inputSource, sourceId, tag);
            Rotation = rotation;
        }
    }
}