// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that involves an Input Source's spatial position AND rotation.
    /// </summary>
    public class PoseInputEventData : InputEventData
    {
        /// <summary>
        /// The <see cref="Vector3"/> and <see cref="Quaternion"/> input data.
        /// </summary>
        public MixedRealityPose InputData { get; private set; } = new MixedRealityPose(Vector3.zero, Quaternion.identity);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public PoseInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        public void Initialize(IMixedRealityInputSource inputSource, MixedRealityInputAction inputAction, MixedRealityPose inputData)
        {
            Initialize(inputSource, inputAction);
            InputData = inputData;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, MixedRealityInputAction inputAction, MixedRealityPose inputData)
        {
            Initialize(inputSource, handedness, inputAction);
            InputData = inputData;
        }
    }
}