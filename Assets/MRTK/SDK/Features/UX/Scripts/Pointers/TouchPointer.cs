// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Touch Pointer Implementation.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/TouchPointer")]
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

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] TouchPointer.OnPreSceneQuery");

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                Rays[0].CopyRay(TouchRay, PointerExtent);

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
                    Debug.DrawRay(TouchRay.origin, TouchRay.direction * PointerExtent, Color.yellow);
                }
            }
        }

        /// <inheritdoc />
        public override Vector3 Position
        {
            get
            {
                return TouchRay.origin;
            }
        }

        /// <inheritdoc />
        public override Quaternion Rotation
        {
            get
            {
                return Quaternion.LookRotation(TouchRay.direction);
            }
        }

        private static readonly ProfilerMarker OnSourceDetectedPerfMarker = new ProfilerMarker("[MRTK] TouchPointer.OnSourceDetected");

        /// <inheritdoc />
        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            using (OnSourceDetectedPerfMarker.Auto())
            {
                base.OnSourceDetected(eventData);

                if (eventData.InputSource.SourceId == Controller.InputSource.SourceId)
                {
                    isInteractionEnabled = true;
                }
            }
        }

        private static readonly ProfilerMarker OnSourceLostPerfMarker = new ProfilerMarker("[MRTK] TouchPointer.OnSourceLost");

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            using (OnSourceLostPerfMarker.Auto())
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
}