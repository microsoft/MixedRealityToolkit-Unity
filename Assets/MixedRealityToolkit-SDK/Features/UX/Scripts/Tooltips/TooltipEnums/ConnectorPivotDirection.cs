// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// In which direction does the tooltip connector project.
    /// </summary>
    public enum ConnectorPivotDirection
    {
        /// <summary>
        /// Direction will be specified manually
        /// </summary>
        Manual = 0,
        North,
        Northeast,
        East,
        Southeast,
        South,
        Southwest,
        West,
        Northwest,
        InFront,
    }
}