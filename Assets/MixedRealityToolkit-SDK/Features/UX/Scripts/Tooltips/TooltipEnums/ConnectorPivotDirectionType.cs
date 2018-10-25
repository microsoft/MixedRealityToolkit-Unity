// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// In which direction does the tooltip connector project.
    /// </summary>
    public enum ConnectorPivotDirectionType
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