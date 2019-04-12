// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    /// <summary>
    /// This class manages how a gameobject rotates and/or scales
    /// when activated as part of a Progress Indicator effect.
    /// </summary>
    public class ProgressIndicatorObjectDisplay : MonoBehaviour, IProgressIndicator
    {
        public ProgressIndicatorState State { get { return state; } }
        public float Progress { set { progress = value; } }
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
        private float elapsedTime;

        public async Task OpenAsync()
        {
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

            state = ProgressIndicatorState.Displaying;
        }

        public async Task CloseAsync()
        {
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
            elapsedTime = 0.0f;
            currentScale = minScale;
        }

        private void Update()
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (state == ProgressIndicatorState.Displaying)
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
