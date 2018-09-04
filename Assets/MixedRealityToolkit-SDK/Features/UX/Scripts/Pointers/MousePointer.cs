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
    /// Internal Touch Pointer Implementation.
    /// </summary>
    public class MousePointer : BaseControllerPointer, IMixedRealityMousePointer
    {
        /// <inheritdoc />
        public Vector3 ScreenPosition { get; private set; }

        private bool isInteractionEnabled = false;

        /// <inheritdoc />
        public override bool IsInteractionEnabled => isInteractionEnabled;

        /// <inheritdoc />
        public override void OnPreRaycast()
        {
            Ray pointingRay;
            if (TryGetPointingRay(out pointingRay))
            {
                Rays[0].CopyRay(pointingRay, PointerExtent);

                if (MixedRealityRaycaster.DebugEnabled)
                {
                    Debug.DrawRay(pointingRay.origin, pointingRay.direction * PointerExtent, Color.green);
                }
            }
        }

        /// <inheritdoc />
        public override bool TryGetPointerPosition(out Vector3 position)
        {
            position = Vector3.zero;
            if (UnityEngine.Input.mousePosition.x > Screen.width || UnityEngine.Input.mousePosition.x < 0 ||
                UnityEngine.Input.mousePosition.y > Screen.height || UnityEngine.Input.mousePosition.y < 0)
            {
                ScreenPosition = Vector3.zero;
                return false;
            }

            ScreenPosition = UnityEngine.Input.mousePosition;
            position = Result?.Details.Point ?? CameraCache.Main.ScreenPointToRay(UnityEngine.Input.mousePosition).GetPoint(PointerExtent);
            return true;
        }

        /// <inheritdoc />
        public override bool TryGetPointingRay(out Ray pointingRay)
        {
            Vector3 pointerPosition;

            if (TryGetPointerPosition(out pointerPosition))
            {
                pointingRay = CameraCache.Main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                return true;
            }

            pointingRay = default(Ray);
            return false;
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
            if (RayStabilizer != null)
            {
                RayStabilizer = null;
            }

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