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
        AnchorOnly = 0x0,   // The anchor will follow the target - pivot remains unaffected
        Position   = 0x1,   // Anchor and pivot will follow target position, but not rotation
        YRotation  = 0x2,   // Anchor and pivot will follow target like it's parented, but only on Y axis
        XRotation  = 0x4,   // Anchor and pivot will follow target like it's parented
    }
}