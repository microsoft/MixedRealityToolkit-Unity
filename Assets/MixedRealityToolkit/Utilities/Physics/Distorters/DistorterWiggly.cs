// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Physics.Distorters
{
    public class DistorterWiggly : Distorter
    {
        private const float MinScaleMultiplier = 0.05f;
        private const float MaxScaleMultiplier = 1f;
        private const float MinSpeedMultiplier = -25f;
        private const float MaxSpeedMultiplier = 25f;
        private const float MinStrengthMultiplier = 0.00001f;
        private const float MaxStrengthMultiplier = 1f;
        private const float GlobalScale = 0.1f;

        [SerializeField]
        [Range(MinScaleMultiplier, MaxScaleMultiplier)]
        private float scaleMultiplier = 0.1f;

        public float ScaleMultiplier
        {
            get { return scaleMultiplier; }
            set
            {
                if (value > MaxScaleMultiplier)
                {
                    scaleMultiplier = MaxScaleMultiplier;
                }
                else if (value < MinScaleMultiplier)
                {
                    scaleMultiplier = MinScaleMultiplier;
                }
                else
                {
                    scaleMultiplier = value;
                }
            }
        }

        [SerializeField]
        [Range(MinSpeedMultiplier, MaxSpeedMultiplier)]
        private float speedMultiplier = 3f;

        public float SpeedMultiplier
        {
            get { return speedMultiplier; }
            set
            {
                if (value > MaxSpeedMultiplier)
                {
                    speedMultiplier = MaxSpeedMultiplier;
                }
                else if (value < MinSpeedMultiplier)
                {
                    speedMultiplier = MinSpeedMultiplier;
                }
                else
                {
                    speedMultiplier = value;
                }
            }
        }

        [SerializeField]
        [Range(MinStrengthMultiplier, MaxStrengthMultiplier)]
        private float strengthMultiplier = 0.01f;

        public float StrengthMultiplier
        {
            get { return strengthMultiplier; }
            set
            {
                if (value > MaxStrengthMultiplier)
                {
                    strengthMultiplier = MaxStrengthMultiplier;
                }
                else if (value < MinStrengthMultiplier)
                {
                    strengthMultiplier = MinStrengthMultiplier;
                }
                else
                {
                    strengthMultiplier = value;
                }
            }
        }

        [SerializeField]
        private Vector3 axisStrength = new Vector3(0.5f, 0.1f, 0.5f);

        public Vector3 AxisStrength
        {
            get { return axisStrength; }
            set { axisStrength = value; }
        }

        [SerializeField]
        private Vector3 axisSpeed = new Vector3(0.2f, 0.5f, 0.7f);

        public Vector3 AxisSpeed
        {
            get { return axisSpeed; }
            set { axisSpeed = value; }
        }

        [SerializeField]
        private Vector3 axisOffset = new Vector3(0.2f, 0.5f, 0.7f);

        public Vector3 AxisOffset
        {
            get { return axisOffset; }
            set { axisOffset = value; }
        }

        protected override Vector3 DistortPointInternal(Vector3 point, float strength)
        {
            Vector3 wiggly = point;
            float scale = scaleMultiplier * GlobalScale;
            wiggly.x = Wiggle(axisSpeed.x * speedMultiplier, (wiggly.x + axisOffset.x) / scale, axisStrength.x * strengthMultiplier);
            wiggly.y = Wiggle(axisSpeed.y * speedMultiplier, (wiggly.y + axisOffset.y) / scale, axisStrength.y * strengthMultiplier);
            wiggly.z = Wiggle(axisSpeed.z * speedMultiplier, (wiggly.z + axisOffset.z) / scale, axisStrength.z * strengthMultiplier);
            return point + (wiggly * strength);
        }

        protected override Vector3 DistortScaleInternal(Vector3 point, float strength)
        {
            return point;
        }

        private float Wiggle(float speed, float offset, float strength)
        {
            return Mathf.Sin((Time.unscaledTime * speed) + offset) * strength;
        }
    }
}