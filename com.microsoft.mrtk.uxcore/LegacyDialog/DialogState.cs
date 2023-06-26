// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Describes the current state of a Dialog.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public enum DialogState
    {
        /// <summary>
        /// The dialog has not been opened or closed yet.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// The dialog is the the process of opening.
        /// </summary>
        Opening,

        /// <summary>
        /// The dialog is opened and is ready of input.
        /// </summary>
        WaitingForInput,

        /// <summary>
        /// The dialog is now closed.
        /// </summary>
        Closed
    }
}