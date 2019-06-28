// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This class manages how a gameobject rotates and/or scales
    /// when activated as part of a Progress Indicator effect.
    /// </summary>
    public class ProgressIndicatorObjectDisplay : MonoBehaviour, IProgressIndicator
    {
        /// <inheritdoc/>
        public Transform MainTransform { get { return transform; } }

        /// <inheritdoc/>
        public ProgressIndicatorState State { get { return state; } }

        /// <inheritdoc/>
        public float Progress { set { progress = value; } }

        /// <inheritdoc/>
        public string Message { set { messageText.text = value; } }

        [SerializeField]
        private Transform scaleTargetObject = null;
        [SerializeField]
        private Transform rotateTargetObject = null;

        [Header("How fast does object rotate?")]
        [SerializeField]
        private float rotationIncrement = 200;

        [Header("Start scale of the object?")]
        [SerializeField]
        private float minScale = 1.0f;

        [Header("Final scale of the object?")]
        [SerializeField]
        private float maxScale = 9.0f;

        [Header("Should object rotate?")]
        [SerializeField]
        private bool rotationActive = false;

        [Header("Rotation occurs about which axes?")]
        [SerializeField]
        private bool xAxisRotation = false;
        [SerializeField]
        private bool yAxisRotation = true;
        [SerializeField]
        private bool zAxisRotation = false;

        [Header("Grow / Shrink curve on open / close")]
        [SerializeField]
        [Tooltip("Curve for opening animation. MUST end with value >= 1 or transition will not complete.")]
        private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        [Tooltip("Curve for opening animation. MUST end with value <= 0 or transition will not complete.")]
        private AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [SerializeField]
        private TextMeshPro messageText = null;

        [SerializeField]
        private ProgressIndicatorState state = ProgressIndicatorState.Closed;

        [SerializeField]
        [Range(0f, 1f)]
        private float progress;

        private float currentScale;

        /// <inheritdoc/>
        public async Task OpenAsync()
        {
            if (openCurve.length == 0)
            {
                Debug.LogWarning("Open curve length is zero - this may result in an infinite loop.");
            }

            float maxScale = openCurve.Evaluate(Mathf.Infinity);
            if (maxScale < 1f)
            {
                Debug.LogWarning("Open curve value never reaches 1 - this may result in an infinite loop.");
            }

            if (state != ProgressIndicatorState.Closed)
            {
                throw new System.Exception("Can't open in state " + state);
            }

            gameObject.SetActive(true);

            Reset();

            state = ProgressIndicatorState.Opening;

            float startTime = Time.unscaledTime;
            float openScale = 0f;
            while (openScale < 1)
            {
                openScale = openCurve.Evaluate(Time.unscaledTime - startTime);
                scaleTargetObject.transform.localScale = Vector3.one * currentScale * openScale;
                await Task.Yield();
            }

            state = ProgressIndicatorState.Open;
        }

        /// <inheritdoc/>
        public async Task CloseAsync()
        {
            if (closeCurve.length == 0)
            {
                Debug.LogWarning("Open curve length is zero - this may result in an infinite loop.");
            }

            float minScale = closeCurve.Evaluate(Mathf.Infinity);
            if (minScale > 0)
            {
                Debug.LogWarning("Open curve value never reaches 0 - this may result in an infinite loop.");
            }

            if (state != ProgressIndicatorState.Open)
            {
                throw new System.Exception("Can't close in state " + state);
            }

            state = ProgressIndicatorState.Closing;

            float startTime = Time.unscaledTime;
            float closeScale = 1f;
            while (closeScale > 0)
            {
                closeScale = closeCurve.Evaluate(Time.unscaledTime - startTime);
                scaleTargetObject.transform.localScale = Vector3.one * currentScale * closeScale;
                await Task.Yield();
            }

            state = ProgressIndicatorState.Closed;

            gameObject.SetActive(false);
        }

        private void Reset()
        {
            currentScale = minScale;
        }

        private void Update()
        {
            if (state == ProgressIndicatorState.Open)
            {
                // Only scale while we're not opening or closing
                currentScale = Mathf.Lerp(minScale, maxScale, progress);
                scaleTargetObject.localScale = Vector3.one * currentScale;
            }

            if (rotationActive)
            {
                float increment = Time.unscaledDeltaTime * rotationIncrement;
                float xRotation = xAxisRotation ? increment : 0;
                float yRotation = yAxisRotation ? increment : 0;
                float zRotation = zAxisRotation ? increment : 0;
                rotateTargetObject.Rotate(xRotation, yRotation, zRotation);
            }
        }
    }
}
