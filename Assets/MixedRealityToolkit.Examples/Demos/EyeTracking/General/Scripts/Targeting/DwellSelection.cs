// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    [RequireComponent(typeof(Interactable))]
    [AddComponentMenu("Scripts/MRTK/Examples/DwellSelection")]
    public class DwellSelection : BaseEyeFocusHandler, IMixedRealityPointerHandler
    {
        private static bool? useDwell = null;

        [SerializeField]
        [Tooltip("If true, the attached target will be selectable via dwell on startup.")]
        private bool startEnabledOnStartup = false;

        [SerializeField]
        [Tooltip("Root feedback game object that will be enabled once dwell is initiated.")]
        private GameObject rootFeedbackToEnable = null;

        [SerializeField]
        [Tooltip("Game object to manipulate for providing feedback about the dwell state.")]
        private GameObject feedbackToChangeInSize = null;

        [SerializeField]
        [Tooltip("Game object to represent the max (initial) extension.")]
        private GameObject feedbackStartSize = null;

        [SerializeField]
        [Tooltip("Game object to represent the min (initial) extension.")]
        private GameObject feedbackEndSize = null;

        [SerializeField]
        [Tooltip("Delay in seconds until dwell feedback is started to be shown.")]
        [Range(0, 5)]
        private float feedbackDelayInSeconds = 0.5f;

        [SerializeField]
        [Tooltip("Additional time (not including the initial delay time) the user needs to keep looking at the UI to activate it.")]
        [Range(0, 5)]
        private float dwellTimeInSecondsToSelect = 2.0f;

        [SerializeField]
        [Tooltip("Start target size")]
        private Vector3 startScale = Vector3.zero;

        [SerializeField]
        [Tooltip("End target size")]
        private Vector3 endScale = Vector3.zero;

        [SerializeField]
        [Tooltip("Initial transparency to start with.")]
        [Range(0, 1)]
        float startTransp = 0.1f;

        [SerializeField]
        [Tooltip("Final transparency to end with.")]
        [Range(0, 1)]
        float endTransp = 1.0f;

        private bool isEyeDwelling = false;
        private bool isFinished = false;
        private DateTime startTime_lookAt;
        private DateTime startTime_dwellFeedback;

        private Interactable routingTarget;

        void Start()
        {
            if (!useDwell.HasValue)
            {
                useDwell = startEnabledOnStartup;
            }

            routingTarget = GetComponent<Interactable>();

            ResetDwellFeedback();
        }

        private void EtTarget_OnTargetSelected(object sender, TargetEventArgs e)
        {
            ResetDwellFeedback();
        }

        public void EnableDwell()
        {
            Debug.Log("Enable dwell activations.");
            useDwell = true;
        }

        public void DisableDwell()
        {
            Debug.Log("Disable dwell activations.");
            useDwell = false;
        }

        private bool UseDwell
        {
            get { return (useDwell.HasValue ? useDwell.Value : false); }
        }

        protected override void OnEyeFocusStart()
        {
            if ((routingTarget != null) && (!routingTarget.HasPress) && UseDwell)
            {
                ResetDwellFeedback();
                isFinished = false;
                startTime_lookAt = DateTime.UtcNow;
            }
        }

        protected override void OnEyeFocusStay()
        {
            if (this != null)
            {
                if (!isFinished && !isEyeDwelling && ((DateTime.UtcNow - startTime_lookAt).TotalSeconds > feedbackDelayInSeconds))
                {
                    StartDwellFeedback();
                }
                else if (isEyeDwelling && ((DateTime.UtcNow - startTime_dwellFeedback).TotalSeconds > dwellTimeInSecondsToSelect))
                {
                    if (routingTarget != null)
                    {
                        routingTarget.TriggerOnClick();
                    }
                    ResetDwellFeedback();
                }
            }
        }

        protected override void OnEyeFocusStop()
        {
            ResetDwellFeedback();
        }


        private void StartDwellFeedback()
        {
            isEyeDwelling = true;
            startTime_dwellFeedback = DateTime.UtcNow;

            if (feedbackToChangeInSize != null)
            {
                feedbackToChangeInSize.SetActive(true);
            }

            if (feedbackStartSize != null)
            {
                feedbackStartSize.SetActive(true);
            }

            if (feedbackEndSize != null)
            {
                feedbackEndSize.SetActive(true);
            }

            if (rootFeedbackToEnable != null)
            {
                rootFeedbackToEnable.SetActive(true);
            }
        }

        private void ResetDwellFeedback()
        {
            isFinished = true;
            isEyeDwelling = false;

            if (feedbackToChangeInSize != null)
            {
                feedbackToChangeInSize.transform.localScale = startScale;
                feedbackToChangeInSize.SetActive(false);
            }

            if (feedbackStartSize != null)
            {
                feedbackStartSize.transform.localScale = startScale;
                feedbackStartSize.SetActive(false);
            }

            if (feedbackEndSize != null)
            {
                feedbackEndSize.transform.localScale = endScale;
                feedbackEndSize.SetActive(false);
            }

            if (rootFeedbackToEnable != null)
            {
                rootFeedbackToEnable.SetActive(false);
            }
        }

        private Vector3 SpeedSizeChangePerSecond
        {
            get
            {
                if (dwellTimeInSecondsToSelect != 0)
                {
                    return (endScale - startScale) / dwellTimeInSecondsToSelect;
                }
                return Vector3.zero;
            }
        }

        protected override void Update()
        {
            base.Update();

            if ((isEyeDwelling) && (feedbackToChangeInSize != null))
            {
                Vector3 newScale = startScale + SpeedSizeChangePerSecond * (float)(DateTime.UtcNow - startTime_dwellFeedback).TotalSeconds;
                feedbackToChangeInSize.transform.localScale = ClampVector3(newScale, startScale, endScale);

                float normalizedProgress = Mathf.Clamp((float)(DateTime.UtcNow - startTime_dwellFeedback).TotalSeconds, 0, dwellTimeInSecondsToSelect) / dwellTimeInSecondsToSelect;
                UpdateTransparency(normalizedProgress);
            }
        }

        private Vector3 ClampVector3(Vector3 vec, Vector3 vecStart, Vector3 vecEnd)
        {
            Vector3 result = Vector3.zero;
            result.x = Mathf.Clamp(vec.x, Mathf.Min(vecStart.x, vecEnd.x), Mathf.Max(vecStart.x, vecEnd.x));
            result.y = Mathf.Clamp(vec.y, Mathf.Min(vecStart.y, vecEnd.y), Mathf.Max(vecStart.y, vecEnd.y));
            result.z = Mathf.Clamp(vec.z, Mathf.Min(vecStart.z, vecEnd.z), Mathf.Max(vecStart.z, vecEnd.z));
            return result;
        }

        private void UpdateTransparency(float normalizedProgress)
        {
            if (feedbackToChangeInSize != null)
            {
                if (feedbackRenderer == null)
                {
                    feedbackRenderer = feedbackToChangeInSize.GetComponent<Renderer>();
                }

                if (feedbackRenderer != null)
                {

                    Material[] mats = feedbackRenderer.materials;
                    if ((mats != null) && (mats.Length > 0))
                    {

                        // Blend the colors from the original to the highlight color based on the normalized interest
                        // Unity Standard Shaders
                        if (!originalColor.HasValue)
                        {
                            mats[0].EnableKeyword(c_DefaultTextureShaderProperty);
                            originalColor = mats[0].GetColor(textureTargetID);
                        }

                        Color newColor = new Color(originalColor.Value.r, originalColor.Value.g, originalColor.Value.b, LerpTransparency(startTransp, endTransp, normalizedProgress));
                        mats[0].SetColor(textureTargetID, newColor);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an individual color channel that is blended between an original and final value based on normalized blend factor.
        /// </summary>
        private float LerpTransparency(float startVal, float endVal, float normalizedBlendFactor)
        {
            return (startVal + (endVal - startVal) * normalizedBlendFactor);
        }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
        }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            ResetDwellFeedback();
        }

        private Color? originalColor = null;
        private Renderer feedbackRenderer = null;
        private const string c_DefaultTextureShaderProperty = "_Color";
        private int textureTargetID = Shader.PropertyToID(c_DefaultTextureShaderProperty);
        private string textureShaderProperty = c_DefaultTextureShaderProperty;
        public string TextureShaderProperty
        {
            get
            {
                return textureShaderProperty;
            }
            set
            {
                textureShaderProperty = value;
                textureTargetID = Shader.PropertyToID(textureShaderProperty);
            }
        }
    }
}
