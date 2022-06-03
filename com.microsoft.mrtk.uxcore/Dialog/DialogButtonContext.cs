// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Represents a button with its type and an optional custom label.
    /// </summary>
    [Serializable]
    public struct DialogButtonContext
    {
        public DialogButtonContext(DialogButtonType buttonType, string label = null)
        {
            ButtonType = buttonType;
            Label = label;
        }

        /// <summary>
        /// The type of this button.
        /// </summary>
        [field: SerializeField, Tooltip("The type of this button.")]
        public DialogButtonType ButtonType { get; internal set; }

        /// <summary>
        /// The optional label for this button.
        /// </summary>
        /// <remarks>If none is provided, the string representation of <see cref="ButtonType"/> will be used.</remarks>
        [field: SerializeField, Tooltip("The optional label for this button. If none is provided, the string representation of Button Type will be used.")]
        public string Label { get; internal set; }
    }
}
