// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Flags used to represent a combination of different types of transformation
    /// </summary>
    [System.Flags]
    public enum TransformFlags
    {
        /// <summary>
        /// No transformations
        /// </summary>
        None = 0,

        /// <summary>
        /// Position transformations
        /// </summary>
        Move = 1 << 0,

        /// <summary>
        /// Rotation transformations
        /// </summary>
        Rotate = 1 << 1,

        /// <summary>
        /// Scale transformations
        /// </summary>
        Scale = 1 << 2
    }

    /// <summary>
    /// Extension methods specific to the <see cref="TransformFlags"/> enum.
    /// </summary>
    public static class TransformFlagsExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="TransformFlags"/> value.</param>
        /// <param name="b"><see cref="TransformFlags"/> mask.</param>
        /// <returns>True if all of the bits in the specified mask are set in the
        /// current value.</returns>
        public static bool IsMaskSet(this TransformFlags a, TransformFlags b)
        {
            return ((a & b) == b);
        }
    }
}
