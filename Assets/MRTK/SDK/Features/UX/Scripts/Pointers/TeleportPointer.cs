// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;
using UnityEngine;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    /// <summary>
    /// Implementation for teleportation pointer to support movement based on teleport raycasts and requests with the MRTK Teleport system
    /// </summary>
    [RequireComponent(typeof(DistorterGravity))]
    [AddComponentMenu("Scripts/MRTK/SDK/TeleportPointer")]
    public class TeleportPointer : CurvePointer, IMixedRealityTeleportPointer, IMixedRealityTeleportHandler
    {
        /// <summary>
        /// True if a teleport request is being raised, false otherwise.
        /// </summary>
        public bool TeleportRequestRaised { get; private set; } = false;

        /// <summary>
        /// The result from the last raycast.
        /// </summary>
        public TeleportSurfaceResult TeleportSurfaceResult { get; private set; } = TeleportSurfaceResult.None;

        /// <inheritdoc />
        public IMixedRealityTeleportHotSpot TeleportHotSpot { get; set; }

        [SerializeField]
        [Tooltip("Teleport Pointer will only respond to input events for teleportation that match this MixedRealityInputAction")]
        private MixedRealityInputAction teleportAction = MixedRealityInputAction.None;

        /// <summary>
        /// Teleport pointer will only respond to input events for teleportation that match this MixedRealityInputAction.
        /// </summary>
        public MixedRealityInputAction TeleportInputAction => teleportAction;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The threshold amount for joystick input (Dead Zone)")]
        private float inputThreshold = 0.5f;

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("If Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like the forward direction, we apply this offset to make navigation feel more natural")]
        private float angleOffset = 0f;

        [SerializeField]
        [Range(5f, 90f)]
        [Tooltip("The angle from the pointer's forward position that will activate the teleport.")]
        private float teleportActivationAngle = 45f;

        [SerializeField]
        [Range(5f, 90f)]
        [Tooltip("The angle from the joystick left and right position that will activate a rotation")]
        private float rotateActivationAngle = 22.5f;

        [SerializeField]
        [Range(5f, 180f)]
        [Tooltip("The amount to rotate the camera when rotation is activated")]
        private float rotationAmount = 90f;

        [SerializeField]
        [Range(5, 90f)]
        [Tooltip("The angle from the joystick down position that will activate a strafe that will move the camera back")]
        private float backStrafeActivationAngle = 45f;

        [SerializeField]
        [Tooltip("The distance to move the camera when the strafe is activated")]
        private float strafeAmount = 0.25f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The up direction threshold to use when determining if a surface is 'flat' enough to teleport to.")]
        private float upDirectionThreshold = 0.2f;

        [SerializeField]
        protected Gradient LineColorHotSpot = new Gradient();

        [SerializeField]
        [Tooltip("Layers that are considered 'valid' for navigation")]
        protected LayerMask ValidLayers = UnityPhysics.DefaultRaycastLayers;

        [SerializeField]
        [Tooltip("Layers that are considered 'invalid' for navigation")]
        protected LayerMask InvalidLayers = UnityPhysics.IgnoreRaycastLayer;

        [SerializeField]
        private DistorterGravity gravityDistorter = null;

        /// <summary>
        /// The Gravity Distorter that is affecting the <see cref="Utilities.BaseMixedRealityLineDataProvider"/> attached to this pointer.
        /// </summary>
        public DistorterGravity GravityDistorter => gravityDistorter;

        private float cachedInputThreshold = 0f;

        private float inputThresholdSquared = 0f;

        /// <summary>
        /// The square of the InputThreshold value.
        /// </summary>
        private float InputThresholdSquared
        {
            get
            {
                if (!Mathf.Approximately(cachedInputThreshold, inputThreshold))
                {
                    inputThresholdSquared = Mathf.Pow(inputThreshold, 2f);
                    cachedInputThreshold = inputThreshold;
                }
                return inputThresholdSquared;
            }
        }


        #region Audio Management
        [Header("Audio management")]
        [SerializeField]
        private AudioSource pointerAudioSource = null;

        [SerializeField]
        private AudioClip teleportRequestedClip = null;

        [SerializeField]
        private AudioClip teleportCompletedClip = null;
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            // Disable renderers so that they don't display before having been processed (which manifests as a flash at the origin).
            var renderers = GetComponents<Renderer>();
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.enabled = false;
                }
            }

            if (gravityDistorter == null)
            {
                gravityDistorter = GetComponent<DistorterGravity>();
            }

            if (!lateRegisterTeleport)
            {
                CoreServices.TeleportSystem?.RegisterHandler<IMixedRealityTeleportHandler>(this);
            }
        }

        protected override async void Start()
        {
            base.Start();

            if (lateRegisterTeleport)
            {
                if (CoreServices.TeleportSystem == null)
                {
                    await new WaitUntil(() => CoreServices.TeleportSystem != null);

                    // We've been destroyed during the await.
                    if (this == null)
                    {
                        return;
                    }

                    // The pointer's input source was lost during the await.
                    if (Controller == null)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }
                lateRegisterTeleport = false;
                CoreServices.TeleportSystem.RegisterHandler<IMixedRealityTeleportHandler>(this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            CoreServices.TeleportSystem?.UnregisterHandler<IMixedRealityTeleportHandler>(this);
        }

        private Vector2 currentInputPosition = Vector2.zero;

        protected bool isTeleportRequestActive = false;

        private bool lateRegisterTeleport = true;

        private bool canTeleport = false;

        private bool canMove = false;

        protected Gradient GetLineGradient(TeleportSurfaceResult targetResult)
        {
            switch (targetResult)
            {
                case TeleportSurfaceResult.None:
                    return LineColorNoTarget;
                case TeleportSurfaceResult.Valid:
                    return LineColorValid;
                case TeleportSurfaceResult.Invalid:
                    return LineColorInvalid;
                case TeleportSurfaceResult.HotSpot:
                    return LineColorHotSpot;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetResult), targetResult, null);
            }
        }

        #region IMixedRealityPointer Implementation

        /// <inheritdoc />
        public override bool IsInteractionEnabled => !isTeleportRequestActive && TeleportRequestRaised && MixedRealityToolkit.IsTeleportSystemEnabled;

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("The Y orientation of the pointer - used for rotation and navigation")]
        private float pointerOrientation = 0f;

        /// <inheritdoc />
        public float PointerOrientation
        {
            get
            {
                if (TeleportHotSpot != null &&
                    TeleportHotSpot.OverrideTargetOrientation &&
                    TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                {
                    return TeleportHotSpot.TargetOrientation;
                }

                return pointerOrientation + (raycastOrigin != null ? raycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set
            {
                pointerOrientation = value < 0
                    ? Mathf.Clamp(value, -360f, 0f)
                    : Mathf.Clamp(value, 0f, 360f);
            }
        }

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] TeleportPointer.OnPreSceneQuery");

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                // Set up our rays
                // Turn off gravity so we get accurate rays
                GravityDistorter.enabled = false;

                base.OnPreSceneQuery();

                // Re-enable gravity if we're looking at a hotspot
                GravityDistorter.enabled = (TeleportSurfaceResult == TeleportSurfaceResult.HotSpot);
            }
        }

        private static readonly ProfilerMarker OnPostSceneQueryPerfMarker = new ProfilerMarker("[MRTK] TeleportPointer.OnPostSceneQuery");

        /// <inheritdoc />
        public override void OnPostSceneQuery()
        {
            using (OnPostSceneQueryPerfMarker.Auto())
            {
                if (IsSelectPressed)
                {
                    CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Handedness);
                }

                if (currentInputPosition != Vector2.zero && Controller != null)
                {
                    CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Controller.ControllerHandedness);
                }

                // Use the results from the last update to set our NavigationResult
                float clearWorldLength = 0f;
                TeleportSurfaceResult = TeleportSurfaceResult.None;
                GravityDistorter.enabled = false;

                if (IsInteractionEnabled)
                {
                    LineBase.enabled = true;

                    // If we hit something
                    if (Result.CurrentPointerTarget != null)
                    {
                        // Check if it's in our valid layers
                        if (((1 << Result.CurrentPointerTarget.layer) & ValidLayers.value) != 0)
                        {
                            // See if it's a hot spot
                            if (TeleportHotSpot != null && TeleportHotSpot.IsActive)
                            {
                                TeleportSurfaceResult = TeleportSurfaceResult.HotSpot;
                                // Turn on gravity, point it at hotspot
                                GravityDistorter.WorldCenterOfGravity = TeleportHotSpot.Position;
                                GravityDistorter.enabled = true;
                            }
                            else
                            {
                                // If it's NOT a hotspot, check if the hit normal is too steep
                                // (Hotspots override dot requirements)
                                TeleportSurfaceResult = Vector3.Dot(Result.Details.LastRaycastHit.normal, Vector3.up) > upDirectionThreshold
                                    ? TeleportSurfaceResult.Valid
                                    : TeleportSurfaceResult.Invalid;
                            }
                        }
                        else if (((1 << Result.CurrentPointerTarget.layer) & InvalidLayers) != 0)
                        {
                            TeleportSurfaceResult = TeleportSurfaceResult.Invalid;
                        }
                        else
                        {
                            TeleportSurfaceResult = TeleportSurfaceResult.None;
                        }

                        clearWorldLength = Result.Details.RayDistance;

                        // Clamp the end of the parabola to the result hit's point
                        LineBase.LineEndClamp = LineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, LineCastResolution);
                        BaseCursor?.SetVisibility(TeleportSurfaceResult == TeleportSurfaceResult.Valid || TeleportSurfaceResult == TeleportSurfaceResult.HotSpot);
                    }
                    else
                    {
                        BaseCursor?.SetVisibility(false);
                        LineBase.LineEndClamp = 1f;
                    }

                    // Set the line color
                    for (int i = 0; i < LineRenderers.Length; i++)
                    {
                        LineRenderers[i].LineColor = GetLineGradient(TeleportSurfaceResult);
                    }
                }
                else
                {
                    LineBase.enabled = false;
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            if (gameObject == null) return;

            if (TeleportHotSpot != null)
            {
                CoreServices.TeleportSystem?.RaiseTeleportCanceled(this, TeleportHotSpot);
                TeleportHotSpot = null;
            }
        }

        #endregion IMixedRealityPointer Implementation

        #region IMixedRealityInputHandler Implementation

        private static readonly ProfilerMarker OnInputChangedPerfMarker = new ProfilerMarker("[MRTK] TeleportPointer.OnInputChanged");

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            using (OnInputChangedPerfMarker.Auto())
            {
                // Don't process input if we've got an active teleport request in progress.
                if (isTeleportRequestActive || CoreServices.TeleportSystem == null)
                {
                    return;
                }

                if (eventData.SourceId == InputSourceParent.SourceId &&
                    eventData.Handedness == Handedness &&
                    eventData.MixedRealityInputAction == teleportAction)
                {
                    currentInputPosition = eventData.InputData;
                }

                if (currentInputPosition.sqrMagnitude > InputThresholdSquared)
                {
                    // Get the angle of the pointer input
                    float angle = Mathf.Atan2(currentInputPosition.x, currentInputPosition.y) * Mathf.Rad2Deg;

                    // Offset the angle so it's 'forward' facing
                    angle += angleOffset;
                    PointerOrientation = angle;

                    if (!TeleportRequestRaised)
                    {
                        float absoluteAngle = Mathf.Abs(angle);

                        if (absoluteAngle < teleportActivationAngle)
                        {
                            TeleportRequestRaised = true;

                            CoreServices.TeleportSystem?.RaiseTeleportRequest(this, TeleportHotSpot);
                            if (pointerAudioSource != null && teleportRequestedClip != null)
                            {
                                pointerAudioSource.PlayOneShot(teleportRequestedClip);
                            }
                        }
                        else if (canMove)
                        {
                            // wrap the angle value.
                            if (absoluteAngle > 180f)
                            {
                                absoluteAngle = Mathf.Abs(absoluteAngle - 360f);
                            }

                            // Calculate the offset rotation angle from the 90 degree mark.
                            // Half the rotation activation angle amount to make sure the activation angle stays centered at 90.
                            float offsetRotationAngle = 90f - rotateActivationAngle;

                            // subtract it from our current angle reading
                            offsetRotationAngle = absoluteAngle - offsetRotationAngle;

                            // if it's less than zero, then we don't have activation
                            if (offsetRotationAngle > 0)
                            {
                                // check to make sure we're still under our activation threshold.
                                if (offsetRotationAngle < 2 * rotateActivationAngle)
                                {
                                    canMove = false;
                                    // Rotate the camera by the rotation amount.  If our angle is positive then rotate in the positive direction, otherwise in the opposite direction.
                                    MixedRealityPlayspace.RotateAround(CameraCache.Main.transform.position, Vector3.up, angle >= 0.0f ? rotationAmount : -rotationAmount);
                                }
                                else // We may be trying to strafe backwards.
                                {
                                    // Calculate the offset rotation angle from the 180 degree mark.
                                    // Half the strafe activation angle to make sure the activation angle stays centered at 180f
                                    float offsetStrafeAngle = 180f - backStrafeActivationAngle;
                                    // subtract it from our current angle reading
                                    offsetStrafeAngle = absoluteAngle - offsetStrafeAngle;

                                    // Check to make sure we're still under our activation threshold.
                                    if (offsetStrafeAngle > 0 && offsetStrafeAngle <= backStrafeActivationAngle)
                                    {
                                        canMove = false;
                                        var height = MixedRealityPlayspace.Position.y;
                                        var newPosition = -CameraCache.Main.transform.forward * strafeAmount + MixedRealityPlayspace.Position;
                                        newPosition.y = height;
                                        MixedRealityPlayspace.Position = newPosition;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    canTeleport = TeleportSurfaceResult == TeleportSurfaceResult.Valid || TeleportSurfaceResult == TeleportSurfaceResult.HotSpot;
                    if (!canTeleport && !TeleportRequestRaised)
                    {
                        // Reset the move flag when the user stops moving the joystick
                        // but hasn't yet started teleport request.
                        canMove = true;
                    }

                    if (canTeleport)
                    {
                        TeleportRequestRaised = false;

                        if (TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                            TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                        {
                            CoreServices.TeleportSystem?.RaiseTeleportStarted(this, TeleportHotSpot);
                            if (pointerAudioSource != null && teleportCompletedClip != null)
                            {
                                pointerAudioSource.PlayOneShot(teleportCompletedClip);
                            }
                        }
                    }

                    if (TeleportRequestRaised)
                    {
                        TeleportRequestRaised = false;
                        CoreServices.TeleportSystem?.RaiseTeleportCanceled(this, TeleportHotSpot);
                    }
                }
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region IMixedRealityTeleportHandler Implementation

        private static readonly ProfilerMarker OnTeleportRequestPerfMarker = new ProfilerMarker("[MRTK] TeleportPointer.OnPreSceneQuery");

        /// <inheritdoc />
        public virtual void OnTeleportRequest(TeleportEventData eventData)
        {
            using (OnTeleportRequestPerfMarker.Auto())
            {
                // Only turn off the pointer if we're not the one sending the request
                if (eventData.Pointer.PointerId == PointerId)
                {
                    isTeleportRequestActive = false;
                    BaseCursor?.SetVisibility(true);
                }
                else
                {
                    isTeleportRequestActive = true;
                    BaseCursor?.SetVisibility(false);
                }
            }
        }

        private static readonly ProfilerMarker OnTeleportStartedPerfMarker = new ProfilerMarker("[MRTK] TeleportPointer.OnTeleportStarted");

        /// <inheritdoc />
        public virtual void OnTeleportStarted(TeleportEventData eventData)
        {
            using (OnTeleportStartedPerfMarker.Auto())
            {
                // Turn off all pointers while we teleport.
                isTeleportRequestActive = true;
                BaseCursor?.SetVisibility(false);
            }
        }

        private static readonly ProfilerMarker OnTeleportCompletedPerfMarker = new ProfilerMarker("[MRTK] TeleportPointer.OnTeleportCompleted");

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(TeleportEventData eventData)
        {
            using (OnTeleportCompletedPerfMarker.Auto())
            {
                isTeleportRequestActive = false;
                BaseCursor?.SetVisibility(false);
            }
        }

        private static readonly ProfilerMarker OnTeleportCanceledPerfMarker = new ProfilerMarker("[MRTK] TeleportPointer.OnTeleportCanceled");

        /// <inheritdoc />
        public virtual void OnTeleportCanceled(TeleportEventData eventData)
        {
            using (OnTeleportCanceledPerfMarker.Auto())
            {
                TeleportRequestRaised = false;
                isTeleportRequestActive = false;
                BaseCursor?.SetVisibility(false);
            }
        }

        #endregion IMixedRealityTeleportHandler Implementation
    }
}
