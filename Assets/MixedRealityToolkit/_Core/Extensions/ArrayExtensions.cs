// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Internal.Extensions
{
    public static class ArrayExtensions
    {
        public static int WrapIndex(this Array array, int index)
        {
            int length = array.Length;
            return ((index % length) + length) % length;
        }
    }
}