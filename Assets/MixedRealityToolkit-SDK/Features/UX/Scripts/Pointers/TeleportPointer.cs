// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Teleport;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    public class TeleportPointer : LinePointer
    {
        private static IMixedRealityTeleportSystem teleportSystem = null;
        protected static IMixedRealityTeleportSystem TeleportSystem => teleportSystem ?? (teleportSystem = MixedRealityManager.Instance.GetManager<IMixedRealityTeleportSystem>());

        [SerializeField]
        private MixedRealityInputAction teleportAction = MixedRealityInputAction.None;

        /// <summary>
        /// Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like
        /// the forward direction, so we apply this offset to make navigation feel more natural
        /// </summary>
        [SerializeField]
        private float angleOffset = -82.5f;

        [SerializeField]
        private float minValidDot = 0.2f;

        [SerializeField]
        protected Gradient LineColorHotSpot = new Gradient();

        [SerializeField]
        [Tooltip("Layers that are considered 'valid' for navigation")]
        protected LayerMask ValidLayers = 1 << 0; // Default

        [SerializeField]
        [Tooltip("Layers that are considered 'invalid' for navigation")]
        protected LayerMask InvalidLayers = 1 << 2; // Ignore raycast

        [SerializeField]
        private float inputThreshold = 0.05f;

        private Vector2 currentInputPosition = Vector2.zero;

        private bool teleportEnabled = false;

        private bool canTeleport = false;

        /// <summary>
        /// The position of the teleport target
        /// </summary>
        protected virtual Vector3 TeleportTargetPosition
        {
            get
            {
                if (!IsInteractionEnabled)
                {
                    return Vector3.zero;
                }

                if (TeleportTarget != null && TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                {
                    return TeleportTarget.Position;
                }

                Debug.Assert(Result != null);
                return Result.Details.Point;
            }
        }

        /// <summary>
        /// The normal of the teleport target
        /// </summary>
        protected virtual Vector3 TeleportTargetNormal
        {
            get
            {
                if (!IsInteractionEnabled)
                {
                    return Vector3.up;
                }

                if (TeleportTarget != null && TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                {
                    return TeleportTarget.Normal;
                }

                return Result.StartPoint;
            }
        }

        /// <summary>
        /// The result from the last raycast.
        /// </summary>
        protected TeleportSurfaceResult TeleportSurfaceResult = TeleportSurfaceResult.None;

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
        public override bool IsInteractionEnabled => !IsTeleportRequestActive && teleportEnabled;

        /// <inheritdoc />
        public override float PointerOrientation
        {
            get
            {
                if (!IsInteractionEnabled)
                {
                    return 0f;
                }

                if (TeleportSurfaceResult == TeleportSurfaceResult.HotSpot &&
                    TeleportTarget != null && TeleportTarget.OverrideTargetOrientation)
                {
                    return TeleportTarget.TargetOrientation;
                }

                // Use the camera orientation by default
                return CameraCache.Main.transform.eulerAngles.y - PointerOrientation;
            }
            set
            {
                // Store pointer orientation as the difference between camera and input
                base.PointerOrientation = value;
            }
        }

        public override void OnPreRaycast()
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
                Rays[i] = new RayStep(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }

            // Re-enable gravity if we're looking at a hotspot
            GravityDistorter.enabled = (TeleportSurfaceResult == TeleportSurfaceResult.HotSpot);
        }

        public override void OnPostRaycast()
        {
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
                        if (TeleportTarget != null && TeleportTarget.IsActive)
                        {
                            TeleportSurfaceResult = TeleportSurfaceResult.HotSpot;
                            // Turn on gravity, point it at hotspot
                            GravityDistorter.WorldCenterOfGravity = TeleportTarget.Position;
                            GravityDistorter.enabled = true;
                        }
                        else
                        {
                            // If it's NOT a hotspot, check if the hit normal is too steep 
                            // (Hotspots override dot requirements)
                            TeleportSurfaceResult = Vector3.Dot(Result.StartPoint, Vector3.up) < minValidDot
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

                    // Use the step index to determine the length of the hit
                    for (int i = 0; i <= Result.RayStepIndex; i++)
                    {
                        if (i == Result.RayStepIndex)
                        {
                            if (MixedRealityRaycaster.DebugEnabled)
                            {
                                Color debugColor = TeleportSurfaceResult != TeleportSurfaceResult.None
                                    ? Color.yellow
                                    : Color.cyan;

                                Debug.DrawLine(Result.StartPoint + Vector3.up * 0.1f, Result.StartPoint + Vector3.up * 0.1f, debugColor);
                            }

                            // Only add the distance between the start point and the hit
                            clearWorldLength += Vector3.Distance(Result.StartPoint, Result.StartPoint);
                        }
                        else if (i < Result.RayStepIndex)
                        {
                            // Add the full length of the step to our total distance
                            clearWorldLength += Rays[i].Length;
                        }
                    }

                    // Clamp the end of the parabola to the result hit's point
                    LineBase.LineEndClamp = LineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, LineCastResolution);
                }
                else
                {
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
        public override void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
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
                float angle = Mathf.Atan2(currentInputPosition.y, currentInputPosition.x) * Mathf.Rad2Deg;

                // Offset the angle so it's 'forward' facing
                angle += angleOffset;
                PointerOrientation = angle;
                //teleportEnabled = true;
            }
            else
            {
                //teleportEnabled = false;
                if (canTeleport)
                {
                    TeleportSystem.RaiseTeleportStarted(this, TeleportTarget);
                }
            }

            if (IsInteractionEnabled &&
                TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
            {
                TeleportSystem.RaiseTeleportRequest(this, TeleportTarget);
                //canTeleport = true;
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region IMixedRealityTeleportHandler Implementation

        /// <inheritdoc />
        public override void OnTeleportRequest(TeleportEventData eventData)
        {
            base.OnTeleportRequest(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportStarted(TeleportEventData eventData)
        {
            base.OnTeleportStarted(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportCompleted(TeleportEventData eventData)
        {
            base.OnTeleportCompleted(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportCanceled(TeleportEventData eventData)
        {
            base.OnTeleportCanceled(eventData);
        }

        #endregion IMixedRealityTeleportHandler Implementation
    }
}