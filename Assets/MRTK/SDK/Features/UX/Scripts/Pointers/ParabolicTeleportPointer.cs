// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    [RequireComponent(typeof(ParabolaPhysicalLineDataProvider))]
    [AddComponentMenu("Scripts/MRTK/SDK/ParabolicTeleportPointer")]
    public class ParabolicTeleportPointer : TeleportPointer
    {
        [SerializeField]
        private float minParabolaVelocity = 1f;

        [SerializeField]
        private float maxParabolaVelocity = 5f;

        [SerializeField]
        private float minDistanceModifier = 1f;

        [SerializeField]
        private float maxDistanceModifier = 5f;

        [SerializeField]
        private ParabolaPhysicalLineDataProvider parabolicLineData;

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();
            EnsureSetup();
        }

        private void EnsureSetup()
        {
            if (parabolicLineData == null)
            {
                parabolicLineData = gameObject.GetComponent<ParabolaPhysicalLineDataProvider>();
            }

            if (parabolicLineData.LineTransform == transform)
            {
                Debug.LogWarning("Missing Parabolic line helper.\nThe Parabolic Teleport Pointer requires an empty GameObject child for calculating the parabola arc. Creating one now.");

                var pointerHelper = transform.Find("ParabolicLinePointerHelper");

                if (pointerHelper == null)
                {
                    pointerHelper = new GameObject("ParabolicLinePointerHelper").transform;
                    pointerHelper.transform.SetParent(transform);
                }

                pointerHelper.transform.localPosition = Vector3.zero;
                parabolicLineData.LineTransform = pointerHelper.transform;
            }
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealityPointer Implementation

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] ParabolicTeleportPointer.OnPreSceneQuery");

        private StabilizedRay stabilizedRay = new StabilizedRay(0.5f);
        private Ray stabilizationRay = new Ray();

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                if (!IsInteractionEnabled)
                {
                    return;
                }

                stabilizationRay.origin = transform.position;
                stabilizationRay.direction = transform.forward;
                stabilizedRay.AddSample(stabilizationRay);

                parabolicLineData.LineTransform.rotation = Quaternion.identity;
                parabolicLineData.Direction = stabilizedRay.StabilizedDirection;

                // when pointing straight up, angle should be close to 1.
                // when pointing straight down, angle should be close to -1.	
                // when pointing straight forward in any direction, upDot should be 0.
                var angle = (Vector3.Angle(stabilizedRay.StabilizedDirection, Vector3.down) - 90.0f) / 90.0f;
                var sqr_angle = angle * angle;

                var velocity = minParabolaVelocity;
                var distance = minDistanceModifier;

                // If we're pointing below the horizon, always use the minimum modifiers.
                // We use square angle so that the velocity change is less noticeable the closer the teleport point
                // is to the user
                if (sqr_angle > 0)
                {
                    velocity = Mathf.Lerp(minParabolaVelocity, maxParabolaVelocity, sqr_angle);
                    distance = Mathf.Lerp(minDistanceModifier, maxDistanceModifier, sqr_angle);
                }

                parabolicLineData.Velocity = velocity;
                parabolicLineData.DistanceMultiplier = distance;
                base.OnPreSceneQuery();
            }
        }

        #endregion IMixedRealityPointer Implementation
    }
}
