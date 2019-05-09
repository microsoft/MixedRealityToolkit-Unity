// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    public abstract class Distorter : MonoBehaviour, IComparable<Distorter>
    {
        [Range(0, 10)]
        [SerializeField]
        private int distortOrder = 0;

        [Range(0, 1)]
        [SerializeField]
        private float distortStrength = 1f;

        private bool distortionEnabled;

        public bool DistortionEnabled
        {
            get { return distortionEnabled; }
        }

        public float DistortStrength
        {
            get { return distortStrength; }
            set { distortStrength = Mathf.Clamp01(value); }
        }

        public int DistortOrder
        {
            get { return distortOrder; }
            set
            {
                distortOrder = Mathf.Clamp(value, 0, 10);
            }
        }

        public int CompareTo(Distorter other)
        {
            return other == null ? 0 : DistortOrder.CompareTo(other.DistortOrder);
        }

        /// <summary>
        /// Distorts a world-space point
        /// Automatically applies DistortStrength and ensures that strength never exceeds 1
        /// </summary>
        /// <param name="point"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public Vector3 DistortPoint(Vector3 point, float strength = 1f)
        {
            strength = Mathf.Clamp01(strength * DistortStrength);

            return strength <= 0 ? point : DistortPointInternal(point, strength);
        }

        /// <summary>
        /// Distorts a world-space scale
        /// Automatically applies DistortStrength and ensures that strength never exceeds 1
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public Vector3 DistortScale(Vector3 scale, float strength = 1f)
        {
            if (!isActiveAndEnabled)
            {
                return scale;
            }

            strength = Mathf.Clamp01(strength * DistortStrength);

            return DistortScaleInternal(scale, strength);
        }

        /// <summary>
        /// Internal function where position distortion is done
        /// </summary>
        /// <param name="point"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        protected abstract Vector3 DistortPointInternal(Vector3 point, float strength);

        /// <summary>
        /// Internal function where scale distortion is done
        /// </summary>
        /// <param name="point"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        protected abstract Vector3 DistortScaleInternal(Vector3 point, float strength);

        #region MonoBehaviour Implementation

        protected virtual void OnEnable()
        {
            distortionEnabled = true;
        }

        protected virtual void OnDisable()
        {
            distortionEnabled = false;
        }

        #endregion MonoBehaviour Implementation
    }
}