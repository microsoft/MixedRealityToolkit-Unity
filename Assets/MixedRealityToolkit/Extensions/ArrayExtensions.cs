// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Extensions
{
    /// <summary>
    /// <see cref="Array"/> type method extensions.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Wraps the index around to the beginning of the array if the provided index is longer than the array.
        /// </summary>
        /// <param name="array">The array to wrap the index around.</param>
        /// <param name="index">The index to look for.</param>
        /// <returns></returns>
        public static int WrapIndex(this Array array, int index)
        {
            int length = array.Length;
            return ((index % length) + length) % length;
        }
    }
}