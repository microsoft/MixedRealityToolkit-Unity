// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// The style of a legacy dialog button.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public enum DialogButtonType
    {
        /// <summary>
        /// An unspecified button type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a close dialog button.
        /// </summary>
        Close = 1,

        /// <summary>
        /// Represents a confirmation dialog button.
        /// </summary>
        Confirm = 2,

        /// <summary>
        /// Represents a cancel dialog button.
        /// </summary>
        Cancel = 3,

        /// <summary>
        /// Represents an accept dialog button.
        /// </summary>
        Accept = 4,

        /// <summary>
        /// Represents a "yes" dialog button.
        /// </summary>
        Yes = 5,

        /// <summary>
        /// Represents a "no" dialog button.
        /// </summary>
        No = 6,

        /// <summary>
        /// Represents an "okay" dialog button.
        /// </summary>
        OK = 7,
    }

    /// <summary>
    /// The style of a legacy dialog button.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Flags]
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public enum DialogButtonTypes
    {
        /// <summary>
        /// No button style has been specified.
        /// </summary>
        None = 0 << 0,

        /// <summary>
        /// Represents a close dialog button.
        /// </summary>
        Close = 1 << 0,

        /// <summary>
        /// Represents a confirmation dialog button.
        /// </summary>
        Confirm = 1 << 1,

        /// <summary>
        /// Represents a cancel dialog button.
        /// </summary>
        Cancel = 1 << 2,

        /// <summary>
        /// Represents an accept dialog button.
        /// </summary>
        Accept = 1 << 3,
        
        /// <summary>
        /// Represents a "yes" dialog button.
        /// </summary>
        Yes = 1 << 4,

        /// <summary>
        /// Represents a "no" dialog button.
        /// </summary>
        No = 1 << 5,

        /// <summary>
        /// Represents an "okay" dialog button.
        /// </summary>
        OK = 1 << 6,
    }
}
