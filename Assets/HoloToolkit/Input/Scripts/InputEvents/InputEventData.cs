// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that has a source id. 
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        /// <summary>
        /// The id of the source the event is from, for instance the hand id in a 
        /// tap event. 
        /// </summary>
        public uint SourceId { get; private set; }

        public InputEventData(EventSystem eventSystem, IInputSource inputSource, uint sourceId) : base(eventSystem, inputSource)
        {
            SourceId = sourceId;
        }
    }
}