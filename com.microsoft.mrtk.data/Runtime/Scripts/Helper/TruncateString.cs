// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Utility functions for string truncation
    /// </summary>
    public static class TruncateString
    {
        private const char Ellipsis = '\u2026';

        /// <summary>
        /// Truncates a string by removing extra characters from the middle.
        /// </summary>
        /// <param name="input"> String to be truncated </param>
        /// <param name="maxChars"> Max allowed characters (including ellipsis) </param>
        /// <param name="trailingChars"> Max allowed characters for the end of the string </param>
        /// <returns> The truncated string </returns>
        public static string TruncateStringMiddle(string input, int maxChars, int trailingChars)
        {
            // If no truncation is needed, return input
            if (input.Length <= maxChars)
            {
                return input;
            }

            // Make room for ellipses
            maxChars--;

            // if the characters wanted to display for trailing chars is longer than the max allowed,
            // set trailing chars to the max allowed.
            if (trailingChars > maxChars)
            {
                trailingChars = maxChars;
            }

            var leadingChars = maxChars - trailingChars;
            var output = input.Substring(0, leadingChars) + Ellipsis;

            if (trailingChars > 0)
            {
                output += input.Substring(input.Length - trailingChars, trailingChars);
            }

            return output;
        }

        /// <summary>
        /// Truncates a string by removing extra characters from the middle at roughly halfway.
        /// </summary>
        /// <param name="input"> String to be truncated </param>
        /// <param name="maxChars"> Max allowed characters (including ellipsis) </param>
        /// <returns> The truncated string </returns>
        public static string TruncateStringMiddle(string input, int maxChars)
        {
            var trailingChars = maxChars / 2;
            return (TruncateStringMiddle(input, maxChars, trailingChars));
        }
    }
}
