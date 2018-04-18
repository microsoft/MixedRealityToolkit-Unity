// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.SpatialSound.Effects;
using UnityEngine;

namespace MixedRealityToolkit.Examples.SpatialSound
{
    public class LoFiFilterSelection : MonoBehaviour, IPointerHandler
    {
        [Tooltip("Material used when the emitter is set to Narrow Band Telephony")]
        [SerializeField]
        private Material NarrowBandTelephony;

        [Tooltip("Material used when the emitter is set to AM Radio")]
        [SerializeField]
        private Material AmRadio;

        [Tooltip("Material used when the emitter is set to Full Range")]
        [SerializeField]
        private Material FullRange;

        [Tooltip("Material used when the emitter is set to an unknown quality")]
        [SerializeField]
        private Material UnknownQuality;

        private AudioLoFiEffect loFiEffect;

        private Renderer emitterRenderer;

        private void Start()
        {
            // Cache the emitter game object's renderer.
            emitterRenderer = gameObject.GetComponent<Renderer>();
            if (emitterRenderer == null)
            {
                Debug.LogError("Failed to retrieve the renderer for this game object.");
            }

            // Get the attached AudioLoFiEffect script.
            loFiEffect = gameObject.GetComponent<AudioLoFiEffect>();
            if (loFiEffect == null)
            {
                Debug.LogError("LoFiFilterSelection requires an AudioLoFiEffect to be attached to the game object.");
                return;
            }

            // Set the material of the emitter object to match that of the
            // initial AudioLoFiEffect.SourceQuality value.
            SetEmitterMaterial(loFiEffect.SourceQuality);
        }

        private void SetEmitterMaterial(AudioLoFiSourceQuality sourceQuality)
        {
            Material emitterMaterial = UnknownQuality;

            // Determine the material for the emitter based on the source quality.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQuality.NarrowBandTelephony:
                    emitterMaterial = NarrowBandTelephony;
                    break;

                case AudioLoFiSourceQuality.AmRadio:
                    emitterMaterial = AmRadio;
                    break;

                case AudioLoFiSourceQuality.FullRange:
                    emitterMaterial = FullRange;
                    break;
            }

            // Set the material on the emitter.
            if (emitterRenderer != null)
            {
                emitterRenderer.sharedMaterial = emitterMaterial;
            }
        }

        public void OnPointerUp(ClickEventData eventData) { }

        public void OnPointerDown(ClickEventData eventData) { }

        public void OnPointerClicked(ClickEventData eventData)
        {
            // Make sure we found an AudioLoFiEffect script.
            if (loFiEffect == null) { return; }

            // Get the current source quality setting.
            AudioLoFiSourceQuality sourceQuality = loFiEffect.SourceQuality;

            // Increment the source quality.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQuality.NarrowBandTelephony:
                    sourceQuality = AudioLoFiSourceQuality.AmRadio;
                    break;

                case AudioLoFiSourceQuality.AmRadio:
                    sourceQuality = AudioLoFiSourceQuality.FullRange;
                    break;

                case AudioLoFiSourceQuality.FullRange:
                    sourceQuality = AudioLoFiSourceQuality.NarrowBandTelephony;
                    break;

            }

            // Update the emitter material to match the new source quality.
            SetEmitterMaterial(sourceQuality);

            // Update the source quality.
            loFiEffect.SourceQuality = sourceQuality;

            // Mark the event as used, so it doesn't fall through to other handlers.
            eventData.Use();
        }
    }
}
