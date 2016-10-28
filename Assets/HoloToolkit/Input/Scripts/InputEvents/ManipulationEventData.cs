//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves hand manipulation.
    /// </summary>
    public class ManipulationEventData : InputEventData
    {
        /// <summary>
        /// The amount of manipulation that has occurred. Usually in the form of
        /// delta position of a hand. 
        /// </summary>
        public Vector3 CumulativeDelta { get; private set; }

        public ManipulationEventData(EventSystem eventSystem, IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta) : base(eventSystem, inputSource, sourceId)
        {
            CumulativeDelta = cumulativeDelta;
        }
    }
}