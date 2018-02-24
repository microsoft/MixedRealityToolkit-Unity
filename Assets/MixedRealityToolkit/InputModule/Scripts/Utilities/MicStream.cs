// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Utilities
{
    public class MicStream
    {
        // This class replaces Unity's Microphone object.
        // This class is made for HoloLens mic stream selection but should work well on all Windows 10 devices.
        // Choose from one of three possible microphone modes on HoloLens.
        // There is an example of how to use this script in MixedRealityToolkit-Examples\Input\Scripts\MicStreamDemo.cs.

        // Streams: LOW_QUALITY_VOICE is optimized for speech analysis.
        //          COMMUNICATIONS is higher quality voice and is probably preferred.
        //          ROOM_CAPTURE tries to get the sounds of the room more than the voice of the user.
        // This can only be set on initialization.
        public enum StreamCategory { LOW_QUALITY_VOICE, HIGH_QUALITY_VOICE, ROOM_CAPTURE }

        public enum ErrorCodes { ALREADY_RUNNING = -10, NO_AUDIO_DEVICE, NO_INPUT_DEVICE, ALREADY_RECORDING, GRAPH_NOT_EXIST, CHANNEL_COUNT_MISMATCH, FILE_CREATION_PERMISSION_ERROR, NOT_ENOUGH_DATA, NEED_ENABLED_MIC_CAPABILITY };

        const int MAX_PATH = 260; // 260 is maximum path length in windows, to be returned when we MicStopRecording

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] // If included in MicStartStream, this callback will be triggered when audio data is ready. This is not the preferred method for Game Engines and can probably be ignored.
        public delegate void LiveMicCallback();

        /// <summary>
        /// Called before calling MicStartStream or MicstartRecording to initialize microphone
        /// </summary>
        /// <param name="category">One of the entries in the StreamCategory enumeration</param>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicInitializeDefault(int category);

        /// <summary>
        /// Called before calling MicStartStream or MicstartRecording to initialize microphone
        /// </summary>
        /// <param name="category">One of the entries in the StreamCategory enumeration</param>
        /// <param name="samplerate">Desired number of samples per second</param>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicInitializeCustomRate(int category, int samplerate);

        /// <summary>
        /// Call this to start receiving data from a microphone. Then, each frame, call MicGetFrame.
        /// </summary>
        /// <param name="keepData">If true, all data will stay in the queue, if the client code is running behind. This can lead to significant audio lag, so is not appropriate for low-latency situations like real-time voice chat.</param>
        /// <param name="previewOnDevice">If true, the audio from the microphone will be played through your speakers.</param>
        /// <param name="micsignal">Optional (can be null): This callback will be called when data is ready for MicGetFrame</param>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicStartStream(bool keepData, bool previewOnDevice, LiveMicCallback micsignal);

        /// <summary>
        /// Call this to start receiving data from a microphone. Then, each frame, call MicGetFrame.
        /// </summary>
        /// <param name="keepData">If true, all data will stay in the queue, if the client code is running behind. This can lead to significant audio lag, so is not appropriate for low-latency situations like real-time voice chat.</param>
        /// <param name="previewOnDevice">If true, the audio from the microphone will be played through your speakers.</param>
        /// <returns>error code or 0</returns>
        public static int MicStartStream(bool keepData, bool previewOnDevice)
        {
            return MicStartStream(keepData, previewOnDevice, null);
        }

        /// <summary>
        /// Shuts down the connection to the microphone. Data will not longer be received from the microphone.
        /// </summary>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicStopStream();

        /// <summary>
        /// Begins recording microphone data to the specified file.
        /// </summary>
        /// <param name="filename">The file will be saved to this name. Specify only the wav file's name with extensions, aka "myfile.wav", not full path</param>
        /// <param name="previewOnDevice">If true, will play mic stream in speakers</param>
        /// <returns></returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicStartRecording(string filename, bool previewOnDevice);

        /// <summary>
        /// Finishes writing the file recording started with MicStartRecording.
        /// </summary>
        /// <param name="sb">returns the full path to the recorded audio file</param>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern void MicStopRecording(StringBuilder sb);

        /// <summary>
        /// Finishes writing the file recording started with MicStartRecording.
        /// </summary>
        /// <returns>the full path to the recorded audio file</returns>
        public static string MicStopRecording()
        {
            StringBuilder builder = new StringBuilder(MAX_PATH); 
            MicStopRecording(builder);
            return builder.ToString();
        }

        /// <summary>
        /// Cleans up data associated with microphone recording. Counterpart to MicInitialize*
        /// </summary>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicDestroy();

        /// <summary>
        /// Pauses streaming of microphone data to MicGetFrame (and/or file specified with MicStartRecording)
        /// </summary>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]

        public static extern int MicPause();

        /// <summary>
        /// Unpauses streaming of microphone data to MicGetFrame (and/or file specified with MicStartRecording)
        /// </summary>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicResume();

        /// <summary>
        /// Sets amplification factor for microphone samples returned by MicGetFrame (and/or file specified with MicStartRecording)
        /// </summary>
        /// <param name="g">gain factor</param>
        /// <returns>error code or 0</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicSetGain(float g);

        /// <summary>
        /// Queries the default microphone audio frame sample size. Useful if doing default initializations with callbacks to know how much data it wants to hand you.
        /// </summary>
        /// <returns>the number of samples in the default audio buffer</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicGetDefaultBufferSize();

        /// <summary>
        /// Queries the number of channels supported by the microphone.  Useful if doing default initializations with callbacks to know how much data it wants to hand you.
        /// </summary>
        /// <returns>the number of channels</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        private static extern int MicGetDefaultNumChannels();

        /// <summary>
        /// Read from the microphone buffer. Usually called once per frame.
        /// </summary>
        /// <param name="buffer">the buffer into which to store the microphone audio samples</param>
        /// <param name="length">the length of the buffer</param>
        /// <param name="numchannels">the number of audio channels to store in the buffer</param>
        /// <returns>error code (or 0 if no error)</returns>
        [DllImport("MicStreamSelector", ExactSpelling = true)]
        public static extern int MicGetFrame(float[] buffer, int length, int numchannels);

        /// <summary>
        /// Prints useful error/warning messages based on error codes returned from the functions in this class
        /// </summary>
        /// <param name="returnCode">An error code returned by another function in this class</param>
        /// <returns>True if no error or warning message was printed, false if a message was printed</returns>
        public static bool CheckForErrorOnCall(int returnCode)
        {
            switch (returnCode)
            {
                case (int)ErrorCodes.ALREADY_RECORDING:
                    Debug.LogWarning("WARNING: Tried to start recording when you were already doing so. You need to stop your previous recording before you can start again.");
                    return false;
                case (int)ErrorCodes.ALREADY_RUNNING:
                    Debug.LogWarning("WARNING: Tried to initialize microphone more than once");
                    return false;
                case (int)ErrorCodes.GRAPH_NOT_EXIST:
                    Debug.LogError("ERROR: Tried to do microphone things without a properly initialized microphone. \n Do you have a mic plugged into a functional audio system and did you call MicInitialize() before anything else ??");
                    return false;
                case (int)ErrorCodes.NO_AUDIO_DEVICE:
                    Debug.LogError("ERROR: Tried to start microphone, but you don't appear to have a functional audio device. check your OS audio settings.");
                    return false;
                case (int)ErrorCodes.NO_INPUT_DEVICE:
                    Debug.LogError("ERROR: Tried to start microphone, but you don't have one plugged in, do you?");
                    return false;
                case (int)ErrorCodes.CHANNEL_COUNT_MISMATCH:
                    Debug.LogError("ERROR: Microphone had a channel count mismatch internally on device. Try setting different mono/stereo options in OS mic settings.");
                    return false;
                case (int)ErrorCodes.FILE_CREATION_PERMISSION_ERROR:
                    Debug.LogError("ERROR: Didn't have access to create file in Music library. Make sure permissions to write to Music library are set granted.");
                    return false;
                case (int)ErrorCodes.NOT_ENOUGH_DATA:
                    // usually not an error, means the device hasn't produced enough data yet because it just started running
                    return false;
                case (int)ErrorCodes.NEED_ENABLED_MIC_CAPABILITY:
                    Debug.LogError("ERROR: Seems like you forgot to enable the microphone capabilities in your Unity permissions");
                    return false;
            }
            return true;
        }
    }
}
