﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Handling click event and dismiss dialog
    /// </summary>
    [AddComponentMenu("MRTK/UX/Dialog Button")]
    public class DialogButton : MonoBehaviour
    {
        [SerializeField, Tooltip("The TextMeshPro component for displaying text on the button itself.")]
        private TMP_Text buttonText;

        /// <summary>
        /// The TextMeshPro component for displaying text on the button itself.
        /// </summary>
        public TMP_Text ButtonText
        {
            get => buttonText;
            set => buttonText = value;
        }

        /// <summary>
        /// A reference to the Dialog that this button is on.
        /// </summary>
        public Dialog ParentDialog { get; set; }

        /// <summary>
        /// The type description of the button.
        /// </summary>
        [Obsolete("Use ButtonContext instead.")]
        public DialogButtonTypes ButtonTypeEnum => DialogProperty.Convert(ButtonContext.ButtonType);

        [SerializeField, Tooltip("The type description of the button.")]
        private DialogButtonContext buttonContext;

        /// <summary>
        /// The context, including type and an optional label, of the button.
        /// </summary>
        public DialogButtonContext ButtonContext => buttonContext;

        private void OnValidate()
        {
            SetButtonContext(ButtonContext);
        }

        /// <summary>
        /// Event handler that runs when button is clicked.
        /// Dismisses the parent dialog.
        /// </summary>
        public void OnButtonClicked()
        {
            if (ParentDialog != null)
            {
                ParentDialog.Property.ResultContext = ButtonContext;
                ParentDialog.Dismiss();
            }
        }

        [Obsolete("Use SetLabel or SetButtonContext instead.")]
        public void SetTitle(string title) => SetLabel(title);

        /// <summary>
        /// Sets the label text on the button.
        /// </summary>
        /// <param name="label">New label text for the button.</param>
        public void SetLabel(string label)
        {
            buttonContext.Label = label;

            if (ButtonText != null)
            {
                ButtonText.text = !string.IsNullOrWhiteSpace(buttonContext.Label) ? buttonContext.Label : buttonContext.ButtonType.ToString();
            }
        }

        /// <summary>
        /// Sets the button context and the label text on the button.
        /// </summary>
        /// <param name="buttonContext">New button context for this button.</param>
        public void SetButtonContext(DialogButtonContext buttonContext)
        {
            this.buttonContext = buttonContext;
            SetLabel(buttonContext.Label);
        }
    }
}
