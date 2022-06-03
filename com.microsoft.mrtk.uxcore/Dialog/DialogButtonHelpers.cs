// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Provides a pre-defined set of button contexts for ease of dialog creation.
    /// </summary>
    public static class DialogButtonHelpers
    {
        /// <summary>
        /// Represents a single OK button.
        /// </summary>
        public static DialogButtonContext[] OK { get; } = new DialogButtonContext[] { new DialogButtonContext(DialogButtonType.OK) };

        /// <summary>
        /// Represents a pair of OK and Cancel buttons.
        /// </summary>
        public static DialogButtonContext[] OKCancel { get; } = new DialogButtonContext[] { new DialogButtonContext(DialogButtonType.OK), new DialogButtonContext(DialogButtonType.Cancel) };

        /// <summary>
        /// Represents a pair of Yes and No buttons.
        /// </summary>
        public static DialogButtonContext[] YesNo { get; } = new DialogButtonContext[] { new DialogButtonContext(DialogButtonType.Yes), new DialogButtonContext(DialogButtonType.No) };
    }
}
