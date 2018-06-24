// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    public interface IPointerResult
    {
        Vector3 StartPoint { get; }

        FocusDetails Details { get; }

        GameObject CurrentPointerTarget { get; }

        GameObject PreviousPointerTarget { get; }

        /// <summary>
        /// The index of the step that produced the last raycast hit
        /// 0 when no raycast hit
        /// </summary>
        int RayStepIndex { get; }
    }
}