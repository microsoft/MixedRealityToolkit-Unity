// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    /// <summary>
    /// Interface to allow multiple parsers to attempt to parse calibration data.
    /// </summary>
    public interface ICalibrationParser
    {
        /// <summary>
        /// Tries to parse the provided string as calibration data.
        /// </summary>
        /// <param name="rawCalibrationData">The string representation of calibration data to deserialize.</param>
        /// <param name="calibrationData">The parsed calibration data, which can be used to configure a stereo camera rig.</param>
        /// <returns>True if the string was successfully parsed, otherwise false.</returns>
        bool TryParse(string rawCalibrationData, out ICalibrationData calibrationData);
    }
}