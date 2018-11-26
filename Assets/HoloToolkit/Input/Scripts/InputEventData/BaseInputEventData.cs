// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class of all input events.
    /// </summary>
    public abstract class BaseInputEventData : BaseEventData, IInputSourceInfoProvider
    {
        /// <summary>
        /// The source the input event originates from.
        /// </summary>
        public IInputSource InputSource { get; private set; }

        /// <summary>
        /// The id of the source the event is from, for instance the hand id.
        /// </summary>
        public uint SourceId { get; private set; }

        /// <summary>
        /// An optional, input-source-dependent object to be associated with this event.
        /// </summary>
        public object Tag { get; private set; }

        public BaseInputEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        protected virtual void BaseInitialize(IInputSource inputSource, uint sourceId, object tag)
        {
            Reset();
            InputSource = inputSource;
            SourceId = sourceId;
            Tag = tag;
        }
    }
}