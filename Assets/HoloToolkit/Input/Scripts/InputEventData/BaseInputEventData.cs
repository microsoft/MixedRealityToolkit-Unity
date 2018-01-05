// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

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

#if UNITY_WSA
        public InteractionSourcePressType PressType { get; private set; }
#endif

        public Handedness Handedness { get; private set; }

        /// <summary>
        /// An optional, input-source-dependent object to be associated with this event.
        /// </summary>
        public object[] Tags { get; private set; }

        public BaseInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        protected void BaseInitialize(IInputSource inputSource, uint sourceId, object[] tags)
        {
            Reset();
            InputSource = inputSource;
            SourceId = sourceId;
            Tags = tags;
            Handedness = Handedness.None;
#if UNITY_WSA
            PressType = InteractionSourcePressType.None;
#endif
        }

        protected void BaseInitialize(IInputSource inputSource, uint sourceId, Handedness handedness, object[] tags)
        {
            Reset();
            InputSource = inputSource;
            SourceId = sourceId;
            Tags = tags;
            Handedness = handedness;
#if UNITY_WSA
            PressType = InteractionSourcePressType.None;
#endif
        }

#if UNITY_WSA
        protected void BaseInitialize(IInputSource inputSource, uint sourceId, InteractionSourcePressType pressType, Handedness handedness, object[] tags)
        {
            Reset();
            InputSource = inputSource;
            SourceId = sourceId;
            Tags = tags;
            Handedness = handedness;
            PressType = pressType;
        }
#endif
    }
}