// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.UX.AppBarControl
{
    [Flags]
    public enum ButtonTypeEnum
    {
        Custom = 0,
        Remove = 1,
        Adjust = 2,
        Hide = 4,
        Show = 8,
        Done = 16
    }
}