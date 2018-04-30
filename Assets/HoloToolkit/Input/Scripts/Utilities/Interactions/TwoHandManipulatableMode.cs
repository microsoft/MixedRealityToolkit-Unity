// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace HoloToolkit.Unity.InputModule.Utilities.Interactions
{
    /// <summary>
    /// Sorting type for collections
    /// </summary>
    [Flags]
    public enum TwoHandManipulatableMode
    {
        Scale = (1 << 0),
        Rotate = (1 << 1),
        MoveScale = (1 << 2),
        RotateScale = (1 << 3),
        MoveRotateScale = (1 << 4),
    }
}
