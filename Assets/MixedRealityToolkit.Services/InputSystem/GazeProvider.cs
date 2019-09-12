// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This class provides Gaze as an Input Source so users can interact with objects using their head.
    /// </summary>
    /// <remarks>
    /// The gaze provider primarily services as the source for consumers to get data about the currently gazed
    /// targets and information about both head and eye gaze. Note that this class doesn't actually handle
    /// the raycasts itself - it instead serves as an information sink (for example, see UpdateGazeInfoFromHit)
    /// which is used to update its internal state.
    /// </remarks>
    [DisallowMultipleComponent]
    public class GazeProvider :
        BaseService,
        IMixedRealityGazeProvider,
        IMixedRealityEyeGazeProvider,
        IMixedRealityInputHandler
    {
        public GazeProvider(
            IMixedRealityServiceRegistrar registrar,
            MixedRealityInputSystemProfile profile)
        {
            this.profile = profile.GazeProfile;
            gazeTransform = this.profile.GazeTransform;
            GazeCursorPrefab = this.profile.GazeCursorPrefab;
        }

        private MixedRealityGazeProfile profile;
        private Transform gazeTransform;

        private const float VelocityThreshold = 0.1f;

        private const float MovementThreshold = 0.01f;

        /// <inheritdoc />
        public bool UseEyeTracking
        {
            get { return profile.UseEyeTracking; }
            set { profile.UseEyeTracking = value; }
        }

        /// <inheritdoc />
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <inheritdoc />
        public IMixedRealityInputSource GazeInputSource
        {
            get
            {
                if (gazeInputSource == null)
                {
                    gazeInputSource = new BaseGenericInputSource("Gaze", sourceType: InputSourceType.Head);
                    gazePointer.SetGazeInputSourceParent(gazeInputSource);
                }

                return gazeInputSource;
            }
        }

        private BaseGenericInputSource gazeInputSource;

        /// <inheritdoc />
        public IMixedRealityPointer GazePointer => gazePointer ?? InitializeGazePointer();
        private InternalGazePointer gazePointer = null;

        /// <inheritdoc />
        public GameObject GazeCursorPrefab { private get; set; }

        /// <inheritdoc />
        public IMixedRealityCursor GazeCursor => GazePointer.BaseCursor;

        /// <inheritdoc />
        public GameObject GazeTarget { get; private set; }

        /// <inheritdoc />
        public MixedRealityRaycastHit HitInfo { get; private set; }

        /// <inheritdoc />
        public Vector3 HitPosition { get; private set; }

        /// <inheritdoc />
        public Vector3 HitNormal { get; private set; }

        /// <inheritdoc />
        public Vector3 GazeOrigin => gazePointer != null ? gazePointer.Rays[0].Origin : Vector3.zero;

        /// <inheritdoc />
        public Vector3 GazeDirection => GazePointer.Rays[0].Direction;

        /// <inheritdoc />
        public Vector3 HeadVelocity { get; private set; }

        /// <inheritdoc />
        public Vector3 HeadMovementDirection { get; private set; }

        /// <inheritdoc />
        public GameObject GameObjectReference => CameraCache.Main.gameObject;

        private float lastHitDistance = 2.0f;

        private Vector3 lastHeadPosition = Vector3.zero;

        /// <inheritdoc />
        public bool IsEyeGazeValid => IsEyeTrackingAvailable && UseEyeTracking;

        /// <inheritdoc />
        public DateTime Timestamp { get; private set; }

        private Ray latestEyeGaze = default(Ray);
        private DateTime latestEyeTrackingUpdate = DateTime.MinValue;
        private readonly float maxEyeTrackingTimeoutInSeconds = 2.0f;

        #region InternalGazePointer Class

        private class InternalGazePointer : GenericPointer
        {
            private readonly Transform gazeTransform;
            private readonly BaseRayStabilizer stabilizer;
            private readonly GazeProvider gazeProvider;

            public InternalGazePointer(GazeProvider gazeProvider, string pointerName, IMixedRealityInputSource inputSourceParent, LayerMask[] raycastLayerMasks, float pointerExtent, Transform gazeTransform, BaseRayStabilizer stabilizer)
                    : base(pointerName, inputSourceParent)
            {
                this.gazeProvider = gazeProvider;
                PrioritizedLayerMasksOverride = raycastLayerMasks;
                this.pointerExtent = pointerExtent;
                this.gazeTransform = gazeTransform;
                this.stabilizer = stabilizer;
                IsInteractionEnabled = true;
            }

            #region IMixedRealityPointer Implementation

            /// <inheritdoc />
            public override IMixedRealityController Controller { get; set; }

            /// <inheritdoc />
            public override IMixedRealityInputSource InputSourceParent { get; protected set; }

            private float pointerExtent;

            /// <inheritdoc />
            public override float PointerExtent
            {
                get { return pointerExtent; }
                set { pointerExtent = value; }
            }

            // Is the pointer currently down
            private bool isDown = false;

            // Input source that raised pointer down
            private IMixedRealityInputSource currentInputSource;

            // Handedness of the input source that raised pointer down
            private Handedness currentHandedness = Handedness.None;

            /// <summary>
            /// Only for use when initializing Gaze Pointer on startup.
            /// </summary>
            internal void SetGazeInputSourceParent(IMixedRealityInputSource gazeInputSource)
            {
                InputSourceParent = gazeInputSource;
            }

            /// <inheritdoc />
            public override void OnPreSceneQuery()
            {
                Vector3 newGazeOrigin = Vector3.zero;
                Vector3 newGazeNormal = Vector3.zero;

                if (gazeProvider.UseEyeTracking && gazeProvider.IsEyeTrackingAvailable)
                {
                    gazeProvider.gazeInputSource.SourceType = InputSourceType.Eyes;
                    newGazeOrigin = gazeProvider.latestEyeGaze.origin;
                    newGazeNormal = gazeProvider.latestEyeGaze.direction;
                }
                else
                {
                    gazeProvider.gazeInputSource.SourceType = InputSourceType.Head;
                    newGazeOrigin = gazeTransform.position;
                    newGazeNormal = gazeTransform.forward;

                    // Update gaze info from stabilizer
                    if (stabilizer != null)
                    {
                        stabilizer.UpdateStability(gazeTransform.localPosition, gazeTransform.localRotation * Vector3.forward);
                        newGazeOrigin = gazeTransform.parent.TransformPoint(stabilizer.StablePosition);
                        newGazeNormal = gazeTransform.parent.TransformDirection(stabilizer.StableRay.direction);
                    }
                }

                Vector3 endPoint = newGazeOrigin + (newGazeNormal * pointerExtent);
                Rays[0].UpdateRayStep(ref newGazeOrigin, ref endPoint);

                gazeProvider.HitPosition = Rays[0].Origin + (gazeProvider.lastHitDistance * Rays[0].Direction);
            }

            public override void OnPostSceneQuery()
            {
                if (isDown)
                {
                    InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, currentHandedness, currentInputSource);
                }
            }

            /// <inheritdoc />
            public override void OnPreCurrentPointerTargetChange() { }

            /// <inheritdoc />
            public override Vector3 Position => gazeTransform.position;

            /// <inheritdoc />
            public override Quaternion Rotation => gazeTransform.rotation;

            #endregion IMixedRealityPointer Implementation

            /// <summary>
            /// Press this pointer. This sends a pointer down event across the input system.
            /// </summary>
            /// <param name="mixedRealityInputAction">The input action that corresponds to the pressed button or axis.</param>
            /// <param name="handedness">Optional handedness of the source that pressed the pointer.</param>
            public void RaisePointerDown(MixedRealityInputAction mixedRealityInputAction, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null)
            {
                isDown = true;
                currentHandedness = handedness;
                currentInputSource = inputSource;
                InputSystem.RaisePointerDown(this, mixedRealityInputAction, handedness, inputSource);
            }

            /// <summary>
            /// Release this pointer. This sends pointer clicked and pointer up events across the input system.
            /// </summary>
            /// <param name="mixedRealityInputAction">The input action that corresponds to the released button or axis.</param>
            /// <param name="handedness">Optional handedness of the source that released the pointer.</param>
            public void RaisePointerUp(MixedRealityInputAction mixedRealityInputAction, Handedness handedness = Handedness.None, IMixedRealityInputSource inputSource = null)
            {
                isDown = false;
                currentHandedness = Handedness.None;
                currentInputSource = null;
                InputSystem.RaisePointerClicked(this, mixedRealityInputAction, 0, handedness, inputSource);
                InputSystem.RaisePointerUp(this, mixedRealityInputAction, handedness, inputSource);
            }
        }

        #endregion InternalGazePointer Class

        /// <inheritdoc />
        public override void Enable()
        {
            // The lifetime of the gaze provider is controlled by multiple forces:
            // The MixedRealityToolkit object itself during instantiation (to be consistent with
            // how the other focus and raycast providers work, and the input system itself (when
            // it is enabled/disabled). As a result, certain functions have to guard against the
            // current enabled/disabled state.
            if (Enabled)
            {
                return;
            }

#if UNITY_EDITOR
            if (profile.MinHeadVelocityThreshold > profile.MaxHeadVelocityThreshold)
            {
                Debug.LogWarning("Minimum head velocity threshold should be less than the maximum velocity threshold. Changing now.");
                profile.MinHeadVelocityThreshold = profile.MaxHeadVelocityThreshold;
            }
#endif

            base.Enable();
            CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler>(this);
            GazePointer.BaseCursor?.SetVisibility(true);
            CoreServices.InputSystem.RaiseSourceDetected(GazeInputSource);
            Enabled = true;
        }

        public override void Update()
        {
            if (!Enabled)
            {
                return;
            }

            if (MixedRealityRaycaster.DebugEnabled && gazeTransform != null)
            {
                Debug.DrawRay(GazeOrigin, (HitPosition - GazeOrigin), Color.white);
            }

            // If flagged to do so (setCursorInvisibleWhenFocusLocked) and active (IsInteractionEnabled), set the visibility to !IsFocusLocked,
            // but don't touch the visibility when not active or not flagged.
            if (profile.SetCursorInvisibleWhenFocusLocked && gazePointer != null &&
                gazePointer.IsInteractionEnabled && GazeCursor != null && gazePointer.IsFocusLocked == GazeCursor.IsVisible)
            {
                GazeCursor.SetVisibility(!gazePointer.IsFocusLocked);
            }
        }

        public override void LateUpdate()
        {
            if (!Enabled)
            {
                return;
            }

            // Update head velocity.
            Vector3 headPosition = GazeOrigin;
            Vector3 headDelta = headPosition - lastHeadPosition;

            if (headDelta.sqrMagnitude < MovementThreshold * MovementThreshold)
            {
                headDelta = Vector3.zero;
            }

            if (Time.fixedDeltaTime > 0)
            {
                float velocityAdjustmentRate = 3f * Time.fixedDeltaTime;
                HeadVelocity = HeadVelocity * (1f - velocityAdjustmentRate) + headDelta * velocityAdjustmentRate / Time.fixedDeltaTime;

                if (HeadVelocity.sqrMagnitude < VelocityThreshold * VelocityThreshold)
                {
                    HeadVelocity = Vector3.zero;
                }
            }

            // Update Head Movement Direction
            float multiplier = Mathf.Clamp01(Mathf.InverseLerp(profile.MinHeadVelocityThreshold, profile.MaxHeadVelocityThreshold, HeadVelocity.magnitude));
            Vector3 newHeadMoveDirection = Vector3.Lerp(headPosition, HeadVelocity, multiplier).normalized;
            lastHeadPosition = headPosition;
            float directionAdjustmentRate = Mathf.Clamp01(5f * Time.fixedDeltaTime);

            HeadMovementDirection = Vector3.Slerp(HeadMovementDirection, newHeadMoveDirection, directionAdjustmentRate);

            if (MixedRealityRaycaster.DebugEnabled && gazeTransform != null)
            {
                Debug.DrawLine(lastHeadPosition, lastHeadPosition + HeadMovementDirection * 10f, Color.Lerp(Color.red, Color.green, multiplier));
                Debug.DrawLine(lastHeadPosition, lastHeadPosition + HeadVelocity, Color.yellow);
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            // The lifetime of the gaze provider is controlled by multiple forces:
            // The MixedRealityToolkit object itself during instantiation (to be consistent with
            // how the other focus and raycast providers work, and the input system itself (when
            // it is enabled/disabled). As a result, certain functions have to guard against the
            // current enabled/disabled state.
            if (!Enabled)
            {
                return;
            }

            base.Disable();
            CoreServices.InputSystem.RaiseSourceLost(GazeInputSource);
            GazePointer?.BaseCursor?.SetVisibility(false);
            CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler>(this);
            Enabled = false;
        }

        #region IMixedRealityInputHandler Implementation

        public void OnInputUp(InputEventData eventData)
        {
            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == GazePointer.PointerId)
                {
                    gazePointer.RaisePointerUp(eventData.MixedRealityInputAction, eventData.Handedness, eventData.InputSource);
                    return;
                }
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == GazePointer.PointerId)
                {
                    gazePointer.RaisePointerDown(eventData.MixedRealityInputAction, eventData.Handedness, eventData.InputSource);
                    return;
                }
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region Utilities

        private IMixedRealityPointer InitializeGazePointer()
        {
            if (gazeTransform == null)
            {
                gazeTransform = CameraCache.Main.transform;
            }

            Debug.Assert(gazeTransform != null, "No gaze transform to raycast from!");

            gazePointer = new InternalGazePointer(this, "Gaze Pointer", null, profile.RaycastLayerMasks, profile.MaxGazeCollisionDistance, gazeTransform, profile.Stabilizer);

            if ((GazeCursor == null) &&
                (GazeCursorPrefab != null))
            {
                GameObject cursor = UnityEngine.Object.Instantiate(GazeCursorPrefab);
                MixedRealityPlayspace.AddChild(cursor.transform);
                SetGazeCursor(cursor);
            }

            return gazePointer;
        }

        // All of the functions in the following sections update the internal state of the gaze pointer
        // based on raycast hits and eye gaze updates from other parts of the system. Note that these aren't
        // gated behind an if (Enabled) check - this is done because the gaze provider is disabled in tandem
        // with other systems that feed information into it, and ensuring that the pipe is open even when
        // this is disabled ensures that when re-enable happens, this class doesn't have to call out into
        // all of the other callers to requery the current state.

        /// <inheritdoc />
        public void UpdateGazeInfoFromHit(MixedRealityRaycastHit raycastHit)
        {
            HitInfo = raycastHit;
            if (raycastHit.transform != null)
            {
                GazeTarget = raycastHit.transform.gameObject;
                var ray = GazePointer.Rays[0];
                var lhd = (raycastHit.point - ray.Origin).magnitude;
                lastHitDistance = lhd;
                HitPosition = ray.Origin + lhd * ray.Direction;
                HitNormal = raycastHit.normal;
            }
            else
            {
                GazeTarget = null;
                HitPosition = Vector3.zero;
                HitNormal = Vector3.zero;
            }
        }

        /// <summary>
        /// Set the gaze cursor.
        /// </summary>
        public void SetGazeCursor(GameObject cursor)
        {
            Debug.Assert(cursor != null);
            cursor.transform.parent = CameraCache.Main.transform.parent;
            GazePointer.BaseCursor = cursor.GetComponent<IMixedRealityCursor>();
            Debug.Assert(GazePointer.BaseCursor != null, "Failed to load cursor");
            GazePointer.BaseCursor.SetVisibilityOnSourceDetected = false;
            GazePointer.BaseCursor.Pointer = GazePointer;
        }

        public void UpdateEyeGaze(IMixedRealityEyeGazeDataProvider provider, Ray eyeRay, DateTime timestamp)
        {
            latestEyeGaze = eyeRay;
            latestEyeTrackingUpdate = DateTime.UtcNow;
            Timestamp = timestamp;
        }

        public void UpdateEyeTrackingStatus(IMixedRealityEyeGazeDataProvider provider, bool userIsEyeCalibrated)
        {
            this.IsEyeCalibrationValid = userIsEyeCalibrated;
        }

        /// <summary>
        /// Ensure that we work with recent Eye Tracking data. Return false if we haven't received any
        /// new Eye Tracking data for more than 'maxETTimeoutInSeconds' seconds.
        /// </summary>
        private bool IsEyeTrackingAvailable => (DateTime.UtcNow - latestEyeTrackingUpdate).TotalSeconds <= maxEyeTrackingTimeoutInSeconds;

        /// <summary>
        /// Boolean to check whether the user went through the eye tracking calibration. 
        /// Initially the parameter will return null until it has received valid information from the eye tracking system.
        /// </summary>
        public bool? IsEyeCalibrationValid { get; private set; } = null;
        #endregion Utilities
    }
}
