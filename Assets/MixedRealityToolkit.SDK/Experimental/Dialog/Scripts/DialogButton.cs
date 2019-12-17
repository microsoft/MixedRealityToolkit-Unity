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
        //private Button buttonComponent;
        public TextMeshPro buttonText;

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
        public DialogButtonType ButtonTypeEnum;

        private void OnEnable()
        {
            //buttonComponent = GetComponent<Button>();
            //buttonComponent.OnButtonClicked += OnButtonClicked;
        }

        private void OnDisable()
        {
            //if (buttonComponent != null)
            //{
            //    buttonComponent.OnButtonClicked -= OnButtonClicked;
            //}
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
            if (buttonText)
            {
                buttonText.text = title;
            }
        }
    }
}