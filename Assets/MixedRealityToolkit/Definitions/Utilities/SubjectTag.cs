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
    public enum SubjectTag
    {
        Input = 1,
        Pointers = 2,
        Buttons = 4,
        Hands = 8,
        Scenes = 16,
        Editor = 32,
        Camera = 64,
        Speech = 128,
        Simulation = 256,
        Prefabs = 512,

        All = Input | Pointers | Buttons | Hands | Scenes | Editor | Camera | Simulation | Speech | Prefabs
    }
}