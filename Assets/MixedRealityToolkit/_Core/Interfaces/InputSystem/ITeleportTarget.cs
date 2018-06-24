// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    public interface ITeleportTarget : IMixedRealityFocusChangedHandler
    {
        Vector3 Position { get; }
        Vector3 Normal { get; }
        bool IsActive { get; }
        bool OverrideTargetOrientation { get; }
        float TargetOrientation { get; }
    }
}