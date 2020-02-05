using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dialog
{
    /// <summary>
    /// Enum describing the style (caption) of button on a Dialog.
    /// </summary>
    [Flags]
    public enum DialogButtonType
    {
        None = 0,
        Close = 1,
        Confirm = 2,
        Cancel = 4,
        Accept = 8,
        Yes = 16,
        No = 32,
        OK = 64
    }
}