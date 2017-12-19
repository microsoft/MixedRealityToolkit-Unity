// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Class used to store pointing source data
    /// Handled by FocusManager
    /// </summary>
    [Serializable]
    public struct PointerResult
    {
        public PointerResult (IPointingSource pointingSource)
        {
            PointingSource = pointingSource;
            StartPoint = Vector3.zero;
            Point = Vector3.zero;
            Normal = Vector3.forward;
            LastRaycastHit = default(RaycastHit);
            RayStepIndex = 0;
        }

        public IPointingSource PointingSource { get; private set; }

        public Vector3 StartPoint { get; private set; }

        public Vector3 Point { get; private set; }

        public Vector3 Normal { get; private set; }
        
        public RaycastHit LastRaycastHit { get; private set; }

        /// <summary>
        /// The index of the step that produced the last raycast hit
        /// 0 when no raycast hit
        /// </summary>
        public int RayStepIndex { get; private set; }

        public void Initialize (IPointingSource pointingSource)
        {
            PointingSource = pointingSource;
        }

        /// <summary>
        /// Used by FocusManager to update hit information. Avoid calling outside of FocusManager.
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="sourceRay"></param>
        /// <param name="rayStepIndex"></param>
        public void UpdateHit(RaycastHit hit, RayStep sourceRay, int rayStepIndex)
        {
            Debug.Assert(PointingSource != null);

            LastRaycastHit = hit;
            PointingSource.PreviousTarget = PointingSource.Target;
            RayStepIndex = rayStepIndex;
            StartPoint = sourceRay.origin;
            Point = hit.point;
            Normal = hit.normal;
            PointingSource.Target = hit.transform.gameObject;
        }

        /// <summary>
        /// Used by FocusManager to update hit information. Avoid calling outside of FocusManager.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="hit"></param>
        /// <param name="sourceRay"></param>
        /// <param name="rayStepIndex"></param>
        public void UpdateHit(RaycastResult result, RaycastHit hit, RayStep sourceRay, int rayStepIndex)
        {
            Debug.Assert(PointingSource != null);

            // We do not update the PreviousEndObject here because
            // it's already been updated in the first physics raycast.

            RayStepIndex = rayStepIndex;
            StartPoint = sourceRay.origin;
            Point = hit.point;
            Normal = hit.normal;
            PointingSource.Target = result.gameObject;
        }

        /// <summary>
        /// Used by FocusManager to update hit information. Avoid calling outside of FocusManager.
        /// </summary>
        /// <param name="extent"></param>
        public void UpdateHit(float extent)
        {
            Debug.Assert(PointingSource != null);

            PointingSource.PreviousTarget = PointingSource.Target;

            RayStep firstStep = PointingSource.Rays[0];
            RayStep finalStep = PointingSource.Rays[PointingSource.Rays.Length - 1];
            RayStepIndex = 0;

            StartPoint = firstStep.origin;
            Point = finalStep.terminus;
            Normal = (-finalStep.direction);
            PointingSource.Target = null;
        }

        /// <summary>
        /// Used by FocusManager to update hit information. Avoid calling outside of FocusManager.
        /// </summary>
        /// <param name="clearPreviousObject"></param>
        public void ResetFocusedObjects(bool clearPreviousObject = true)
        {
            Debug.Assert(PointingSource != null);

            if (clearPreviousObject)
            {
                PointingSource.PreviousTarget = null;
            }

            PointingSource.Target = null;
        }
    }
}