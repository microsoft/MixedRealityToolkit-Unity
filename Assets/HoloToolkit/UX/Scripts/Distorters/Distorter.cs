// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public abstract class Distorter : MonoBehaviour, IComparable<Distorter>
    {
        [SerializeField]
        [Range(0, 10)]
        protected int distortOrder = 0;
        [SerializeField]
        [Range(0, 1)]
        protected float distortStrength = 1f;

        public int CompareTo(Distorter other)
        {
            if (other == null) {
                return 0;
            }
            
            return DistortOrder.CompareTo(other.DistortOrder);
        }

        /// <summary>
        /// Distorts a world-space point
        /// Automatically applies DistortStrength and ensures that strength never exceeds 1
        /// </summary>
        /// <param name="point"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public Vector3 DistortPoint (Vector3 point, float strength = 1f)
        {
            if (!isActiveAndEnabled) {
                return point;
            }

            strength = Mathf.Clamp01 (strength * DistortStrength);

            if (strength <= 0)
            {
                return point;
            }

            return DistortPointInternal(point, strength);
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
            if (!isActiveAndEnabled) {
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

        protected virtual void OnEnable()
        {
            // Makes script enableable in editor
        }

        protected virtual void OnDisable()
        {
            // Makes script enableable in editor            
        }

        public float DistortStrength
        {
            get { return distortStrength; }
            set { distortStrength = Mathf.Clamp01(value); }
        }

        public int DistortOrder
        {
            get { return distortOrder; }
            set { distortOrder = value; } // TODO implement auto-sort
        }
    }
}