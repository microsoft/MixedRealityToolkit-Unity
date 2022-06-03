// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Describes the current state of a Dialog.
    /// </summary>
    public enum DialogState
    {
        Uninitialized = 0,
        Opening,
        WaitingForInput,
        Closed
    }
}