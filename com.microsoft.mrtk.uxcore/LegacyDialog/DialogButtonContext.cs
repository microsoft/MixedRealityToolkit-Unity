// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Represents a button with its type and an optional custom label.
    /// </summary>
    [Serializable]
    [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
    public struct DialogButtonContext
    {
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public DialogButtonContext(DialogButtonType buttonType, string label = null)
        {
            ButtonType = buttonType;
            Label = label;
        }

        /// <summary>
        /// The type of this button.
        /// </summary>
        [field: SerializeField, Tooltip("The type of this button.")]
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public DialogButtonType ButtonType { get; internal set; }

        /// <summary>
        /// The optional label for this button.
        /// </summary>
        /// <remarks>If none is provided, the string representation of <see cref="ButtonType"/> will be used.</remarks>
        [field: SerializeField, Tooltip("The optional label for this button. If none is provided, the string representation of Button Type will be used.")]
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public string Label { get; internal set; }
    }
}
