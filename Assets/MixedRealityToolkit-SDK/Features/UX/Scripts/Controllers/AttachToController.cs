// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Controllers
{
    /// <summary>
    /// Waits for a controller to be initialized, then attaches itself to a specified element
    /// </summary>
    public class AttachToController : InputSystemGlobalListener, IMixedRealitySourcePoseHandler
    {
        [SerializeField]
        protected Vector3 PositionOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 RotationOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 ScaleOffset = Vector3.one;

        [SerializeField]
        protected bool SetScaleOnAttach = false;

        [SerializeField]
        protected Handedness Handedness = Handedness.Left;

        [SerializeField]
        [Tooltip("Input System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType = null;

        [SerializeField]
        [Tooltip("Disable child objects when detached from controller.")]
        private bool setChildrenInactiveWhenDetached = true;

        public bool SetChildrenInactiveWhenDetached
        {
            get { return setChildrenInactiveWhenDetached; }
            set { setChildrenInactiveWhenDetached = value; }
        }

        public bool IsTracked { get; private set; } = false;

        protected TrackingState LastTrackingState { get; private set; } = TrackingState.NotTracked;

        protected uint ControllerInputSourceId { get; private set; } = 0;

        #region IMixedRealitySourcePoseHandler Implementation

        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            if (eventData.Controller.GetType() == controllerType.Type && eventData.Controller.ControllerHandedness == Handedness)
            {
                ControllerInputSourceId = eventData.Controller.InputSource.SourceId;

                if (SetChildrenInactiveWhenDetached)
                {
                    gameObject.SetChildrenActive(true);
                }
            }
        }

        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.Controller.GetType() == controllerType.Type && eventData.Controller.ControllerHandedness == Handedness)
            {
                ControllerInputSourceId = 0;

                if (SetChildrenInactiveWhenDetached)
                {
                    gameObject.SetChildrenActive(false);
                }
            }
        }

        public virtual void OnSourcePoseChanged(SourcePoseEventData eventData)
        {
            if (eventData.Controller.InputSource.SourceId != ControllerInputSourceId) { return; }

            if (eventData.TrackingState != LastTrackingState)
            {
                IsTracked = eventData.TrackingState == TrackingState.Tracked;
                LastTrackingState = eventData.TrackingState;
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region Monobehaviour Implementation

        protected override void OnEnable()
        {
            // Subscribe to interaction events
            base.OnEnable();

            if (SetChildrenInactiveWhenDetached)
            {
                gameObject.SetChildrenActive(false);
            }
        }

        #endregion Monobehaviour Implementation
    }
}