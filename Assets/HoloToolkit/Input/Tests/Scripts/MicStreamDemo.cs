// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Text;

namespace HoloToolkit.Unity
{
    [RequireComponent(typeof(AudioSource))]
    public class MicStreamDemo : MonoBehaviour
    {
        /// <summary>
        /// Which type of microphone/quality to access
        /// </summary>
        public MicStream.StreamCategory StreamType = MicStream.StreamCategory.HIGH_QUALITY_VOICE;

        /// <summary>
        /// can boost volume here as desired. 1 is default but probably too quiet. can change during operation. 
        /// </summary>
        public float InputGain = 1;

        /// <summary>
        /// if keepAllData==false, you'll always get the newest data no matter how long the program hangs for any reason, but will lose some data if the program does hang 
        /// can only be set on initialization
        /// </summary>
        public bool KeepAllData = false;

        /// <summary>
        /// Should the mic stream start automatically when this component is enabled?
        /// </summary>
        public bool AutomaticallyStartStream = true;

        /// <summary>
        /// Do you want to hear what the microphone sounds like by listening to the AudioSource in Unity?
        /// </summary>
        public bool ListenToAudioSource = true;

        /// <summary>
        /// The name of the file to which to save audio (for commands that save to a file)
        /// </summary>
        public string SaveFileName = "MicrophoneTest.wav";

        /// <summary>
        /// Records estimation of volume from the microphone to affect other elements of the game object
        /// </summary>
        private float averageAmplitude = 0;

        /// <summary>
        /// how small can our object be in this demo?
        /// </summary>
        private float minSize = .3f;

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            // this is where we call into the DLL and let it fill our audio buffer for us
            CheckForErrorOnCall(MicStream.MicGetFrame(buffer, buffer.Length, numChannels));

            float sumOfValues = 0;
            // figure out the average amplitude from this new data
            for (int i=0; i<buffer.Length; i++)
            {
                sumOfValues += Mathf.Abs(buffer[i]);
            }
            averageAmplitude = sumOfValues / buffer.Length;
        }

        private void Awake()
        {
            CheckForErrorOnCall(MicStream.MicInitializeCustomRate((int)StreamType, AudioSettings.outputSampleRate));
            CheckForErrorOnCall(MicStream.MicSetGain(InputGain));

            if (!ListenToAudioSource)
            {
                this.gameObject.GetComponent<AudioSource>().volume = 0; // can set to zero to mute mic monitoring
            }

            if (AutomaticallyStartStream)
            {
                CheckForErrorOnCall(MicStream.MicStartStream(KeepAllData, false));
            }

            print("MicStream selector demo");
            print("press Q to start stream to audio source, W will stop that stream");
            print("It will start a recording and save it to a wav file. S will stop that recording.");
            print("Since this all goes through the AudioSource, you can mute the mic while using it there, or do anything else you would do with an AudioSource");
            print("In this demo, we start the stream automatically, and then change the size of the gameobject based on microphone signal amplitude");
        }

        private void OnDestroy()
        {
            CheckForErrorOnCall(MicStream.MicDestroy());
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

            this.gameObject.transform.localScale = new Vector3(minSize + averageAmplitude, minSize + averageAmplitude, minSize + averageAmplitude);
        }

        private void CheckForErrorOnCall(int returnCode)
        {
            MicStream.CheckForErrorOnCall(returnCode);
        }

#if DOTNET_FX
        // on device, deal with all the ways that we could suspend our program in as few lines as possible
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                CheckForErrorOnCall(MicStream.MicPause());
            }
            else
            {
                CheckForErrorOnCall(MicStream.MicResume());
            }
        }

        private void OnApplicationFocus(bool focused)
        {
            this.OnApplicationPause(!focused);
        }

        private void OnDisable()
        {
            this.OnApplicationPause(true);
        }

        private void OnEnable()
        {
            this.OnApplicationPause(false);
        }
#endif
    }
}