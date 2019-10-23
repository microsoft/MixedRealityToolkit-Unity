// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// <see cref="System.Text.StringBuilder"/> Extensions.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Append new line
        /// </summary>
        public static StringBuilder NewLine(this StringBuilder sb)
        {
            sb.Append("\n");
            return sb;
        }
    }
}
