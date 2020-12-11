// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The style (caption) of button on a Dialog.
    /// </summary>
    [Flags]
    public enum DialogButtonType
    {
        None = 0 << 0,
        Close = 1 << 0,
        Confirm = 1 << 1,
        Cancel = 1 << 2,
        Accept = 1 << 3,
        Yes = 1 << 4,
        No = 1 << 5,
        OK = 1 << 6
    }
}