// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities
{
    /// <summary>
    /// An enumeration for controlling the types (and often the volume) of diagnostic
    /// log messages that are emitted from Mixed Reality Toolkit components.
    /// </summary>
    [Flags]
    public enum LoggingLevels
    {
        /// <summary>
        /// Informational messages.
        /// </summary>
        Informational = 1 << 0,     // Hex: 0x00000001, Decimal: 1

        /// <summary>
        /// Assertion messages.
        /// </summary>
        Assert = 1 << 1,            // Hex: 0x00000002, Decimal: 2

        /// <summary>
        /// Warning messages.
        /// </summary>
        Warning = 1 << 2,           // Hex: 0x00000004, Decimal: 4

        /// <summary>
        /// Error messages.
        /// </summary>
        Error = 1 << 3,             // Hex: 0x00000008, Decimal: 8

        /// <summary>
        /// Critical / foundational error messages, generally results in a significant loss 
        /// of functionality or an application exception.
        /// </summary>
        CriticalError = 1 << 4,     // Hex: 0x00000010, Decimal: 16
    }
}