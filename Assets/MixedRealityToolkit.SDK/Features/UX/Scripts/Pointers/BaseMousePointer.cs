// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base Mouse Pointer Implementation.
    /// </summary>
    public abstract class BaseMousePointer : BaseControllerPointer, IMixedRealityMousePointer
    {
        protected float timeoutTimer = 0.0f;

        private bool isInteractionEnabled = false;

        private bool cursorWasDisabledOnDown = false;

        protected bool isDisabled = true;

        #region IMixedRealityMousePointer Implementation

        [SerializeField]
        [Tooltip("If true, the cursor will hide on movement timeout")]
        private bool hideCursorWhenInactive = true;

        /// <summary>
        /// If true, the cursor will hide on movement timeout
        /// </summary>
        public bool HideCursorWhenInactive => hideCursorWhenInactive;

        [SerializeField]
        [Range(0.01f, 1f)]
        [Tooltip("Movement threshold to reach before un-hiding the mouse cursor")]
        private float movementThresholdToUnHide = 0.1f;

        /// <summary>
        /// Movement threshold to reach before un-hiding the mouse cursor
        /// </summary>
        public float MovementThresholdToUnHide => movementThresholdToUnHide;

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("Time the mouse cursor must stay immobile to be hidden")]
        private float hideTimeout = 3.0f;

        /// <summary>
        /// Time the mouse cursor must stay immobile to be hidden
        /// </summary>
        public float HideTimeout => hideTimeout;

        #endregion IMixedRealityMousePointer Implementation

        #region IMixedRealityPointer Implementation

        /// <inheritdoc />
        public override bool IsInteractionEnabled => isInteractionEnabled;

        protected abstract string ControllerName { get; }

        private IMixedRealityController controller;

        /// <inheritdoc />
        public override IMixedRealityController Controller
        {
            get { return controller; }
            set
            {
                controller = value;
                TrackingState = TrackingState.NotApplicable;

                if (controller != null && gameObject != null)
                {
                    InputSourceParent = controller.InputSource;
                    Handedness = controller.ControllerHandedness;
                    gameObject.name = ControllerName;
                }
            }
        }

        #endregion IMixedRealityPointer Implementation

        public override Vector3 Position => CameraCache.Main.transform.position + transform.forward * DefaultPointerExtent;

        protected virtual void SetVisibility(bool visible) => isDisabled = !visible;

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            if (RayStabilizer != null)
            {
                RayStabilizer = null;
            }

            base.OnSourceDetected(eventData);

            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                isInteractionEnabled = true;
            }
        }


        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                isInteractionEnabled = false;
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            cursorWasDisabledOnDown = isDisabled;

            if (cursorWasDisabledOnDown)
            {
                ResetCursor();
            }
            else
            {
                base.OnInputDown(eventData);
            }
        }

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            if (!cursorWasDisabledOnDown)
            {
                base.OnInputUp(eventData);

                if (isDisabled)
                {
                    ResetCursor();
                }
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        private void ResetCursor()
        {
            SetVisibility(true);
            transform.rotation = CameraCache.Main.transform.rotation;
        }

        #region MonoBehaviour Implementation

        protected override void Start()
        {
            isDisabled = DisableCursorOnStart;

            base.Start();

            if (RayStabilizer != null)
            {
                RayStabilizer = null;
            }

            foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
            {
                if (inputSource.SourceId == Controller.InputSource.SourceId)
                {
                    isInteractionEnabled = true;
                    break;
                }
            }
        }

        private void Update()
        {
            if (!hideCursorWhenInactive || isDisabled) { return; }

            timeoutTimer += Time.unscaledDeltaTime;

            if (timeoutTimer >= hideTimeout)
            {
                timeoutTimer = 0.0f;
                SetVisibility(false);
            }
        }

        #endregion MonoBehaviour Implementation
    }
}
