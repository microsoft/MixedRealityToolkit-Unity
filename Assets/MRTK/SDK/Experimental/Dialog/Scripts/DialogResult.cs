// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Experimental.Dialog
{
    public class DialogResult
    {
        /// <summary>
        /// The public property to get and set the Title
        /// string (topmost) on the Dialog.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The public property to get and set the Message
        /// string of the dialog.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Property defining the button type[s]
        /// on the dialog.
        /// </summary>
        public DialogButtonType Buttons { get; set; } = DialogButtonType.Close;

        /// <summary>
        /// Property reporting the Result of the Dialog:
        /// Which button was clicked to dismiss it.
        /// </summary>
        public DialogButtonType Result { get; set; } = DialogButtonType.Close;

        /// <summary>
        /// The public property to get and set the variable
        /// object of the dialog
        /// </summary>
        public object Variable { get; set; }
    }
}
