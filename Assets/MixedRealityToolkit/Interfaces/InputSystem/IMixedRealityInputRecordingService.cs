// Copyright (c) Microsoft Corporation. All rights reserved.
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
        bool UseBufferTimeLimit { get; }

        /// <summary>
        /// Size of the input recording buffer.
        /// </summary>
        float RecordingBufferTimeLimit { get; }

        /// <summary>
        /// Start unlimited input recording.
        /// </summary>
        void StartRecording(bool useTimeLimit = false);

        /// <summary>
        /// Start limited input recording.
        /// </summary>
        /// <param name="bufferTimeLimit">Time limit after which to discard keyframes.</param>
        void StartRecording(float bufferTimeLimit);

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
        /// <remarks>
        /// Filename is determined automatically.
        /// </remarks>
        void ExportRecordedInput();

        /// <summary>
        /// Export recorded input animation to a file.
        /// </summary>
        void ExportRecordedInput(string filename);
    }
}
