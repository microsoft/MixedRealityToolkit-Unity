// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes a source change event.
    /// <remarks>Source State events do not have an associated <see cref="Definitions.InputSystem.MixedRealityInputAction"/>.</remarks>
    /// </summary>
    public class SourcePositionEventData : SourceStateEventData
    {
        /// <summary>
        /// The new tracking state of the input source.
        /// </summary>
        public TrackingState TrackingState { get; private set; } = TrackingState.None;

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
        public SixDof SixDofPosition { get; private set; } = SixDof.ZeroIdentity;

        /// <inheritdoc />
        public SourcePositionEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IMixedRealityInputSource inputSource, TrackingState trackingState)
        {
            Initialize(inputSource);
            TrackingState = trackingState;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Vector2 position)
        {
            Initialize(inputSource);
            TwoDofPosition = position;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Vector3 position)
        {
            Initialize(inputSource);
            ThreeDofPosition = position;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="rotation"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Quaternion rotation)
        {
            Initialize(inputSource);
            ThreeDofRotation = rotation;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, SixDof position)
        {
            Initialize(inputSource);
            SixDofPosition = position;
        }
    }
}