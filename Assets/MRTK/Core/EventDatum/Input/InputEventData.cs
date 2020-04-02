// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Describes an Input Event that has a source id.
    /// </summary>
    public class InputEventData : BaseInputEventData, IMixedRealityEventPropagationData
    {
        /// <summary>
        /// Handedness of the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource"/>.
        /// </summary>
        public Handedness Handedness { get; private set; } = Handedness.None;

        /// <inheritdoc />
        public virtual EventPropagation Propagation { get; set; }

        /// <inheritdoc />
        public PropagationPhase Phase { get; set; }

        /// <inheritdoc />
        public LifeStatus Status { get; set; }

        /// <inheritdoc />
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, MixedRealityInputAction inputAction)
        {
            BaseInitialize(inputSource, inputAction);
            Handedness = handedness;

            skipElements.Clear();

            Status = LifeStatus.None;
            Propagation = EventPropagation.BubblesUp | EventPropagation.TricklesDown;
            Phase = PropagationPhase.None;
        }

        /// <inheritdoc />
        public void PreventDefault()
        {
            Status |= LifeStatus.DefaultPrevented;
        }

        /// <inheritdoc />
        public void StopPropagation()
        {
            Status |= LifeStatus.PropagationStopped;
        }

        /// <inheritdoc />
        public void StopPropagationImmediately()
        {
            Status |= LifeStatus.PropagationStoppedImmediately;
        }

        /// <inheritdoc />
        public bool Skip(GameObject gameObject)
        {
            return skipElements.Contains(gameObject);
        }

        private List<GameObject> skipElements = new List<GameObject>();
    }

    /// <summary>
    /// Describes and input event with a specific type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputEventData<T> : InputEventData
    {
        /// <summary>
        /// The input data of the event.
        /// </summary>
        public T InputData { get; private set; }

        /// <inheritdoc />
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, MixedRealityInputAction inputAction, T data)
        {
            Initialize(inputSource, handedness, inputAction);
            InputData = data;
        }
    }
}
