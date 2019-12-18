using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dialog
{
    public class DialogShell : Dialog
    {
        private GameObject[] twoButtonSet;

        /// <summary>
        /// This is called after the buttons are generated and
        /// the title and message have been set.
        /// Perform here any operations that you'd like
        /// </summary>
        protected override void FinalizeLayout()
        {
        }

        protected override void GenerateButtons()
        {
            //Get List of ButtonTypes that should be created on Dialog
            List<DialogButtonType> buttonTypes = new List<DialogButtonType>();
            foreach (DialogButtonType buttonType in Enum.GetValues(typeof(DialogButtonType)))
            {
                if (buttonType == DialogButtonType.None)
                {
                    continue;
                }

                // If this button type flag is set
                if ((buttonType & result.Buttons) == buttonType)
                {
                    buttonTypes.Add(buttonType);
                }
            }

            twoButtonSet = new GameObject[2];

            //Find all buttons on dialog...
            List<DialogButton> buttonsOnDialog = GetAllDialogButtons();

            //set desired buttons active and the rest inactive
            SetButtonsActiveStates(buttonsOnDialog, buttonTypes.Count);

            //set titles and types
            if (buttonTypes.Count > 0)
            {
                // If we have two buttons then do step 1, else 0
                int step = buttonTypes.Count == 2 ? 1 : 0;
                for (int i = 0; i < buttonTypes.Count; ++i)
                {
                    twoButtonSet[i] = buttonsOnDialog[i + step].gameObject;
                    buttonsOnDialog[i + step].SetTitle(buttonTypes[i].ToString());
                    buttonsOnDialog[i + step].ButtonTypeEnum = buttonTypes[i];
                }
            }
        }

        private void SetButtonsActiveStates(List<DialogButton> buttons, int count)
        {
            for (int i = 0; i < buttons.Count; ++i)
            {
                var flag1 = (count == 1) && (i == 0);
                var flag2 = (count == 2) && (i > 0);
                buttons[i].ParentDialog = this;
                buttons[i].gameObject.SetActive(flag1 || flag2);
            }
        }

        private List<DialogButton> GetAllDialogButtons()
        {
            List<DialogButton> buttonsOnDialog = new List<DialogButton>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == "ButtonParent")
                {
                    var buttons = child.GetComponentsInChildren<DialogButton>();
                    if (buttons != null)
                    {
                        buttonsOnDialog.AddRange(buttons);
                    }
                }
            }
            return buttonsOnDialog;
        }

        /// <summary>
        /// Set Title and Text on the Dialog.
        /// </summary>
        protected override void SetTitleAndMessage()
        {
            var textContentChild = transform.GetChild(0);
            foreach (Transform child in textContentChild)
            {
                if (child != null && child.name == "TitleText")
                {
                    if (child.GetComponent<TextMeshPro>()) {
                        child.GetComponent<TextMeshPro>().text = Result.Title;
                    }
                }
                else if (child != null && child.name == "Description")
                {
                    if (child.GetComponent<TextMeshPro>())
                    {
                        child.GetComponent<TextMeshPro>().text = Result.Message;
                    }
                }
            }
        }

        /// <summary>
        /// Function to destroy the Dialog.
        /// </summary>
        public void DismissDialog()
        {
            state = DialogState.InputReceived;
        }

    }
}