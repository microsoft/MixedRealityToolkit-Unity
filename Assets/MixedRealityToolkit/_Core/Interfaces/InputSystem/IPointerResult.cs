// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem
{
    /// <summary>
    /// Interface defining a pointer result.
    /// </summary>
    public interface IPointerResult
    {
        /// <summary>
        /// The starting point of the Pointer RaySteps.
        /// </summary>
        Vector3 StartPoint { get; }

        /// <summary>
        /// Details about the currently focused <see cref="GameObject"/>.
        /// </summary>
        FocusDetails Details { get; }

        /// <summary>
        /// The current pointer's target <see cref="GameObject"/>
        /// </summary>
        GameObject CurrentPointerTarget { get; }

        /// <summary>
        /// The previous pointer target.
        /// </summary>
        GameObject PreviousPointerTarget { get; }

        /// <summary>
        /// The index of the step that produced the last raycast hit, 0 when no raycast hit.
        /// </summary>
        int RayStepIndex { get; }
    }
}