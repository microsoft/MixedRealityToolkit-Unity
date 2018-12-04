// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics
{
    public struct MemoryReading
    {
        public long GcMemoryInBytes { get; set; }

        public MemoryReading Reset()
        {
            GcMemoryInBytes = 0;
            return this;
        }

        public static MemoryReading operator +(MemoryReading a, MemoryReading b)
        {
            a.GcMemoryInBytes += b.GcMemoryInBytes;
            return a;
        }

        public static MemoryReading operator /(MemoryReading a, int b)
        {
            a.GcMemoryInBytes /= b;
            return a;
        }
    }
}