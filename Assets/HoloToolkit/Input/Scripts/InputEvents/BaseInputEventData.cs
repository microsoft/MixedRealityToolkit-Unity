//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class of all input events.
    /// </summary>
    public abstract class BaseInputEventData : BaseEventData
    {
        /// <summary>
        /// The source the input event originates from.
        /// </summary>
        public IInputSource InputSource { get; private set; }

        public BaseInputEventData(EventSystem eventSystem, IInputSource inputSource) : base(eventSystem)
        {
            InputSource = inputSource;
        }
    }
}