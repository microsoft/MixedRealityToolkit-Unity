// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

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

        /// <inheritdoc />
        public override IMixedRealityController Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
                TrackingState = TrackingState.NotApplicable;
            }
        }

        #endregion IMixedRealityPointer Implementation

        public override Vector3 Position => CameraCache.Main.transform.position + transform.forward * DefaultPointerExtent;

        protected virtual void SetVisibility(bool visible) => isDisabled = !visible;

        #region IMixedRealitySourcePoseHandler Implementation

        private static readonly ProfilerMarker OnSourceDetectedPerfMarker = new ProfilerMarker("[MRTK] BaseMousePointer.OnSourceDetected");

        /// <inheritdoc />
        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            using (OnSourceDetectedPerfMarker.Auto())
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
        }


        private static readonly ProfilerMarker OnSourceLostPerfMarker = new ProfilerMarker("[MRTK] BaseMousePointer.OnSourceLost");

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            using (OnSourceLostPerfMarker.Auto())
            {
                base.OnSourceLost(eventData);

                if (eventData.SourceId == Controller?.InputSource.SourceId)
                {
                    isInteractionEnabled = false;
                }
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region IMixedRealityInputHandler Implementation

        private static readonly ProfilerMarker OnInputDownPerfMarker = new ProfilerMarker("[MRTK] BaseMousePointer.OnInputDown");

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            using (OnInputDownPerfMarker.Auto())
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
        }

        private static readonly ProfilerMarker OnInputUpPerfMarker = new ProfilerMarker("[MRTK] BaseMousePointer.OnInputUp");

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            using (OnInputUpPerfMarker.Auto())
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
        }

        #endregion IMixedRealityInputHandler Implementation

        private static readonly ProfilerMarker ResetCursorPerfMarker = new ProfilerMarker("[MRTK] BaseMousePointer.ResetCursor");

        private void ResetCursor()
        {
            using (ResetCursorPerfMarker.Auto())
            {
                SetVisibility(true);
                transform.rotation = CameraCache.Main.transform.rotation;
            }
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

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] BaseMousePointer.Update");

        private void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!hideCursorWhenInactive || isDisabled) { return; }

                timeoutTimer += Time.unscaledDeltaTime;

                if (timeoutTimer >= hideTimeout)
                {
                    timeoutTimer = 0.0f;
                    SetVisibility(false);
                }
            }
        }

        #endregion MonoBehaviour Implementation
    }
}
