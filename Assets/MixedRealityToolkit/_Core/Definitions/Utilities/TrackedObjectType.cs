// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities
{
    /// <summary>
    /// Defines the types of automatic objects that can be tracked
    /// </summary>
    public enum TrackedObjectType
    {
        /// <summary>
        /// Do not automatically track an object, use the manual reference instead.
        /// </summary>
        None = 0,
        /// <summary>
        /// Calculates position and orientation from the main camera.
        /// </summary>
        Head,
        /// <summary>
        /// Calculates position and orientation from the left tracked controller.
        /// </summary>
        LeftController,
        /// <summary>
        /// Calculates position and orientation from the right tracked controller.
        /// </summary>
        RightController
    }
}