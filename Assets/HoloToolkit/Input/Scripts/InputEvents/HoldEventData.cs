//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Event dispatched when a hold gesture is detected.
    /// </summary>
    public class HoldEventData : BaseInputEventData
    {
        /// <summary>
        /// Source id of the input doing the hold.
        /// </summary>
        public uint SourceId { get; private set; }

        public HoldEventData(EventSystem eventSystem, IInputSource inputSource, uint sourceId) : base(eventSystem, inputSource)
        {
            SourceId = sourceId;
        }
    }
}