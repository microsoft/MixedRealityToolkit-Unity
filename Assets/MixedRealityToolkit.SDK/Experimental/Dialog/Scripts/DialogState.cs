using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dialog
{
    /// <summary>
    /// Enum that describes the current state of a Dialog.
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