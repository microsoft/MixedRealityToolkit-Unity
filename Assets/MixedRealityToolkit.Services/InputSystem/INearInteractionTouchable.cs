// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface INearInteractionTouchable
    {
        Vector3 LocalCenter { get; }
        Vector3 LocalForward { get; }
        Vector3 LocalUp { get; }
        Vector3 LocalRight { get; }

        Vector3 Forward { get; }
        Vector2 Bounds { get; }
        Transform transform { get; }
    }
}
