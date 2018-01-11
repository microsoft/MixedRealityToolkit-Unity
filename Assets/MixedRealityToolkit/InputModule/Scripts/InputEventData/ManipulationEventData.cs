// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Describes an input event that involves content manipulation.
    /// </summary>
    public class ManipulationEventData : BaseInputEventData
    {
        /// <summary>
        /// The amount of manipulation that has occurred. Usually in the form of
        /// delta position of a hand. 
        /// </summary>
        public Vector3 CumulativeDelta { get; private set; }

        public ManipulationEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, Vector3 cumulativeDelta)
        {
            BaseInitialize(inputSource, sourceId, tag);
            CumulativeDelta = cumulativeDelta;
        }
    }
}