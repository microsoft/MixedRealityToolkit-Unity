// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// todo: only support windows standalone and uwp builds

using Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Audio
{
    /// <summary>
    /// todo
    /// </summary>
    public class WindowsMicrophoneStreamSelector
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WindowsMicrophoneStreamSelector()

        {
            // todo
        }

        private float gain = 1.0f; // todo: what is a reasonable range?

        /// <summary>
        /// todo
        /// </summary>
        public float Gain
        {
            get => gain;

            set
            {
                if ((gain != value) &&
                    ValidateResult((WindowsMicrophoneStreamErrorCode)MicSetGain(value)))
                {
                    gain = value;
                }
            }
        }

        private bool initialized = false;

        /// <summary>
        /// todo
        /// </summary>
        public void Initialize(WindowsMicrophoneStreamType streamType)
        {
            if (!initialized &&
                ValidateResult((WindowsMicrophoneStreamErrorCode)MicInitializeCustomRate((int)streamType, AudioSettings.outputSampleRate)))
            {
                initialized = true;
            }
        }

        private bool paused = false;

        /// <summary>
        /// todo
        /// </summary>
        public void Pause()
        {
            if (!paused &&
                ValidateResult((WindowsMicrophoneStreamErrorCode)MicPause()))
            {
                paused = true;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public void Resume()
        {
            if (paused &&
                ValidateResult((WindowsMicrophoneStreamErrorCode)MicResume()))
            {
                paused = false;
            }
        }

        private bool recording = false;

        /// <summary>
        /// todo
        /// </summary>
        public void StartRecording(string fileName, bool preview)
        {
            if (!recording &&
                ValidateResult((WindowsMicrophoneStreamErrorCode)MicStartRecording(fileName, preview)))
            {
                recording = true;
            }
        }

        private bool streaming = false;

        /// <summary>
        /// todo
        /// </summary>
        public void StartStream(bool keepData, bool preview)
        {
            if (!streaming &&
                ValidateResult((WindowsMicrophoneStreamErrorCode)MicStartStream(keepData, preview, null)))
            {
                streaming = true;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public string StopRecording()
        {
            if (!recording)
            {
                // todo: log message
                return string.Empty;
            }

            StringBuilder fullPath = new StringBuilder();
            MicStopRecording(fullPath);
            recording = false;
            return fullPath.ToString();
        }

        /// <summary>
        /// todo
        /// </summary>
        public void StopStream()
        {
            if (streaming &&
                ValidateResult((WindowsMicrophoneStreamErrorCode)MicStopStream()))
            {
                streaming = false;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public void Uninitialize()
        {
            if (!initialized &&
                ValidateResult((WindowsMicrophoneStreamErrorCode)MicDestroy()))
            {
                initialized = false;
            }
        }

        /// <summary>
        /// Validates the success or failure of return value and prints a developer friendly message as appropriate.
        /// </summary>
        /// <param name="returnValue">The return value to validate.</param>
        /// <returns>
        /// True if the return value indicates success, false otherwise.
        /// </returns>
        private bool ValidateResult(WindowsMicrophoneStreamErrorCode returnValue)
        {
            bool success = false;

            switch (returnValue)
            {
                case WindowsMicrophoneStreamErrorCode.Success:
                    success = true;
                    break;
                    
                case WindowsMicrophoneStreamErrorCode.AlreadyRecording:
                    Debug.LogWarning("The microphone stream is currently recording.");
                    success = false;
                    break;

                case WindowsMicrophoneStreamErrorCode.AlreadyRunning:
                    Debug.LogWarning("The microphone has already been initialized.");
                    success = false;
                    break;

                case WindowsMicrophoneStreamErrorCode.GraphDoesNotExist:
                    Debug.LogError("A microphone is not connected or the stream has not been initialized.");
                    success = false;
                    break;

                case WindowsMicrophoneStreamErrorCode.NoAudioDevice:
                case WindowsMicrophoneStreamErrorCode.NoInputDevice:
                    Debug.LogError("A microphone does not appear to be configured on this system.");
                    success = false;
                    break;

                case WindowsMicrophoneStreamErrorCode.ChannelCountMismatch:
                    Debug.LogError("The microphone appears to be misconfigured. Pleae try setting different mono/stereo options in the operating system settings.");
                    success = false;
                    break;

                case WindowsMicrophoneStreamErrorCode.FileCreationPermissionError:
                    Debug.LogError("Unable to create a file in the Music Library. Please ensure the proper permissions are configured.");
                    success = false;
                    break;

                case WindowsMicrophoneStreamErrorCode.NeedMicCapabilityEnabled:
                    Debug.LogError("Unable to access the microphone. Please ensure the proper capabilies are configured.");
                    success = false;
                    break;

                case WindowsMicrophoneStreamErrorCode.NotEnoughData:
                    // Generally the device has recently been started and has not produced enough data.
                    success = false;
                    break;

                default:
                    Debug.LogError($"An unexpected error ({(int)returnValue}) has occcured.");
                    success = false;
                    break;
            }

            return success;
        }

        #region MicStream.dll methods

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void LiveMicCallback();

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicDestroy();

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicGetFrame(float[] buffer, int length, int numchannels);

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicInitializeCustomRate(int category, int samplerate);

        [DllImport("MicStreamSelector", ExactSpelling = true)]

        private static extern int MicPause();

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicResume();

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicSetGain(float g);

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicStartRecording(string filename, bool previewOnDevice);

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicStartStream(bool keepData, bool previewOnDevice, LiveMicCallback micsignal = null);

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern void MicStopRecording(StringBuilder sb);

        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicStopStream();

        #endregion MicStream.dll methods
    }

    /// <summary>
    /// todo
    /// </summary>
    enum WindowsMicrophoneStreamErrorCode
    {
        /// <summary>
        /// todo
        /// </summary>
        Success = 0,

        /// <summary>
        /// todo
        /// </summary>
        AlreadyRunning = -10,

        /// <summary>
        /// todo
        /// </summary>
        NoAudioDevice,

        /// <summary>
        /// todo
        /// </summary>
        NoInputDevice,

        /// <summary>
        /// todo
        /// </summary>
        AlreadyRecording,

        /// <summary>
        /// todo
        /// </summary>
        GraphDoesNotExist,

        /// <summary>
        /// todo
        /// </summary>
        ChannelCountMismatch,

        /// <summary>
        /// todo
        /// </summary>
        FileCreationPermissionError,

        /// <summary>
        /// todo
        /// </summary>
        NotEnoughData,

        /// <summary>
        /// todo
        /// </summary>
        NeedMicCapabilityEnabled
    }
}