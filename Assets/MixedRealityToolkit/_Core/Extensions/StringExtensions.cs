// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;

namespace Microsoft.MixedReality.Toolkit.Core.Extensions
{
    /// <summary>
    /// <see cref="String"/> Extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Encodes the string to base 64 ASCII.
        /// </summary>
        /// <param name="toEncode">String to encode.</param>
        /// <returns>Encoded string.</returns>
        public static string EncodeTo64(this string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        /// <summary>
        /// Decodes string from base 64 ASCII.
        /// </summary>
        /// <param name="encodedData">String to decode.</param>
        /// <returns>Decoded string.</returns>
        public static string DecodeFrom64(this string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return Encoding.ASCII.GetString(encodedDataAsBytes);
        }

        /// <summary>
        /// Capitalize the first character and add a space before
        /// each capitalized letter (except the first character).
        /// </summary>
        /// <param name="value"></param>
        public static string ToProperCase(this string value)
        {
            // If there are 0 or 1 characters, just return the string.
            if (value == null) { return value; }
            if (value.Length < 2) { return value.ToUpper(); }
            // If there's already spaces in the string, return.
            if (value.Contains(" ")) { return value; }

            // Start with the first character.
            string result = value.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (int i = 1; i < value.Length; i++)
            {
                var wasLastCharUpper = char.IsUpper(value[i - 1]);
                var nextIsLower = i + 1 < value.Length && char.IsLower(value[i + 1]);
                var isUpper = char.IsLetter(value[i]) && char.IsUpper(value[i]);

                if (isUpper && !wasLastCharUpper && nextIsLower)
                {
                    result += " ";
                }

                result += value[i];

                if (isUpper && wasLastCharUpper && !nextIsLower)
                {
                    result += " ";
                }
            }

            return result;
        }
    }
}
