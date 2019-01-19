// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    /// <summary>
    /// Touch Pointer Implementation.
    /// </summary>
    public class TouchPointer : BaseControllerPointer, IMixedRealityTouchPointer
    {
        private bool isInteractionEnabled = false;

        /// <inheritdoc />
        public override bool IsInteractionEnabled => isInteractionEnabled;

        private int fingerId = -1;

        /// <inheritdoc />
        public int FingerId
        {
            get { return fingerId; }
            set
            {
                if (fingerId < 0)
                {
                    fingerId = value;
                }
            }
        }

        /// <inheritdoc />
        public Ray TouchRay { get; set; } = default(Ray);

        /// <inheritdoc />
        public override void OnPreRaycast()
        {
            Ray pointingRay;
            if (TryGetPointingRay(out pointingRay))
            {
                Rays[0].CopyRay(pointingRay, PointerExtent);

                if (RayStabilizer != null)
                {
                    RayStabilizer.UpdateStability(Rays[0].Origin, Rays[0].Direction);
                    Rays[0].CopyRay(RayStabilizer.StableRay, PointerExtent);

                    if (MixedRealityRaycaster.DebugEnabled)
                    {
                        Debug.DrawRay(RayStabilizer.StableRay.origin, RayStabilizer.StableRay.direction * PointerExtent, Color.green);
                    }
                }
                else if (MixedRealityRaycaster.DebugEnabled)
                {
                    Debug.DrawRay(pointingRay.origin, pointingRay.direction * PointerExtent, Color.yellow);
                }
            }
        }

        /// <inheritdoc />
        public override bool TryGetPointerPosition(out Vector3 position)
        {
            position = Vector3.zero;
            if (fingerId < 0) { return false; }
            position = Result?.Details.Point ?? CameraCache.Main.ScreenPointToRay(UnityEngine.Input.GetTouch(FingerId).position).GetPoint(PointerExtent);
            return true;
        }

        /// <inheritdoc />
        public override bool TryGetPointingRay(out Ray pointingRay)
        {
            pointingRay = TouchRay;
            return true;
        }

        /// <inheritdoc />
        public override bool TryGetPointerRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        /// <inheritdoc />
        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            base.OnSourceDetected(eventData);

            if (eventData.InputSource.SourceId == Controller.InputSource.SourceId)
            {
                isInteractionEnabled = true;
            }
        }

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (Controller != null &&
                eventData.Controller != null &&
                eventData.Controller.InputSource.SourceId == Controller.InputSource.SourceId)
            {
                isInteractionEnabled = false;
            }
        }
    }
}