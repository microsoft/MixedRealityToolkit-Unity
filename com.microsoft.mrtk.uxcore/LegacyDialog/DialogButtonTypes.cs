// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// The style of a legacy dialog button.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog>Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommend that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see> system.
    /// </remarks>
    [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog system.")]
    public enum DialogButtonType
    {
        /// <summary>
        /// An unspecified button type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represent a close dialog button.
        /// </summary>
        Close = 1,

        /// <summary>
        /// Represent a confirmation dialog button.
        /// </summary>
        Confirm = 2,

        /// <summary>
        /// Represent a cancel dialog button.
        /// </summary>
        Cancel = 3,

        /// <summary>
        /// Represent an accept dialog button.
        /// </summary>
        Accept = 4,

        /// <summary>
        /// Represent a "yes" dialog button.
        /// </summary>
        Yes = 5,

        /// <summary>
        /// Represent a "no" dialog button.
        /// </summary>
        No = 6,

        /// <summary>
        /// Represent an "okay" dialog button.
        /// </summary>
        OK = 7,
    }

    /// <summary>
    /// The style of button on a dialog.
    /// </summary>
    [Flags]
    [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
    public enum DialogButtonTypes
    {
        None = 0 << 0,
        Close = 1 << 0,
        Confirm = 1 << 1,
        Cancel = 1 << 2,
        Accept = 1 << 3,
        Yes = 1 << 4,
        No = 1 << 5,
        OK = 1 << 6,
    }
}
