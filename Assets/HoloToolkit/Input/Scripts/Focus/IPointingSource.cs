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
    public interface IPointingSource
    {
        bool InteractionEnabled { get; }

        float? ExtentOverride { get; }

        [Obsolete("Will be removed in a later version. For equivalent behavior return a RayStep array with a single element.")]
        Ray Ray { get; }

        RayStep[] Rays { get; }

        LayerMask[] PrioritizedLayerMasksOverride { get; }

        PointerResult Result { get; set; }

        void OnPreRaycast();

        void OnPostRaycast();

        bool OwnsInput(BaseEventData eventData);
    }
}
