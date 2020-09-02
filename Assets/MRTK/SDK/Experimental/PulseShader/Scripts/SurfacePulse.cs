// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Experimental.SurfacePulse
{
    /// <summary>
    /// Script for generating pulse shader effect on the surface.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/SurfacePulse")]
    public class SurfacePulse : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField]
        [Tooltip("Shader parameter name to drive the pulse radius")]
        [FormerlySerializedAs("ParamName")]
        private string paramName = "_Pulse_";
        /// <summary>
        /// Shader parameter name to drive the pulse radius
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
        [Tooltip("Shader parameter name to set the pulse origin, in local space")]
        [FormerlySerializedAs("OriginParamName")]
        private string originParamName = "_Pulse_Origin_";
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
        private float pulseDuration = 5f;
        public float PulseDuration
        {
            get { return pulseDuration; }
            set
            {
                if (pulseDuration != value)
                {
                    pulseDuration = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Minimum time to wait between each pulse")]
        [FormerlySerializedAs("PulseRepeatMinDelay")]
        private float pulseRepeatMinDelay = 1f;
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
        [Tooltip("Automatically begin repeated pulsing")]
        [FormerlySerializedAs("AutoStart")]
        private bool autoStart = false;
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
        [Tooltip("Automatically set pulse origin to the main camera location")]
        [FormerlySerializedAs("OriginFollowCamera")]
        private bool originFollowCamera = false;
        public bool OriginFollowCamera
        {
            get { return originFollowCamera; }
            set
            {
                if (originFollowCamera != value)
                {
                    originFollowCamera = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("The material to animate")]
        [FormerlySerializedAs("SurfaceMat")]
        private Material surfaceMat;
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

        [SerializeField]
        [Tooltip("Pulse on select(air-tap)")]
        private bool pulseOnSelect = true;
        public bool PulseOnSelect
        {
            get { return pulseOnSelect; }
            set
            {
                if (pulseOnSelect != value)
                {
                    pulseOnSelect = value;
                }
            }
        }

        // Internal state
        Coroutine RepeatPulseCoroutine;

        float pulseStartedTime;
        bool repeatingPulse;
        bool cancelPulse;


        // Reset the material property when exiting play mode so it won't be changed on disk
#if UNITY_EDITOR

        SurfacePulse()
        {
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
        }

        void HandleOnPlayModeChanged(PlayModeStateChange change)
        {
            // This method is run whenever the playmode state is changed.
            if (!EditorApplication.isPlaying)
            {
                // do stuff when the editor is paused.
                ResetPulseMaterial();
            }
        }

#endif // UNITY_EDITOR

        private void OnDestroy()
        {
            ResetPulseMaterial();
        }

        private void Start()
        {
            if(pulseOnSelect)
            {
                // Add PointerHandler script to the parent of dynamically generated spatial mesh on the device
                CoreServices.SpatialAwarenessSystem.SpatialAwarenessObjectParent.AddComponent<PointerHandler>();
                CoreServices.SpatialAwarenessSystem.SpatialAwarenessObjectParent.GetComponent<PointerHandler>().OnPointerClicked.AddListener(this.OnPointerClicked);
            }

            if (autoStart)
            {
                StartPulsing();
            }
        }

        private void Update()
        {
            if (originFollowCamera)
            {
                SetLocalOrigin(CameraCache.Main.transform.position);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Material control
        /////////////////////////////////////////////////////////////////////////////////////////
        public void SetLocalOrigin(Vector3 origin)
        {
            SurfaceMat.SetVector(OriginParamName, origin);
        }

        public void ResetPulseMaterial()
        {
            ApplyPulseRadiusToMaterial(0);
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Pulse control
        /////////////////////////////////////////////////////////////////////////////////////////
        public void PulseOnce()
        {
            cancelPulse = false;
            StartCoroutine(CoSinglePulse());
        }

        public void StartPulsing()
        {
            repeatingPulse = true;
            cancelPulse = false;
            if (RepeatPulseCoroutine == null)
            {
                RepeatPulseCoroutine = StartCoroutine(CoRepeatPulse());
            }
        }

        public void StopPulsing(bool bFinishCurrentPulse = true)
        {
            repeatingPulse = false;
            if (!bFinishCurrentPulse)
            {
                cancelPulse = true;
                ApplyPulseRadiusToMaterial(0);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Implementation
        /////////////////////////////////////////////////////////////////////////////////////////
        IEnumerator CoSinglePulse()
        {
            yield return CoWaitForRepeatDelay();
            if (!cancelPulse)
            {
                yield return CoAnimatePulse();
            }
        }

        IEnumerator CoRepeatPulse()
        {
            while (repeatingPulse && !cancelPulse)
            {
                yield return CoSinglePulse();
            }

            RepeatPulseCoroutine = null;
        }

        private IEnumerator CoAnimatePulse()
        {
            pulseStartedTime = Time.time;
            float t = 0;
            while (t < PulseDuration && !cancelPulse)
            {
                t += Time.deltaTime;
                ApplyPulseRadiusToMaterial(t / PulseDuration);
                yield return null;
            }
        }

        IEnumerator CoWaitForRepeatDelay()
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

        void ApplyPulseRadiusToMaterial(float radius)
        {
            surfaceMat.SetFloat(paramName, radius);
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            cancelPulse = true;
            SetLocalOrigin(eventData.Pointer.Result.Details.Point);
            PulseOnce();
        }
    }
}
