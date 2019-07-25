using Microsoft.MixedReality.Toolkit.Experimental.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental
{
    [RequireComponent(typeof(AudioSource))]
    public class AmbientSoundAmplitude : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The text field in which to display the ambient sound amplitude.")]
        private Text amplitudeDisplay = null;

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
                // todo
            }

            micStream.Gain = 1.0f;
        }

        private void OnDestroy()
        {
            micStream.Uninitialize();
            micStream = null;
        }

        private void Update()
        {
            // todo
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            WindowsMicrophoneStreamErrorCode result = micStream.ReadAudioFrame(buffer, numChannels);
            
            if (result != WindowsMicrophoneStreamErrorCode.Success)
            {
                // todo
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

            // todo consider how often we want to update this data in the UX
            // averageAmplitude = sumOfValues / buffer.Length;
        }
    }
}