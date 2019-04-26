// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    [RequireComponent(typeof(DistorterGravity))]
    public class TeleportPointer : LinePointer, IMixedRealityTeleportPointer, IMixedRealityTeleportHandler
    {
        public bool TeleportRequestRaised { get { return teleportEnabled; } }

        [SerializeField]
        private MixedRealityInputAction teleportAction = MixedRealityInputAction.None;

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

        protected override void OnEnable()
        {
            base.OnEnable();

            if (gravityDistorter == null)
            {
                gravityDistorter = GetComponent<DistorterGravity>();
            }

            if (MixedRealityToolkit.IsInitialized && MixedRealityToolkit.TeleportSystem != null && !lateRegisterTeleport)
            {
                MixedRealityToolkit.TeleportSystem.Register(gameObject);
            }
        }

        protected override async void Start()
        {
            base.Start();

            if (lateRegisterTeleport && MixedRealityToolkit.Instance.ActiveProfile.IsTeleportSystemEnabled)
            {
                if (MixedRealityToolkit.TeleportSystem == null)
                {
                    await new WaitUntil(() => MixedRealityToolkit.TeleportSystem != null);

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
                MixedRealityToolkit.TeleportSystem.Register(gameObject);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MixedRealityToolkit.TeleportSystem?.Unregister(gameObject);
        }

        private Vector2 currentInputPosition = Vector2.zero;

        protected bool isTeleportRequestActive = false;

        private bool lateRegisterTeleport = true;

        private bool teleportEnabled = false;

        private bool canTeleport = false;

        private bool canMove = false;

        /// <summary>
        /// The result from the last raycast.
        /// </summary>
        public TeleportSurfaceResult TeleportSurfaceResult { get; private set; } = TeleportSurfaceResult.None;

        /// <inheritdoc />
        public IMixedRealityTeleportHotSpot TeleportHotSpot { get; set; }

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
        public override bool IsInteractionEnabled => !isTeleportRequestActive && teleportEnabled && MixedRealityToolkit.IsTeleportSystemEnabled;

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

        public override void OnPreSceneQuery()
        {
            if (LineBase == null)
            {
                return;
            }

            // Make sure our array will hold
            if (Rays == null || Rays.Length != LineCastResolution)
            {
                Rays = new RayStep[LineCastResolution];
            }

            // Set up our rays
            // Turn off gravity so we get accurate rays
            GravityDistorter.enabled = false;

            float stepSize = 1f / Rays.Length;
            Vector3 lastPoint = LineBase.GetUnClampedPoint(0f);

            for (int i = 0; i < Rays.Length; i++)
            {
                Vector3 currentPoint = LineBase.GetUnClampedPoint(stepSize * (i + 1));
                Rays[i].UpdateRayStep(ref lastPoint, ref currentPoint);
                lastPoint = currentPoint;
            }

            // Re-enable gravity if we're looking at a hotspot
            GravityDistorter.enabled = (TeleportSurfaceResult == TeleportSurfaceResult.HotSpot);
        }

        public override void OnPostSceneQuery()
        {
            if (IsSelectPressed)
            {
                MixedRealityToolkit.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Handedness);
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

        #endregion IMixedRealityPointer Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            // Don't process input if we've got an active teleport request in progress.
            if (isTeleportRequestActive || !MixedRealityToolkit.IsTeleportSystemEnabled)
            {
                return;
            }

            if (eventData.SourceId == InputSourceParent.SourceId &&
                eventData.Handedness == Handedness &&
                eventData.MixedRealityInputAction == teleportAction)
            {
                currentInputPosition = eventData.InputData;
            }

            if (Mathf.Abs(currentInputPosition.y) > inputThreshold ||
                Mathf.Abs(currentInputPosition.x) > inputThreshold)
            {
                // Get the angle of the pointer input
                float angle = Mathf.Atan2(currentInputPosition.x, currentInputPosition.y) * Mathf.Rad2Deg;

                // Offset the angle so it's 'forward' facing
                angle += angleOffset;
                PointerOrientation = angle;

                if (!teleportEnabled)
                {
                    float absoluteAngle = Mathf.Abs(angle);

                    if (absoluteAngle < teleportActivationAngle)
                    {
                        teleportEnabled = true;

                        MixedRealityToolkit.TeleportSystem?.RaiseTeleportRequest(this, TeleportHotSpot);
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
                            if (offsetRotationAngle < rotateActivationAngle)
                            {
                                canMove = false;
                                // Rotate the camera by the rotation amount.  If our angle is positive then rotate in the positive direction, otherwise in the opposite direction.
                                MixedRealityToolkit.Instance.MixedRealityPlayspace.RotateAround(CameraCache.Main.transform.position, Vector3.up, angle >= 0.0f ? rotationAmount : -rotationAmount);
                            }
                            else // We may be trying to strafe backwards.
                            {
                                // Calculate the offset rotation angle from the 180 degree mark.
                                // Half the strafe activation angle to make sure the activation angle stays centered at 180f
                                float offsetStrafeAngle = 180f - backStrafeActivationAngle;
                                // subtract it from our current angle reading
                                offsetStrafeAngle = absoluteAngle - offsetStrafeAngle;

                                // Check to make sure we're still under our activation threshold.
                                if (offsetStrafeAngle > 0 && offsetStrafeAngle < backStrafeActivationAngle)
                                {
                                    canMove = false;
                                    var height = MixedRealityToolkit.Instance.MixedRealityPlayspace.position.y;
                                    var newPosition = -CameraCache.Main.transform.forward * strafeAmount + MixedRealityToolkit.Instance.MixedRealityPlayspace.position;
                                    newPosition.y = height;
                                    MixedRealityToolkit.Instance.MixedRealityPlayspace.position = newPosition;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!canTeleport && !teleportEnabled)
                {
                    // Reset the move flag when the user stops moving the joystick
                    // but hasn't yet started teleport request.
                    canMove = true;
                }

                if (canTeleport)
                {
                    canTeleport = false;
                    teleportEnabled = false;

                    if (TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                        TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                    {
                        MixedRealityToolkit.TeleportSystem?.RaiseTeleportStarted(this, TeleportHotSpot);
                    }
                }

                if (teleportEnabled)
                {
                    canTeleport = false;
                    teleportEnabled = false;
                    MixedRealityToolkit.TeleportSystem?.RaiseTeleportCanceled(this, TeleportHotSpot);
                }
            }

            if (teleportEnabled &&
                TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
            {
                canTeleport = true;
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region IMixedRealityTeleportHandler Implementation

        /// <inheritdoc />
        public virtual void OnTeleportRequest(TeleportEventData eventData)
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

        /// <inheritdoc />
        public virtual void OnTeleportStarted(TeleportEventData eventData)
        {
            // Turn off all pointers while we teleport.
            isTeleportRequestActive = true;
            BaseCursor?.SetVisibility(false);
        }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(TeleportEventData eventData)
        {
            isTeleportRequestActive = false;
            BaseCursor?.SetVisibility(false);
        }

        /// <inheritdoc />
        public virtual void OnTeleportCanceled(TeleportEventData eventData)
        {
            isTeleportRequestActive = false;
            BaseCursor?.SetVisibility(false);
        }

        #endregion IMixedRealityTeleportHandler Implementation
    }
}
