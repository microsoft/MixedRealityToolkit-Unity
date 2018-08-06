// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input.Handlers
{
    /// <summary>
    /// Waits for a controller to be initialized, then synchronizes its transform position to a specified handedness.
    /// </summary>
    public class ControllerPoseSynchronizer : InputSystemGlobalListener, IMixedRealitySourcePoseHandler
    {
        [SerializeField]
        [Tooltip("The handedness this controller should synchronize with.")]
        private Handedness handedness = Handedness.Left;

        /// <summary>
        /// The handedness this controller should synchronize with.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        [Tooltip("Disables child GameObjects when the controller source is lost.")]
        private bool disableChildren = true;

        /// <summary>
        ///Disables child <see cref="GameObject"/>s the controller source is lost.
        /// </summary>
        public bool DisableChildren
        {
            get { return disableChildren; }
            set { disableChildren = value; }
        }

        [SerializeField]
        [Tooltip("Should this GameObject clean itself up after source is lost?")]
        private bool destroyOnSourceLost = true;

        /// <summary>
        /// Should this GameObject clean itself up after source is lost?
        /// </summary>
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

        /// <summary>
        /// The currently assigned Controller.
        /// </summary>
        public virtual IMixedRealityController Controller
        {
            get { return controller; }
            set
            {
                handedness = value.ControllerHandedness;
                controller = value;
            }
        }

        private IMixedRealityController controller;

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            if (Controller == null ||
                eventData.Controller == null ||
                eventData.Controller.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }

            if (eventData.Controller.ControllerHandedness == Handedness &&
                eventData.Controller.InputSource.SourceId == Controller.InputSource.SourceId)
            {

                if (disableChildren)
                {
                    gameObject.SetChildrenActive(true);
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            if (Controller == null ||
                eventData.Controller == null ||
                eventData.Controller.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }

            if (eventData.Controller?.ControllerHandedness == Handedness)
            {
                IsTracked = false;
                TrackingState = TrackingState.NotTracked;

                if (disableChildren)
                {
                    gameObject.SetChildrenActive(false);
                }

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
        public virtual void OnSourcePoseChanged(SourcePoseEventData eventData)
        {
            if (Controller == null ||
                eventData.Controller == null ||
                eventData.Controller.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }

            if (eventData.TrackingState != TrackingState)
            {
                IsTracked = eventData.TrackingState == TrackingState.Tracked;
                TrackingState = eventData.TrackingState;
            }

            if (TrackingState == TrackingState.Tracked)
            {
                transform.localPosition = eventData.MixedRealityPose.Position;
                transform.localRotation = eventData.MixedRealityPose.Rotation;
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation
    }
}