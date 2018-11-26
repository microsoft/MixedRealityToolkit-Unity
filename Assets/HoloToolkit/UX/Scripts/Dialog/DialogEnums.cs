// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.UX.Dialog
{
    /// <summary>
    /// Enum that describes the current state of a Dialog.
    /// </summary>
    public enum DialogState
    {
        Uninitialized,
        Opening,
        WaitingForInput,
        InputReceived,
        Closing,
        Closed,
    }
}
