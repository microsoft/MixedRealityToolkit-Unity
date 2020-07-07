// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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