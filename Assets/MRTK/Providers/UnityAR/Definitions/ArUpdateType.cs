// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    /// <summary>
    /// Enumeration defining when, during frame processing, the tracked pose will be sampled.
    /// </summary>
    public enum ArUpdateType
    {
        /// <summary>
        /// Sampling occurs during update and just before rendering. This is the recommended value for smooth tracking.
        /// </summary>
        UpdateAndBeforeRender = 0,

        /// <summary>
        /// Sampling occurs during update.
        /// </summary>
        Update = 1,

        /// <summary>
        /// Sampling occurs just before rendering.
        /// </summary>
        BeforeRender = 2
    }
}
