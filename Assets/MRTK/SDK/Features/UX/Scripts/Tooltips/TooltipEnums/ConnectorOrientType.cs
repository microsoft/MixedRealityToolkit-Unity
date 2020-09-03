// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UI
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