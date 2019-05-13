// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Types of changes that occur for a <see cref="SynchronizedTransform"/>
    /// </summary>
    [Flags]
    public enum SynchronizedTransformChangeType : byte
    {
        Position = 0x1,
        Rotation = 0x2,
        Scale = 0x4,
        Parent = 0x8,
        Name = 0x10,
        IsActive = 0x20,
        RectTransform = 0x40,
        Layer = 0x80,
    }
}
