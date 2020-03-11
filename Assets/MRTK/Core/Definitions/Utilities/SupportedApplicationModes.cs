// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The supported Application modes for specific features.
    /// </summary>
    /// <remarks>
    /// This enum can be used to configure specific features to have differing behaviors when run in editor.
    /// </remarks>
    [Flags]
    public enum SupportedApplicationModes
    {
        /// <summary>
        /// This indicates that the feature is relevant in editor scenarios.
        /// </summary>
        Editor = 1 << 0,

        /// <summary>
        /// This indicates that the feature is relevant in player scenarios.
        /// </summary>
        Player = 1 << 1,
    }
}