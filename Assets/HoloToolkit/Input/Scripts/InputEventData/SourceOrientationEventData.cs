// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a source's orientation changing.
    /// </summary>
    public class SourceOrientationEventData : BaseInputEventData
    {
        /// <summary>
        /// The new orientation of the source.
        /// </summary>
        public Quaternion Orientation { get; private set; }

        public SourceOrientationEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, Quaternion orientation)
        {
            BaseInitialize(inputSource, sourceId, tag);
            Orientation = orientation;
        }
    }
}