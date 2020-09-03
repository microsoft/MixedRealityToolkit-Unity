// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// <see cref="System.String"/> Extensions.
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
                if (char.IsLetter(value[i]) &&
                    char.IsUpper(value[i]))
                {
                    // Add a space if the previous character is not upper-case.
                    // e.g. "LeftHand" -> "Left Hand"
                    if (i != 1 && // First character is upper-case in result.
                        (!char.IsLetter(value[i - 1]) || char.IsLower(value[i - 1])))
                    {
                        result += " ";
                    }
                    // If previous character is upper-case, only add space if the next
                    // character is lower-case. Otherwise assume this character to be inside
                    // an acronym.
                    // e.g. "OpenVRLeftHand" -> "Open VR Left Hand"
                    else if (i < value.Length - 1 &&
                        char.IsLetter(value[i + 1]) && char.IsLower(value[i + 1]))
                    {
                        result += " ";
                    }
                }

                result += value[i];
            }

            return result;
        }

        /// <summary>
        /// Ensures directory separator chars in provided string are platform independent. Given path might use \ or / but not all platforms support both.
        /// </summary>
        public static string NormalizeSeparators(this string path)
            => path?.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
}
