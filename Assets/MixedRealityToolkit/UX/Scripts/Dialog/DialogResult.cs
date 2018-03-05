// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.UX.Dialog
{
    public class DialogResult
    {
        /// <summary>
        /// The button press that closed the dialog
        /// </summary>
        private Dialog.ButtonTypeEnum result = Dialog.ButtonTypeEnum.Close;

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
        private Dialog.ButtonTypeEnum buttons = Dialog.ButtonTypeEnum.Close;

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

        public Dialog.ButtonTypeEnum Buttons
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

        public Dialog.ButtonTypeEnum Result
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

