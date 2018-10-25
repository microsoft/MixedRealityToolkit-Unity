// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// How does the Tooltip track with its parent object
    /// </summary>
    [Flags]
    public enum ConnectorFollowType
    {
        /// <summary>
        /// The anchor will follow the target - pivot remains unaffected
        /// </summary>
        AnchorOnly = 0x0,
        /// <summary>
        /// Anchor and pivot will follow target position, but not rotation
        /// </summary>
        Position = 0x1,
        /// <summary>
        /// Anchor and pivot will follow target like it's parented, but only on Y axis
        /// </summary>
        YRotation = 0x2,
        /// <summary>
        /// Anchor and pivot will follow target like it's parented
        /// </summary>
        XRotation = 0x4,
    }
}