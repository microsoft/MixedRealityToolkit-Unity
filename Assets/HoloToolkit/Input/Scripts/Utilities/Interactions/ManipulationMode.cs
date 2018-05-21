// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace HoloToolkit.Unity.InputModule.Utilities.Interactions
{
    /// <summary>
    /// Sorting type for collections
    /// </summary>
    [Flags]
    public enum ManipulationMode
    {
        None = 0,
        Move = 1 << 0,
        Scale = 1 << 1,
        Rotate = 1 << 2,
        MoveAndScale = Move | Scale,
        MoveAndRotate = Move | Rotate,
        RotateAndScale = Rotate | Scale,
        MoveScaleAndRotate = Move | Scale | Rotate,
    }
}
