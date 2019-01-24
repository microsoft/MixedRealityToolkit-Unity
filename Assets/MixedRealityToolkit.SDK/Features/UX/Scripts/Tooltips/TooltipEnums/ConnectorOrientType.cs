// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// How does the Tooltip rotate about the connector
    /// </summary>
    public enum ConnectorOrientType
    {
        /// <summary>
        /// Tooltip will maintain anchor-pivot relationship relative to target object
        /// </summary>
        OrientToObject = 0,
        /// <summary>
        /// Tooltip will maintain anchor-pivot relationship relative to camera
        /// </summary>
        OrientToCamera,
    }
}