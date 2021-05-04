// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI.PulseShader
{
    /// <summary>
    /// Script for generating pulse shader effect on the surface of a mesh.  
    /// </summary>
    public class PulseShaderHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Shader parameter name to drive the pulse radius.")]
        [FormerlySerializedAs("ParamName")]
        private string paramName = "_Pulse_";

        /// <summary>
        /// Shader parameter name to drive the pulse radius.
        /// </summary>
        public string ParamName
        {
            get { return paramName; }
            set
            {
                if (paramName != value)
                {
                    paramName = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Shader parameter name to set the pulse origin, in local space.")]
        [FormerlySerializedAs("OriginParamName")]
        private string originParamName = "_Pulse_Origin_";

        /// <summary>
        /// Shader parameter name to set the pulse origin, in local space.
        /// </summary>
        public string OriginParamName
        {
            get { return originParamName; }
            set
            {
                if (originParamName != value)
                {
                    originParamName = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("How long in seconds the pulse should animate")]
        [FormerlySerializedAs("PulseDuration")]
        private float pulseAnimationDuration = 5f;

        /// <summary>
        /// How long in seconds the pulse should animate.
        /// </summary>
        public float PulseAnimationDuration
        {
            get { return pulseAnimationDuration; }
            set
            {
                if (pulseAnimationDuration != value)
                {
                    pulseAnimationDuration = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Minimum time to wait between each pulse.")]
        [FormerlySerializedAs("PulseRepeatMinDelay")]
        private float pulseRepeatMinDelay = 1f;

        /// <summary>
        /// Minimum time to wait between each pulse.
        /// </summary>
        public float PulseRepeatMinDelay
        {
            get { return pulseRepeatMinDelay; }
            set
            {
                if (pulseRepeatMinDelay != value)
                {
                    pulseRepeatMinDelay = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Automatically begin repeated pulsing.")]
        [FormerlySerializedAs("AutoStart")]
        private bool autoStart = false;

        /// <summary>
        /// Automatically begin repeated pulsing.
        /// </summary>
        public bool AutoStart
        {
            get { return autoStart; }
            set
            {
                if (autoStart != value)
                {
                    autoStart = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("The material to animate.")]
        [FormerlySerializedAs("SurfaceMat")]
        private Material surfaceMat;

        /// <summary>
        /// The material to animate.
        /// </summary>
        public Material SurfaceMat
        {
            get { return surfaceMat; }
            set
            {
                if (surfaceMat != value)
                {
                    surfaceMat = value;
                }
            }
        }

        // Internal state
        private Coroutine RepeatPulseCoroutine;
        private Coroutine CurrentCoroutine;

        private float pulseStartedTime;
        private bool repeatingPulse;
        private bool cancelPulse;

        protected virtual void Start()
        {
            if (AutoStart)
            {
                StartPulsing();
            }
        }

        protected virtual void OnDestroy()
        {
            ResetPulseMaterial();
        }

        protected virtual void Update() { }

        #region Material Control

        public void SetLocalOrigin(Vector3 origin)
        {
            SurfaceMat.SetVector(OriginParamName, origin);
        }

        protected void ResetPulseMaterial()
        {
            ApplyPulseRadiusToMaterial(0);
        }

        protected void ApplyPulseRadiusToMaterial(float radius)
        {
            surfaceMat.SetFloat(paramName, radius);
        }

        #endregion

        #region Pulse Control

        public void PulseOnce(bool cancelPreviousPulse = true)
        {
            cancelPulse = false;

            if (CurrentCoroutine != null && cancelPreviousPulse)
            {
                // Stop the previous coroutine
                StopCoroutine(CurrentCoroutine);
            }

            CurrentCoroutine = StartCoroutine(CoSinglePulse());
        }

        // Start the repeat animation coroutine 
        protected void StartPulsing()
        {
            repeatingPulse = true;
            cancelPulse = false;
            if (RepeatPulseCoroutine == null)
            {
                RepeatPulseCoroutine = StartCoroutine(CoRepeatPulse());
            }
        }

        // Auto start stopping
        protected void StopPulsing(bool finishCurrentPulse = true)
        {
            repeatingPulse = false;
            if (!finishCurrentPulse)
            {
                cancelPulse = true;
                ApplyPulseRadiusToMaterial(0);
            }
        }

        #endregion

        #region Animation Timing

        private IEnumerator CoSinglePulse()
        {
            yield return CoWaitForRepeatDelay();

            if (!cancelPulse)
            {
                yield return CoAnimatePulse();
            }
        }

        // Delay the animation of shader values based on the PulseRepeatMinDelay
        private IEnumerator CoWaitForRepeatDelay()
        {
            // Wait for minimum time between pulses starting
            if (pulseStartedTime > 0)
            {
                float timeSincePulseStarted = Time.time - pulseStartedTime;
                float delayTime = PulseRepeatMinDelay - timeSincePulseStarted;
                if (delayTime > 0)
                {
                    yield return new WaitForSeconds(delayTime);
                }
            }
        }

        // Animate shader values over time based on the PulseAnimationDuration
        private IEnumerator CoAnimatePulse()
        {
            pulseStartedTime = Time.time;
            float t = 0;
            while (t < PulseAnimationDuration && !cancelPulse)
            {
                t += Time.deltaTime;
                ApplyPulseRadiusToMaterial(t / PulseAnimationDuration);
                yield return null;
            }
        }

        // Repeat the pulse shader animation
        private IEnumerator CoRepeatPulse()
        {
            while (repeatingPulse && !cancelPulse)
            {
                yield return CoSinglePulse();
            }

            RepeatPulseCoroutine = null;
        }

        #endregion
    }
}
