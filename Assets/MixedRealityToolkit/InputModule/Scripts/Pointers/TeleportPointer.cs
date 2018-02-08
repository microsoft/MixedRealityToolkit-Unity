// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.Utilities.Attributes;
using MixedRealityToolkit.UX.Distorters;
using System;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Pointers
{
    [RequireComponent(typeof(DistorterGravity))]
    public class TeleportPointer : LinePointer
    {
        [SerializeField]
        protected float MinValidDot = 0.2f;

        [Header("Navigation Colors")]
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Green, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient LineColorHotSpot;

        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Red, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient LineColorInvalid;

        [Header("Layers & Tags")]
        [SerializeField]
        [Tooltip("Layers that are considered 'valid' for navigation")]
        protected LayerMask ValidLayers = 1; // Default

        [SerializeField]
        [Tooltip("Layers that are considered 'invalid' for navigation")]
        protected LayerMask InvalidLayers = 1 << 2; // Ignore raycast

        // Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like
        // the forward direction, so we apply this offset to make navigation feel more natural
        private const float ThumbstickAngleOffset = -82.5f;

        [SerializeField]
        private float thumbstickThreshold = 0.05f;

        private bool processingInput = false;
        private Vector2 thumbstickPosition = Vector2.zero;

        /// The result of our hit
        public TeleportSurfaceResult TeleportSurfaceResult { get; protected set; }

        private void Update()
        {
            processingInput = false;

            if (Mathf.Abs(thumbstickPosition.y) > thumbstickThreshold || Mathf.Abs(thumbstickPosition.x) > thumbstickThreshold)
            {
                // Get the angle of the pointer input
                float angle = Mathf.Atan2(thumbstickPosition.y, thumbstickPosition.x) * Mathf.Rad2Deg;
                // Offset the angle so it's 'forward' facing
                angle += ThumbstickAngleOffset;
                PointerOrientation = angle;
                processingInput = true;
            }

            if (processingInput)
            {
                if (!InteractionEnabled)
                {
                    InteractionEnabled = true;
                    OnSelectPressed();
                }
            }
            else
            {
                if (InteractionEnabled)
                {
                    InteractionEnabled = false;
                    OnInputReleased();
                }
            }
        }

        /// The position of the navigation target
        public virtual Vector3 TeleportTargetPosition
        {
            get
            {
                if (!InteractionEnabled)
                {
                    return Vector3.zero;
                }

                if (TeleportTarget != null && TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
                {
                    return TeleportTarget.Position;
                }

                return Result.StartPoint;
            }
        }

        /// The normal of the navigation target
        public virtual Vector3 TeleportTargetNormal
        {
            get
            {
                if (!InteractionEnabled)
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

        /// The Y rotation of the target in world-space degrees.
        public override float PointerOrientation
        {
            get
            {
                if (!InteractionEnabled)
                {
                    return 0f;
                }

                if (TeleportSurfaceResult == TeleportSurfaceResult.HotSpot &&
                    TeleportTarget != null && TeleportTarget.OverrideTargetOrientation)
                {
                    return TeleportTarget.TargetOrientation;
                }

                // Use the camera orientation by default
                return CameraCache.Main.transform.eulerAngles.y - CurrentPointerOrientation;
            }
            set
            {
                // Store pointer orientation as the difference between camera and input
                CurrentPointerOrientation = value;
            }
        }

        public override bool InteractionEnabled
        {
            get
            {
                return SelectPressed || base.InteractionEnabled;
            }
        }

        public override void OnSelectPressed()
        {
            SelectPressed = true;

            if (TeleportSurfaceResult == TeleportSurfaceResult.Valid ||
                TeleportSurfaceResult == TeleportSurfaceResult.HotSpot)
            {
                PointerTeleportManager.Instance.InitiateTeleport(this);
            }
        }

        public override void OnInputReleased()
        {
            SelectPressed = false;

            PointerTeleportManager.Instance.TryToTeleport();
        }

        public Gradient GetColor(TeleportSurfaceResult targetResult)
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
                    throw new ArgumentOutOfRangeException("targetResult", targetResult, null);
            }
        }

        public override void OnPreRaycast()
        {
            if (LineBase == null) { return; }

            // Make sure our array will hold
            if (Rays == null || Rays.Length != LineCastResolution)
            {
                Rays = new RayStep[LineCastResolution];
            }

            // Set up our rays
            // Turn off gravity so we get accurate rays
            DistorterGravity.enabled = false;

            float stepSize = 1f / Rays.Length;
            Vector3 lastPoint = LineBase.GetUnclampedPoint(0f);

            for (int i = 0; i < Rays.Length; i++)
            {
                Vector3 currentPoint = LineBase.GetUnclampedPoint(stepSize * (i + 1));
                Rays[i] = new RayStep(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }

            // Re-enable gravity if we're looking at a hotspot
            DistorterGravity.enabled = (TeleportSurfaceResult == TeleportSurfaceResult.HotSpot);
        }

        public override void OnPostRaycast()
        {
            // Use the results from the last update to set our NavigationResult
            float clearWorldLength = 0f;
            TeleportSurfaceResult = TeleportSurfaceResult.None;
            DistorterGravity.enabled = false;

            if (InteractionEnabled)
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
                            DistorterGravity.WorldCenterOfGravity = TeleportTarget.Position;
                            DistorterGravity.enabled = true;
                        }
                        else
                        {
                            // If it's NOT a hotspot, check if the hit normal is too steep 
                            // (Hotspots override dot requirements)
                            TeleportSurfaceResult = Vector3.Dot(Result.StartPoint, Vector3.up) < MinValidDot
                                    ? TeleportSurfaceResult.Invalid
                                    : TeleportSurfaceResult.Valid;
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
                            Debug.DrawLine(
                                    Result.StartPoint + Vector3.up * 0.1f,
                                    Result.StartPoint + Vector3.up * 0.1f,
                                    (TeleportSurfaceResult != TeleportSurfaceResult.None) ? Color.yellow : Color.cyan);
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
                    LineRenderers[i].LineColor = GetColor(TeleportSurfaceResult);
                }

            }
            else
            {
                LineBase.enabled = false;
            }
        }

        /// <summary>
        /// Updates target point orientation via thumbstick
        /// </summary>
        public override void OnInputPositionChanged(InputPositionEventData eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId &&
                eventData.Handedness == Handedness &&
                eventData.InputPositionType == InputPositionType.Thumbstick)
            {
                thumbstickPosition = eventData.InputPosition;
            }
        }
    }
}