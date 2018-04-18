// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Dialog;
using UnityEngine;

namespace HoloToolkit.UX.Buttons
{
    /// <summary>
    /// Handling click event and dismiss dialog
    /// </summary>
    public class DialogButton : MonoBehaviour
    {
        private Button buttonComponent;

        private DialogShell parentDialog;

        /// <summary>
        /// A reference to the Dialog that this button is on.
        /// </summary>
        public DialogShell ParentDialog
        {
            get
            {
                return parentDialog;
            }
            set
            {
                parentDialog = value;
            }
        }

        /// <summary>
        /// The type description of the button
        /// </summary>
        public Dialog.DialogButtonType ButtonTypeEnum;

        private void OnEnable()
        {
            buttonComponent = GetComponent<Button>();
            buttonComponent.OnButtonClicked += OnButtonClicked;
        }

        private void OnDisable()
        {
            if (buttonComponent != null)
            {
                buttonComponent.OnButtonClicked -= OnButtonClicked;
            }
        }

        /// <summary>
        /// event handler that runs when button is clicked.
        /// Dismisses the parent dialog.
        /// </summary>
        /// <param name="obj"></param>
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
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            CompoundButtonText compoundButtonText = GetComponent<CompoundButtonText>();
            if (compoundButtonText)
            {
                compoundButtonText.Text = title;
            }
        }
    }
}