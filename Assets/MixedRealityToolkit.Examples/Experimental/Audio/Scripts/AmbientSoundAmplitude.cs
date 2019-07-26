using Microsoft.MixedReality.Toolkit.Experimental.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental
{
    [RequireComponent(typeof(AudioSource))]
    public class AmbientSoundAmplitude : MonoBehaviour
    {
        // todo: replace with spatial observer :)
        [SerializeField]
        [Tooltip("The text field in which to display the ambient sound amplitude.")]
        private Text amplitudeDisplay = null;

        private float averageAmplitude = 0.0f;

        /// <summary>
        /// 
        /// </summary>
        private WindowsMicrophoneStream micStream = null;

        private void Awake()
        {
            micStream = new WindowsMicrophoneStream();

            WindowsMicrophoneStreamErrorCode result = micStream.Initialize(WindowsMicrophoneStreamType.RoomCapture);
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to initialize the microphone stream. {result}");
                amplitudeDisplay.text = $"Failed to initialize the microphone stream. {result}";
            }

            micStream.Gain = 1.0f;

            result = micStream.StartStream(false, false);
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to start the microphone stream. {result}");
                amplitudeDisplay.text = $"Failed to start the microphone stream. {result}";
            }

            // We do not wish to play the ambient room sound from the audio source.
            gameObject.GetComponent<AudioSource>().volume = 0.0f;
        }

        private void OnDestroy()
        {
            WindowsMicrophoneStreamErrorCode result = micStream.StopStream();
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to stop the microphone stream. {result}");
                amplitudeDisplay.text = $"Failed to stop the microphone stream. {result}";
            }

            micStream.Uninitialize();
            micStream = null;
        }

        private void OnDisable()
        {
            WindowsMicrophoneStreamErrorCode result = micStream.Pause();
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to pause the microphone stream. {result}");
                amplitudeDisplay.text = $"Failed to pause the microphone stream. {result}";
            }
        }

        private void OnEnable()
        {
            WindowsMicrophoneStreamErrorCode result = micStream.Resume();
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                Debug.Log($"Failed to resume the microphone stream. {result}");
                amplitudeDisplay.text = $"Failed to resume the microphone stream. {result}";
            }
        }

        private void Update()
        {
            amplitudeDisplay.text = averageAmplitude.ToString();
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
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