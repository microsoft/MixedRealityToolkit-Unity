// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides input recording into an internal buffer and exporting to files.
    /// </summary>
    public interface IMixedRealityInputRecordingService : IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// True if input is being recorded.
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        /// Limit the size of the recording buffer.
        /// </summary>
        /// <remarks>
        /// If recording is limited any input older than the RecordingBufferTimeLimit will be discarded.
        /// </remarks>
        bool UseBufferTimeLimit { get; set; }

        /// <summary>
        /// Maximum duration in seconds of the input recording if UseBufferTimeLimit is enabled.
        /// </summary>
        /// <remarks>
        /// If UseBufferTimeLimit is enabled then keyframes older than this limit will be discarded.
        /// </remarks>
        float RecordingBufferTimeLimit { get; set; }

        /// <summary>
        /// Start unlimited input recording.
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Stop recording input.
        /// </summary>
        void StopRecording();

        /// <summary>
        /// Discard all recorded input
        /// </summary>
        void DiscardRecordedInput();

        /// <summary>
        /// Save recorded input animation to a file.
        /// </summary>
        /// <param name="directory">Directory in which to create the file. If null the persistent data path of the app is used.</param>
        /// <returns>File path where input has been recorded.</returns>
        /// <remarks>
        /// Filename is determined automatically.
        /// </remarks>
        string SaveInputAnimation(string directory = null);

        /// <summary>
        /// Save recorded input animation to a file.
        /// </summary>
        /// <param name="filename">Name of the file to create.</param>
        /// <param name="directory">Directory in which to create the file. If null the persistent data path of the app is used.</param>
        /// <returns>File path where input has been recorded.</returns>
        string SaveInputAnimation(string filename, string directory = null);
    }
}
