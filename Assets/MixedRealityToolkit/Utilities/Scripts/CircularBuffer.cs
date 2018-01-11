// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// Helper class for transmitting data over network.
    /// </summary>
    public class CircularBuffer
    {
        public CircularBuffer(int size, bool allowOverwrite = false, int padding = 4)
        {
            data = new byte[size];
            readWritePadding = padding;
            this.allowOverwrite = allowOverwrite;
        }

        public int TotalCapacity
        {
            get
            {
                return data.Length - readWritePadding;
            }
        }

        public int UsedCapacity
        {
            get
            {
                if (writeOffset >= readOffset)
                {
                    return writeOffset - readOffset;
                }
                int firstChunk = data.Length - readOffset;
                int secondChunk = writeOffset;
                return firstChunk + secondChunk;
            }
        }

        public void Reset()
        {
            readOffset = 0;
            writeOffset = 0;
        }

        public int Write(Array src, int srcReadPosBytes, int byteCount)
        {
            int maxWritePos;
            bool wrappedAround = writeOffset < readOffset;
            if (!wrappedAround)
            {
                maxWritePos = (readOffset != 0 || allowOverwrite) ? data.Length : data.Length - readWritePadding;
            }
            else
            {
                maxWritePos = allowOverwrite ? data.Length : readOffset - readWritePadding;
            }

            int chunkSize = Math.Min(byteCount, maxWritePos - writeOffset);
            int writeEnd = writeOffset + chunkSize;
            bool needToMoveReadOffset = wrappedAround ? writeEnd >= readOffset : (writeEnd == data.Length && readOffset == 0);
            if (needToMoveReadOffset)
            {
                if (!allowOverwrite)
                {
                    throw new Exception("Circular buffer logic error. Overwriting data.");
                }
                readOffset = (writeEnd + readWritePadding) % data.Length;
            }

            Buffer.BlockCopy(src, srcReadPosBytes, data, writeOffset, chunkSize);
            writeOffset = (writeOffset + chunkSize) % data.Length;

            int bytesWritten = chunkSize;
            int remaining = byteCount - bytesWritten;
            if (bytesWritten > 0 && remaining > 0)
            {
                bytesWritten += Write(src, srcReadPosBytes + chunkSize, remaining);
            }

            return bytesWritten;
        }

        public int Read(Array dst, int dstWritePosBytes, int byteCount)
        {
            if (readOffset == writeOffset)
            {
                return 0;
            }

            int maxReadPos;
            if (readOffset > writeOffset)
            {
                maxReadPos = data.Length;
            }
            else
            {
                maxReadPos = writeOffset;
            }

            int chunkSize = Math.Min(byteCount, maxReadPos - readOffset);

            Buffer.BlockCopy(data, readOffset, dst, dstWritePosBytes, chunkSize);
            readOffset = (readOffset + chunkSize) % data.Length;

            int bytesRead = chunkSize;
            int remaining = byteCount - bytesRead;
            if (bytesRead > 0 && remaining > 0)
            {
                bytesRead += Read(dst, dstWritePosBytes + bytesRead, remaining);
            }

            return bytesRead;
        }

        private byte[] data;
        private int writeOffset;
        private int readOffset;

        private readonly int readWritePadding;
        private readonly bool allowOverwrite;
    }
}