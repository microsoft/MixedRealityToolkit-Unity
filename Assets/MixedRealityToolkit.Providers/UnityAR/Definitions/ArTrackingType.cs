// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    /// <summary>
    /// Enumeration indicating the portion of the pose that will be used when tracking.
    /// </summary>
    public enum ArTrackingType
    {
        /// <summary>
        /// The pose rotation and position will be used.
        /// </summary>
        RotationAndPosition = 0,

        /// <summary>
        /// The pose rotation will be used.
        /// </summary>
        Rotation = 1,

        /// <summary>
        /// The pose rotation will be used.
        /// </summary>
        Position = 2
    }
}
