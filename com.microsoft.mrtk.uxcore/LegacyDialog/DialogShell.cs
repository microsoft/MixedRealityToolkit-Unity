// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// This class implements the abstract class Dialog.
    /// DialogShell class manages a dialog object that can have one or two option buttons.
    /// If you try to open a dialog with more than two option buttons, it will show the first two.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public class DialogShell : Dialog
    {
        [SerializeField]
        [Tooltip("Title text of the dialog")]
        private TMP_Text titleText = null;

        /// <summary>
        /// Title text of the dialog
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public TMP_Text TitleText
        {
            get { return titleText; }
            set { titleText = value; }
        }

        [SerializeField]
        [Tooltip("Description text of the dialog")]
        private TMP_Text descriptionText = null;

        /// <summary>
        /// Description text of the dialog
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public TMP_Text DescriptionText
        {
            get { return descriptionText; }
            set { descriptionText = value; }
        }

        /// <inheritdoc />
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        protected override void FinalizeLayout() { }

        /// <inheritdoc />
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        protected override void GenerateButtons()
        {
            // Find all buttons on dialog...
            List<DialogButton> buttonsOnDialog = GetAllDialogButtons();

            int buttonContextsCount = Property.ButtonContexts.Count;

            // Set desired buttons active and the rest inactive
            SetButtonsActiveStates(buttonsOnDialog, buttonContextsCount);

            // Set titles and types
            if (buttonContextsCount > 0)
            {
                // If we have two button contexts, skip the first button GameObject.
                // This is due to the set-up of the prefab, where there's one GameObject
                // which takes up the whole width of the dialog and two GameObjects
                // after that in the hierarchy representing a split-width two-button dialog.
                int step = buttonContextsCount >= 2 ? 1 : 0;
                for (int i = 0; i < buttonContextsCount && i < 2; ++i)
                {
                    buttonsOnDialog[i + step].SetButtonContext(Property.ButtonContexts[i]);
                }
            }
        }
        
        private void SetButtonsActiveStates(List<DialogButton> buttons, int count)
        {
            for (int i = 0; i < buttons.Count; ++i)
            {
                bool flag1 = (count == 1) && (i == 0);
                bool flag2 = (count >= 2) && (i > 0);
                buttons[i].ParentDialog = this;
                buttons[i].gameObject.SetActive(flag1 || flag2);
            }
        }

        private List<DialogButton> GetAllDialogButtons()
        {
            List<DialogButton> buttonsOnDialog = new List<DialogButton>();
            foreach (Transform child in transform)
            {
                if (child.name == "ButtonParent")
                {
                    DialogButton[] buttons = child.GetComponentsInChildren<DialogButton>();
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
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        protected override void SetTitleAndMessage()
        {
            if (titleText != null)
            {
                titleText.text = Property.Title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = Property.Message;
            }
        }
    }
}
