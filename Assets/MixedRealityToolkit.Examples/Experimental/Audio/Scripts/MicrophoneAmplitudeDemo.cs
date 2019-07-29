// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.Audio;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental
{
    /// <summary>
    /// Demonstration class using <see cref="WindowsMicrophoneStream"/> to select the voice microphone and
    /// adjust the spatial awareness mesh based on the amplitude of the user's voice.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneAmplitudeDemo : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Factor by which to boost the microphone amplitude when changing the mesh display.")]
        [Range(0, 50)]
        private int amplitudeBoostFactor = 10;

        private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// Instance of the spatial awareness system.
        /// </summary>
        private IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
        {
            get
            {
                if (spatialAwarenessSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
                }
                return spatialAwarenessSystem;
            }
        }

        private IMixedRealitySpatialAwarenessMeshObserver spatialMeshObserver = null;

        /// <summary>
        /// Instance of a spatial awareness mesh observer. Used to acquire the visible mesh material.
        /// </summary>
        private IMixedRealitySpatialAwarenessMeshObserver SpatialMeshObserver
        {
            get
            {
                if (spatialMeshObserver == null)
                {
                    spatialMeshObserver = (SpatialAwarenessSystem as IMixedRealityDataProviderAccess)?.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
                }
                return spatialMeshObserver;
            }
        }

        private Material visibleMaterial = null;

        /// <summary>
        /// The visuble mesh material in use by the spatial mesh observer.
        /// </summary>
        private Material VisibleMaterial
        {
            get
            {
                if (visibleMaterial == null)
                {
                    visibleMaterial = SpatialMeshObserver?.VisibleMaterial;
                }
                return visibleMaterial;
            }
        }

        /// <summary>
        /// Class providing microphone stream management support on Microsoft Windows based devices.
        /// </summary>
        private WindowsMicrophoneStream micStream = null;

        /// <summary>
        /// The average amplitude of the sound captured during the most recent microphone update.
        /// </summary>
        private float averageAmplitude = 0.0f;

        #if UNITY_EDITOR
        /// <summary>
        /// Cached material values used to restore initial settings when running the demo in the editor.
        /// </summary>
        private Color defaultMaterialColor = Color.black;
        private int defaultWireThickness = 0;
#endif // UNITY_EDITOR

        private void Awake()
        {
            // We do not wish to play the ambient room sound from the audio source.
            gameObject.GetComponent<AudioSource>().volume = 0.0f;

            if (VisibleMaterial != null)
            {
#if UNITY_EDITOR
                // Cache the initial material settings.
                defaultMaterialColor = VisibleMaterial.GetColor("_WireColor");
                defaultWireThickness = VisibleMaterial.GetInt("_WireThickness");
#endif // UNITY_EDITOR

                VisibleMaterial.SetColor("_WireColor", Color.blue);
            }

            micStream = new WindowsMicrophoneStream();
            micStream.Gain = 1.0f;

            // Initialize the microphone stream.
            WindowsMicrophoneStreamErrorCode result = micStream.Initialize(WindowsMicrophoneStreamType.HighQualityVoice);
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to initialize the microphone stream. {result}");
            }

            // Start the microphone stream.
            // Do not keep the data and do not preview.
            result = micStream.StartStream(false, false);
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to start the microphone stream. {result}");
            }
        }

        private void OnDestroy()
        {
            // Stop the microphone stream.
            WindowsMicrophoneStreamErrorCode result = micStream.StopStream();
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to stop the microphone stream. {result}");
            }

            // Uninitialize the microphone stream.
            micStream.Uninitialize();
            micStream = null;

#if UNITY_EDITOR
            // Restore the initial material settings.
            if (VisibleMaterial != null)
            {
                VisibleMaterial.SetColor("_WireColor", defaultMaterialColor);
                VisibleMaterial.SetInt("_WireThickness", defaultWireThickness);
            }
#endif // UNITY_EDITOR
        }

        private void OnDisable()
        {
            // Pause the microphone stream.
            WindowsMicrophoneStreamErrorCode result = micStream.Pause();
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to pause the microphone stream. {result}");
            }
        }

        private void OnEnable()
        {
            // Resume the microphone stream.
            WindowsMicrophoneStreamErrorCode result = micStream.Resume();
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to resume the microphone stream. {result}");
            }
        }

        private static int maxWireThickness = 750;

        private void Update()
        {
            if (VisibleMaterial != null)
            {
                // Artificially increase the amplitude to make the visible effect more pronounced.
                int wireThickness = (int)(averageAmplitude * amplitudeBoostFactor * maxWireThickness);
                wireThickness = Mathf.Clamp(wireThickness, 0, maxWireThickness);
                VisibleMaterial.SetInt("_WireThickness", wireThickness);
            }
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            // Read the microphone stream data.
            WindowsMicrophoneStreamErrorCode result = micStream.ReadAudioFrame(buffer, numChannels);
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to read the microphone stream data. {result}");
            }

            float sumOfValues = 0;

            // Calculate this frame's average amplitude.
            for (int i = 0; i < buffer.Length; i++)
            {
                if (float.IsNaN(buffer[i]))
                {
                    buffer[i] = 0;
                }

                buffer[i] = Mathf.Clamp(buffer[i], -1.0f, 1.0f);
                sumOfValues += Mathf.Clamp01(Mathf.Abs(buffer[i]));
            }

            averageAmplitude = sumOfValues / buffer.Length;
        }
    }
}