// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Object containing properties about a dialog.
    /// </summary>
    [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
    public class DialogProperty
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">The title bar string (top-most) on the dialog.</param>
        /// <param name="message">The message content string of the dialog.</param>
        /// <param name="buttonContexts">The button type(s) available on the dialog.</param>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public DialogProperty(string title, string message, params DialogButtonContext[] buttonContexts)
        {
            Title = title;
            Message = message;

            // This is done for back compat with the obsolete property.
            foreach (DialogButtonContext buttonContext in buttonContexts)
            {
                ButtonTypes |= Convert(buttonContext.ButtonType);
            }

            ButtonContexts = buttonContexts;
        }

        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
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

        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
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

        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
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
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public string Title { get; } = string.Empty;

        /// <summary>
        /// The message content string of the dialog.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public string Message { get; } = string.Empty;

        /// <summary>
        /// The button type(s) available on the dialog.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public DialogButtonTypes ButtonTypes { get; } = DialogButtonTypes.Close;

        /// <summary>
        /// Contexts for the buttons, in order of their appearance on the dialog.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public IReadOnlyList<DialogButtonContext> ButtonContexts { get; } = null;

        /// <summary>
        /// Which button was clicked to dismiss the dialog.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public DialogButtonTypes Result => Convert(ResultContext.ButtonType);

        /// <summary>
        /// Which button was clicked to dismiss the dialog.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public DialogButtonContext ResultContext { get; internal set; } = default;

        /// <summary>
        /// Reference to the dialog this property applies to.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public Dialog TargetDialog { get; internal set; }
    }
}
