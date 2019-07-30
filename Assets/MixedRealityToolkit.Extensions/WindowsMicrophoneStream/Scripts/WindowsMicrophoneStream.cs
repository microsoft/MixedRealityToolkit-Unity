// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Audio
{
    #if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

    /// <summary>
    /// Allows the selection and control of a specific microphone type on Microsoft Windows platforms, including HoloLens.
    /// </summary>
    public class WindowsMicrophoneStream : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WindowsMicrophoneStream()
        { }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~WindowsMicrophoneStream()
        {
            Dispose(false);
        }

        #region IDisposable implementation

        private bool hasBeenDisposed = false;

        /// <summary>
        /// Free all resources used by this class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Free the resources used by this class.
        /// </summary>
        /// <param name="disposing">
        /// If true, frees both managed and native resources. If false, only frees native resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (hasBeenDisposed) { return; }

            //  Release managed resources.
            if (disposing)
            {  }

            // Release native resources.
            Uninitialize();

            // Tell the system to not run the finalizer for this class.
            GC.SuppressFinalize(this);

            hasBeenDisposed = true;
        }

        #endregion IDisposable implementation

        private float gain = 1.0f; // todo: what is a reasonable range?

        /// <summary>
        /// Gets or sets the microphone stream's input gain.
        /// </summary>
        public float Gain
        {
            get => gain;

            set
            {
                if ((gain != value) &&
                    (WindowsMicrophoneStreamErrorCode.Success == (WindowsMicrophoneStreamErrorCode)MicSetGain(value)))
                {
                    gain = value;
                }
            }
        }

        private bool initialized = false;

        /// <summary>
        /// Initializes the microphone stream.
        /// </summary>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        public WindowsMicrophoneStreamErrorCode Initialize(WindowsMicrophoneStreamType streamType)
        {
            if (initialized)
            {
                // The microphone stream is already initialized, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

           return (WindowsMicrophoneStreamErrorCode)MicInitializeCustomRate((int)streamType, AudioSettings.outputSampleRate);
        }

        private bool paused = false;

        /// <summary>
        /// Pauses the microphone stream.
        /// </summary>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        public WindowsMicrophoneStreamErrorCode Pause()
        {
            if (paused)
            {
                // The microphone stream is already paused, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

            return (WindowsMicrophoneStreamErrorCode)MicPause();
        }

        /// <summary>
        /// Reads data from the microphone stream.
        /// </summary>
        /// <param name="buffer"/>The buffer in which to plce the data.</param>
        /// <param name="numChannels">The number of audio channels to read.</param>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        public WindowsMicrophoneStreamErrorCode ReadAudioFrame(float[] buffer, int numChannels)
        {
            return (WindowsMicrophoneStreamErrorCode)MicGetFrame(buffer, buffer.Length, numChannels);
        }

        /// <summary>
        /// Resumes the microphone stream
        /// </summary>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        public WindowsMicrophoneStreamErrorCode Resume()
        {
            if (!paused)
            {
                // The microphone stream is already resumed, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

            return (WindowsMicrophoneStreamErrorCode)MicResume();
        }

        private bool recording = false;

        /// <summary>
        /// Starts a recording of the microphone stream.
        /// </summary>
        /// <param name="fileName">The name of the file in which the data will be written.</param>
        /// <param name="preview">
        /// Indicates whether or not the microphone data is to be previewed using the default
        /// audio device.
        /// </param>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        /// <remarks>
        /// Files are created in the Music Library folder.
        /// </remarks>
        public WindowsMicrophoneStreamErrorCode StartRecording(string fileName, bool preview)
        {
            if (recording)
            {
                // The microphone stream is already recording, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

            return (WindowsMicrophoneStreamErrorCode)MicStartRecording(fileName, preview);
        }

        private bool streaming = false;

        /// <summary>
        /// Starts the microphone stream.
        /// </summary>
        /// <param name="keepData">
        /// Indicates whether or not the microphone data should be retained if the application
        /// becomes unresponsive.
        /// </param>
        /// <param name="preview">
        /// Indicates whether or not the microphone data is to be previewed using the default
        /// audio device.
        /// </param>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        /// <remarks>
        /// When keepData is set to false, the application will always receive the latest
        /// audio data from the microphone. Data received during any unresponsive periods
        /// will be discarded.
        /// </remarks>
        public WindowsMicrophoneStreamErrorCode StartStream(bool keepData, bool preview)
        {
            if (streaming)
            {
                // The microphone stream is already streaming, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

            return (WindowsMicrophoneStreamErrorCode)MicStartStream(keepData, preview);
        }

        /// <summary>
        /// Stops the recording.
        /// </summary>
        /// <returns>The full path to the file containing the recorded microphone data./// </returns>
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
        /// Stops the microphone stream.
        /// </summary>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        public WindowsMicrophoneStreamErrorCode StopStream()
        {
            if (!streaming)
            {
                // The microphone stream is already stopped, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

            return (WindowsMicrophoneStreamErrorCode)MicStopStream();
        }

        /// <summary>
        /// Uninitializes the microphone stream.
        /// </summary>
        /// <returns>
        /// A <see cref="WindowsMicrophoneStreamErrorCode"/> value indicating success or the
        /// reason that the call failed.
        /// </returns>
        public WindowsMicrophoneStreamErrorCode Uninitialize()
        {
            if (!initialized)
            {
                // The microphone stream is not initialized, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

            return (WindowsMicrophoneStreamErrorCode)MicDestroy();
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

    #endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
}