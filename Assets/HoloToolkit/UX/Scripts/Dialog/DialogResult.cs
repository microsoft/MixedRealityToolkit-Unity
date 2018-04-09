// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.UX.Dialog
{
    public class DialogResult
    {
        /// <summary>
        /// The button press that closed the dialog
        /// </summary>
        private DialogButtonType result = DialogButtonType.Close;

        /// <summary>
        /// Title for the dialog to display
        /// </summary>
        private string title = string.Empty;

        /// <summary>
        /// Message for the dialog to display
        /// </summary>
        private string message = string.Empty;

        /// <summary>
        /// Which buttons to generate
        /// </summary>
        private DialogButtonType buttons = DialogButtonType.Close;

        /// <summary>
        /// The public property to get and set the Title
        /// string (topmost) on the Dialog.
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        /// <summary>
        /// The public property to get and set the Message
        /// string of the dialog.
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        /// <summary>
        /// Property defining the button type[s]
        /// on the dialog.
        /// </summary>
        public DialogButtonType Buttons
        {
            get
            {
                return buttons;
            }

            set
            {
                buttons = value;
            }
        }

        /// <summary>
        /// Property reporting the Result of the Dialog:
        /// Which button was clicked to dismiss it.
        /// </summary>
        public DialogButtonType Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }
}

