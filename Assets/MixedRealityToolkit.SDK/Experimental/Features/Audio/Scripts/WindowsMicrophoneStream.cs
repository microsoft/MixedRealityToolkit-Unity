// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// todo: only support windows standalone and uwp builds

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Audio
{
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
        /// todo
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (hasBeenDisposed)
            { /* Release cached managed resources */ }

            // Release native resources.
            Uninitialize();

            // Tell the system to not run the finalizer for this class.
            GC.SuppressFinalize(this);
        }

    #endregion IDisposable implementation

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
                    (WindowsMicrophoneStreamErrorCode.Success == (WindowsMicrophoneStreamErrorCode)MicSetGain(value)))
                {
                    gain = value;
                }
            }
        }

        private bool initialized = false;

        /// <summary>
        /// todo
        /// </summary>
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
        /// todo
        /// </summary>
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
        /// todo
        /// </summary>
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
        /// todo
        /// </summary>
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
        /// todo
        /// </summary>
        public WindowsMicrophoneStreamErrorCode StartStream(bool keepData, bool preview)
        {
            if (streaming)
            {
                // The microphone stream is already streaming, no need to alarm the calling code.
                return WindowsMicrophoneStreamErrorCode.Success;
            }

            return (WindowsMicrophoneStreamErrorCode)MicStartStream(keepData, preview, null);
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
        /// todo
        /// </summary>
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
}