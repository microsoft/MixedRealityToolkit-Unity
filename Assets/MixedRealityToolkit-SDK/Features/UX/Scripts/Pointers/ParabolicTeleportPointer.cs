// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    [RequireComponent(typeof(ParabolaPhysicalLineDataProvider))]
    public class ParabolicTeleportPointer : TeleportPointer
    {
        [SerializeField]
        private float minParabolaVelocity = 3f;

        [SerializeField]
        private float maxParabolaVelocity = 3f;

        [SerializeField]
        private ParabolaPhysicalLineDataProvider parabolicLineData;

        #region Monobehaviour Implementation

        protected override void OnValidate()
        {
            base.OnValidate();

            if (parabolicLineData == null)
            {
                parabolicLineData = GetComponent<ParabolaPhysicalLineDataProvider>();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (parabolicLineData == null)
            {
                parabolicLineData = GetComponent<ParabolaPhysicalLineDataProvider>();
            }
        }

        #endregion Monobehaviour Implementation

        #region IMixedRealityPointer Implementation

        public override void OnPreRaycast()
        {
            // Make sure our parabola only rotates on y/x axis
            // NOTE: Parabola's custom line transform field should be set to a transform OTHER than its gameObject's transform
            parabolicLineData.Direction = transform.forward + Vector3.up;
            parabolicLineData.LineTransform.rotation = Quaternion.identity;

            // Use our up angle and distance curve to determine the velocity
            // This can be used to make the parabola point farther when aimed up
            float angle = Mathf.Clamp01(Vector3.Angle(transform.forward, Vector3.up) / 180f);
            float velocity = Mathf.Lerp(minParabolaVelocity, maxParabolaVelocity, angle);
            parabolicLineData.Velocity = velocity;

            base.OnPreRaycast();
        }

        #endregion IMixedRealityPointer Implementation
    }
}