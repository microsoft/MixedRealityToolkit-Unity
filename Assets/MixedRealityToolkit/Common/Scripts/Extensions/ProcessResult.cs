// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Common.Extensions
{
    public struct ProcessResult
    {
        public int ExitCode { get; }
        public string[] Errors { get; }
        public string[] Output { get; }

        public ProcessResult(int exitCode, string[] errors, string[] output) : this()
        {
            ExitCode = exitCode;
            Errors = errors;
            Output = output;
        }
    }
}