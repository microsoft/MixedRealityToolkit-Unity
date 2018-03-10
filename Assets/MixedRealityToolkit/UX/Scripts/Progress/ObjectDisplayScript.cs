// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX
{
    public class ObjectDisplayScript : MonoBehaviour
    {
        [Header("How fast does object rotate?")]
        [SerializeField]
        private float rotationIncrement = 200;

        [Header("Start scale of the object?")]
        [SerializeField]
        private float minScale = 1.0f;

        [Header("Final scale of the object?")]
        [SerializeField]
        private float maxScale = 9.0f;

        [Header("How fast does object grow?")]
        [SerializeField]
        private float scaleSpeed = 30.0f;

        [Header("Should object rotate after growing?")]
        [SerializeField]
        private bool rotationActive = false;

        [Header("Should object grow before rotating?")]
        [SerializeField]
        private bool growingActive = true;

        [Header("Rotation occurs about which axes?")]
        [SerializeField]
        private bool xAxisRotation = false;
        [SerializeField]
        private bool yAxisRotation = true;
        [SerializeField]
        private bool zAxisRotation = false;

        private float currentScale;
        private float elapsedTime;

        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            elapsedTime = 0.0f;
            currentScale = minScale;
        }

        private void Update()
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (growingActive && currentScale < maxScale)
            {
                currentScale = minScale + (scaleSpeed * (maxScale * Mathf.Pow(elapsedTime, 2.0f)));
            }

            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            if (rotationActive)
            {
                float increment = Time.deltaTime * rotationIncrement;
                float xRotation = xAxisRotation ? increment : 0;
                float yRotation = yAxisRotation ? increment : 0;
                float zRotation = zAxisRotation ? increment : 0;
                transform.Rotate(xRotation, yRotation, zRotation);
            }
        }
    }
}
