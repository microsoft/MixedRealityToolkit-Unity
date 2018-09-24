// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Controls how the tooltip will pivot relative to the camera/other objects.
    /// </summary>
    public enum ConnnectorPivotModeType
    {
        Manual = 0,     // Tooltip pivot will be set manually
        Automatic,      // Tooltip pivot will be set relative to object/camera based on specified direction and line length
    }
}