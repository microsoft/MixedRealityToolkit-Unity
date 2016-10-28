//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an source state event that has a source id. 
    /// </summary>
    public class SourceStateEventData : BaseInputEventData
    {
        /// <summary>
        /// The id of the source the event is from, for instance the hand id.
        /// </summary>
        public uint SourceId { get; private set; }

        public SourceStateEventData(EventSystem eventSystem, IInputSource inputSource, uint sourceId) : base(eventSystem, inputSource)
        {
            SourceId = sourceId;
        }
    }
}