// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The style (caption) of button on a Dialog.
    /// </summary>
    [Flags]
    public enum DialogButtonType
    {
        None = 0 << 0,
        Close = 1 << 0,
        Confirm = 1 << 1,
        Cancel = 1 << 2,
        Accept = 1 << 3,
        Yes = 1 << 4,
        No = 1 << 5,
        OK = 1 << 6
    }

    /// <summary>
    /// Extension methods specific to the <see cref="DialogButtonType"/> enum.
    /// </summary>
    public static class DialogButtonTypeExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="DialogButtonType"/> value.</param>
        /// <param name="b"><see cref="DialogButtonType"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this DialogButtonType a, DialogButtonType b)
        {
            return (a & b) == b;
        }
    }
}