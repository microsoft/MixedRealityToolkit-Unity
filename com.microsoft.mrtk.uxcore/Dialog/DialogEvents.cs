// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// DialogButtonEvents are fired when the user makes a selection on a dialog.
    /// </summary>
    [Serializable]
    public sealed class DialogButtonEvent : UnityEvent<DialogButtonEventArgs> { }

    /// <summary>
    /// Every dialog event emits these parameters.
    /// </summary>
    public abstract class BaseDialogEventArgs
    {
        public Dialog Dialog { get; set; }
    }

    /// <summary>
    /// Button events emit these parameters.
    /// </summary>
    public class DialogButtonEventArgs : BaseDialogEventArgs
    {
        public DialogButtonType ButtonType { get; set; }

        public string ButtonText { get; set; }
    }

}