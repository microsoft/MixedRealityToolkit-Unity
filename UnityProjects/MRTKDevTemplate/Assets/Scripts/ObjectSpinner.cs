// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Spins object on events
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Object Spinner")]
    public class ObjectSpinner : MonoBehaviour
    {
        private Quaternion initialRotation;
        private Vector3 initialScale;
        private float initialSliderValue = -1;
        private bool rotateObject = false;

        [Tooltip("Rotation speed factor")]
        [SerializeField]
        private float angularVelocity = 300.0f;

        [Tooltip("Rotation axis")]
        [SerializeField]
        private Vector3 rotationAxis = Vector3.up;

        /// <summary>
        /// Hook up to a slider's OnValueChanged event.
        /// </summary>
        public void SpinObjectWithSlider(SliderEventData args)
        {
            // If this is our first slider event, let's record the initial values.
            if (initialSliderValue < 0)
            {
                initialRotation = transform.localRotation;
                initialScale = transform.localScale;
                initialSliderValue = args.NewValue;
            }

            // Adjust the gem based on the difference between the current slider's value and where it started.
            float sliderDelta = args.NewValue - initialSliderValue;
            transform.localRotation = initialRotation * Quaternion.AngleAxis(sliderDelta * -90, Vector3.up);
            transform.localScale = initialScale * (1 + sliderDelta * 0.2f);
        }

        /// <summary>
        /// Start rotating the object
        /// </summary>
        public void StartRotation()
        {
            rotateObject = true;
        }

        /// <summary>
        /// Stop rotating the object
        /// </summary>
        public void StopRotation()
        {
            rotateObject = false;
        }

        public void Update()
        {
            if(rotateObject == true)
            {
                transform.localRotation = Quaternion.AngleAxis(angularVelocity * Time.deltaTime, rotationAxis) * transform.localRotation;
            }
        }
    }
}
