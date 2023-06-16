// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Represents a button with its type and an optional custom label.
    /// </summary>
    /// <remarks>
    /// This and the <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> are deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Serializable]
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public struct DialogButtonContext
    {
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public DialogButtonContext(DialogButtonType buttonType, string label = null)
        {
            ButtonType = buttonType;
            Label = label;
        }

        /// <summary>
        /// The type of this button.
        /// </summary>
        [field: SerializeField, Tooltip("The type of this button.")]
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public DialogButtonType ButtonType { get; internal set; }

        /// <summary>
        /// The optional label for this button.
        /// </summary>
        /// <remarks>If none is provided, the string representation of <see cref="ButtonType"/> will be used.</remarks>
        [field: SerializeField, Tooltip("The optional label for this button. If none is provided, the string representation of Button Type will be used.")]
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public string Label { get; internal set; }
    }
}
