// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// todo: only support windows standalone and uwp builds

using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
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

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~WindowsMicrophoneStreamSelector()
        {
           Dispose(false);
        }

        private float gain = 1.0f; // todo: what is a reasonable range?

        /// <inheritdoc />
        public float Gain
        {
            get => gain;

            set
            {
                if (0 == MicSetGain(value))
                {
                    gain = value;
                }
                else
                {
                    // todo: errror
                }
            }
        }

        /// <inheritdoc />
        public int InitializeMicrophone()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int Pause()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int Resume()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int StartRecording()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int StartStream()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int StopRecording()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int StopStream()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int UninitializeMicrophone()
        {
            throw new System.NotImplementedException();
        }

        #region IDisposable implementation

        private bool hasBeenDisposed = false;

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!hasBeenDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                hasBeenDisposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        #endregion IDisposable implementation

        #region MicStream.dll p/invoke methods

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

        #endregion MicStream.dll p/invoke methods
    }

    /// <summary>
    /// todo
    /// </summary>
    enum WindowsMicrophoneStreamErrorCode
    {
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