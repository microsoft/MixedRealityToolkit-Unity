// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes a source change event.
    /// <remarks>Source State events do not have an associated <see cref="Definitions.InputSystem.MixedRealityInputAction"/>.</remarks>
    /// </summary>
    public class SourcePoseEventData : SourceStateEventData
    {
        /// <summary>
        /// The new tracking state of the input source.
        /// </summary>
        public TrackingState TrackingState { get; private set; } = TrackingState.NotTracked;

        /// <summary>
        /// The new position of the input source.
        /// </summary>
        public Vector2 TwoDofPosition { get; private set; } = Vector2.zero;

        /// <summary>
        /// The new position of the input source.
        /// </summary>
        public Vector3 ThreeDofPosition { get; private set; } = Vector3.zero;

        /// <summary>
        /// The new rotation of the input source.
        /// </summary>
        public Quaternion ThreeDofRotation { get; private set; } = Quaternion.identity;

        /// <summary>
        /// The new position and rotation of the input source.
        /// </summary>
        public MixedRealityPose MixedRealityPose { get; private set; } = MixedRealityPose.ZeroIdentity;

        /// <inheritdoc />
        public SourcePoseEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, TrackingState trackingState)
        {
            Initialize(inputSource, controller);
            TrackingState = trackingState;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, Vector2 position)
        {
            Initialize(inputSource, controller);
            TwoDofPosition = position;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, Vector3 position)
        {
            Initialize(inputSource, controller);
            ThreeDofPosition = position;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        /// <param name="rotation"></param>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, Quaternion rotation)
        {
            Initialize(inputSource, controller);
            ThreeDofRotation = rotation;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        /// <param name="pose"></param>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller, MixedRealityPose pose)
        {
            Initialize(inputSource, controller);
            ThreeDofPosition = pose.Position;
            ThreeDofRotation = pose.Rotation;
            MixedRealityPose = pose;
        }
    }
}