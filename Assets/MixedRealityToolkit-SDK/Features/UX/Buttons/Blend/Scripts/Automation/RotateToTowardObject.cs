// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend.Automation
{
    /// <summary>
    /// Makes the assigned object rotate toward another object.
    /// </summary>using System.Collections;


    public class RotateToTowardObject : MonoBehaviour
    {

        [Tooltip("The game object this object will follow : Main Camera by default")]
        public GameObject ReferenceObject;

        [Tooltip("If set, the indicator will be hidden when the threshold distance is met")]
        public GameObject IndicatorObject;

        [Tooltip("Auto start? or status")]
        public bool IsRunning = false;

        [Tooltip("rotation speed : higher is faster")]
        public float LerpRotationSpeed = 0.5f;

        [Tooltip("distance from object to trigger an event")]
        public float ThresholdDistance = 0.1f;

        public UnityEvent OnThresholdMet;
        public UnityEvent OnThresholdLost;

        public bool hasThreshold = false;
        private Quaternion defaultRotation;

        private void Awake()
        {
            defaultRotation = transform.rotation;
        }

        // start the object following the reference object
        public void StartRunning()
        {
            IsRunning = true;
        }

        /// <summary>
        /// stop the object from following
        /// </summary>
        public void StopRunning()
        {
            IsRunning = false;
        }

        /// <summary>
        /// update the position of the object based on the reference object and configuration
        /// </summary>
        /// <param name="position"></param>
        /// <param name="time"></param>
        protected virtual void UpdateRotation(Vector3 direction, float time)
        {
            // update the roation
            Vector3 rotation = Vector3.ProjectOnPlane(direction, -1 * Camera.main.transform.forward);
            rotation.Normalize();
            Quaternion lookAt = Quaternion.LookRotation(Camera.main.transform.forward, rotation) * defaultRotation;

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookAt, LerpRotationSpeed * time);

            float xDistance = Vector3.Dot(direction, Camera.main.transform.right);
            float yDistance = Vector3.Dot(direction, Camera.main.transform.up);
            float zDistance = Vector3.Dot(direction, Camera.main.transform.forward);

            bool threshold = Mathf.Abs(xDistance) < ThresholdDistance && Mathf.Abs(yDistance) < ThresholdDistance && zDistance > -1;

            if (threshold != hasThreshold)
            {
                hasThreshold = threshold;
                if (hasThreshold)
                {
                    OnThresholdMet.Invoke();
                }
                else
                {
                    OnThresholdLost.Invoke();
                }

                if (IndicatorObject != null)
                {
                    IndicatorObject.SetActive(!hasThreshold);
                }
            }

        }

        /// <summary>
        /// Animate!
        /// </summary>
        protected virtual void Update()
        {
            if (IsRunning && ReferenceObject != null)
            {
                UpdateRotation(ReferenceObject.transform.position - transform.position, Time.deltaTime);
            }
        }
    }
}
