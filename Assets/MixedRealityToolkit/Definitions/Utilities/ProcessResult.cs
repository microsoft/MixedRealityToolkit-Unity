// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Result from a completed asynchronous process.
    /// </summary>
    public struct ProcessResult
    {
        /// <summary>
        /// Exit code from completed process.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Errors from completed process.
        /// </summary>
        public string[] Errors { get; }

        /// <summary>
        /// Output from completed process.
        /// </summary>
        public string[] Output { get; }

        /// <summary>
        /// Constructor for Process Result.
        /// </summary>
        /// <param name="exitCode">Exit code from completed process.</param>
        /// <param name="errors">Errors from completed process.</param>
        /// <param name="output">Output from completed process.</param>
        public ProcessResult(int exitCode, string[] errors, string[] output) : this()
        {
            ExitCode = exitCode;
            Errors = errors;
            Output = output;
        }
    }
}