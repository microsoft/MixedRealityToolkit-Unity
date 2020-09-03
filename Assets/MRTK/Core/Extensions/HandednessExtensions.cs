// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// <see cref=" Microsoft.MixedReality.Toolkit.Utilities.Handedness"/> type method extensions.
    /// </summary>
    public static class HandednessExtensions
    {
        /// <summary>
        /// Gets the opposite "hand" flag for the current Handedness value.
        /// </summary>
        /// <remarks>
        /// If current = Left, returns Right.
        /// If current = Right, returns Left.
        /// Otherwise, returns None
        /// </remarks>
        public static Handedness GetOppositeHandedness(this Handedness current)
        {
            if (current == Handedness.Left)
            {
                return Handedness.Right;
            }
            else if (current == Handedness.Right)
            {
                return Handedness.Left;
            }
            else
            {
                return Handedness.None;
            }
        }

        /// <summary>
        /// Returns true if the current Handedness is the Right (i.e == Handedness.Right), false otherwise
        /// </summary>
        public static bool IsRight(this Handedness current)
        {
            return current == Handedness.Right;
        }

        /// <summary>
        /// Returns true if the current Handedness is the Right (i.e == Handedness.Right), false otherwise
        /// </summary>
        public static bool IsLeft(this Handedness current)
        {
            return current == Handedness.Left;
        }

        /// <summary>
        /// Returns true if the current Handedness is the Right (i.e == Handedness.Right), false otherwise
        /// </summary>
        public static bool IsNone(this Handedness current)
        {
            return current == Handedness.None;
        }

        /// <summary>
        /// Returns true if the current Handedness flags are a match with the comparison Handedness flags, false otherwise
        /// </summary>
        public static bool IsMatch(this Handedness current, Handedness compare)
        {
            return (current & compare) != 0;
        }
    }
}
