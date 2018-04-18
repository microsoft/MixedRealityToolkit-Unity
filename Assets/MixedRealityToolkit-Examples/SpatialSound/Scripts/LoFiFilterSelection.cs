// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.SpatialSound.Effects;

namespace MixedRealityToolkit.Examples.SpatialSound
{
    public class LoFiFilterSelection : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("Material used when the emitter is set to Narrow Band Telephony")]
        [SerializeField]
        private Material narrowBandTelephony = null;

        [Tooltip("Material used when the emitter is set to AM Radio")]
        [SerializeField]
        private Material amRadio = null;

        [Tooltip("Material used when the emitter is set to Full Range")]
        [SerializeField]
        private Material fullRange = null;

        [Tooltip("Material used when the emitter is set to an unknown quality")]
        [SerializeField]
        private Material unknownQuality = null;

        private AudioLoFiEffect loFiEffect = null;

        private Renderer emitterRenderer = null;

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
            }

            // Set the material of the emitter object to match that of the
            // initial AudioLoFiEffect.SourceQuality value.
            SetEmitterMaterial(loFiEffect.SourceQuality);
        }

        /// <summary>
        /// Changes the AudioLoFiEffect source quality on click.
        /// </summary>
        public void OnInputClicked(InputClickedEventData data)
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
            data.Use();
        }

        private void SetEmitterMaterial(AudioLoFiSourceQuality sourceQuality)
        {
            Material emitterMaterial = unknownQuality;

            // Determine the material for the emitter based on the source quality.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQuality.NarrowBandTelephony:
                    emitterMaterial = narrowBandTelephony;
                    break;

                case AudioLoFiSourceQuality.AmRadio:
                    emitterMaterial = amRadio;
                    break;

                case AudioLoFiSourceQuality.FullRange:
                    emitterMaterial = fullRange;
                    break;
            }

            // Set the material on the emitter.
            if (emitterRenderer != null)
            {
                emitterRenderer.sharedMaterial = emitterMaterial;
            }
        }
    }
}
