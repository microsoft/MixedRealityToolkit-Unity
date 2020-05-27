// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;
using UnityEngine;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This class provides Gaze as an Input Source so users can interact with objects using their head.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Scripts/MRTK/Services/GazeProvider")]
    public class GazeProvider :
        InputSystemGlobalHandlerListener,
        IMixedRealityGazeProvider,
        IMixedRealityGazeProviderHeadOverride,
        IMixedRealityEyeGazeProvider,
        IMixedRealityInputHandler
    {
        private const float VelocityThreshold = 0.1f;

        private const float MovementThreshold = 0.01f;

        /// <summary>
        /// Used on Gaze Pointer initialization. To make the object lock/not lock when focus locked during runtime, use the IsTargetPositionLockedOnFocusLock 
        /// attribute of <see cref="GazePointer.IsTargetPositionLockedOnFocusLock"/>
        /// </summary>
        [SerializeField]
        [Tooltip("If true, initializes the gaze cursor to stay locked on the object when the cursor's focus is locked, otherwise it will continue following the head's direction")]
        private bool lockCursorWhenFocusLocked = true;

        [SerializeField]
        [Tooltip("If true, the gaze cursor will disappear when the pointer's focus is locked, to prevent the cursor from floating idly in the world.")]
        private bool setCursorInvisibleWhenFocusLocked = false;

        [SerializeField]
        [Tooltip("Maximum distance at which the gaze can hit a GameObject.")]
        private float maxGazeCollisionDistance = 10.0f;

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.
        /// <example>
        /// <para>Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)</para>
        /// <code language="csharp"><![CDATA[
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers &amp; ~sr;
        /// GazeProvider.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// ]]></code>
        /// </example>
        /// </summary>
        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.")]
        private LayerMask[] raycastLayerMasks = { UnityPhysics.DefaultRaycastLayers };

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        [SerializeField]
        [Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        private GazeStabilizer stabilizer = null;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        private Transform gazeTransform = null;

        [SerializeField]
        [Range(0.01f, 1f)]
        [Tooltip("Minimum head velocity threshold")]
        private float minHeadVelocityThreshold = 0.5f;

        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("Maximum head velocity threshold")]
        private float maxHeadVelocityThreshold = 2f;

        #region IMixedRealityGazeProvider Implementation

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
        public Vector3 GazeOrigin => GazePointer != null ? GazePointer.Rays[0].Origin : Vector3.zero;

        /// <inheritdoc />
        public Vector3 GazeDirection => GazePointer != null ? GazePointer.Rays[0].Direction : Vector3.forward;

        /// <inheritdoc />
        public Vector3 HeadVelocity { get; private set; }

        /// <inheritdoc />
        public Vector3 HeadMovementDirection { get; private set; }

        /// <inheritdoc />
        public GameObject GameObjectReference => gameObject;

        #endregion IMixedRealityGazeProvider Implementation

        private float lastHitDistance = 2.0f;
        private bool delayInitialization = true;
        private Vector3 lastHeadPosition = Vector3.zero;

        private Vector3? overrideHeadPosition = null;
        private Vector3? overrideHeadForward = null;

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
                get => pointerExtent;
                set => pointerExtent = value;
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

            private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] InternalGazePointer.OnPreSceneQuery");

            /// <inheritdoc />
            public override void OnPreSceneQuery()
            {
                using (OnPreSceneQueryPerfMarker.Auto())
                {
                    Vector3 newGazeOrigin;
                    Vector3 newGazeNormal;

                    if (gazeProvider.IsEyeTrackingEnabledAndValid)
                    {
                        gazeProvider.gazeInputSource.SourceType = InputSourceType.Eyes;
                        newGazeOrigin = gazeProvider.LatestEyeGaze.origin;
                        newGazeNormal = gazeProvider.LatestEyeGaze.direction;
                    }
                    else
                    {
                        gazeProvider.gazeInputSource.SourceType = InputSourceType.Head;

                        if (gazeProvider.UseHeadGazeOverride && gazeProvider.overrideHeadPosition.HasValue && gazeProvider.overrideHeadForward.HasValue)
                        {
                            newGazeOrigin = gazeProvider.overrideHeadPosition.Value;
                            newGazeNormal = gazeProvider.overrideHeadForward.Value;
                            // Reset values in case the override source is removed
                            gazeProvider.overrideHeadPosition = null;
                            gazeProvider.overrideHeadForward = null;
                        }
                        else
                        {
                            newGazeOrigin = gazeTransform.position;
                            newGazeNormal = gazeTransform.forward;
                        }

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
            }

            private static readonly ProfilerMarker OnPostSceneQueryPerfMarker = new ProfilerMarker("[MRTK] InternalGazePointer.OnPostSceneQuery");

            public override void OnPostSceneQuery()
            {
                using (OnPostSceneQueryPerfMarker.Auto())
                {
                    if (isDown)
                    {
                        CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, currentHandedness, currentInputSource);
                    }
                }
            }

            /// <inheritdoc />
            public override void OnPreCurrentPointerTargetChange() { }

            /// <inheritdoc />
            public override Vector3 Position => gazeTransform.position;

            /// <inheritdoc />
            public override Quaternion Rotation => gazeTransform.rotation;
            
            /// <inheritdoc />
            public override void Reset()
            {
                Controller = null;
            }

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
                CoreServices.InputSystem?.RaisePointerDown(this, mixedRealityInputAction, handedness, inputSource);
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
                CoreServices.InputSystem?.RaisePointerClicked(this, mixedRealityInputAction, 0, handedness, inputSource);
                CoreServices.InputSystem?.RaisePointerUp(this, mixedRealityInputAction, handedness, inputSource);
            }
        }

        #endregion InternalGazePointer Class

        #region MonoBehaviour Implementation

        private void OnValidate()
        {
            if (minHeadVelocityThreshold > maxHeadVelocityThreshold)
            {
                Debug.LogWarning("Minimum head velocity threshold should be less than the maximum velocity threshold. Changing now.");
                minHeadVelocityThreshold = maxHeadVelocityThreshold;
            }
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                RaiseSourceDetected();
            }
        }

        /// <inheritdoc />
        protected override async void Start()
        {
            base.Start();

            await EnsureInputSystemValid();

            if (this == null)
            {
                // We've been destroyed during the await.
                return;
            }

            GazePointer.BaseCursor?.SetVisibility(true);

            if (delayInitialization)
            {
                delayInitialization = false;
                RaiseSourceDetected();
            }
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] GazeProvider.Update");

        private void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (MixedRealityRaycaster.DebugEnabled && gazeTransform != null)
                {
                    Debug.DrawRay(GazeOrigin, (HitPosition - GazeOrigin), Color.white);
                }

                // If flagged to do so (setCursorInvisibleWhenFocusLocked) and active (IsInteractionEnabled), set the visibility to !IsFocusLocked,
                // but don't touch the visibility when not active or not flagged.
                if (setCursorInvisibleWhenFocusLocked && gazePointer != null &&
                    gazePointer.IsInteractionEnabled && GazeCursor != null && gazePointer.IsFocusLocked == GazeCursor.IsVisible)
                {
                    GazeCursor.SetVisibility(!gazePointer.IsFocusLocked);
                }
            }
        }

        private static readonly ProfilerMarker LateUpdatePerfMarker = new ProfilerMarker("[MRTK] GazeProvider.LateUpdate");

        private void LateUpdate()
        {
            using (LateUpdatePerfMarker.Auto())
            {
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
                float multiplier = Mathf.Clamp01(Mathf.InverseLerp(minHeadVelocityThreshold, maxHeadVelocityThreshold, HeadVelocity.magnitude));

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
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            GazePointer?.BaseCursor?.SetVisibility(false);

            // if true, component has never started and never fired onSourceDetected event
            if (!delayInitialization)
            {
                CoreServices.InputSystem?.RaiseSourceLost(GazeInputSource);
            }
        }

        #endregion MonoBehaviour Implementation

        #region InputSystemGlobalHandlerListener Implementation

        /// <inheritdoc />
        protected override void RegisterHandlers()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
        }

        /// <inheritdoc />
        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
        }

        #endregion InputSystemGlobalHandlerListener Implementation

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

        private static readonly ProfilerMarker InitializeGazePointerPerfMarker = new ProfilerMarker("[MRTK] GazeProvider.InitializeGazePointer");

        private IMixedRealityPointer InitializeGazePointer()
        {
            using (InitializeGazePointerPerfMarker.Auto())
            {
                if (gazeTransform == null)
                {
                    gazeTransform = CameraCache.Main.transform;
                }

                Debug.Assert(gazeTransform != null, "No gaze transform to raycast from!");

                gazePointer = new InternalGazePointer(this, "Gaze Pointer", null, raycastLayerMasks, maxGazeCollisionDistance, gazeTransform, stabilizer);

                if ((GazeCursor == null) &&
                    (GazeCursorPrefab != null))
                {
                    GameObject cursor = Instantiate(GazeCursorPrefab);
                    MixedRealityPlayspace.AddChild(cursor.transform);
                    SetGazeCursor(cursor);
                }

                // Initialize gaze pointer
                gazePointer.IsTargetPositionLockedOnFocusLock = lockCursorWhenFocusLocked;

                return gazePointer;
            }
        }

        private static readonly ProfilerMarker RaiseSourceDetectedPerfMarker = new ProfilerMarker("[MRTK] GazeProvider.RaiseSourceDetectec");

        private async void RaiseSourceDetected()
        {
            using (RaiseSourceDetectedPerfMarker.Auto())
            {
                await EnsureInputSystemValid();

                if (this == null)
                {
                    // We've been destroyed during the await.
                    return;
                }

                CoreServices.InputSystem?.RaiseSourceDetected(GazeInputSource);
                GazePointer.BaseCursor?.SetVisibility(true);
            }
        }

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
            cursor.transform.parent = transform.parent;
            GazePointer.BaseCursor = cursor.GetComponent<IMixedRealityCursor>();
            Debug.Assert(GazePointer.BaseCursor != null, "Failed to load cursor");
            GazePointer.BaseCursor.SetVisibilityOnSourceDetected = false;
            GazePointer.BaseCursor.Pointer = GazePointer;
        }

        #endregion Utilities

        #region IMixedRealityEyeGazeProvider Implementation

        private DateTime latestEyeTrackingUpdate = DateTime.MinValue;
        private static readonly float maxEyeTrackingTimeoutInSeconds = 2.0f;

        /// <inheritdoc />
        public bool IsEyeTrackingEnabledAndValid => IsEyeTrackingDataValid && IsEyeTrackingEnabled;

        /// <inheritdoc />
        public bool IsEyeTrackingDataValid => (DateTime.UtcNow - latestEyeTrackingUpdate).TotalSeconds <= maxEyeTrackingTimeoutInSeconds;

        /// <inheritdoc />
        public bool? IsEyeCalibrationValid { get; private set; } = null;

        /// <inheritdoc />
        public Ray LatestEyeGaze { get; private set; } = default(Ray);

        /// <inheritdoc />
        public bool IsEyeTrackingEnabled { get; set; }

        /// <inheritdoc />
        public DateTime Timestamp { get; private set; }

        /// <inheritdoc />
        public void UpdateEyeGaze(IMixedRealityEyeGazeDataProvider provider, Ray eyeRay, DateTime timestamp)
        {
            LatestEyeGaze = eyeRay;
            latestEyeTrackingUpdate = DateTime.UtcNow;
            Timestamp = timestamp;
        }

        /// <inheritdoc />
        public void UpdateEyeTrackingStatus(IMixedRealityEyeGazeDataProvider provider, bool userIsEyeCalibrated)
        {
            IsEyeCalibrationValid = userIsEyeCalibrated;
        }

        #endregion IMixedRealityEyeGazeProvider Implementation

        #region IMixedRealityGazeProviderHeadOverride Implementation

        /// <inheritdoc />
        public bool UseHeadGazeOverride { get; set; }

        /// <inheritdoc />
        public void OverrideHeadGaze(Vector3 position, Vector3 forward)
        {
            overrideHeadPosition = position;
            overrideHeadForward = forward;
        }

        #endregion IMixedRealityGazeProviderHeadOverride Implementation
    }
}
