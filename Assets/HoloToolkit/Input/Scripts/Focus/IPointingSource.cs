// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Implement this interface to register your pointer as a pointing source. This could be gaze based or motion controller based.
    /// </summary>
    public interface IPointingSource : IFocuser
    {
        float? ExtentOverride { get; }

        RayStep[] Rays { get; }

        LayerMask[] PrioritizedLayerMasksOverride { get; }

        Cursor CursorOverride { get; }

        void OnPreRaycast();

        void OnPostRaycast();
    }
}
