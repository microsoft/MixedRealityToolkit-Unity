// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// <see cref="System.Text.StringBuilder"/> Extensions.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Append new line for current Environment to this StringBuilder buffer
        /// </summary>
        public static StringBuilder AppendNewLine(this StringBuilder sb)
        {
            sb.Append(Environment.NewLine);
            return sb;
        }
    }
}
