// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implements the Gaze Provider for an Input Source.
    /// </summary>
    public interface IMixedRealityInputRecordingService
    {
        /// <summary>
        /// Size of the input recording buffer.
        /// </summary>
        /// <remarks>
        /// Any input older than this time span will be discarded.
        /// </remarks>
        float RecordingBufferLength { get; set; }

        /// <summary>
        /// Start recording input.
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
        /// <param name="filename">Name of the file to export to.</param>
        /// <param name="appendTimestamp">Append the current time to the file name.</param>
        void ExportRecordedInput(Ray eyeRay, bool appendTimestamp = true);
    }
}
