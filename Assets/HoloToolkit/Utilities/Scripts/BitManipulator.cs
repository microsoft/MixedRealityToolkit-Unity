//
// Copyright (C) Microsoft. All rights reserved.
//

using System;

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
        return (input & this.mask) >> this.shift;
    }

    public void SetBits(ref Int32 value, Int32 bitsValue)
    {
        Int32 iT = bitsValue << this.shift;
        iT = iT & this.mask;
        value = value | iT;
    }
}
