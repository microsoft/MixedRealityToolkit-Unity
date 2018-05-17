// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that involves an Input Source's spatial position AND rotation.
    /// </summary>
    public class SixDoFInputEventData : ThreeDoFInputEventData
    {
        /// <summary>
        /// The <see cref="Vector3"/> and <see cref="Quaternion"/> input data.
        /// </summary>
        public Tuple<Vector3, Quaternion> InputData { get; private set; } = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public SixDoFInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputType"></param>
        /// <param name="inputData"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputType inputType, Tuple<Vector3, Quaternion> inputData)
        {
            Initialize(inputSource, inputType);
            Position = inputData.Item1;
            Rotation = inputData.Item2;
            InputData = inputData;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputType"></param>
        /// <param name="inputData"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, InputType inputType, Tuple<Vector3, Quaternion> inputData)
        {
            Initialize(inputSource, handedness, inputType);
            Position = inputData.Item1;
            Rotation = inputData.Item2;
            InputData = inputData;
        }
    }
}