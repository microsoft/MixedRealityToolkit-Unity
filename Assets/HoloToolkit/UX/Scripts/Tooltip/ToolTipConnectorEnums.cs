// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace HoloToolkit.UX.ToolTips
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
    /// <summary>
    /// how does the tooltip rotate about the connector
    /// </summary>
    public enum ConnectorOrientType
    {
        OrientToObject = 0,     // Tooltip will maintain anchor-pivot relationship relative to target object
        OrientToCamera,     // Tooltip will maintain anchor-pivot relationship relative to camera
    }
    /// <summary>
    /// how is the pivot of the tooltip determined?
    /// </summary>
    public enum ConnnectorPivotMode
    {
        Manual = 0,         // Tooltip pivot will be set manually
        Automatic,      // Tooltip pivot will be set relative to object/camera based on specified direction and line length
    }
    /// <summary>
    /// In which direction does the tooltip connector project?
    /// </summary>
    public enum ConnectorPivotDirection
    {
        Manual = 0,         // Direction will be specified manually
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
        InFront,
    }
}