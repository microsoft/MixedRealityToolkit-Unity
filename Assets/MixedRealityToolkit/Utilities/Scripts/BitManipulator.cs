// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// Helper class for bit manipulation.
    /// </summary>
    public class BitManipulator
    {
        Int32 mask;
        Int32 shift;

        public BitManipulator(Int32 mask, Int32 shift)
        {
            this.mask = mask;
            this.shift = shift;
        }

        public Int32 GetBitsValue(Int32 input)
        {
            return (input & mask) >> shift;
        }

        public void SetBits(ref Int32 value, Int32 bitsValue)
        {
            Int32 iT = bitsValue << shift;
            iT = iT & mask;
            value = value | iT;
        }
    }
}
