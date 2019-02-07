// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    [RequireComponent(typeof(AudioSource))]
    public class MicStreamDemo : MonoBehaviour
    {
        /// <summary>
        /// Which type of microphone/quality to access.
        /// </summary>
        public MicStream.StreamCategory StreamType = MicStream.StreamCategory.HIGH_QUALITY_VOICE;

        /// <summary>
        /// Can boost volume here as desired. 1 is default.
        /// <remarks>Can be updated at runtime.</remarks> 
        /// </summary>
        public float InputGain = 1;

        /// <summary>
        /// if keepAllData==false, you'll always get the newest data no matter how long the program becomes unresponsive
        /// for any reason. It will lose some data if the program does not respond.
        /// <remarks>Can only be set on initialization.</remarks>
        /// </summary>
        public bool KeepAllData;

        /// <summary>
        /// If true, Starts the mic stream automatically when this component is enabled.
        /// </summary>
        public bool AutomaticallyStartStream = true;

        /// <summary>
        /// Plays back the microphone audio source though default audio device.
        /// </summary>
        public bool PlaybackMicrophoneAudioSource = true;

        /// <summary>
        /// The name of the file to which to save audio (for commands that save to a file).
        /// </summary>
        public string SaveFileName = "MicrophoneTest.wav";

        /// <summary>
        /// Records estimation of volume from the microphone to affect other elements of the game object.
        /// </summary>
        private float averageAmplitude;

        /// <summary>
        /// Minimum size the demo cube can be during runtime.
        /// </summary>
        [SerializeField]
        private float minObjectScale = .3f;

        private bool isRunning;

        public bool IsRunning
        {
            get { return isRunning; }
            private set
            {
                isRunning = value;
                CheckForErrorOnCall(isRunning ? MicStream.MicPause() : MicStream.MicResume());
            }
        }

        #region Unity Methods

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            // this is where we call into the DLL and let it fill our audio buffer for us
            CheckForErrorOnCall(MicStream.MicGetFrame(buffer, buffer.Length, numChannels));

            float sumOfValues = 0;

            // figure out the average amplitude from this new data
            for (int i = 0; i < buffer.Length; i++)
            {
                if (float.IsNaN(buffer[i]))
                {
                    buffer[i] = 0;
                }

                buffer[i] = Mathf.Clamp(buffer[i], -1f, 1f);
                sumOfValues += Mathf.Clamp01(Mathf.Abs(buffer[i]));
            }
            averageAmplitude = sumOfValues / buffer.Length;
        }

        private void OnEnable()
        {
            IsRunning = true;
        }

        private void Awake()
        {
            CheckForErrorOnCall(MicStream.MicInitializeCustomRate((int)StreamType, AudioSettings.outputSampleRate));
            CheckForErrorOnCall(MicStream.MicSetGain(InputGain));

            if (!PlaybackMicrophoneAudioSource)
            {
                gameObject.GetComponent<AudioSource>().volume = 0; // can set to zero to mute mic monitoring
            }

            if (AutomaticallyStartStream)
            {
                CheckForErrorOnCall(MicStream.MicStartStream(KeepAllData, false));
            }

            print("MicStream selector demo");
            print("press Q to start stream to audio source, W will stop that stream");
            print("press A to start a recording and S to stop that recording and save it to a wav file.");
            print("Since this all goes through the AudioSource, you can mute the mic while using it there, or do anything else you would do with an AudioSource");
            print("In this demo, we start the stream automatically, and then change the size of the GameObject based on microphone signal amplitude");
            isRunning = true;
        }

        private void Update()
        {
            CheckForErrorOnCall(MicStream.MicSetGain(InputGain));

            if (Input.GetKeyDown(KeyCode.Q))
            {
                CheckForErrorOnCall(MicStream.MicStartStream(KeepAllData, false));
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                CheckForErrorOnCall(MicStream.MicStopStream());
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                CheckForErrorOnCall(MicStream.MicStartRecording(SaveFileName, false));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                string outputPath = MicStream.MicStopRecording();
                Debug.Log("Saved microphone audio to " + outputPath);
                CheckForErrorOnCall(MicStream.MicStopStream());
            }

            gameObject.transform.localScale = new Vector3(minObjectScale + averageAmplitude, minObjectScale + averageAmplitude, minObjectScale + averageAmplitude);
        }

        private void OnApplicationPause(bool pause)
        {
            IsRunning = !pause;
        }

        private void OnDisable()
        {
            IsRunning = false;
        }

        private void OnDestroy()
        {
            CheckForErrorOnCall(MicStream.MicDestroy());
        }

#if !UNITY_EDITOR
        private void OnApplicationFocus(bool focused)
        {
            IsRunning = focused;
        }
#endif
        #endregion

        private static void CheckForErrorOnCall(int returnCode)
        {
            MicStream.CheckForErrorOnCall(returnCode);
        }

        public void Enable()
        {
            IsRunning = true;
        }

        public void Disable()
        {
            IsRunning = false;
        }
    }
}
