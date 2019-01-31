// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Options for how the spatial mesh is to be displayed by the spatial awareness system.
    /// </summary>
    public enum SpatialAwarenessMeshDisplayOptions
    {
        /// <summary>
        /// Do not display the spatial mesh
        /// </summary>
        None = 0,

        /// <summary>
        /// Display the spatial mesh using the configured material
        /// </summary>
        Visible,

        /// <summary>
        /// Display the spatial mesh using the configured occlusion material
        /// </summary>
        Occlusion
    }
}