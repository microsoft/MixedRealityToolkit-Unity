// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// How does the Tooltip rotate about the connector
    /// </summary>
    public enum ConnectorOrientType
    {
        OrientToObject = 0,     // Tooltip will maintain anchor-pivot relationship relative to target object
        OrientToCamera,     // Tooltip will maintain anchor-pivot relationship relative to camera
    }
}