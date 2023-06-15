// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Provides a pre-defined set of button contexts for ease of dialog creation.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public static class DialogButtonHelpers
    {
        /// <summary>
        /// Represents a single OK button.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public static DialogButtonContext[] OK { get; } = new DialogButtonContext[] { new DialogButtonContext(DialogButtonType.OK) };

        /// <summary>
        /// Represents a pair of OK and Cancel buttons.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public static DialogButtonContext[] OKCancel { get; } = new DialogButtonContext[] { new DialogButtonContext(DialogButtonType.OK), new DialogButtonContext(DialogButtonType.Cancel) };

        /// <summary>
        /// Represents a pair of Yes and No buttons.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public static DialogButtonContext[] YesNo { get; } = new DialogButtonContext[] { new DialogButtonContext(DialogButtonType.Yes), new DialogButtonContext(DialogButtonType.No) };
    }
}
