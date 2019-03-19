// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
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
        /// Details about the currently focused <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>.
        /// </summary>
        FocusDetails Details { get; }

        /// <summary>
        /// The current pointer's target <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>
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