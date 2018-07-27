// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
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
        public Handedness Handedness
        {
            get { return handedness; }
            set
            {
                handedness = value;
                ResetControllerReference();
            }
        }

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

        /// <summary>
        /// Is the controller this Synchronizer is registered to currently tracked?
        /// </summary>
        public bool IsTracked { get; private set; } = false;

        private TrackingState lastTrackingState = TrackingState.NotTracked;

        private uint controllerInputSourceId = 0;

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            SetControllerReference(eventData);
        }

        /// <inheritdoc />
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.Controller.ControllerHandedness == Handedness)
            {
                ResetControllerReference();
            }
        }

        /// <inheritdoc />
        public virtual void OnSourcePoseChanged(SourcePoseEventData eventData)
        {
            if (controllerInputSourceId == 0)
            {
                SetControllerReference(eventData);
            }

            if (eventData.Controller.InputSource.SourceId != controllerInputSourceId)
            {
                return;
            }

            if (eventData.TrackingState != lastTrackingState)
            {
                IsTracked = eventData.TrackingState == TrackingState.Tracked;
                lastTrackingState = eventData.TrackingState;
            }

            if (lastTrackingState == TrackingState.Tracked)
            {
                transform.position = eventData.MixedRealityPose.Position;
                transform.rotation = eventData.MixedRealityPose.Rotation;
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region Monobehaviour Implementation

        protected override void OnEnable()
        {
            ResetControllerReference();

            // Subscribe to interaction events
            base.OnEnable();
        }

        #endregion Monobehaviour Implementation

        private void ResetControllerReference()
        {
            controllerInputSourceId = 0;

            if (DisableChildren)
            {
                gameObject.SetChildrenActive(false);
            }
        }

        private void SetControllerReference(SourceStateEventData eventData)
        {
            if (eventData.Controller.ControllerHandedness == Handedness)
            {
                controllerInputSourceId = eventData.Controller.InputSource.SourceId;

                if (DisableChildren)
                {
                    gameObject.SetChildrenActive(true);
                }
            }
        }
    }
}