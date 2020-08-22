// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    /// <summary>
    /// A basic lost tracking visual for HoloLens devices.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Extensions/BasicLostTrackingVisual")]
    public class BasicLostTrackingVisual : MonoBehaviour, ILostTrackingVisual
    {
        [SerializeField]
        [Tooltip("The renderer for this lost tracking visual.")]
        private MeshRenderer gridRenderer = null;

        [SerializeField]
        [Tooltip("The audio to play while the lost tracking visual is active.")]
        private AudioClip loopClip = null;

        [SerializeField]
        [Tooltip("The AudioSource to play from while the lost tracking visual is active.")]
        private AudioSource audioSource = null;

        [SerializeField]
        [Tooltip("How long the lost tracking visual's pulse has been running (up to Pulse Duration).")]
        private float pulseTimer = 0.0f;

        [SerializeField]
        [Tooltip("How long the lost tracking visual's pulse runs.")]
        private float pulseDuration = 2.0f;

        /// <inheritdoc />
        public bool Enabled
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void SetLayer(int layer)
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layer;
            }
        }

        private void Update()
        {
            // Using unscaled delta time is necessary to avoid the effect pausing when Timescale is set to 0.0f
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