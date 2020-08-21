// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Controls how the tooltip will pivot relative to the camera/other objects.
    /// </summary>
    public enum ConnectorPivotMode
    {
        /// <summary>
        /// Tooltip pivot will be set manually
        /// </summary>
        Manual = 0,
        /// <summary>
        /// Tooltip pivot will be set relative to object/camera based on specified direction and line length
        /// </summary>
        Automatic,
        /// <summary>
        /// Tooltip pivot will be set relative to target based on a local position
        /// </summary>
        LocalPosition,
    }
}