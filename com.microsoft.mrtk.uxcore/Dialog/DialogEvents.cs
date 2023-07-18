// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Every dialog event emits these parameters.
    /// </summary>
    public abstract class BaseDialogEventArgs
    {
        /// <summary>
        /// A reference to the <see cref="Dialog"/> that
        /// emitted this event.
        /// </summary>
        public IDialog Dialog { get; set; }
    }

    /// <summary>
    /// Event arguments used when buttons in a <see cref="Microsoft.MixedReality.Toolkit.UX.IDialog">IDialog</see>
    /// are clicked or activated.
    /// </summary>
    public class DialogButtonEventArgs : BaseDialogEventArgs
    {
        /// <summary>
        /// The semantic type of the button that generated this event.
        /// </summary>
        /// <remarks>
        /// Custom dialogs may specify additional button types. In such as case, the <see cref="ButtonType"/> value
        /// should be <see cref="DialogButtonType.Other"/> and the event receiver should cast the event arguments 
        /// to a custom type so to obtain more specific information about the action.
        /// </remarks>
        public DialogButtonType ButtonType { get; set; }

        /// <summary>
        /// The text on the button that generated this event.
        /// </summary>
        public string ButtonText { get; set; }
    }

    /// <summary>
    /// Event arguments used a <see cref="Microsoft.MixedReality.Toolkit.UX.IDialog">IDialog</see>
    /// is dismissed or closed.
    /// </summary>
    public class DialogDismissedEventArgs : BaseDialogEventArgs
    {
        /// <summary>
        /// If the user selected an option, this is the result
        /// of the associated DialogButtonEvent. Null otherwise.
        /// </summary>
        public DialogButtonEventArgs Choice { get; set; }
    }
}