// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that has a source id.
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        /// <summary>
        /// Handedness of the <see cref="IMixedRealityInputSource"/>.
        /// </summary>
        public Handedness Handedness { get; private set; } = Handedness.None;

        /// <inheritdoc />
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        public void Initialize(IMixedRealityInputSource inputSource, MixedRealityInputAction inputAction)
        {
            BaseInitialize(inputSource, inputAction);
            Handedness = Handedness.None;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, MixedRealityInputAction inputAction)
        {
            BaseInitialize(inputSource, inputAction);
            Handedness = handedness;
        }
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
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="data"></param>
        public void Initialize(IMixedRealityInputSource inputSource, MixedRealityInputAction inputAction, T data)
        {
            Initialize(inputSource, inputAction);
            InputData = data;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="data"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, MixedRealityInputAction inputAction, T data)
        {
            Initialize(inputSource, handedness, inputAction);
            InputData = data;
        }
    }
}
