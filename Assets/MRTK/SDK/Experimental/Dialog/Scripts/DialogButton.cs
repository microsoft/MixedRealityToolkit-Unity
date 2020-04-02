// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dialog
{
    /// <summary>
    /// Handling click event and dismiss dialog
    /// </summary>
    public class DialogButton : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro buttonText;
        public TextMeshPro ButtonText
        {
            get => buttonText;
            set => buttonText = value;
        }

        private DialogShell parentDialog;
        /// <summary>
        /// A reference to the Dialog that this button is on.
        /// </summary>
        public DialogShell ParentDialog
        {
            get => parentDialog;
            set => parentDialog = value;
        }

        /// <summary>
        /// The type description of the button
        /// </summary>
        public DialogButtonType ButtonTypeEnum;

        /// <summary>
        /// event handler that runs when button is clicked.
        /// Dismisses the parent dialog.
        /// </summary>
        /// <param name="obj">Caller GameObject</param>
        public void OnButtonClicked(GameObject obj)
        {
            if (parentDialog != null)
            {
                parentDialog.Result.Result = ButtonTypeEnum;
                parentDialog.DismissDialog();
            }
        }

        /// <summary>
        /// Setter Method to set the Text at the top of the Dialog.
        /// </summary>
        /// <param name="title">Title of the button</param>
        public void SetTitle(string title)
        {
            if (ButtonText)
            {
                ButtonText.text = title;
            }
        }
    }
}