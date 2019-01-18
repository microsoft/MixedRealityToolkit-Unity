// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Services.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input.Handlers
{
    /// <summary>
    /// Waits for a controller to be initialized, then synchronizes its transform position to a specified handedness.
    /// </summary>
    public class ControllerPoseSynchronizer : InputSystemGlobalListener, IMixedRealityControllerPoseSynchronizer
    {
        #region IMixedRealityControllerPoseSynchronizer Implementation

        [SerializeField]
        [Tooltip("The handedness this controller should synchronize with.")]
        private Handedness handedness = Handedness.Left;

        /// <inheritdoc />
        public Handedness Handedness
        {
            get { return handedness; }
            set { handedness = value; }
        }

        [SerializeField]
        [Tooltip("Should this GameObject clean itself up when it's controller is lost?")]
        private bool destroyOnSourceLost = true;

        /// <inheritdoc />
        public bool DestroyOnSourceLost
        {
            get { return destroyOnSourceLost; }
            set { destroyOnSourceLost = value; }
        }

        /// <summary>
        /// Is the controller this Synchronizer is registered to currently tracked?
        /// </summary>
        public bool IsTracked { get; protected set; } = false;

        /// <summary>
        /// The current tracking state of the assigned <see cref="IMixedRealityController"/>
        /// </summary>
        protected TrackingState TrackingState = TrackingState.NotTracked;

        private IMixedRealityController controller;

        /// <inheritdoc />
        public virtual IMixedRealityController Controller
        {
            get { return controller; }
            set
            {
                handedness = value.ControllerHandedness;
                controller = value;
                gameObject.name = $"{handedness}_{gameObject.name}";
            }
        }

        [SerializeField]
        [Tooltip("Should the Transform's position be driven from the source pose or from input handler?")]
        private bool useSourcePoseData = true;

        /// <inheritdoc />
        public bool UseSourcePoseData
        {
            get { return useSourcePoseData; }
            set { useSourcePoseData = value; }
        }

        [SerializeField]
        [Tooltip("The input action that will drive the Transform's pose, position, or rotation.")]
        private MixedRealityInputAction poseAction = MixedRealityInputAction.None;

        /// <inheritdoc />
        public MixedRealityInputAction PoseAction
        {
            get { return poseAction; }
            set { poseAction = value; }
        }

        #endregion IMixedRealityControllerPoseSynchronizer Implementation

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public virtual void OnSourceDetected(SourceStateEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId &&
                eventData.Controller?.ControllerHandedness == Handedness)
            {
                IsTracked = false;
                TrackingState = TrackingState.NotTracked;

                if (destroyOnSourceLost)
                {
                    if (Application.isEditor)
                    {
                        DestroyImmediate(gameObject);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<TrackingState> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                IsTracked = eventData.SourceData == TrackingState.Tracked;
                TrackingState = eventData.SourceData;
            }
        }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<Vector2> eventData) { }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<Vector3> eventData) { }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<Quaternion> eventData) { }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (UseSourcePoseData && TrackingState == TrackingState.Tracked)
                {
                    transform.localPosition = eventData.SourceData.Position;
                    transform.localRotation = eventData.SourceData.Rotation;
                }
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public virtual void OnInputUp(InputEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnInputDown(InputEventData eventData) { }

        /// <inheritdoc />
        [Obsolete("Use ControllerPoseSynchronizer.OnInputChanged(InputEventData<float> eventData)")]
        public virtual void OnInputPressed(InputEventData<float> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<float> eventData) { }

        [Obsolete("Use ControllerPoseSynchronizer.OnInputChanged(InputEventData<Vector2> eventData)")]
        public virtual void OnPositionInputChanged(InputEventData<Vector2> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Vector2> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Vector3> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.localPosition = eventData.InputData;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Quaternion> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.localRotation = eventData.InputData;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                if (!UseSourcePoseData &&
                    PoseAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.localPosition = eventData.InputData.Position;
                    transform.localRotation = eventData.InputData.Rotation;
                }
            }
        }

        #endregion  IMixedRealityInputHandler Implementation

        #region IMixedRealitySpatialInputHandler Implementation

        [Obsolete("Use ControllerPoseSynchronizer.OnInputChanged(InputEventData<Vector3> eventData)")]
        public virtual void OnPositionChanged(InputEventData<Vector3> eventData) { }

        [Obsolete("Use ControllerPoseSynchronizer.OnInputChanged(InputEventData<Quaternion> eventData)")]
        public virtual void OnRotationChanged(InputEventData<Quaternion> eventData) { }

        [Obsolete("Use ControllerPoseSynchronizer.OnInputChanged(InputEventData<MixedRealityPose> eventData)")]
        public virtual void OnPoseInputChanged(InputEventData<MixedRealityPose> eventData) { }

        #endregion IMixedRealitySpatialInputHandler Implementation 
    }
}