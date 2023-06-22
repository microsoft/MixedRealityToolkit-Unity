// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Object containing properties about a dialog.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public class DialogProperty
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">The title bar string (top-most) on the dialog.</param>
        /// <param name="message">The message content string of the dialog.</param>
        /// <param name="buttonContexts">The button type(s) available on the dialog.</param>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
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

        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
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

        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
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

        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
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
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public string Title { get; } = string.Empty;

        /// <summary>
        /// The message content string of the dialog.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public string Message { get; } = string.Empty;

        /// <summary>
        /// The button type(s) available on the dialog.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public DialogButtonTypes ButtonTypes { get; } = DialogButtonTypes.Close;

        /// <summary>
        /// Contexts for the buttons, in order of their appearance on the dialog.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public IReadOnlyList<DialogButtonContext> ButtonContexts { get; } = null;

        /// <summary>
        /// Which button was clicked to dismiss the dialog.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public DialogButtonTypes Result => Convert(ResultContext.ButtonType);

        /// <summary>
        /// Which button was clicked to dismiss the dialog.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public DialogButtonContext ResultContext { get; internal set; } = default;

        /// <summary>
        /// Reference to the dialog this property applies to.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public Dialog TargetDialog { get; internal set; }
    }
}
