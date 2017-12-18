// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    [Serializable]
    public class FocusResult
    {
        public IPointingSource PointingSource { get; protected set; }

        public Vector3 StartPoint { get; protected set; }

        public Vector3 Point { get; protected set; }

        public Vector3 Normal { get; protected set; }

        public GameObject Target { get; protected set; }
        
        public GameObject PreviousTarget { get; protected set; }
        
        public RaycastHit LastRaycastHit { get; protected set; }

        /// <summary>
        /// The index of the step that produced the last raycast hit
        /// 0 when no raycast hit
        /// </summary>
        public int RayStepIndex { get; protected set; }

        public PointerInputEventData UnityUIPointerData
        {
            get
            {
                if (pointerData == null)
                {
                    pointerData = new PointerInputEventData(EventSystem.current);
                }

                return pointerData;
            }
        }

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
            LastRaycastHit = hit;
            PreviousTarget = Target;
            RayStepIndex = rayStepIndex;
            StartPoint = sourceRay.origin;
            Point = hit.point;
            Normal = hit.normal;
            Target = hit.transform.gameObject;
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
            // We do not update the PreviousEndObject here because
            // it's already been updated in the first physics raycast.

            RayStepIndex = rayStepIndex;
            StartPoint = sourceRay.origin;
            Point = hit.point;
            Normal = hit.normal;
            Target = result.gameObject;
        }

        /// <summary>
        /// Used by FocusManager to update hit information. Avoid calling outside of FocusManager.
        /// </summary>
        /// <param name="extent"></param>
        public void UpdateHit(float extent)
        {
            PreviousTarget = Target;

            RayStep firstStep = PointingSource.Rays[0];
            RayStep finalStep = PointingSource.Rays[PointingSource.Rays.Length - 1];
            RayStepIndex = 0;

            StartPoint = firstStep.origin;
            Point = finalStep.terminus;
            Normal = (-finalStep.direction);
            Target = null;
        }

        /// <summary>
        /// Used by FocusManager to update hit information. Avoid calling outside of FocusManager.
        /// </summary>
        /// <param name="clearPreviousObject"></param>
        public void ResetFocusedObjects(bool clearPreviousObject = true)
        {
            if (clearPreviousObject)
            {
                PreviousTarget = null;
            }

            Target = null;
        }

        private PointerInputEventData pointerData;
    }
}