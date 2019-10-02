// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Used by SubjectAttribute and profile search system to categorize fields by subject.
    /// </summary>
    [Flags]
    [Serializable]
    public enum SubjectTag : ulong
    {
        Input = 1,
        Pointers = 2,
        Hands = 4,
        Scenes = 8,
        Editor = 16,
        Camera = 32,
        Speech = 64,
        Simulation = 128,
        Prefabs = 256,
        Controllers = 512,
        EyeTracking = 1024,
        Performance = 2048,
        Gestures = 4096,
        Spatial = 8192,
        Visuals = 16384,

        All = Input | Pointers | Hands |  Scenes | 
            Editor | Camera | Simulation | Speech | 
            Prefabs | Controllers | Performance | 
            EyeTracking | Gestures | Spatial | Visuals
    }
}