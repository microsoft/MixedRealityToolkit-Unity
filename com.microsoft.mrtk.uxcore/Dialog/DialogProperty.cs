// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Object containing properties about a dialog.
    /// </summary>
    public class DialogProperty
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">The title bar string (top-most) on the dialog.</param>
        /// <param name="message">The message content string of the dialog.</param>
        /// <param name="buttonContexts">The button type(s) available on the dialog.</param>
        public DialogProperty(string title, string message, params DialogButtonContext[] buttonContexts)
        {
            Title = title;
            Message = message;

            // This is done for back compat with the obsolete property.
            foreach (DialogButtonContext buttonContext in buttonContexts)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ButtonTypes |= Convert(buttonContext.ButtonType);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            ButtonContexts = buttonContexts;
        }

        [Obsolete("Use the constructor that takes in explicit DialogButtonContext instances to create the buttons.")]
        public DialogProperty(string title, string message, DialogButtonTypes buttonTypes) : this(title, message)
        {
            List<DialogButtonContext> buttonTypesList = new List<DialogButtonContext>();
            foreach (DialogButtonTypes buttonType in Enum.GetValues(typeof(DialogButtonTypes)))
            {
                if (buttonType != DialogButtonTypes.None && (buttonTypes & buttonType) == buttonType)
                {
                    buttonTypesList.Add(new DialogButtonContext(Convert(buttonType)));
                }
            }

            ButtonTypes = buttonTypes;
            ButtonContexts = buttonTypesList.ToArray();
        }

        [Obsolete("Only used for back compat. Do not use elsewhere.")]
        private static DialogButtonType Convert(DialogButtonTypes dialogButtonTypes)
        {
            switch (dialogButtonTypes)
            {
                case DialogButtonTypes.None: return DialogButtonType.None;
                case DialogButtonTypes.Close: return DialogButtonType.Close;
                case DialogButtonTypes.Confirm: return DialogButtonType.Confirm;
                case DialogButtonTypes.Cancel: return DialogButtonType.Cancel;
                case DialogButtonTypes.Accept: return DialogButtonType.Accept;
                case DialogButtonTypes.Yes: return DialogButtonType.Yes;
                case DialogButtonTypes.No: return DialogButtonType.No;
                case DialogButtonTypes.OK: return DialogButtonType.OK;
                default: return DialogButtonType.None;
            }
        }

        [Obsolete("Only used for back compat. Do not use elsewhere.")]
        internal static DialogButtonTypes Convert(DialogButtonType dialogButtonTypes)
        {
            switch (dialogButtonTypes)
            {
                case DialogButtonType.None: return DialogButtonTypes.None;
                case DialogButtonType.Close: return DialogButtonTypes.Close;
                case DialogButtonType.Confirm: return DialogButtonTypes.Confirm;
                case DialogButtonType.Cancel: return DialogButtonTypes.Cancel;
                case DialogButtonType.Accept: return DialogButtonTypes.Accept;
                case DialogButtonType.Yes: return DialogButtonTypes.Yes;
                case DialogButtonType.No: return DialogButtonTypes.No;
                case DialogButtonType.OK: return DialogButtonTypes.OK;
                default: return DialogButtonTypes.None;
            }
        }

        /// <summary>
        /// The title bar string (top-most) on the dialog.
        /// </summary>
        public string Title { get; } = string.Empty;

        /// <summary>
        /// The message content string of the dialog.
        /// </summary>
        public string Message { get; } = string.Empty;

        /// <summary>
        /// The button type(s) available on the dialog.
        /// </summary>
        [Obsolete("Query ButtonContexts instead.")]
        public DialogButtonTypes ButtonTypes { get; } = DialogButtonTypes.Close;

        /// <summary>
        /// Contexts for the buttons, in order of their appearance on the dialog.
        /// </summary>
        public IReadOnlyList<DialogButtonContext> ButtonContexts { get; } = null;

        /// <summary>
        /// Which button was clicked to dismiss the dialog.
        /// </summary>
        [Obsolete("Use ResultContext.ButtonType instead.")]
        public DialogButtonTypes Result => Convert(ResultContext.ButtonType);

        /// <summary>
        /// Which button was clicked to dismiss the dialog.
        /// </summary>
        public DialogButtonContext ResultContext { get; internal set; } = default;

        /// <summary>
        /// Reference to the dialog this property applies to.
        /// </summary>
        public Dialog TargetDialog { get; internal set; }
    }
}
