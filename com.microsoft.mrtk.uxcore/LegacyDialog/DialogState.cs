// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Describes the current state of a Dialog.
    /// </summary>
    [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
    public enum DialogState
    {
        Uninitialized = 0,
        Opening,
        WaitingForInput,
        Closed
    }
}