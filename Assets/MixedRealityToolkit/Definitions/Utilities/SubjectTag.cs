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
        Profiles = 1,
        Focus = 2,
        Pointers = 4,
        Hands = 8,
        Input = 16,
        Editor = 32,

        All = Profiles | Focus | Pointers | Hands | Input | Editor,
    }
}