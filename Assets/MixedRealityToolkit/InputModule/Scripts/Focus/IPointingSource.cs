// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.Focus
{
    /// <summary>
    /// Implement this interface to register your pointer as a pointing source. This could be gaze based or motion controller based.
    /// </summary>
    public interface IPointingSource
    {
        bool InteractionEnabled { get; }

        float? ExtentOverride { get; }

        RayStep[] Rays { get; }

        LayerMask[] PrioritizedLayerMasksOverride { get; }

        PointerResult Result { get; set; }

        void OnPreRaycast();

        void OnPostRaycast();

        bool OwnsInput(BaseEventData eventData);

        bool FocusLocked { get; set; }
    }
}
