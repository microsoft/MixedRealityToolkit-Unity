// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        /// <summary>
        /// A reference to the Dialog that this button is on.
        /// </summary>
        public DialogShell ParentDialog { get; set; }

        /// <summary>
        /// The type description of the button
        /// </summary>
        public DialogButtonType ButtonTypeEnum;

        /// <summary>
        /// Event handler that runs when button is clicked.
        /// Dismisses the parent dialog.
        /// </summary>
        /// <param name="obj">Caller GameObject</param>
        public void OnButtonClicked(GameObject obj)
        {
            if (ParentDialog != null)
            {
                ParentDialog.Result.Result = ButtonTypeEnum;
                ParentDialog.DismissDialog();
            }
        }

        /// <summary>
        /// Setter method to set the text at the top of the Dialog.
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