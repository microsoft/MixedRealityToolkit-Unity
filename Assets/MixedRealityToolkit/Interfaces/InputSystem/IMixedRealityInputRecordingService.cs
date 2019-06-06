﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implements the Gaze Provider for an Input Source.
    /// </summary>
    public interface IMixedRealityInputRecordingService : IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Input is being recorded.
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
        /// Size of the input recording buffer.
        /// </summary>
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
        /// Export recorded input animation to a file.
        /// </summary>
        /// <param name="directory">Directory in which to create the file. If null the persistent data path of the app is used.</param>
        /// <returns>File path where input has been recorded.</returns>
        /// <remarks>
        /// Filename is determined automatically.
        /// </remarks>
        string ExportRecordedInput(string directory = null);

        /// <summary>
        /// Export recorded input animation to a file.
        /// </summary>
        /// <param name="filename">Name of the file to create.</param>
        /// <param name="directory">Directory in which to create the file. If null the persistent data path of the app is used.</param>
        /// <returns>File path where input has been recorded.</returns>
        string ExportRecordedInput(string filename, string directory = null);

        /// <summary>
        /// Generate a file name for export.
        /// </summary>
        string GenerateOutputFilename();
    }
}
