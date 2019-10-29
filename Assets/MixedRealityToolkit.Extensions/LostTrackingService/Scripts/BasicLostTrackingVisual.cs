// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    /// <summary>
    /// A basic lost tracking visual for HoloLens devices.
    /// </summary>
    public class BasicLostTrackingVisual : MonoBehaviour, ILostTrackingVisual
    {
        [SerializeField]
        private MeshRenderer gridRenderer = null;
        [SerializeField]
        private AudioClip loopClip = null;
        [SerializeField]
        private AudioSource audioSource = null;
        [SerializeField]
        private float pulseTimer = 0.0f;
        [SerializeField]
        private float pulseDuration = 2.0f;

        public bool Enabled
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        public void ResetVisual()
        {
            if (audioSource != null && loopClip != null)
            {
                audioSource.clip = loopClip;
                audioSource.loop = true;
                audioSource.Play();
            }

            pulseTimer = 0.0f;

            if (gridRenderer != null)
            {
                gridRenderer.material.SetFloat("_Pulse_", 0.0f);
            }
        }

        public void SetLayer(int layer)
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layer;
            }
        }

        private void Update()
        {
            //Using unscaled delta time is necessary to avoid the effect pausing when Timescale is set to 0.0f
            pulseTimer += Time.unscaledDeltaTime;
            float normalizedPulseValue = Mathf.Clamp01(pulseTimer / pulseDuration);

            if (pulseTimer >= pulseDuration)
            {
                pulseTimer = 0;
            }

            if (gridRenderer != null)
            {
                gridRenderer.material.SetFloat("_Pulse_", normalizedPulseValue);
                gridRenderer.material.SetVector("_Pulse_Origin_", gridRenderer.transform.position);
            }
        }
    }
}