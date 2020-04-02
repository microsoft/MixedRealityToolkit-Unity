// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Experimental.Dialog
{
    /// <summary>
    /// Describes the current state of a Dialog.
    /// </summary>
    public enum DialogState
    {
        Uninitialized = 0,
        Opening,
        WaitingForInput,
        InputReceived,
        Closing,
        Closed,
    }
}