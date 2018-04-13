// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.InputHandlers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Focus
{
    public interface ITeleportTarget : IFocusChangedHandler
    {
        Vector3 Position { get; }
        Vector3 Normal { get; }
        bool IsActive { get; }
        bool OverrideTargetOrientation { get; }
        float TargetOrientation { get; }
    }
}