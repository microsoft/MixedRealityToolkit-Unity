// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement for spatial input position and rotation.
    /// </summary>
    [Obsolete("Use IMixedRealityInputHandler<T>")]
    public interface IMixedRealitySpatialInputHandler : IMixedRealityInputHandler
    {
        /// <summary>
        /// Raised when the input source's position has changed.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the current input position.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        [Obsolete("Use IMixedRealityInputHandler<Vector3>.OnInputChanged(InputEventData<Vector3> eventData)")]
        void OnPositionChanged(InputEventData<Vector3> eventData);

        /// <summary>
        /// Raised when the input source's rotation has changed.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the current input rotation.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        [Obsolete("Use IMixedRealityInputHandler<Quaternion>.OnInputChanged(InputEventData<Quaternion> eventData)")]
        void OnRotationChanged(InputEventData<Quaternion> eventData);

        /// <summary>
        /// Raised when the input source's position and rotation has changed.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the current input position.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        [Obsolete("Use IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)")]
        void OnPoseInputChanged(InputEventData<MixedRealityPose> eventData);
    }
}