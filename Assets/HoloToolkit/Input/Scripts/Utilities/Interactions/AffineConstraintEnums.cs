// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule.Utilities.Interactions
{
    /// <summary>
    /// Enum for Axis Constraints
    /// </summary>
    public enum AxisConstraint
    {
        /// <summary>
        /// Transform all axes.
        /// </summary>
        None,
        /// <summary>
        /// Constrain to X-Axis.
        /// </summary>
        XAxisOnly,
        /// <summary>
        /// Constrain to Y-Axis.
        /// </summary>
        YAxisOnly,
        /// <summary>
        /// Constrain to Z-Axis.
        /// </summary>
        ZAxisOnly
    };
}